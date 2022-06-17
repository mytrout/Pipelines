﻿// <copyright file="ReadZipArchiveEntriesFromZipArchiveStepTests.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2020-2021 Chris Trout
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>

namespace MyTrout.Pipelines.IO.Compression.Tests
{
    using Castle.DynamicProxy.Generators;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualBasic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines.Core;
    using MyTrout.Pipelines.Steps;
    using MyTrout.Pipelines.Steps.IO.Compression;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ReadZipArchiveEntriesFromZipArchiveStepTests
    {
        [TestMethod]
        public async Task Constructs_ReadZipArchiveEntriesFromZipArchiveStep_Successfully()
        {
            // arrange
            ILogger<ReadZipArchiveEntriesFromZipArchiveStep> logger = new Mock<ILogger<ReadZipArchiveEntriesFromZipArchiveStep>>().Object;
            ReadZipArchiveEntriesFromZipArchiveOptions options = new ();
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            // act
            await using (var result = new ReadZipArchiveEntriesFromZipArchiveStep(logger, options, next))
            {
                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(logger, result.Logger);
                Assert.AreEqual(next, result.Next);
            }
        }

        [TestMethod]
        public async Task Returns_Pipeline_Error_From_InvokeAsync_When_Context_Lacks_ZipArchive()
        {
            // arrange
            var context = new PipelineContext();

            var logger = new Mock<ILogger<ReadZipArchiveEntriesFromZipArchiveStep>>().Object;
            ReadZipArchiveEntriesFromZipArchiveOptions options = new ();
            var next = new Mock<IPipelineRequest>().Object;

            int expectedErrorCount = 1;
            string expectedMessage = Pipelines.Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, options.ZipArchiveContextName);

            await using (var source = new ReadZipArchiveEntriesFromZipArchiveStep(logger, options, next))
            {
                // act
                await source.InvokeAsync(context).ConfigureAwait(false);
            }

            // assert
            Assert.AreEqual(expectedErrorCount, context.Errors.Count);
            Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
            Assert.AreEqual(expectedMessage, context.Errors[0].Message);
        }

        [TestMethod]
        public async Task Restores_Pipeline_Context_From_InvokeAsync_When_Exception_Is_Thrown_In_Next()
        {
            // arrange
            var context = new PipelineContext();

            ILogger<ReadZipArchiveEntriesFromZipArchiveStep> logger = new Mock<ILogger<ReadZipArchiveEntriesFromZipArchiveStep>>().Object;
            ReadZipArchiveEntriesFromZipArchiveOptions options = new ();
            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(It.IsAny<IPipelineContext>()))
                                .Throws(new ApplicationException());
            IPipelineRequest next = mockNext.Object;

