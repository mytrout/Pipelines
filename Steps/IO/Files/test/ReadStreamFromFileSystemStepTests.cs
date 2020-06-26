// <copyright file="ReadStreamFromFileSystemStepTests.cs" company="Chris Trout">
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
    public class ReadStreamFromFileSystemStepTests
    {
        [TestMethod]
        public async Task Constructs_ReadStreamFromFileSystemTests_Successfully()
        {
            // arrange
            string inputFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";

            var logger = new Mock<ILogger<ReadStreamFromFileSystemStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;
            var options = new ReadStreamFromFileSystemOptions()
            {
                ReadFileBaseDirectory = inputFilePath
            };

            // act
            var result = new ReadStreamFromFileSystemStep(logger, next, options);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(logger, result.Logger);
            Assert.AreEqual(options, result.Options);

            // cleanup
            await result.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_Exception_Is_Thrown_In_Next()
        {
            // arrange
            int errorCount = 1;

            string inputFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string contents = "Wher is donkey, eh?";
            byte[] body = Encoding.UTF8.GetBytes(contents);

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullPathAndFileName = inputFilePath + fileName;

            await File.WriteAllBytesAsync(fullPathAndFileName, body).ConfigureAwait(false);

            PipelineContext context = new PipelineContext();
            context.Items.Add(PipelineFileConstants.SOURCE_FILE, fileName);

            var logger = new Mock<ILogger<ReadStreamFromFileSystemStep>>().Object;

            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Throws(new InvalidTimeZoneException());

            var next = mockNext.Object;

            ReadStreamFromFileSystemOptions options = new ReadStreamFromFileSystemOptions()
            {
                ReadFileBaseDirectory = inputFilePath
            };

            var source = new ReadStreamFromFileSystemStep(logger, next, options);

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            Assert.IsFalse(context.Items.ContainsKey(PipelineContextConstants.INPUT_STREAM));
            Assert.AreEqual(errorCount, context.Errors.Count);
            Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidTimeZoneException));

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
            File.Delete(fullPathAndFileName);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_Invoke_Async_When_FileName_Is_Not_In_Context()
        {
            // arrange
            int errorCount = 1;

            string inputFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullPathAndFileName = inputFilePath + fileName;

            PipelineContext context = new PipelineContext();

            var logger = new Mock<ILogger<ReadStreamFromFileSystemStep>>().Object;

            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;

            ReadStreamFromFileSystemOptions options = new ReadStreamFromFileSystemOptions()
            {
                ReadFileBaseDirectory = inputFilePath
            };

            var source = new ReadStreamFromFileSystemStep(logger, next, options);

            string expectedMessage = Resources.KEY_DOES_NOT_EXIST_IN_CONTEXT(CultureInfo.CurrentCulture, PipelineFileConstants.SOURCE_FILE);

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
        public async Task Returns_PipelineContext_Error_From_Invoke_Async_When_FileName_Is_Empty()
        {
            // arrange
            int errorCount = 1;

            string inputFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullPathAndFileName = inputFilePath + fileName;

            PipelineContext context = new PipelineContext();
            context.Items.Add(PipelineFileConstants.SOURCE_FILE, string.Empty);

            var logger = new Mock<ILogger<ReadStreamFromFileSystemStep>>().Object;

            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;

            ReadStreamFromFileSystemOptions options = new ReadStreamFromFileSystemOptions()
            {
                ReadFileBaseDirectory = inputFilePath
            };

            var source = new ReadStreamFromFileSystemStep(logger, next, options);

            string expectedMessage = Resources.VALUE_IS_WHITESPACE_IN_CONTEXT(CultureInfo.CurrentCulture, PipelineFileConstants.SOURCE_FILE);

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
        public async Task Returns_PipelineContext_Error_From_Invoke_Async_When_Path_Traversal_Has_Been_Discovered()
        {
            // arrange
            int errorCount = 1;

            string inputFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullPathAndFileName = inputFilePath + fileName;

            PipelineContext context = new PipelineContext();
            context.Items.Add(PipelineFileConstants.SOURCE_FILE, fullPathAndFileName);

            var logger = new Mock<ILogger<ReadStreamFromFileSystemStep>>().Object;

            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;

            ReadStreamFromFileSystemOptions options = new ReadStreamFromFileSystemOptions()
            {
                ReadFileBaseDirectory = $"\\\\createyour.software\\invalid\\{Guid.NewGuid()})"
            };

            var source = new ReadStreamFromFileSystemStep(logger, next, options);

            string expectedMessage = Resources.PATH_TRAVERSAL_ISSUE(CultureInfo.CurrentCulture, options.ReadFileBaseDirectory, fullPathAndFileName);

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
        public async Task Returns_PipelineContext_With_InputStream_From_InvokeAsync()
        {
            // arrange
            string inputFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string contents = "Wher is donkey, eh?";
            byte[] body = Encoding.UTF8.GetBytes(contents);

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullPathAndFileName = inputFilePath + fileName;

            await File.WriteAllBytesAsync(fullPathAndFileName, body).ConfigureAwait(false);

            PipelineContext context = new PipelineContext();
            context.Items.Add(PipelineFileConstants.SOURCE_FILE, fileName);
            context.Items.Add("FILE_CONTENTS", contents);

            var logger = new Mock<ILogger<ReadStreamFromFileSystemStep>>().Object;

            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Callback(() => this.VerifyContext(context))
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;

            ReadStreamFromFileSystemOptions options = new ReadStreamFromFileSystemOptions()
            {
                ReadFileBaseDirectory = inputFilePath
            };

            var source = new ReadStreamFromFileSystemStep(logger, next, options);

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            Assert.IsFalse(context.Items.ContainsKey(PipelineContextConstants.INPUT_STREAM));

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
            File.Delete(fullPathAndFileName);
        }

        [TestMethod]
        public async Task Returns_Pipeline_Context_With_Previous_Stream_From_InvokeAsync()
        {
            // arrange
            string inputFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string contents = "Wher is donkey, eh?";
            byte[] body = Encoding.UTF8.GetBytes(contents);

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullPathAndFileName = inputFilePath + fileName;

            MemoryStream previous = new MemoryStream();

            await File.WriteAllBytesAsync(fullPathAndFileName, body).ConfigureAwait(false);

            PipelineContext context = new PipelineContext();
            context.Items.Add(PipelineFileConstants.SOURCE_FILE, fileName);
            context.Items.Add(PipelineContextConstants.INPUT_STREAM, previous);

            var logger = new Mock<ILogger<ReadStreamFromFileSystemStep>>().Object;

            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;

            ReadStreamFromFileSystemOptions options = new ReadStreamFromFileSystemOptions()
            {
                ReadFileBaseDirectory = inputFilePath
            };

            var source = new ReadStreamFromFileSystemStep(logger, next, options);

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.INPUT_STREAM));
            Assert.AreEqual(previous, context.Items[PipelineContextConstants.INPUT_STREAM]);

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
            File.Delete(fullPathAndFileName);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_With_Previous_Stream_From_Invoke_Async_When_Error_Is_Created()
        {
            // arrange
            string inputFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullPathAndFileName = inputFilePath + fileName;

            MemoryStream stream = new MemoryStream();

            PipelineContext context = new PipelineContext();
            context.Items.Add(PipelineContextConstants.INPUT_STREAM, stream);

            var logger = new Mock<ILogger<ReadStreamFromFileSystemStep>>().Object;

            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;

            ReadStreamFromFileSystemOptions options = new ReadStreamFromFileSystemOptions()
            {
                ReadFileBaseDirectory = inputFilePath
            };

            var source = new ReadStreamFromFileSystemStep(logger, next, options);

            string expectedMessage = Resources.KEY_DOES_NOT_EXIST_IN_CONTEXT(CultureInfo.CurrentCulture, PipelineFileConstants.SOURCE_FILE);

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.INPUT_STREAM));
            Assert.AreEqual(stream, context.Items[PipelineContextConstants.INPUT_STREAM]);

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Returns_Pipeline_Context_With_Previous_Stream_From_InvokeAsync_When_Exception_Is_Thrown_In_Next()
        {
            // arrange
            string inputFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string contents = "Wher is donkey, eh?";
            byte[] body = Encoding.UTF8.GetBytes(contents);

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullPathAndFileName = inputFilePath + fileName;

            MemoryStream previous = new MemoryStream();

            await File.WriteAllBytesAsync(fullPathAndFileName, body).ConfigureAwait(false);

            PipelineContext context = new PipelineContext();
            context.Items.Add(PipelineFileConstants.SOURCE_FILE, fileName);
            context.Items.Add(PipelineContextConstants.INPUT_STREAM, previous);

            var logger = new Mock<ILogger<ReadStreamFromFileSystemStep>>().Object;

            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Throws(new InvalidTimeZoneException());
            var next = mockNext.Object;

            ReadStreamFromFileSystemOptions options = new ReadStreamFromFileSystemOptions()
            {
                ReadFileBaseDirectory = inputFilePath
            };

            var source = new ReadStreamFromFileSystemStep(logger, next, options);

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.INPUT_STREAM));
            Assert.AreEqual(previous, context.Items[PipelineContextConstants.INPUT_STREAM]);

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
            File.Delete(fullPathAndFileName);
        }

        private void VerifyContext(PipelineContext context)
        {
            Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.INPUT_STREAM), "Input Stream does not exist in Pipeline Context.");

            Stream stream = context.Items[PipelineContextConstants.INPUT_STREAM] as Stream;

            Assert.IsNotNull(stream, "Stream is null.");

            using (StreamReader reader = new StreamReader(stream, leaveOpen: true))
            {
                string result = reader.ReadToEnd();
                Assert.AreEqual(context.Items["FILE_CONTENTS"], result);
            }
        }
    }
}
