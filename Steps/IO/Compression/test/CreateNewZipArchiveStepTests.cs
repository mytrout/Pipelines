// <copyright file="CreateNewZipArchiveStepTests.cs" company="Chris Trout">
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
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines.Core;
    using MyTrout.Pipelines.Steps.IO.Compression;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage]
    [TestClass]
    public class CreateNewZipArchiveStepTests
    {
        [TestMethod]
        public async Task Constructs_CreateZipStreamStep_Successfully()
        {
            // arrange
            var logger = new Mock<ILogger<CreateNewZipArchiveStep>>().Object;
            var options = new CreateNewZipArchiveOptions();
            var next = new Mock<IPipelineRequest>().Object;

            // act
            await using (var result = new CreateNewZipArchiveStep(logger, options, next))
            {
                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(logger, result.Logger);
                Assert.AreEqual(next, result.Next);
            }
        }

        [TestMethod]
        public async Task Returns_Completed_Task_From_InvokeAsync()
        {
            // arrange
            var logger = new Mock<ILogger<CreateNewZipArchiveStep>>().Object;
            var options = new CreateNewZipArchiveOptions();
            var context = new PipelineContext();

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Callback(() =>
                                    {
                                        Assert.IsTrue(context.Items.ContainsKey(options.OutputStreamContextName), "OutputStream should exist.");
                                        Assert.IsTrue(context.Items.ContainsKey(options.ZipArchiveContextName), "ZipArchive should exist.");
                                        Assert.IsInstanceOfType(context.Items[options.ZipArchiveContextName], typeof(ZipArchive));

                                        (context.Items[options.ZipArchiveContextName] as ZipArchive).Dispose();
                                        int expectedEntryCount = 0;

                                        using (var outputStream = context.Items[options.OutputStreamContextName] as Stream)
                                        {
                                            // assert that ZipArchive is usable and the update was accepted.
                                            outputStream.Position = 0;
                                            using (var assertArchive = new ZipArchive(outputStream))
                                            {
                                                Assert.AreEqual(expectedEntryCount, assertArchive.Entries.Count);
                                            }
                                        }
                                    })
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;

            await using (var source = new CreateNewZipArchiveStep(logger, options, next))
            {
                // act
                await source.InvokeAsync(context).ConfigureAwait(false);

                // assert
                if (context.Errors.Any())
                {
                    throw context.Errors[0];
                }

                Assert.IsFalse(context.Items.ContainsKey(options.OutputStreamContextName), "OutputStream should not exist.");
                Assert.IsFalse(context.Items.ContainsKey(options.ZipArchiveContextName), "ZipArchive should not exist.");
            }
        }

        [TestMethod]
        public async Task Returns_Completed_Task_From_InvokeAsync_When_Options_Uses_Different_ContextNames()
        {
            // arrange
            var logger = new Mock<ILogger<CreateNewZipArchiveStep>>().Object;
            var options = new CreateNewZipArchiveOptions()
            {
                OutputStreamContextName = "OUT_PUT_STREAM_TEST_ING",
                ZipArchiveContextName = "ZIP_ARCHIVE_TEST_ING",
            };
            var context = new PipelineContext();

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Callback(() =>
                                    {
                                        Assert.IsTrue(context.Items.ContainsKey(options.OutputStreamContextName), "OutputStream should exist.");
                                        Assert.IsTrue(context.Items.ContainsKey(options.ZipArchiveContextName), "ZipArchive should exist.");
                                        Assert.IsInstanceOfType(context.Items[options.ZipArchiveContextName], typeof(ZipArchive));

                                        (context.Items[options.ZipArchiveContextName] as ZipArchive).Dispose();
                                        int expectedEntryCount = 0;

                                        using (var outputStream = context.Items[options.OutputStreamContextName] as Stream)
                                        {
                                            // assert that ZipArchive is usable and the update was accepted.
                                            outputStream.Position = 0;
                                            using (var assertArchive = new ZipArchive(outputStream))
                                            {
                                                Assert.AreEqual(expectedEntryCount, assertArchive.Entries.Count);
                                            }
                                        }
                                    })
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;

            await using (var source = new CreateNewZipArchiveStep(logger, options, next))
            {
                // act
                await source.InvokeAsync(context).ConfigureAwait(false);

                // assert
                if (context.Errors.Any())
                {
                    throw context.Errors[0];
                }

                Assert.IsFalse(context.Items.ContainsKey(options.OutputStreamContextName), "OutputStream should not exist.");
                Assert.IsFalse(context.Items.ContainsKey(options.ZipArchiveContextName), "ZipArchive should not exist.");
            }
        }

        [TestMethod]
        public async Task Returns_Previous_ZipArchive_From_InvokeAsync_When_Previous_ZipArchive_Is_Created()
        {
            // arrange
            var logger = new Mock<ILogger<CreateNewZipArchiveStep>>().Object;
            var options = new CreateNewZipArchiveOptions();
            var context = new PipelineContext();

            string zipFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}Disney.zip";

            await using (var previousOutputStream = File.OpenRead(zipFilePath))
            {
                using (var previousZipArchive = new ZipArchive(previousOutputStream, ZipArchiveMode.Read, leaveOpen: true))
                {
                    var mockNext = new Mock<IPipelineRequest>();
                    mockNext.Setup(x => x.InvokeAsync(context))
                                    .Callback(() =>
                                    {
                                        Assert.IsTrue(context.Items.ContainsKey(options.OutputStreamContextName), "OutputStream should exist.");
                                        Assert.IsTrue(context.Items.ContainsKey(options.ZipArchiveContextName), "ZipArchive should exist.");
                                        Assert.IsInstanceOfType(context.Items[options.ZipArchiveContextName], typeof(ZipArchive));

                                        (context.Items[options.ZipArchiveContextName] as ZipArchive).Dispose();
                                        int expectedEntryCount = 0;

                                        using (var outputStream = context.Items[options.OutputStreamContextName] as Stream)
                                        {
                                            // assert that ZipArchive is usable and the update was accepted.
                                            outputStream.Position = 0;
                                            using (var assertArchive = new ZipArchive(outputStream))
                                            {
                                                Assert.AreEqual(expectedEntryCount, assertArchive.Entries.Count);
                                            }
                                        }
                                    })
                                    .Returns(Task.CompletedTask);
                    var next = mockNext.Object;

                    await using (var source = new CreateNewZipArchiveStep(logger, options, next))
                    {
                        context.Items.Add(options.OutputStreamContextName, previousOutputStream);
                        context.Items.Add(options.ZipArchiveContextName, previousZipArchive);

                        // act
                        await source.InvokeAsync(context).ConfigureAwait(false);

                        // assert
                        if (context.Errors.Any())
                        {
                            throw context.Errors[0];
                        }

                        Assert.IsTrue(context.Items.ContainsKey(options.OutputStreamContextName), "OutputStream should exist.");
                        Assert.IsTrue(previousOutputStream.CanRead, "OutputStream should NOT be closed.");
                        Assert.AreEqual(previousOutputStream, context.Items[options.OutputStreamContextName]);
                        Assert.IsTrue(context.Items.ContainsKey(options.ZipArchiveContextName), "ZipArchive should exist.");
                        Assert.AreEqual(previousZipArchive, context.Items[options.ZipArchiveContextName]);
                    }
                }
            }
        }

        [TestMethod]
        public async Task Returns_Previous_ZipArchive_From_InvokeAsync_When_Current_ZipArchive_Was_Removed_By_Next_Step()
        {
            // arrange
            var logger = new Mock<ILogger<CreateNewZipArchiveStep>>().Object;
            var options = new CreateNewZipArchiveOptions();
            var context = new PipelineContext();

            string zipFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}Disney.zip";

            await using (var previousOutputStream = File.OpenRead(zipFilePath))
            {
                using (var previousZipArchive = new ZipArchive(previousOutputStream, ZipArchiveMode.Read, leaveOpen: true))
                {
                    var next = new RemoveItemFromContextStep(options.ZipArchiveContextName);

                    await using (var source = new CreateNewZipArchiveStep(logger, options, next))
                    {
                        context.Items.Add(options.OutputStreamContextName, previousOutputStream);
                        context.Items.Add(options.ZipArchiveContextName, previousZipArchive);

                        // act
                        await source.InvokeAsync(context).ConfigureAwait(false);

                        // assert
                        if (context.Errors.Any())
                        {
                            throw context.Errors[0];
                        }

                        Assert.IsTrue(context.Items.ContainsKey(options.OutputStreamContextName), "OutputStream should exist.");
                        Assert.IsTrue(previousOutputStream.CanRead, "OutputStream should NOT be closed.");
                        Assert.AreEqual(previousOutputStream, context.Items[options.OutputStreamContextName]);
                        Assert.IsTrue(context.Items.ContainsKey(options.ZipArchiveContextName), "ZipArchive should exist.");
                        Assert.AreEqual(previousZipArchive, context.Items[options.ZipArchiveContextName]);
                    }
                }
            }
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Logger_Is_Null()
        {
            // arrange
            ILogger<CreateNewZipArchiveStep> logger = null;
            var options = new CreateNewZipArchiveOptions();
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            string expectedParamName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new CreateNewZipArchiveStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Next_Is_Null()
        {
            // arrange
            var logger = new Mock<ILogger<CreateNewZipArchiveStep>>().Object;
            var options = new CreateNewZipArchiveOptions();
            IPipelineRequest next = null;

            string expectedParamName = nameof(next);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new CreateNewZipArchiveStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Options_Is_Null()
        {
            // arrange
            var logger = new Mock<ILogger<CreateNewZipArchiveStep>>().Object;
            CreateNewZipArchiveOptions options = null;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            string expectedParamName = nameof(options);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new CreateNewZipArchiveStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }
    }
}