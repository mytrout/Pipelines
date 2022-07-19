// <copyright file="EnumerateFilesInDirectoryStepTests.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.IO.Directories.Tests
{
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines.Core;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class EnumerateFilesInDirectoryStepTests
    {
        public static readonly string BASE_DIRECTORY_PATH = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";

        public static readonly string BASE_DIRECTORY_NAME = Guid.NewGuid().ToString();

        public static readonly string[] BASE_FILE_NAMES =
            {
                $"{BASE_DIRECTORY_PATH}{Path.DirectorySeparatorChar}{BASE_DIRECTORY_NAME}{Path.DirectorySeparatorChar}A{Guid.NewGuid()}.txt",
                $"{BASE_DIRECTORY_PATH}{Path.DirectorySeparatorChar}{BASE_DIRECTORY_NAME}{Path.DirectorySeparatorChar}B{Guid.NewGuid()}.txt",
                $"{BASE_DIRECTORY_PATH}{Path.DirectorySeparatorChar}{BASE_DIRECTORY_NAME}{Path.DirectorySeparatorChar}C{Guid.NewGuid()}.txt"
            };

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            // cleanup after successful run...because of thread locks on the files created during the test run.
            GC.Collect();
            GC.WaitForPendingFinalizers();
            string path = Path.GetDirectoryName(BASE_FILE_NAMES[0]);

            if (Directory.Exists(path))
            {
                File.Delete(BASE_FILE_NAMES[0]);
                File.Delete(BASE_FILE_NAMES[1]);
                File.Delete(BASE_FILE_NAMES[2]);
                Directory.Delete(path);
            }
        }

        [TestMethod]
        public async Task Constructs_EnumerateFilesInDirectoryStep_Successfully()
        {
            // arrange
            var logger = new Mock<ILogger<EnumerateFilesInDirectoryStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;
            var options = new EnumerateFilesInDirectoryOptions();

            // act
            var result = new EnumerateFilesInDirectoryStep(logger, options, next);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(logger, result.Logger);
            Assert.AreEqual(next, result.Next);
            Assert.AreEqual(options, result.Options);

            // cleanup
            await result.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Files_Are_Enumerated_Successfully()
        {
            // arrange
            string sourceDirectoryPath = EnumerateFilesInDirectoryStepTests.BASE_DIRECTORY_PATH;

            string directoryName = BASE_DIRECTORY_NAME;
            string fileSearchPattern = "*.*";
            string fullTargetPathAndDirectoryName = sourceDirectoryPath + directoryName;

            Directory.CreateDirectory(fullTargetPathAndDirectoryName);

            File.Create(BASE_FILE_NAMES[0]);
            File.Create(BASE_FILE_NAMES[1]);
            File.Create(BASE_FILE_NAMES[2]);

            var options = new EnumerateFilesInDirectoryOptions();

            var context = new PipelineContext();
            context.Items.Add(options.FileSearchPatternContextName, fileSearchPattern);
            context.Items.Add(options.SourceBaseDirectoryPathContextName, sourceDirectoryPath);
            context.Items.Add(options.SourceDirectoryContextName, directoryName);

            var logger = new Mock<ILogger<EnumerateFilesInDirectoryStep>>().Object;

            int x = 0;

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Callback(() =>
                                    {
                                        string currentFileName = Path.GetFileName(BASE_FILE_NAMES[x]);

                                        Assert.AreEqual(fullTargetPathAndDirectoryName, context.Items[options.TargetBaseDirectoryPathContextName]);
                                        Assert.AreEqual(currentFileName, context.Items[options.TargetFileContextName]);

                                        x++;
                                    })
                                    .Returns(Task.CompletedTask);

            var next = mockNext.Object;

            using (var source = new EnumerateFilesInDirectoryStep(logger, options, next))
            {
                // act
                await source.InvokeAsync(context).ConfigureAwait(false);
            }

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            // Assembly Cleanup handles the file/directory cleanup because threads are still locking the files.
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Previous_Values_Are_Restored_After_Execution()
        {
            // arrange
            string sourceDirectoryPath = EnumerateFilesInDirectoryStepTests.BASE_DIRECTORY_PATH;

            string directoryName = BASE_DIRECTORY_NAME;
            string fileSearchPattern = "*.*";
            string fullTargetPathAndDirectoryName = sourceDirectoryPath + directoryName;

            string targetDirectoryName = $"C:\\{Guid.NewGuid()}";
            string targetFileName = "something.wicked.this.way.comes.txt";

            var options = new EnumerateFilesInDirectoryOptions();

            var context = new PipelineContext();
            context.Items.Add(options.FileSearchPatternContextName, fileSearchPattern);
            context.Items.Add(options.SourceBaseDirectoryPathContextName, sourceDirectoryPath);
            context.Items.Add(options.SourceDirectoryContextName, directoryName);
            context.Items.Add(options.TargetBaseDirectoryPathContextName, targetDirectoryName);
            context.Items.Add(options.TargetFileContextName, targetFileName);

            var logger = new Mock<ILogger<EnumerateFilesInDirectoryStep>>().Object;

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);

            var next = mockNext.Object;

            using (var source = new EnumerateFilesInDirectoryStep(logger, options, next))
            {
                // act
                await source.InvokeAsync(context).ConfigureAwait(false);
            }

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            Assert.IsTrue(context.Items.ContainsKey(options.TargetBaseDirectoryPathContextName), $"{options.TargetBaseDirectoryPathContextName} value should exist in IPipelineContext.");
            Assert.AreEqual(targetDirectoryName, context.Items[options.TargetBaseDirectoryPathContextName]);
            Assert.IsTrue(context.Items.ContainsKey(options.TargetFileContextName), $"{options.TargetFileContextName} value should exist in IPipelineContext.");
            Assert.AreEqual(targetFileName, context.Items[options.TargetFileContextName]);

            // Assembly Cleanup handles the file/directory cleanup because threads are still locking the files.
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Logger_Is_Null()
        {
            // arrange
            ILogger<EnumerateFilesInDirectoryStep> logger = null;
            var next = new Mock<IPipelineRequest>().Object;
            var options = new EnumerateFilesInDirectoryOptions();

            string expectedParamName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new EnumerateFilesInDirectoryStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Next_Is_Null()
        {
            // arrange
            var logger = new Mock<ILogger<EnumerateFilesInDirectoryStep>>().Object;
            IPipelineRequest next = null;
            var options = new EnumerateFilesInDirectoryOptions();

            string expectedParamName = nameof(next);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new EnumerateFilesInDirectoryStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Options_Is_Null()
        {
            // arrange
            var logger = new Mock<ILogger<EnumerateFilesInDirectoryStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;
            EnumerateFilesInDirectoryOptions options = null;

            string expectedParamName = nameof(options);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new EnumerateFilesInDirectoryStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public async Task Throws_ArgumentNullException_From_InvokeAsync_When_Context_Is_Null()
        {
            // arrange
            var logger = new Mock<ILogger<EnumerateFilesInDirectoryStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;
            var options = new EnumerateFilesInDirectoryOptions();

            IPipelineContext context = null;

            string expectedParamName = nameof(context);

            using (var sut = new EnumerateFilesInDirectoryStep(logger, options, next))
            {
                // act
                var result = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await sut.InvokeAsync(context));

                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedParamName, result.ParamName);
            }
        }

        [TestMethod]
        public async Task Throws_InvalidOperationException_From_InvokeAsync_When_FileSearchPatternContextName_Value_Is_WhiteSpace()
        {
            // arrange
            string fileSearchPattern = "    ";
            string sourceDirectoryPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string sourceDirectoryName = Guid.NewGuid().ToString();

            var options = new EnumerateFilesInDirectoryOptions();

            var context = new PipelineContext();

            context.Items.Add(options.FileSearchPatternContextName, fileSearchPattern);
            context.Items.Add(options.SourceBaseDirectoryPathContextName, sourceDirectoryPath);
            context.Items.Add(options.SourceDirectoryContextName, sourceDirectoryName);

            var logger = new Mock<ILogger<EnumerateFilesInDirectoryStep>>().Object;

            var next = new Mock<IPipelineRequest>().Object;

            var expectedMessage = MyTrout.Pipelines.Resources.CONTEXT_VALUE_IS_WHITESPACE(CultureInfo.CurrentCulture, options.FileSearchPatternContextName);
            var expectedErrorCount = 1;

            using (var sut = new EnumerateFilesInDirectoryStep(logger, options, next))
            {
                // act
                await sut.InvokeAsync(context);

                // assert
                Assert.AreEqual(expectedErrorCount, context.Errors.Count, $"The expected error count should be 1 not {context.Errors.Count}.");
                Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
                Assert.AreEqual(expectedMessage, context.Errors[0].Message);
            }
        }

        [TestMethod]
        public async Task Throws_InvalidOperationException_From_InvokeAsync_When_SourceBaseDirectoryPathContextName_Value_Is_WhiteSpace()
        {
            // arrange
            string fileSearchPattern = "*.*";
            string sourceDirectoryPath = "   ";
            string sourceDirectoryName = Guid.NewGuid().ToString();

            var options = new EnumerateFilesInDirectoryOptions();

            var context = new PipelineContext();
            context.Items.Add(options.FileSearchPatternContextName, fileSearchPattern);
            context.Items.Add(options.SourceBaseDirectoryPathContextName, sourceDirectoryPath);
            context.Items.Add(options.SourceDirectoryContextName, sourceDirectoryName);

            var logger = new Mock<ILogger<EnumerateFilesInDirectoryStep>>().Object;

            var next = new Mock<IPipelineRequest>().Object;

            var expectedMessage = MyTrout.Pipelines.Resources.CONTEXT_VALUE_IS_WHITESPACE(CultureInfo.CurrentCulture, options.SourceBaseDirectoryPathContextName);
            var expectedErrorCount = 1;

            using (var sut = new EnumerateFilesInDirectoryStep(logger, options, next))
            {
                // act
                await sut.InvokeAsync(context);

                // assert
                Assert.AreEqual(expectedErrorCount, context.Errors.Count, $"The expected error count should be 1 not {context.Errors.Count}.");
                Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
                Assert.AreEqual(expectedMessage, context.Errors[0].Message);
            }
        }

        [TestMethod]
        public async Task Throws_InvalidOperationException_From_InvokeAsync_When_SourceDirectoryContextName_Value_Is_WhiteSpace()
        {
            // arrange
            string fileSearchPattern = "*.*";
            string sourceDirectoryPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string sourceDirectoryName = "\r\n\t";

            var options = new EnumerateFilesInDirectoryOptions();

            var context = new PipelineContext();

            context.Items.Add(options.FileSearchPatternContextName, fileSearchPattern);
            context.Items.Add(options.SourceBaseDirectoryPathContextName, sourceDirectoryPath);
            context.Items.Add(options.SourceDirectoryContextName, sourceDirectoryName);

            var logger = new Mock<ILogger<EnumerateFilesInDirectoryStep>>().Object;

            var next = new Mock<IPipelineRequest>().Object;

            var expectedMessage = MyTrout.Pipelines.Resources.CONTEXT_VALUE_IS_WHITESPACE(CultureInfo.CurrentCulture, options.SourceDirectoryContextName);
            var expectedErrorCount = 1;

            using (var sut = new EnumerateFilesInDirectoryStep(logger, options, next))
            {
                // act
                await sut.InvokeAsync(context);

                // assert
                Assert.AreEqual(expectedErrorCount, context.Errors.Count, $"The expected error count should be 1 not {context.Errors.Count}.");
                Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
                Assert.AreEqual(expectedMessage, context.Errors[0].Message);
            }
        }
    }
}
