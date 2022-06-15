// <copyright file="DeleteFileStepTests.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2019-2021 Chris Trout
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
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage]
    [TestClass]
    public class DeleteFileStepTests
    {
        [TestMethod]
        public async Task Constructs_DeleteFileTests_Successfully()
        {
            // arrange
            string targetFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";

            var logger = new Mock<ILogger<DeleteFileStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;
            var options = new DeleteFileOptions()
            {
                DeleteFileBaseDirectory = targetFilePath
            };

            // act
            var result = new DeleteFileStep(logger, options, next);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(logger, result.Logger);
            Assert.AreEqual(next, result.Next);
            Assert.AreEqual(options, result.Options);

            // cleanup
            await result.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_File_Is_Deleted_Successfully()
        {
            // arrange
            string targetFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullTargetPathAndFileName = targetFilePath + fileName;

            string contents = "Et tu, Brute?";
            File.WriteAllText(fullTargetPathAndFileName, contents);

            var context = new PipelineContext();
            context.Items.Add(FileConstants.TARGET_FILE, fileName);

            var logger = new Mock<ILogger<DeleteFileStep>>().Object;

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Callback(() => Assert.IsTrue(File.Exists(fullTargetPathAndFileName), "File does not exist during the InvokeAsync callback and should."))
                                    .Returns(Task.CompletedTask);

            var next = mockNext.Object;

            var options = new DeleteFileOptions()
            {
                DeleteFileBaseDirectory = targetFilePath
            };

            var source = new DeleteFileStep(logger, options, next);

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            Assert.IsFalse(File.Exists(fullTargetPathAndFileName));

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
            File.Delete(fullTargetPathAndFileName);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_ExecutionTimings_Before_Is_Configured_And_File_Is_Deleted_Successfully()
        {
            // arrange
            string targetFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullTargetPathAndFileName = targetFilePath + fileName;

            string contents = "Et tu, Brute?";
            File.WriteAllText(fullTargetPathAndFileName, contents);

            var context = new PipelineContext();
            context.Items.Add(FileConstants.TARGET_FILE, fileName);

            var logger = new Mock<ILogger<DeleteFileStep>>().Object;

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Callback(() => Assert.IsFalse(File.Exists(fullTargetPathAndFileName), "File exists during the InvokeAsync callback and should not."))
                                    .Returns(Task.CompletedTask);

            var next = mockNext.Object;

            var options = new DeleteFileOptions()
            {
                DeleteFileBaseDirectory = targetFilePath,
                ExecutionTimings = ExecutionTimings.Before
            };

            var source = new DeleteFileStep(logger, options, next);

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            Assert.IsFalse(File.Exists(fullTargetPathAndFileName));

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
            File.Delete(fullTargetPathAndFileName);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Options_Uses_Different_Context_Names()
        {
            // arrange
            string targetFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullTargetPathAndFileName = targetFilePath + fileName;

            string contents = "Et tu, Brute?";
            File.WriteAllText(fullTargetPathAndFileName, contents);

            var options = new DeleteFileOptions()
            {
                DeleteFileBaseDirectory = targetFilePath,
                TargetFileContextName = "TARGET_FILE_CONTEXT_NAME_TESTING"
            };

            var context = new PipelineContext();
            context.Items.Add(options.TargetFileContextName, fileName);

            var logger = new Mock<ILogger<DeleteFileStep>>().Object;

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Callback(() => Assert.IsTrue(File.Exists(fullTargetPathAndFileName), "File does not exist during the InvokeAsync callback and should."))
                                    .Returns(Task.CompletedTask);

            var next = mockNext.Object;

            var source = new DeleteFileStep(logger, options, next);

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            Assert.IsFalse(File.Exists(fullTargetPathAndFileName));

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
            File.Delete(fullTargetPathAndFileName);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_File_Does_Not_Exist()
        {
            // arrange
            string targetFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullTargetPathAndFileName = targetFilePath + fileName;

            var context = new PipelineContext();
            context.Items.Add(FileConstants.TARGET_FILE, fileName);

            var logger = new Mock<ILogger<DeleteFileStep>>().Object;

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);

            var next = mockNext.Object;

            var options = new DeleteFileOptions()
            {
                DeleteFileBaseDirectory = targetFilePath
            };

            var source = new DeleteFileStep(logger, options, next);

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            Assert.IsFalse(File.Exists(fullTargetPathAndFileName));

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
            File.Delete(fullTargetPathAndFileName);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_Delete_Path_Traversal_Has_Been_Discovered()
        {
            // arrange
            int errorCount = 1;

            string targetFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}{Guid.NewGuid()}{Path.DirectorySeparatorChar}";

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullTargetPathAndFileName = targetFilePath + fileName;

            var context = new PipelineContext();
            context.Items.Add(FileConstants.TARGET_FILE, fullTargetPathAndFileName);

            var logger = new Mock<ILogger<DeleteFileStep>>().Object;

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);

            var next = mockNext.Object;

            // Default value is Windows.
            string deleteFilePathTraversalDirectory = $"C:\\{Guid.NewGuid()}";

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                deleteFilePathTraversalDirectory = $"{Path.DirectorySeparatorChar}{Guid.NewGuid()}{Path.DirectorySeparatorChar}";
            }

            var options = new DeleteFileOptions()
            {
                DeleteFileBaseDirectory = deleteFilePathTraversalDirectory
            };

            var source = new DeleteFileStep(logger, options, next);

            string expectedMessage = Resources.PATH_TRAVERSAL_ISSUE(CultureInfo.CurrentCulture, options.DeleteFileBaseDirectory, fullTargetPathAndFileName);

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
