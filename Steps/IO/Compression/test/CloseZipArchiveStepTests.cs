// <copyright file="CloseZipArchiveStepTests.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2022 Chris Trout
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
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage]
    [TestClass]
    public class CloseZipArchiveStepTests
    {
        [TestMethod]
        public async Task Constructs_CloseZipArchiveStep_Successfully()
        {
            // arrange
            var logger = new Mock<ILogger<CloseZipArchiveStep>>().Object;
            var options = new CloseZipArchiveOptions();
            var next = new Mock<IPipelineRequest>().Object;

            // act
            await using (var result = new CloseZipArchiveStep(logger, options, next))
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
            var context = new PipelineContext();

            var logger = new Mock<ILogger<CloseZipArchiveStep>>().Object;
            var options = new CloseZipArchiveOptions();
            var next = new Mock<IPipelineRequest>().Object;

            int expectedErrorCount = 1;
            string expectedMessage = Pipelines.Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, options.OutputStreamContextName);

            using (var stream = new MemoryStream())
            {
                // Not adding OUTPUT_STREAM to context.Items here.
                using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
                {
                    context.Items.Add(options.ZipArchiveContextName, zipArchive);

                    using (var source = new CloseZipArchiveStep(logger, options, next))
                    {
                        // act
                        await source.InvokeAsync(context).ConfigureAwait(false);
                    }
                }
            }

            // assert
            Assert.AreEqual(expectedErrorCount, context.Errors.Count);
            Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
            Assert.AreEqual(expectedMessage, context.Errors[0].Message);
        }

        [TestMethod]
        public async Task Returns_Pipeline_Error_From_InvokeAsync_When_Context_Lacks_ZipArchive()
        {
            // arrange
            var context = new PipelineContext();

            var logger = new Mock<ILogger<CloseZipArchiveStep>>().Object;
            var options = new CloseZipArchiveOptions();
            var next = new Mock<IPipelineRequest>().Object;

            int expectedErrorCount = 1;
            string expectedMessage = Pipelines.Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, options.ZipArchiveContextName);

            await using (var source = new CloseZipArchiveStep(logger, options, next))
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
        public async Task Returns_Pipeline_Error_From_InvokeAsync_When_Context_OutputStream_Is_Wrong_Type()
        {
            // arrange
            var context = new PipelineContext();

            var logger = new Mock<ILogger<CloseZipArchiveStep>>().Object;
            var options = new CloseZipArchiveOptions();
            var next = new Mock<IPipelineRequest>().Object;

            int expectedErrorCount = 1;
            string expectedMessage = Pipelines.Resources.CONTEXT_VALUE_NOT_EXPECTED_TYPE(CultureInfo.CurrentCulture, options.OutputStreamContextName, typeof(Stream));

            using (var stream = new MemoryStream())
            {
                context.Items.Add(options.OutputStreamContextName, "This is not a stream object.");

                using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
                {
                    context.Items.Add(options.ZipArchiveContextName, zipArchive);

                    using (var source = new CloseZipArchiveStep(logger, options, next))
                    {
                        // act
                        await source.InvokeAsync(context).ConfigureAwait(false);
                    }
                }
            }

            // assert
            Assert.AreEqual(expectedErrorCount, context.Errors.Count);
            Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
            Assert.AreEqual(expectedMessage, context.Errors[0].Message);
        }

        [TestMethod]
        public async Task Returns_Pipeline_Error_From_InvokeAsync_When_Context_ZipArchive_Is_Wrong_Type()
        {
            // arrange
            var context = new PipelineContext();

            var logger = new Mock<ILogger<CloseZipArchiveStep>>().Object;
            var options = new CloseZipArchiveOptions();
            var next = new Mock<IPipelineRequest>().Object;

            int expectedErrorCount = 1;
            string expectedMessage = Pipelines.Resources.CONTEXT_VALUE_NOT_EXPECTED_TYPE(CultureInfo.CurrentCulture, options.ZipArchiveContextName, typeof(ZipArchive));

            context.Items.Add(options.ZipArchiveContextName, "This is not a ZipArchive object.");

            await using (var source = new CloseZipArchiveStep(logger, options, next))
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
        public async Task Returns_Successfully_From_InvokeAsync_When_ZipArchive_Closes()
        {
            // arrange
            var context = new PipelineContext();
            var options = new CloseZipArchiveOptions();
            var logger = new Mock<ILogger<CloseZipArchiveStep>>().Object;

            var next = new Mock<IPipelineRequest>().Object;

            int expectedErrorCount = 0;
            int expectedEntryCount = 1;

            using (var stream = new MemoryStream())
            {
                context.Items.Add(options.OutputStreamContextName, stream);

                using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
                {
                    var archiveEntry = zipArchive.CreateEntry("disney.txt");

                    context.Items.Add(options.ZipArchiveContextName, zipArchive);

                    using (var source = new CloseZipArchiveStep(logger, options, next))
                    {
                        // act
                        await source.InvokeAsync(context).ConfigureAwait(false);
                    }
                }

                // assert
                Assert.AreEqual(expectedErrorCount, context.Errors.Count);

                using (var outputStream = context.Items[options.OutputStreamContextName] as Stream)
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
        public async Task Returns_Successfully_From_InvokeAsync_When_Options_Uses_Different_ContextNames()
        {
            // arrange
            var context = new PipelineContext();
            var options = new CloseZipArchiveOptions()
            {
                OutputStreamContextName = "OUTPUT_STREAM_TESTING",
                ZipArchiveContextName = "ZIP_ARCHIVE_TESTING"
            };

            var logger = new Mock<ILogger<CloseZipArchiveStep>>().Object;

            var next = new Mock<IPipelineRequest>().Object;

            int expectedErrorCount = 0;
            int expectedEntryCount = 1;

            using (var stream = new MemoryStream())
            {
                context.Items.Add(options.OutputStreamContextName, stream);

                using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
                {
                    var archiveEntry = zipArchive.CreateEntry("disney.txt");

                    context.Items.Add(options.ZipArchiveContextName, zipArchive);

                    using (var source = new CloseZipArchiveStep(logger, options, next))
                    {
                        // act
                        await source.InvokeAsync(context).ConfigureAwait(false);
                    }
                }

                // assert
                Assert.AreEqual(expectedErrorCount, context.Errors.Count);

                using (var outputStream = context.Items[options.OutputStreamContextName] as Stream)
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
        public void Throws_ArgumentNullException_From_Constructor_When_Logger_Is_Null()
        {
            // arrange
            ILogger<CloseZipArchiveStep> logger = null;
            var options = new CloseZipArchiveOptions();
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            string expectedParamName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new CloseZipArchiveStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Next_Is_Null()
        {
            // arrange
            ILogger<CloseZipArchiveStep> logger = new Mock<ILogger<CloseZipArchiveStep>>().Object;
            var options = new CloseZipArchiveOptions();
            IPipelineRequest next = null;

            string expectedParamName = nameof(next);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new CloseZipArchiveStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Options_Is_Null()
        {
            // arrange
            ILogger<CloseZipArchiveStep> logger = new Mock<ILogger<CloseZipArchiveStep>>().Object;
            CloseZipArchiveOptions options = null;
            IPipelineRequest next = null;

            string expectedParamName = nameof(options);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new CloseZipArchiveStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }
    }
}