            string zipFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}Three-Entry.zip";

            int expectedError = 1;
            using (var zipStream = new MemoryStream())
            {
                using (var fileStream = File.OpenRead(zipFilePath))
                {
                    await fileStream.CopyToAsync(zipStream).ConfigureAwait(false);
                }

                using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Update, true))
                {
                    context.Items.Add(options.ZipArchiveContextName, zipArchive);

                    await using (var source = new ReadZipArchiveEntriesFromZipArchiveStep(logger, options, next))
                    {
                        // act
                        await source.InvokeAsync(context).ConfigureAwait(false);
                    }

                    // assert (final to make sure that no errors need to be thrown.
                    Assert.AreEqual(expectedError, context.Errors.Count);
                    Assert.IsInstanceOfType(context.Errors[0], typeof(ApplicationException));
                    Assert.IsFalse(context.Items.ContainsKey(options.OutputStreamContextName), "OutputStream should not exist.");
                    Assert.IsFalse(context.Items.ContainsKey(options.ZipArchiveEntryNameContextName), "ZipArchiveEntryName should not exist.");
                }
            }
        }

        [TestMethod]
        public async Task Returns_Correct_ZipArchiveEntry_From_InvokeAsync()
        {
            // arrange
            var entryNames = new List<string>()
            {
                "MickeyMouse.txt",
                "MinnieMouse.txt",
                "SteamboatWillie.txt"
            };

            var entryValues = new List<string>()
            {
                "What is this Mickey Mouse bullcrap?",
                "Minnie Mouse loves Mickey Mouse.",
                "Who started it all?"
            };

            var context = new PipelineContext();

            ILogger<ReadZipArchiveEntriesFromZipArchiveStep> logger = new Mock<ILogger<ReadZipArchiveEntriesFromZipArchiveStep>>().Object;
            int entryCount = 0;

            ReadZipArchiveEntriesFromZipArchiveOptions options = new ();

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(It.IsAny<IPipelineContext>()))
                                .Callback(() =>
                                {
                                    // assert must be setup on each file.
                                    entryCount++;
                                    Assert.IsTrue(context.Items.ContainsKey(options.ZipArchiveEntryNameContextName), "ZipArchiveEntryName should exist.");
                                    CollectionAssert.Contains(entryNames, context.Items[options.ZipArchiveEntryNameContextName]);
                                    using (var reader = new StreamReader(context.Items[options.OutputStreamContextName] as Stream))
                                    {
                                        CollectionAssert.Contains(entryValues, reader.ReadToEnd());
                                    }
                                })
                                .Returns(Task.CompletedTask);
            IPipelineRequest next = mockNext.Object;

            string zipFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}Three-Entry.zip";

            using (var zipStream = new MemoryStream())
            {
                using (var fileStream = File.OpenRead(zipFilePath))
                {
                    await fileStream.CopyToAsync(zipStream).ConfigureAwait(false);
                }

                using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Update, true))
                {
                    context.Items.Add(options.ZipArchiveContextName, zipArchive);

                    await using (var source = new ReadZipArchiveEntriesFromZipArchiveStep(logger, options, next))
                    {
                        // act
                        await source.InvokeAsync(context).ConfigureAwait(false);
                    }

                    // assert (final to make sure that no errors need to be thrown.
                    if (context.Errors.Any())
                    {
                        throw context.Errors[0];
                    }

                    Assert.AreEqual(3, entryCount);
                    Assert.IsFalse(context.Items.ContainsKey(options.OutputStreamContextName), "OutputStream should not exist.");
                    Assert.IsFalse(context.Items.ContainsKey(options.ZipArchiveEntryNameContextName), "ZipArchiveEntryName should not exist.");
                }
            }
        }

        [TestMethod]
        public async Task Returns_Correct_ZipArchiveEntry_From_InvokeAsync_When_Options_Uses_Different_ContextNames()
        {
            // arrange
            var entryNames = new List<string>()
            {
                "MickeyMouse.txt",
                "MinnieMouse.txt",
                "SteamboatWillie.txt"
            };

            var entryValues = new List<string>()
            {
                "What is this Mickey Mouse bullcrap?",
                "Minnie Mouse loves Mickey Mouse.",
                "Who started it all?"
            };

            var context = new PipelineContext();

            ILogger<ReadZipArchiveEntriesFromZipArchiveStep> logger = new Mock<ILogger<ReadZipArchiveEntriesFromZipArchiveStep>>().Object;
            int entryCount = 0;

            ReadZipArchiveEntriesFromZipArchiveOptions options = new ()
            {
                OutputStreamContextName = "OUTPUT_STREAM_TESTING",
                ZipArchiveContextName = "ZIP_ARCHIVE_TESTING",
                ZipArchiveEntryNameContextName = "ZIP_ARCHIVE_ENTRY_NAME_TESTING"
            };

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(It.IsAny<IPipelineContext>()))
                                .Callback(() =>
                                {
                                    // assert must be setup on each file.
                                    entryCount++;
                                    Assert.IsTrue(context.Items.ContainsKey(options.ZipArchiveEntryNameContextName), "ZipArchiveEntryName should exist.");
                                    CollectionAssert.Contains(entryNames, context.Items[options.ZipArchiveEntryNameContextName]);
                                    using (var reader = new StreamReader(context.Items[options.OutputStreamContextName] as Stream))
                                    {
                                        CollectionAssert.Contains(entryValues, reader.ReadToEnd());
                                    }
                                })
                                .Returns(Task.CompletedTask);
            IPipelineRequest next = mockNext.Object;

            string zipFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}Three-Entry.zip";

            using (var zipStream = new MemoryStream())
            {
                using (var fileStream = File.OpenRead(zipFilePath))
                {
                    await fileStream.CopyToAsync(zipStream).ConfigureAwait(false);
                }

                using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Update, true))
                {
                    context.Items.Add(options.ZipArchiveContextName, zipArchive);

                    await using (var source = new ReadZipArchiveEntriesFromZipArchiveStep(logger, options, next))
                    {
                        // act
                        await source.InvokeAsync(context).ConfigureAwait(false);
                    }

                    // assert (final to make sure that no errors need to be thrown.
                    if (context.Errors.Any())
                    {
                        throw context.Errors[0];
                    }

                    Assert.AreEqual(3, entryCount);
                    Assert.IsFalse(context.Items.ContainsKey(options.OutputStreamContextName), "OutputStream should not exist.");
                    Assert.IsFalse(context.Items.ContainsKey(options.ZipArchiveEntryNameContextName), "ZipArchiveEntryName should not exist.");
                }
            }
        }

        [TestMethod]
        public async Task Returns_Previous_OutputStream_From_InvokeAsync_When_Previous_Output_Stream_Is_Provided()
        {
            // arrange
            var context = new PipelineContext();

            ILogger<ReadZipArchiveEntriesFromZipArchiveStep> logger = new Mock<ILogger<ReadZipArchiveEntriesFromZipArchiveStep>>().Object;
            ReadZipArchiveEntriesFromZipArchiveOptions options = new ();
            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(It.IsAny<IPipelineContext>())).Returns(Task.CompletedTask);
            IPipelineRequest next = mockNext.Object;

            string zipFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}Three-Entry.zip";

            using (var previousOutputStream = new MemoryStream())
            {
                using (var zipStream = new MemoryStream())
                {
                    using (var fileStream = File.OpenRead(zipFilePath))
                    {
                        await fileStream.CopyToAsync(zipStream).ConfigureAwait(false);
                    }

                    using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Update, true))
                    {
                        context.Items.Add(options.OutputStreamContextName, previousOutputStream);
                        context.Items.Add(options.ZipArchiveContextName, zipArchive);

                        await using (var source = new ReadZipArchiveEntriesFromZipArchiveStep(logger, options, next))
                        {
                            // act
                            await source.InvokeAsync(context).ConfigureAwait(false);
                        }

                        // assert
                        if (context.Errors.Any())
                        {
                            throw context.Errors[0];
                        }

                        Assert.IsTrue(context.Items.ContainsKey(options.OutputStreamContextName), "OutputStream should exist.");
                        Assert.AreEqual(previousOutputStream, context.Items[options.OutputStreamContextName]);
                    }
                }
            }
        }

        [TestMethod]
        public async Task Throws_ArgumentNullException_From_Constructor_When_Logger_Is_Null()
        {
            // arrange
            ILogger<ReadZipArchiveEntriesFromZipArchiveStep> logger = null;
            ReadZipArchiveEntriesFromZipArchiveOptions options = new ();
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            string expectedParamName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new ReadZipArchiveEntriesFromZipArchiveStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);

            // cleanup
            await next.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Next_Is_Null()
        {
            // arrange
            ILogger<ReadZipArchiveEntriesFromZipArchiveStep> logger = new Mock<ILogger<ReadZipArchiveEntriesFromZipArchiveStep>>().Object;
            ReadZipArchiveEntriesFromZipArchiveOptions options = new ();
            IPipelineRequest next = null;

            string expectedParamName = nameof(next);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new ReadZipArchiveEntriesFromZipArchiveStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Options_Is_Null()
        {
            // arrange
            ILogger<ReadZipArchiveEntriesFromZipArchiveStep> logger = new Mock<ILogger<ReadZipArchiveEntriesFromZipArchiveStep>>().Object;
            ReadZipArchiveEntriesFromZipArchiveOptions options = null;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            string expectedParamName = nameof(options);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new ReadZipArchiveEntriesFromZipArchiveStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }
    }
}
