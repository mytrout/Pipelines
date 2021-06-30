// <copyright file="MoveFileStepTests.cs" company="Chris Trout">
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
    public class MoveFileStepTests
    {
        // Removes ~/Pipelines/Steps/IO/Files/test/bin/Release/net5.0 from the path.
        public static readonly string RootPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).Parent.Parent.Parent.Parent.Parent.Parent.Parent.FullName + Path.DirectorySeparatorChar;

        [TestMethod]
        public async Task Constructs_MoveFileTests_Successfully()
        {
            // arrange
            string sourceFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string targetFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}archive{Path.DirectorySeparatorChar}";

            var logger = new Mock<ILogger<MoveFileStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;
            var options = new MoveFileOptions()
            {
                MoveSourceFileBaseDirectory = sourceFilePath,
                MoveTargetFileBaseDirectory = targetFilePath
            };

            // act
            var result = new MoveFileStep(logger, next, options);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(logger, result.Logger);
            Assert.AreEqual(next, result.Next);
            Assert.AreEqual(options, result.Options);

            // cleanup
            await result.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_File_Is_Moved_Successfully()
        {
            // arrange
            string sourceFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string targetFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}{Guid.NewGuid()}{Path.DirectorySeparatorChar}";

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullSourcePathAndFileName = sourceFilePath + fileName;
            string fullTargetPathAndFileName = targetFilePath + fileName;

            string contents = "Et tu, Brute?";
            File.WriteAllText(fullSourcePathAndFileName, contents);

            var context = new PipelineContext();
            context.Items.Add(FileConstants.SOURCE_FILE, fileName);
            context.Items.Add(FileConstants.TARGET_FILE, fileName);

            var logger = new Mock<ILogger<MoveFileStep>>().Object;

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);

            var next = mockNext.Object;

            var options = new MoveFileOptions()
            {
                MoveSourceFileBaseDirectory = sourceFilePath,
                MoveTargetFileBaseDirectory = targetFilePath
            };

            using (var source = new MoveFileStep(logger, next, options))
            {
                // act
                await source.InvokeAsync(context).ConfigureAwait(false);

                // assert
                if (context.Errors.Any())
                {
                    throw context.Errors[0];
                }

                Assert.IsFalse(File.Exists(fullSourcePathAndFileName));
                Assert.IsTrue(File.Exists(fullTargetPathAndFileName));
                Assert.AreEqual(contents, File.ReadAllText(fullTargetPathAndFileName));

                // cleanup
                File.Delete(fullTargetPathAndFileName);
                Directory.Delete(targetFilePath);
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_ExecutionTimings_Before_Is_Configured_And_File_Is_Moved_Successfully()
        {
            // arrange
            string sourceFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string targetFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}{Guid.NewGuid()}{Path.DirectorySeparatorChar}";

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullSourcePathAndFileName = sourceFilePath + fileName;
            string fullTargetPathAndFileName = targetFilePath + fileName;

            string contents = "Et tu, Brute?";
            File.WriteAllText(fullSourcePathAndFileName, contents);

            var context = new PipelineContext();
            context.Items.Add(FileConstants.SOURCE_FILE, fileName);
            context.Items.Add(FileConstants.TARGET_FILE, fileName);

            var logger = new Mock<ILogger<MoveFileStep>>().Object;

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Callback(() =>
                                    {
                                        Assert.IsFalse(File.Exists(fullSourcePathAndFileName));
                                        Assert.IsTrue(File.Exists(fullTargetPathAndFileName));
                                        Assert.AreEqual(contents, File.ReadAllText(fullTargetPathAndFileName));
                                    })
                                    .Returns(Task.CompletedTask);

            var next = mockNext.Object;

            var options = new MoveFileOptions()
            {
                ExecutionTimings = ExecutionTimings.Before,
                MoveSourceFileBaseDirectory = sourceFilePath,
                MoveTargetFileBaseDirectory = targetFilePath
            };

            var source = new MoveFileStep(logger, next, options);

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            Assert.IsFalse(File.Exists(fullSourcePathAndFileName));
            Assert.IsTrue(File.Exists(fullTargetPathAndFileName));
            Assert.AreEqual(contents, File.ReadAllText(fullTargetPathAndFileName));

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
            File.Delete(fullTargetPathAndFileName);
            Directory.Delete(targetFilePath);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_Source_File_Does_Not_Exists()
        {
            // arrange
            string sourceFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string targetFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}{Guid.NewGuid()}{Path.DirectorySeparatorChar}";

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullSourcePathAndFileName = sourceFilePath + fileName;

            var context = new PipelineContext();
            context.Items.Add(FileConstants.SOURCE_FILE, fileName);
            context.Items.Add(FileConstants.TARGET_FILE, fileName);

            var logger = new Mock<ILogger<MoveFileStep>>().Object;

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);

            var next = mockNext.Object;

            var options = new MoveFileOptions()
            {
                MoveSourceFileBaseDirectory = sourceFilePath,
                MoveTargetFileBaseDirectory = targetFilePath
            };

            var source = new MoveFileStep(logger, next, options);

            int errorCount = 1;
            var expectedMessage = Resources.FILE_DOES_NOT_EXIST(CultureInfo.CurrentCulture, fullSourcePathAndFileName);

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
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_Target_File_Already_Exists()
        {
            // arrange
            string sourceFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string targetFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}{Guid.NewGuid()}{Path.DirectorySeparatorChar}";

            string fileName = $"{Guid.NewGuid()}.txt";

            string fullSourcePathAndFileName = sourceFilePath + fileName;
            string fullTargetPathAndFileName = targetFilePath + fileName;

            Directory.CreateDirectory(targetFilePath);

            string contents = "Et tu, Brute?";
            File.WriteAllText(fullSourcePathAndFileName, contents);
            File.WriteAllText(fullTargetPathAndFileName, contents);

            var context = new PipelineContext();
            context.Items.Add(FileConstants.SOURCE_FILE, fileName);
            context.Items.Add(FileConstants.TARGET_FILE, fileName);

            var logger = new Mock<ILogger<MoveFileStep>>().Object;

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);

            var next = mockNext.Object;

            var options = new MoveFileOptions()
            {
                MoveSourceFileBaseDirectory = sourceFilePath,
                MoveTargetFileBaseDirectory = targetFilePath
            };

            var source = new MoveFileStep(logger, next, options);

            int errorCount = 1;
            var expectedMessage = Resources.FILE_ALREADY_EXISTS(CultureInfo.CurrentCulture, fullTargetPathAndFileName);

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            Assert.AreEqual(errorCount, context.Errors.Count);
            Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
            Assert.AreEqual(expectedMessage, context.Errors[0].Message);

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
            File.Delete(fullSourcePathAndFileName);
            File.Delete(fullTargetPathAndFileName);
            Directory.Delete(targetFilePath);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_Source_Path_Traversal_Has_Been_Discovered()
        {
            // arrange
            int errorCount = 1;

            string sourceFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string targetFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}{Guid.NewGuid()}{Path.DirectorySeparatorChar}";

            string fileName = $"{Guid.NewGuid()}.txt";
            string fullSourcePath = $"C:\\{Guid.NewGuid()}\\";

            // All other supported OSPlatforms are a flavor of Linux: FreeBSD, Linux, and OSX.
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                fullSourcePath = $"{MoveFileStepTests.RootPath}{Guid.NewGuid()}{Path.DirectorySeparatorChar}";
            }

            string fullSourcePathAndFileName = $"{fullSourcePath}{fileName}";

            Directory.CreateDirectory(fullSourcePath);
            File.WriteAllText(fullSourcePathAndFileName, "How now brown cow?");

            var context = new PipelineContext();
            context.Items.Add(FileConstants.SOURCE_FILE, fullSourcePathAndFileName);
            context.Items.Add(FileConstants.TARGET_FILE, fileName);

            var logger = new Mock<ILogger<MoveFileStep>>().Object;

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);

            var next = mockNext.Object;

            var options = new MoveFileOptions()
            {
                MoveSourceFileBaseDirectory = sourceFilePath,
                MoveTargetFileBaseDirectory = targetFilePath
            };

            using (var source = new MoveFileStep(logger, next, options))
            {
                string expectedMessage = Resources.PATH_TRAVERSAL_ISSUE(CultureInfo.CurrentCulture, sourceFilePath, fullSourcePathAndFileName);

                // act
                await source.InvokeAsync(context).ConfigureAwait(false);

                // assert
                Assert.AreEqual(errorCount, context.Errors.Count);
                Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
                Assert.AreEqual(expectedMessage, context.Errors[0].Message);
            }

            // cleanup
            File.Delete(fullSourcePathAndFileName);
            Directory.Delete(fullSourcePath);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_Target_Path_Traversal_Has_Been_Discovered()
        {
            // arrange
            int errorCount = 1;

            string sourceFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string targetFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}{Guid.NewGuid()}{Path.DirectorySeparatorChar}";

            string fileName = $"{Guid.NewGuid()}.txt";
            string fullTargetPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}{Guid.NewGuid()}{Path.DirectorySeparatorChar}";

            var context = new PipelineContext();
            context.Items.Add(FileConstants.SOURCE_FILE, fileName);
            context.Items.Add(FileConstants.TARGET_FILE, $"{fullTargetPath}{fileName}");

            var logger = new Mock<ILogger<MoveFileStep>>().Object;

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);

            var next = mockNext.Object;

            var options = new MoveFileOptions()
            {
                MoveSourceFileBaseDirectory = sourceFilePath,
                MoveTargetFileBaseDirectory = targetFilePath
            };

            Directory.CreateDirectory(fullTargetPath);
            File.WriteAllText($"{fullTargetPath}{fileName}", "Et Tu, Brute?");

            var source = new MoveFileStep(logger, next, options);

            string expectedMessage = Resources.PATH_TRAVERSAL_ISSUE(CultureInfo.CurrentCulture, targetFilePath, $"{fullTargetPath}{fileName}");

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            Assert.AreEqual(errorCount, context.Errors.Count);
            Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
            Assert.AreEqual(expectedMessage, context.Errors[0].Message);

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
            File.Delete($"{fullTargetPath}{fileName}");
            Directory.Delete(fullTargetPath);
        }
    }
}
