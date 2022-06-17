// <copyright file="AppendStreamToFileStepTests.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.IO.Files.Tests
{
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines.Core;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage]
    [TestClass]
    public class AppendStreamToFileStepTests
    {
        [TestMethod]
        public async Task Constructs_AppendStreamToFileTests_Successfully()
        {
            // arrange
            string outputFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";

            var logger = new Mock<ILogger<AppendStreamToFileStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;
            var options = new AppendStreamToFileOptions()
            {
                AppendFileBaseDirectory = outputFilePath
            };

            // act
            var result = new AppendStreamToFileStep(logger, options, next);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(logger, result.Logger);
            Assert.AreEqual(options, result.Options);

            // cleanup
            await result.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Appends_Data_To_File_From_InvokeAsync_When_ExecutionTimings_Before_Is_Configured()
        {
            // arrange
            string outputFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string contents = "What is the text here?";
            byte[] body = Encoding.UTF8.GetBytes(contents);

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullPathAndFileName = outputFilePath + fileName;

            using (var stream = new MemoryStream(body))
            {
                var context = new PipelineContext();
                context.Items.Add(FileConstants.TARGET_FILE, fileName);
                context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, stream);

                var logger = new Mock<ILogger<AppendStreamToFileStep>>().Object;

                var mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(context))
                        .Callback(() =>
                        {
                            Assert.IsTrue(File.Exists(fullPathAndFileName));
                            Assert.IsTrue((context.Items[PipelineContextConstants.OUTPUT_STREAM] as Stream).CanRead, "The stream should be open.");
                            Assert.AreEqual(contents, File.ReadAllText(fullPathAndFileName));
                        })
                        .Returns(Task.CompletedTask);
                var next = mockNext.Object;

                var options = new AppendStreamToFileOptions()
                {
                    ExecutionTimings = ExecutionTimings.Before,
                    AppendFileBaseDirectory = outputFilePath
                };

                var source = new AppendStreamToFileStep(logger, options, next);

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
        public async Task Appends_Data_To_File_From_InvokeAsync_When_Target_FileName_Already_Exists()
        {
            // arrange
            string outputFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string contents = "What is the text here?";
            byte[] body = Encoding.UTF8.GetBytes(contents);

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullPathAndFileName = outputFilePath + fileName;
            await File.WriteAllBytesAsync(fullPathAndFileName, body).ConfigureAwait(false);

            string expectedContents = $"{contents}{contents}";
            using (var stream = new MemoryStream(body))
            {
                var context = new PipelineContext();
                context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, stream);
                context.Items.Add(FileConstants.TARGET_FILE, fullPathAndFileName);

                var logger = new Mock<ILogger<AppendStreamToFileStep>>().Object;

                var mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
                var next = mockNext.Object;

                var options = new AppendStreamToFileOptions()
                {
                    AppendFileBaseDirectory = outputFilePath
                };

                var source = new AppendStreamToFileStep(logger, options, next);

                int errorCount = 0;

                // act
                await source.InvokeAsync(context).ConfigureAwait(false);

                // assert
                Assert.AreEqual(errorCount, context.Errors.Count);

                string actualContents = File.ReadAllText(fullPathAndFileName);
                Assert.AreEqual(expectedContents, actualContents);

                // cleanup
                File.Delete(fullPathAndFileName);
                await source.DisposeAsync().ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task Appends_Data_To_File_From_InvokeAsync()
        {
            // arrange
            string outputFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string contents = "What is the text here?";
            byte[] body = Encoding.UTF8.GetBytes(contents);

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullPathAndFileName = outputFilePath + fileName;

            using (var stream = new MemoryStream(body))
            {
                var context = new PipelineContext();
                context.Items.Add(FileConstants.TARGET_FILE, fileName);
                context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, stream);

                var logger = new Mock<ILogger<AppendStreamToFileStep>>().Object;

                var mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
                var next = mockNext.Object;

                var options = new AppendStreamToFileOptions()
                {
                    AppendFileBaseDirectory = outputFilePath
                };

                var source = new AppendStreamToFileStep(logger, options, next);

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
        public async Task Appends_Data_To_File_From_InvokeAsync_When_Options_Uses_Different_Context_Names()
        {
            // arrange
            string outputFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string contents = "What is the text here?";
            byte[] body = Encoding.UTF8.GetBytes(contents);

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullPathAndFileName = outputFilePath + fileName;

            using (var stream = new MemoryStream(body))
            {
                var options = new AppendStreamToFileOptions()
                {
                    AppendFileBaseDirectory = outputFilePath,
                    OutputStreamContextName = "OUTPUT_STREAM_CONTEXT_NAME_TESTING",
                    TargetFileContextName = "TARGET_FILE_CONTEXT_NAME_TESTING"
                };

                var context = new PipelineContext();
                context.Items.Add(options.TargetFileContextName, fileName);
                context.Items.Add(options.OutputStreamContextName, stream);

                var logger = new Mock<ILogger<AppendStreamToFileStep>>().Object;

                var mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
                var next = mockNext.Object;

                var source = new AppendStreamToFileStep(logger, options, next);

                // act
                await source.InvokeAsync(context).ConfigureAwait(false);

                // assert
                Assert.IsTrue(File.Exists(fullPathAndFileName));
                Assert.IsTrue((context.Items[options.OutputStreamContextName] as Stream).CanRead, "The stream should be open.");
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

            using (var stream = new MemoryStream(body))
            {
                var context = new PipelineContext();
                context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, stream);

                var logger = new Mock<ILogger<AppendStreamToFileStep>>().Object;

                var mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
                var next = mockNext.Object;

                var options = new AppendStreamToFileOptions()
                {
                    AppendFileBaseDirectory = outputFilePath
                };

                var source = new AppendStreamToFileStep(logger, options, next);

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

            using (var stream = new MemoryStream(body))
            {
                var context = new PipelineContext();
                context.Items.Add(FileConstants.TARGET_FILE, string.Empty);
                context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, stream);

                var logger = new Mock<ILogger<AppendStreamToFileStep>>().Object;

                var mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
                var next = mockNext.Object;

                var options = new AppendStreamToFileOptions()
                {
                    AppendFileBaseDirectory = outputFilePath
                };

                var source = new AppendStreamToFileStep(logger, options, next);

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
                var context = new PipelineContext();
                context.Items.Add(FileConstants.TARGET_FILE, fullPathAndFileName);
                context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, stream);

                var logger = new Mock<ILogger<AppendStreamToFileStep>>().Object;

                var mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
                var next = mockNext.Object;

                var options = new AppendStreamToFileOptions()
                {
                    AppendFileBaseDirectory = $"\\\\argonaut.com\\unavailable-file-share\\{Guid.NewGuid()}\\"
                };

                var source = new AppendStreamToFileStep(logger, options, next);

                int errorCount = 1;

                string expectedMessage = Resources.PATH_TRAVERSAL_ISSUE(CultureInfo.CurrentCulture, options.AppendFileBaseDirectory, fullPathAndFileName);

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

            using (var stream = new MemoryStream(body))
            {
                var context = new PipelineContext();
                context.Items.Add(FileConstants.TARGET_FILE, fileName);

                var logger = new Mock<ILogger<AppendStreamToFileStep>>().Object;

                var mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
                var next = mockNext.Object;

                var options = new AppendStreamToFileOptions()
                {
                    AppendFileBaseDirectory = outputFilePath
                };

                var source = new AppendStreamToFileStep(logger, options, next);

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

            string fileName = $"{Guid.NewGuid()}.txt";

            string stream = "Not an Actual Stream object.";

            var context = new PipelineContext();
            context.Items.Add(FileConstants.TARGET_FILE, fileName);
            context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, stream);

            var logger = new Mock<ILogger<AppendStreamToFileStep>>().Object;

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            var next = mockNext.Object;

            var options = new AppendStreamToFileOptions()
            {
                AppendFileBaseDirectory = outputFilePath
            };

            var source = new AppendStreamToFileStep(logger, options, next);

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
    }
}
