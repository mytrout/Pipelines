﻿// <copyright file="ReadZipArchiveEntriesFromZipArchiveStepTests.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2020 Chris Trout
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
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            // act
            await using (var result = new ReadZipArchiveEntriesFromZipArchiveStep(logger, next))
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
            PipelineContext context = new PipelineContext();

            var logger = new Mock<ILogger<RemoveZipArchiveEntryStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;

            int expectedErrorCount = 1;
            string expectedMessage = Pipelines.Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, CompressionConstants.ZIP_ARCHIVE);

            await using (var source = new RemoveZipArchiveEntryStep(logger, next))
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
            PipelineContext context = new PipelineContext();

            ILogger<ReadZipArchiveEntriesFromZipArchiveStep> logger = new Mock<ILogger<ReadZipArchiveEntriesFromZipArchiveStep>>().Object;

            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
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
                    context.Items.Add(CompressionConstants.ZIP_ARCHIVE, zipArchive);

                    await using (var source = new ReadZipArchiveEntriesFromZipArchiveStep(logger, next))
                    {
                        // act
                        await source.InvokeAsync(context).ConfigureAwait(false);
                    }

                    // assert (final to make sure that no errors need to be thrown.
                    Assert.AreEqual(expectedError, context.Errors.Count);
                    Assert.IsInstanceOfType(context.Errors[0], typeof(ApplicationException));
                    Assert.IsFalse(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_STREAM), "OutputStream should not exist.");
                    Assert.IsFalse(context.Items.ContainsKey(CompressionConstants.ZIP_ARCHIVE_ENTRY_NAME), "ZipArchiveEntryName should not exist.");
                }
            }
        }

        [TestMethod]
        public async Task Returns_Correct_ZipArchiveEntry_From_InvokeAsync()
        {
            // arrange
            List<string> entryNames = new List<string>()
            {
                "MickeyMouse.txt",
                "MinnieMouse.txt",
                "SteamboatWillie.txt"
            };

            List<string> entryValues = new List<string>()
            {
                "What is this Mickey Mouse bullcrap?",
                "Minnie Mouse loves Mickey Mouse.",
                "Who started it all?"
            };

            PipelineContext context = new PipelineContext();

            ILogger<ReadZipArchiveEntriesFromZipArchiveStep> logger = new Mock<ILogger<ReadZipArchiveEntriesFromZipArchiveStep>>().Object;
            int entryCount = 0;

            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(It.IsAny<IPipelineContext>()))
                                .Callback(() =>
                                {
                                    // assert must be setup on each file.
                                    entryCount++;
                                    Assert.IsTrue(context.Items.ContainsKey(CompressionConstants.ZIP_ARCHIVE_ENTRY_NAME), "ZipArchiveEntryName should exist.");
                                    CollectionAssert.Contains(entryNames, context.Items[CompressionConstants.ZIP_ARCHIVE_ENTRY_NAME]);
                                    using (var reader = new StreamReader(context.Items[PipelineContextConstants.OUTPUT_STREAM] as Stream))
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
                    context.Items.Add(CompressionConstants.ZIP_ARCHIVE, zipArchive);

                    await using (var source = new ReadZipArchiveEntriesFromZipArchiveStep(logger, next))
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
                    Assert.IsFalse(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_STREAM), "OutputStream should not exist.");
                    Assert.IsFalse(context.Items.ContainsKey(CompressionConstants.ZIP_ARCHIVE_ENTRY_NAME), "ZipArchiveEntryName should not exist.");
                }
            }
        }

        [TestMethod]
        public async Task Returns_Previous_OutputStream_From_InvokeAsync_When_Previous_Output_Stream_Is_Provided()
        {
            // arrange
            PipelineContext context = new PipelineContext();

            ILogger<ReadZipArchiveEntriesFromZipArchiveStep> logger = new Mock<ILogger<ReadZipArchiveEntriesFromZipArchiveStep>>().Object;

            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
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
                        context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, previousOutputStream);
                        context.Items.Add(CompressionConstants.ZIP_ARCHIVE, zipArchive);

                        await using (var source = new ReadZipArchiveEntriesFromZipArchiveStep(logger, next))
                        {
                            // act
                            await source.InvokeAsync(context).ConfigureAwait(false);
                        }

                        // assert
                        if (context.Errors.Any())
                        {
                            throw context.Errors[0];
                        }

                        Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_STREAM), "OutputStream should exist.");
                        Assert.AreEqual(previousOutputStream, context.Items[PipelineContextConstants.OUTPUT_STREAM]);
                    }
                }
            }
        }

        [TestMethod]
        public async Task Throws_ArgumentNullException_From_Constructor_When_Logger_Is_Null()
        {
            // arrange
            ILogger<ReadZipArchiveEntriesFromZipArchiveStep> logger = null;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            string expectedParamName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new ReadZipArchiveEntriesFromZipArchiveStep(logger, next));

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
            IPipelineRequest next = null;

            string expectedParamName = nameof(next);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new ReadZipArchiveEntriesFromZipArchiveStep(logger, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }
    }
}