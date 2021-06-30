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
    using MyTrout.Pipelines.Steps;
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
            var next = new Mock<IPipelineRequest>().Object;

            // act
            await using (var result = new CreateNewZipArchiveStep(logger, next))
            {
                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(logger, result.Logger);
                Assert.AreEqual(next, result.Next);
            }
        }

        [TestMethod]
        public async Task Returns_OutputStream_From_InvokeAsync()
        {
            // arrange
            var logger = new Mock<ILogger<CreateNewZipArchiveStep>>().Object;

            var context = new PipelineContext();

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Callback(() =>
                                    {
                                        Assert.IsFalse(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_STREAM), "OutputStream should not exist.");
                                        Assert.IsTrue(context.Items.ContainsKey(CompressionConstants.ZIP_ARCHIVE), "ZipArchive should exist.");
                                        Assert.IsInstanceOfType(context.Items[CompressionConstants.ZIP_ARCHIVE], typeof(ZipArchive));
                                    })
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;

            int expectedEntryCount = 0;

            await using (var source = new CreateNewZipArchiveStep(logger, next))
            {
                // act
                await source.InvokeAsync(context).ConfigureAwait(false);

                // assert
                if (context.Errors.Any())
                {
                    throw context.Errors[0];
                }

                Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_STREAM), "OutputStream should exist.");
                Assert.IsFalse(context.Items.ContainsKey(CompressionConstants.ZIP_ARCHIVE), "ZipArchive should not exist.");

                using (var outputStream = context.Items[PipelineContextConstants.OUTPUT_STREAM] as Stream)
                {
                    // assert that ZipArchive is usable and the update was accepted.
                    outputStream.Position = 0;
                    using (var assertArchive = new ZipArchive(outputStream))
                    {
                        Assert.AreEqual(expectedEntryCount, assertArchive.Entries.Count);
                    }
                }
            }
        }

        [TestMethod]
        public async Task Returns_Previous_ZipArchive_From_InvokeAsync_When_Previous_ZipArchive_Is_Created()
        {
            // arrange
            var logger = new Mock<ILogger<CreateNewZipArchiveStep>>().Object;

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
                                                Assert.IsFalse(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_STREAM), "OutputStream should not exist.");
                                                Assert.IsTrue(context.Items.ContainsKey(CompressionConstants.ZIP_ARCHIVE), "ZipArchive should exist.");
                                                Assert.IsInstanceOfType(context.Items[CompressionConstants.ZIP_ARCHIVE], typeof(ZipArchive));
                                                Assert.AreNotEqual(previousZipArchive, context.Items[CompressionConstants.ZIP_ARCHIVE]);
                                            })
                                            .Returns(Task.CompletedTask);
                    var next = mockNext.Object;

                    await using (var source = new CreateNewZipArchiveStep(logger, next))
                    {
                        context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, previousOutputStream);
                        context.Items.Add(CompressionConstants.ZIP_ARCHIVE, previousZipArchive);

                        // act
                        await source.InvokeAsync(context).ConfigureAwait(false);

                        // assert
                        if (context.Errors.Any())
                        {
                            throw context.Errors[0];
                        }

                        Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_STREAM), "OutputStream should exist.");
                        Assert.IsFalse(previousOutputStream.CanRead, "OutputStream should be closed.");
                        Assert.AreNotEqual(previousOutputStream, context.Items[PipelineContextConstants.OUTPUT_STREAM]);
                        Assert.IsTrue(context.Items.ContainsKey(CompressionConstants.ZIP_ARCHIVE), "ZipArchive should exist.");
                        Assert.AreEqual(previousZipArchive, context.Items[CompressionConstants.ZIP_ARCHIVE]);
                    }
                }
            }
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Logger_Is_Null()
        {
            // arrange
            ILogger<CreateNewZipArchiveStep> logger = null;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            string expectedParamName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new CreateNewZipArchiveStep(logger, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Next_Is_Null()
        {
            // arrange
            var logger = new Mock<ILogger<CreateNewZipArchiveStep>>().Object;
            IPipelineRequest next = null;

            string expectedParamName = nameof(next);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new CreateNewZipArchiveStep(logger, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }
    }
}