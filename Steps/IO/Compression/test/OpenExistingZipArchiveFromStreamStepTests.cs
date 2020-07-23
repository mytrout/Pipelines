// <copyright file="OpenExistingZipArchiveFromStreamStepTests.cs" company="Chris Trout">
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
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines.Core;
    using MyTrout.Pipelines.Steps;
    using MyTrout.Pipelines.Steps.IO.Compression;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage]
    [TestClass]
    public class OpenExistingZipArchiveFromStreamStepTests
    {
        [TestMethod]
        public async Task Constructs_OpenExistingZipArchiveFromStreamStep_Successfully()
        {
            // arrange
            ILogger<OpenExistingZipArchiveFromStreamStep> logger = new Mock<ILogger<OpenExistingZipArchiveFromStreamStep>>().Object;
            OpenExistingZipArchiveFromStreamOptions options = new OpenExistingZipArchiveFromStreamOptions();
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            // act
            await using (var result = new OpenExistingZipArchiveFromStreamStep(logger, options, next))
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
            PipelineContext context = new PipelineContext();

            var logger = new Mock<ILogger<OpenExistingZipArchiveFromStreamStep>>().Object;

            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockNext.Object;

            OpenExistingZipArchiveFromStreamOptions options = new OpenExistingZipArchiveFromStreamOptions();

            string zipFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}Disney.zip";
            string entryName = "Disney.txt";
            string entryContents = "Steamboat Willie";
            int expectedEntryCount = 1;

            using (var inputStream = new MemoryStream())
            {
                using (var fileStream = File.OpenRead(zipFilePath))
                {
                    fileStream.CopyTo(inputStream);
                }

                await using (var source = new OpenExistingZipArchiveFromStreamStep(logger, options, next))
                {
                    context.Items.Add(PipelineContextConstants.INPUT_STREAM, inputStream);

                    // act
                    await source.InvokeAsync(context).ConfigureAwait(false);

                    // assert
                    if (context.Errors.Any())
                    {
                        throw context.Errors[0];
                    }

                    Assert.IsFalse(context.Items.ContainsKey(CompressionConstants.ZIP_ARCHIVE), "ZipArchive should not exist.");
                    Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_STREAM), "OutputStream should exist.");
                    Assert.IsInstanceOfType(context.Items[PipelineContextConstants.OUTPUT_STREAM], typeof(Stream));

                    using (var outputStream = context.Items[PipelineContextConstants.OUTPUT_STREAM] as Stream)
                    {
                        outputStream.Position = 0;
                        using (var assertArchive = new ZipArchive(outputStream, ZipArchiveMode.Read, leaveOpen: true))
                        {
                            Assert.AreEqual(expectedEntryCount, assertArchive.Entries.Count);
                            var assertEntry = assertArchive.GetEntry(entryName);
                            using (var assertEntryStream = assertEntry.Open())
                            {
                                using (var reader = new StreamReader(assertEntryStream, leaveOpen: true))
                                {
                                    Assert.AreEqual(entryContents, await reader.ReadToEndAsync().ConfigureAwait(false));
                                }
                            }
                        }
                    }
                }
            }
        }

        [TestMethod]
        public async Task Returns_OutputStream_From_InvokeAsync_When_Previous_OutputStream_Is_Disposed()
        {
            // arrange
            PipelineContext context = new PipelineContext();

            var logger = new Mock<ILogger<OpenExistingZipArchiveFromStreamStep>>().Object;

            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockNext.Object;

            OpenExistingZipArchiveFromStreamOptions options = new OpenExistingZipArchiveFromStreamOptions();

            string zipFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}Disney.zip";

            using (var previousOutputStream = new MemoryStream())
            {
                using (var inputStream = new MemoryStream())
                {
                    using (var fileStream = File.OpenRead(zipFilePath))
                    {
                        fileStream.CopyTo(inputStream);
                        fileStream.CopyTo(previousOutputStream);
                    }

                    await using (var source = new OpenExistingZipArchiveFromStreamStep(logger, options, next))
                    {
                        context.Items.Add(PipelineContextConstants.INPUT_STREAM, inputStream);
                        context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, previousOutputStream);

                        // act
                        await source.InvokeAsync(context).ConfigureAwait(false);

                        // assert
                        if (context.Errors.Any())
                        {
                            throw context.Errors[0];
                        }

                        Assert.IsFalse(context.Items.ContainsKey(CompressionConstants.ZIP_ARCHIVE), "ZipArchive should not exist.");
                        Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_STREAM), "OutputStream should exist.");
                        Assert.IsInstanceOfType(context.Items[PipelineContextConstants.OUTPUT_STREAM], typeof(Stream));
                        Assert.AreNotEqual(previousOutputStream, context.Items[PipelineContextConstants.OUTPUT_STREAM]);
                        Assert.IsFalse(previousOutputStream.CanRead, "Previous OutputStream should be closed.");
                    }
                }
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_Context_Lacks_InputStream()
        {
            // arrange
            PipelineContext context = new PipelineContext();

            var logger = new Mock<ILogger<OpenExistingZipArchiveFromStreamStep>>().Object;

            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockNext.Object;

            OpenExistingZipArchiveFromStreamOptions options = new OpenExistingZipArchiveFromStreamOptions();

            int errorCount = 1;
            string expectedMessage = Pipelines.Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, PipelineContextConstants.INPUT_STREAM);

            await using (var source = new OpenExistingZipArchiveFromStreamStep(logger, options, next))
            {
                // act
                await source.InvokeAsync(context).ConfigureAwait(false);

                // assert
                Assert.AreEqual(errorCount, context.Errors.Count);
                Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
                Assert.AreEqual(expectedMessage, context.Errors[0].Message);
            }
        }

        [TestMethod]
        public async Task Throws_ArgumentNullException_From_Constructor_When_Logger_Is_Null()
        {
            // arrange
            ILogger<OpenExistingZipArchiveFromStreamStep> logger = null;
            OpenExistingZipArchiveFromStreamOptions options = new OpenExistingZipArchiveFromStreamOptions();
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            string expectedParamName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new OpenExistingZipArchiveFromStreamStep(logger, options, next));

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
            ILogger<OpenExistingZipArchiveFromStreamStep> logger = new Mock<ILogger<OpenExistingZipArchiveFromStreamStep>>().Object;
            OpenExistingZipArchiveFromStreamOptions options = new OpenExistingZipArchiveFromStreamOptions();
            IPipelineRequest next = null;

            string expectedParamName = nameof(next);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new OpenExistingZipArchiveFromStreamStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Options_Is_Null()
        {
            // arrange
            ILogger<OpenExistingZipArchiveFromStreamStep> logger = new Mock<ILogger<OpenExistingZipArchiveFromStreamStep>>().Object;
            OpenExistingZipArchiveFromStreamOptions options = null;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            string expectedParamName = nameof(options);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new OpenExistingZipArchiveFromStreamStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }
    }
}
