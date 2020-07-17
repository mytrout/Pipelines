// <copyright file="UpdateZipArchiveEntryStepTests.cs" company="Chris Trout">
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
    using MyTrout.Pipelines.Steps.IO.Compression;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage]
    [TestClass]
    public class UpdateZipArchiveEntryStepTests
    {
        [TestMethod]
        public async Task Constructs_UpdateZipArchiveEntryStep_Successfully()
        {
            // arrange
            var logger = new Mock<ILogger<UpdateZipArchiveEntryStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;

            // act
            await using (var result = new UpdateZipArchiveEntryStep(logger, next))
            {
                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(logger, result.Logger);
                Assert.AreEqual(next, result.Next);
            }
        }

        [TestMethod]
        public async Task Returns_Pipeline_Error_From_InvokeAsync_When_Context_Lacks_OutputStream()
        {
            // arrange
            PipelineContext context = new PipelineContext();

            var logger = new Mock<ILogger<UpdateZipArchiveEntryStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;

            string entryName = "Disney.txt";
            string entryContents = "Why are you still whining?";
            string zipFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}Disney.zip";

            int expectedErrorCount = 1;
            string expectedMessage = Pipelines.Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, PipelineContextConstants.OUTPUT_STREAM);

            using (var zipStream = new MemoryStream())
            {
                using (var fileStream = File.OpenRead(zipFilePath))
                {
                    await fileStream.CopyToAsync(zipStream).ConfigureAwait(false);
                }

                using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Update, true))
                {
                    using (var entryStream = new MemoryStream(Encoding.UTF8.GetBytes(entryContents)))
                    {
                        context.Items.Add(CompressionConstants.ZIP_ARCHIVE, zipArchive);
                        context.Items.Add(CompressionConstants.ZIP_ARCHIVE_ENTRY_NAME, entryName);

                        await using (var source = new UpdateZipArchiveEntryStep(logger, next))
                        {
                            // act
                            await source.InvokeAsync(context).ConfigureAwait(false);
                        }

                        // assert
                        Assert.AreEqual(expectedErrorCount, context.Errors.Count);
                        Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
                        Assert.AreEqual(expectedMessage, context.Errors[0].Message);
                    }
                }
            }
        }

        [TestMethod]
        public async Task Returns_Pipeline_Error_From_InvokeAsync_When_Context_Lacks_ZipArchive()
        {
            // arrange
            PipelineContext context = new PipelineContext();

            var logger = new Mock<ILogger<UpdateZipArchiveEntryStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;

            string entryName = "Disney.txt";
            string entryContents = "Why are you still whining?";
            string zipFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}Disney.zip";

            int expectedErrorCount = 1;
            string expectedMessage = Pipelines.Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, CompressionConstants.ZIP_ARCHIVE);

            using (var zipStream = new MemoryStream())
            {
                using (var fileStream = File.OpenRead(zipFilePath))
                {
                    await fileStream.CopyToAsync(zipStream).ConfigureAwait(false);
                }

                using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Update, true))
                {
                    using (var entryStream = new MemoryStream(Encoding.UTF8.GetBytes(entryContents)))
                    {
                        // context.Items.Add(CompressionConstants.ZIP_ARCHIVE, zipArchive);
                        context.Items.Add(CompressionConstants.ZIP_ARCHIVE_ENTRY_NAME, entryName);
                        context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, entryStream);

                        await using (var source = new UpdateZipArchiveEntryStep(logger, next))
                        {
                            // act
                            await source.InvokeAsync(context).ConfigureAwait(false);
                        }

                        // assert
                        Assert.AreEqual(expectedErrorCount, context.Errors.Count);
                        Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
                        Assert.AreEqual(expectedMessage, context.Errors[0].Message);
                    }
                }
            }
        }

        [TestMethod]
        public async Task Returns_Pipeline_Error_From_InvokeAsync_When_Context_Lacks_ZipArchiveEntryName()
        {
            // arrange
            PipelineContext context = new PipelineContext();

            var logger = new Mock<ILogger<UpdateZipArchiveEntryStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;

            string entryContents = "Why are you still whining?";
            string zipFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}Disney.zip";

            int expectedErrorCount = 1;
            string expectedMessage = Pipelines.Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, CompressionConstants.ZIP_ARCHIVE_ENTRY_NAME);

            using (var zipStream = new MemoryStream())
            {
                using (var fileStream = File.OpenRead(zipFilePath))
                {
                    await fileStream.CopyToAsync(zipStream).ConfigureAwait(false);
                }

                using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Update, true))
                {
                    using (var entryStream = new MemoryStream(Encoding.UTF8.GetBytes(entryContents)))
                    {
                        context.Items.Add(CompressionConstants.ZIP_ARCHIVE, zipArchive);
                        context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, entryStream);

                        await using (var source = new UpdateZipArchiveEntryStep(logger, next))
                        {
                            // act
                            await source.InvokeAsync(context).ConfigureAwait(false);
                        }

                        // assert
                        Assert.AreEqual(expectedErrorCount, context.Errors.Count);
                        Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
                        Assert.AreEqual(expectedMessage, context.Errors[0].Message);
                    }
                }
            }
        }

        [TestMethod]
        public async Task Returns_ZipArchive_From_InvokeAsync_When_ZipArchiveEntry_Is_Updated()
        {
            // arrange
            PipelineContext context = new PipelineContext();

            ILogger<UpdateZipArchiveEntryStep> logger = new Mock<ILogger<UpdateZipArchiveEntryStep>>().Object;

            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockNext.Object;

            string entryName = "Disney.txt";
            string entryContents = "Why are you still whining?";
            string zipFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}Disney.zip";
            int expectedEntryCount = 1;

            using (var zipStream = new MemoryStream())
            {
                using (var fileStream = File.OpenRead(zipFilePath))
                {
                    await fileStream.CopyToAsync(zipStream).ConfigureAwait(false);
                }

                using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Update, true))
                {
                    using (var entryStream = new MemoryStream(Encoding.UTF8.GetBytes(entryContents)))
                    {
                        context.Items.Add(CompressionConstants.ZIP_ARCHIVE, zipArchive);
                        context.Items.Add(CompressionConstants.ZIP_ARCHIVE_ENTRY_NAME, entryName);
                        context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, entryStream);

                        await using (var source = new UpdateZipArchiveEntryStep(logger, next))
                        {
                            // act
                            await source.InvokeAsync(context).ConfigureAwait(false);
                        }

                        // assert
                        if (context.Errors.Any())
                        {
                            throw context.Errors[0];
                        }

                        Assert.IsTrue(context.Items.ContainsKey(CompressionConstants.ZIP_ARCHIVE), "ZipArchive should exist in context.");
                        Assert.AreEqual(zipArchive, context.Items[CompressionConstants.ZIP_ARCHIVE]);
                        Assert.IsTrue(context.Items.ContainsKey(CompressionConstants.ZIP_ARCHIVE_ENTRY_NAME), "ZipArchiveEntryName should exist in context.");
                        Assert.AreEqual(entryName, context.Items[CompressionConstants.ZIP_ARCHIVE_ENTRY_NAME]);
                        Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_STREAM), "OutputStream should exist in context.");
                        Assert.IsTrue((context.Items[PipelineContextConstants.OUTPUT_STREAM] as Stream).CanRead, "OutputStream should not be closed.");
                    }
                }

                // assert that ZipArchive is usable and the update was accepted.
                zipStream.Position = 0;
                using (var assertArchive = new ZipArchive(zipStream))
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

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Logger_Is_Null()
        {
            // arrange
            ILogger<UpdateZipArchiveEntryStep> logger = null;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            string expectedParamName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new UpdateZipArchiveEntryStep(logger, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Next_Is_Null()
        {
            // arrange
            ILogger<UpdateZipArchiveEntryStep> logger = new Mock<ILogger<UpdateZipArchiveEntryStep>>().Object;
            IPipelineRequest next = null;

            string expectedParamName = nameof(next);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new UpdateZipArchiveEntryStep(logger, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public async Task Throws_InvalidOperationException_From_InvokeAsync_When_ZipArchive_Is_ReadOnly()
        {
            // arrange
            ILogger<UpdateZipArchiveEntryStep> logger = new Mock<ILogger<UpdateZipArchiveEntryStep>>().Object;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            PipelineContext context = new PipelineContext();

            string entryName = "Disney.txt";
            string entryContents = "Why are you still whining?";
            string zipFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}Disney.zip";

            int errorCount = 1;
            string expectedMessage = Resources.ZIP_ARCHIVE_IS_READ_ONLY(CultureInfo.CurrentCulture);

            using (var zipStream = new MemoryStream())
            {
                using (var fileStream = File.OpenRead(zipFilePath))
                {
                    await fileStream.CopyToAsync(zipStream).ConfigureAwait(false);
                }

                using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read, true))
                {
                    using (var entryStream = new MemoryStream(Encoding.UTF8.GetBytes(entryContents)))
                    {
                        context.Items.Add(CompressionConstants.ZIP_ARCHIVE, zipArchive);
                        context.Items.Add(CompressionConstants.ZIP_ARCHIVE_ENTRY_NAME, entryName);
                        context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, entryStream);

                        await using (var source = new UpdateZipArchiveEntryStep(logger, next))
                        {
                            // act
                            await source.InvokeAsync(context).ConfigureAwait(false);
                        }

                        // assert
                        Assert.AreEqual(errorCount, context.Errors.Count);
                        Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
                        Assert.AreEqual(expectedMessage, context.Errors[0].Message);
                    }
                }
            }
        }
    }
}
