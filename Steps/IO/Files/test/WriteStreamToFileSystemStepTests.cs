// <copyright file="WriteStreamToFileSystemStepTests.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2019-2020 Chris Trout
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

namespace MyTrout.Pipelines.Steps.IO.Files.Tests
{
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage]
    [TestClass]
    public class WriteStreamToFileSystemStepTests
    {
        [TestMethod]
        public async Task Constructs_WriteStreamToFileSystemTests_Successfully()
        {
            // arrange
            string outputFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";

            var logger = new Mock<ILogger<WriteStreamToFileSystemStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;
            var options = new WriteStreamToFileSystemOptions()
            {
                WriteFileBaseDirectory = outputFilePath
            };

            // act
            var result = new WriteStreamToFileSystemStep(logger, next, options);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(logger, result.Logger);
            Assert.AreEqual(options, result.Options);

            // cleanup
            await result.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Writes_File_From_InvokeAsync()
        {
            // arrange
            string outputFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string contents = "What is the text here?";
            byte[] body = Encoding.UTF8.GetBytes(contents);

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullPathAndFileName = outputFilePath + fileName;

            using (var stream = new MemoryStream(body))
            {
                PipelineContext context = new PipelineContext();
                context.Items.Add(FileConstants.TARGET_FILE, fileName);
                context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, stream);

                var logger = new Mock<ILogger<WriteStreamToFileSystemStep>>().Object;

                Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
                var next = mockNext.Object;

                WriteStreamToFileSystemOptions options = new WriteStreamToFileSystemOptions()
                {
                    WriteFileBaseDirectory = outputFilePath
                };

                var source = new WriteStreamToFileSystemStep(logger, next, options);

                // act
                await source.InvokeAsync(context).ConfigureAwait(false);

                // assert
                Assert.IsTrue(File.Exists(fullPathAndFileName));
                Assert.IsTrue((context.Items[PipelineContextConstants.OUTPUT_STREAM] as Stream).CanRead, "The stream should be open.");
                Assert.AreEqual(contents, File.ReadAllText(fullPathAndFileName));

                // cleanup
                await source.DisposeAsync().ConfigureAwait(false);
                File.Delete(fullPathAndFileName);
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_FileName_Is_Not_In_The_Context()
        {
            // arrange
            string outputFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string contents = "What is the text here?";
            byte[] body = Encoding.UTF8.GetBytes(contents);

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullPathAndFileName = outputFilePath + fileName;

            using (var stream = new MemoryStream(body))
            {
                PipelineContext context = new PipelineContext();
                context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, stream);

                var logger = new Mock<ILogger<WriteStreamToFileSystemStep>>().Object;

                Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
                var next = mockNext.Object;

                WriteStreamToFileSystemOptions options = new WriteStreamToFileSystemOptions()
                {
                    WriteFileBaseDirectory = outputFilePath
                };

                var source = new WriteStreamToFileSystemStep(logger, next, options);

                int errorCount = 1;

                string expectedMessage = Pipelines.Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, FileConstants.TARGET_FILE);

                // act
                await source.InvokeAsync(context).ConfigureAwait(false);

                // assert
                Assert.AreEqual(errorCount, context.Errors.Count);
                Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
                Assert.AreEqual(expectedMessage, context.Errors[0].Message);

                // cleanup
                await source.DisposeAsync().ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_FileName_Is_Empty()
        {
            // arrange
            string outputFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string contents = "What is the text here?";
            byte[] body = Encoding.UTF8.GetBytes(contents);

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullPathAndFileName = outputFilePath + fileName;

            using (var stream = new MemoryStream(body))
            {
                PipelineContext context = new PipelineContext();
                context.Items.Add(FileConstants.TARGET_FILE, string.Empty);
                context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, stream);

                var logger = new Mock<ILogger<WriteStreamToFileSystemStep>>().Object;

                Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
                var next = mockNext.Object;

                WriteStreamToFileSystemOptions options = new WriteStreamToFileSystemOptions()
                {
                    WriteFileBaseDirectory = outputFilePath
                };

                var source = new WriteStreamToFileSystemStep(logger, next, options);

                int errorCount = 1;

                string expectedMessage = Pipelines.Resources.CONTEXT_VALUE_IS_WHITESPACE(CultureInfo.CurrentCulture, FileConstants.TARGET_FILE);

                // act
                await source.InvokeAsync(context).ConfigureAwait(false);

                // assert
                Assert.AreEqual(errorCount, context.Errors.Count);
                Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
                Assert.AreEqual(expectedMessage, context.Errors[0].Message);

                // cleanup
                await source.DisposeAsync().ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_Path_Traversal_Error_Has_Been_Discovered()
        {
            // arrange
            string outputFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string contents = "What is the text here?";
            byte[] body = Encoding.UTF8.GetBytes(contents);

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullPathAndFileName = outputFilePath + fileName;

            using (var stream = new MemoryStream(body))
            {
                PipelineContext context = new PipelineContext();
                context.Items.Add(FileConstants.TARGET_FILE, fullPathAndFileName);
                context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, stream);

                var logger = new Mock<ILogger<WriteStreamToFileSystemStep>>().Object;

                Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
                var next = mockNext.Object;

                WriteStreamToFileSystemOptions options = new WriteStreamToFileSystemOptions()
                {
                    WriteFileBaseDirectory = $"\\\\argonaut.com\\unavailable-file-share\\{Guid.NewGuid()}\\"
                };

                var source = new WriteStreamToFileSystemStep(logger, next, options);

                int errorCount = 1;

                string expectedMessage = Resources.PATH_TRAVERSAL_ISSUE(CultureInfo.CurrentCulture, options.WriteFileBaseDirectory, fullPathAndFileName);

                // act
                await source.InvokeAsync(context).ConfigureAwait(false);

                // assert
                Assert.AreEqual(errorCount, context.Errors.Count);
                Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
                Assert.AreEqual(expectedMessage, context.Errors[0].Message);

                // cleanup
                await source.DisposeAsync().ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_OutputStream_Is_Not_In_The_Context()
        {
            // arrange
            string outputFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string contents = "What is the text here?";
            byte[] body = Encoding.UTF8.GetBytes(contents);

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullPathAndFileName = outputFilePath + fileName;

            using (var stream = new MemoryStream(body))
            {
                PipelineContext context = new PipelineContext();
                context.Items.Add(FileConstants.TARGET_FILE, fileName);

                var logger = new Mock<ILogger<WriteStreamToFileSystemStep>>().Object;

                Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
                var next = mockNext.Object;

                WriteStreamToFileSystemOptions options = new WriteStreamToFileSystemOptions()
                {
                    WriteFileBaseDirectory = outputFilePath
                };

                var source = new WriteStreamToFileSystemStep(logger, next, options);

                int errorCount = 1;

                string expectedMessage = Pipelines.Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, PipelineContextConstants.OUTPUT_STREAM);

                // act
                await source.InvokeAsync(context).ConfigureAwait(false);

                // assert
                Assert.AreEqual(errorCount, context.Errors.Count);
                Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
                Assert.AreEqual(expectedMessage, context.Errors[0].Message);

                // cleanup
                await source.DisposeAsync().ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_OutputStream_Is_Not_A_Stream()
        {
            // arrange
            string outputFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string contents = "What is the text here?";
            byte[] body = Encoding.UTF8.GetBytes(contents);

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullPathAndFileName = outputFilePath + fileName;

            string stream = "Not an Actual Stream object.";

            PipelineContext context = new PipelineContext();
            context.Items.Add(FileConstants.TARGET_FILE, fileName);
            context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, stream);

            var logger = new Mock<ILogger<WriteStreamToFileSystemStep>>().Object;

            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            var next = mockNext.Object;

            WriteStreamToFileSystemOptions options = new WriteStreamToFileSystemOptions()
            {
                WriteFileBaseDirectory = outputFilePath
            };

            var source = new WriteStreamToFileSystemStep(logger, next, options);

            int errorCount = 1;

            string expectedMessage = Pipelines.Resources.CONTEXT_VALUE_NOT_EXPECTED_TYPE(CultureInfo.CurrentCulture, PipelineContextConstants.OUTPUT_STREAM, typeof(Stream));

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            Assert.AreEqual(errorCount, context.Errors.Count);
            Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
            Assert.AreEqual(expectedMessage, context.Errors[0].Message);

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_Target_FileName_Already_Exists()
        {
            // arrange
            string outputFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string contents = "What is the text here?";
            byte[] body = Encoding.UTF8.GetBytes(contents);

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullPathAndFileName = outputFilePath + fileName;
            await File.WriteAllBytesAsync(fullPathAndFileName, body).ConfigureAwait(false);

            using (var stream = new MemoryStream(body))
            {
                PipelineContext context = new PipelineContext();
                context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, stream);
                context.Items.Add(FileConstants.TARGET_FILE, fullPathAndFileName);

                var logger = new Mock<ILogger<WriteStreamToFileSystemStep>>().Object;

                Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
                var next = mockNext.Object;

                WriteStreamToFileSystemOptions options = new WriteStreamToFileSystemOptions()
                {
                    WriteFileBaseDirectory = outputFilePath
                };

                var source = new WriteStreamToFileSystemStep(logger, next, options);

                int errorCount = 1;

                string expectedMessage = Resources.FILE_ALREADY_EXISTS(CultureInfo.CurrentCulture, fullPathAndFileName);

                // act
                await source.InvokeAsync(context).ConfigureAwait(false);

                // assert
                Assert.AreEqual(errorCount, context.Errors.Count);
                Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
                Assert.AreEqual(expectedMessage, context.Errors[0].Message);

                // cleanup
                File.Delete(fullPathAndFileName);
                await source.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
