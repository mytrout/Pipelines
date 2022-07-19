// <copyright file="MoveDirectoryStepTests.cs" company="Chris Trout">
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
    public class MoveDirectoryStepTests
    {
        [TestMethod]
        public async Task Constructs_MoveDirectoryStep_Successfully()
        {
            // arrange
            var logger = new Mock<ILogger<MoveDirectoryStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;
            var options = new MoveDirectoryOptions();

            // act
            var result = new MoveDirectoryStep(logger, options, next);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(logger, result.Logger);
            Assert.AreEqual(next, result.Next);
            Assert.AreEqual(options, result.Options);

            // cleanup
            await result.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Directory_Is_Moved_Successfully()
        {
            // arrange
            string sourceDirectoryPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string targetDirectoryPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";

            string sourceDirectoryName = Guid.NewGuid().ToString();
            string targetDirectoryName = Guid.NewGuid().ToString();

            string fullSourcePathAndDirectoryName = sourceDirectoryPath + sourceDirectoryName;
            string fullTargetPathAndDirectoryName = targetDirectoryPath + targetDirectoryName;

            Directory.CreateDirectory(fullSourcePathAndDirectoryName);
            Directory.CreateDirectory($"{fullSourcePathAndDirectoryName}{Path.DirectorySeparatorChar}{sourceDirectoryName}{Path.DirectorySeparatorChar}");

            var options = new MoveDirectoryOptions();

            var context = new PipelineContext();
            context.Items.Add(options.SourceBaseDirectoryPathContextName, sourceDirectoryPath);
            context.Items.Add(options.SourceDirectoryContextName, sourceDirectoryName);
            context.Items.Add(options.TargetBaseDirectoryPathContextName, targetDirectoryPath);
            context.Items.Add(options.TargetDirectoryContextName, targetDirectoryName);

            var logger = new Mock<ILogger<MoveDirectoryStep>>().Object;

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);

            var next = mockNext.Object;

            try
            {
                using (var source = new MoveDirectoryStep(logger, options, next))
                {
                    // act
                    await source.InvokeAsync(context).ConfigureAwait(false);
                }

                // assert
                if (context.Errors.Any())
                {
                    throw context.Errors[0];
                }

                Assert.IsTrue(Directory.Exists(fullTargetPathAndDirectoryName));
                Assert.IsTrue(Directory.Exists($"{fullTargetPathAndDirectoryName}{Path.DirectorySeparatorChar}{sourceDirectoryName}"));
            }
            finally
            {
                // cleanup
                if (Directory.Exists(fullTargetPathAndDirectoryName))
                {
                    Directory.Delete(fullTargetPathAndDirectoryName, true);
                }
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Downstream_Request_Values_Are_Checked()
        {
            // arrange
            string sourceDirectoryPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string targetDirectoryPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";

            string sourceDirectoryName = Guid.NewGuid().ToString();
            string targetDirectoryName = Guid.NewGuid().ToString();

            string fullSourcePathAndDirectoryName = sourceDirectoryPath + sourceDirectoryName;
            string fullTargetPathAndDirectoryName = targetDirectoryPath + targetDirectoryName;

            Directory.CreateDirectory(fullSourcePathAndDirectoryName);
            Directory.CreateDirectory($"{fullSourcePathAndDirectoryName}{Path.DirectorySeparatorChar}{sourceDirectoryName}{Path.DirectorySeparatorChar}");

            var options = new MoveDirectoryOptions();

            var context = new PipelineContext();
            context.Items.Add(options.SourceBaseDirectoryPathContextName, sourceDirectoryPath);
            context.Items.Add(options.SourceDirectoryContextName, sourceDirectoryName);
            context.Items.Add(options.TargetBaseDirectoryPathContextName, targetDirectoryPath);
            context.Items.Add(options.TargetDirectoryContextName, targetDirectoryName);

            var logger = new Mock<ILogger<MoveDirectoryStep>>().Object;

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Callback(() =>
                                    {
                                        Assert.IsTrue(Directory.Exists(fullTargetPathAndDirectoryName));
                                        Assert.IsTrue(Directory.Exists($"{fullTargetPathAndDirectoryName}{Path.DirectorySeparatorChar}{sourceDirectoryName}"));
                                    })
                                    .Returns(Task.CompletedTask);

            var next = mockNext.Object;

            try
            {
                using (var source = new MoveDirectoryStep(logger, options, next))
                {
                    // act
                    await source.InvokeAsync(context).ConfigureAwait(false);
                }

                // assert
                if (context.Errors.Any())
                {
                    throw context.Errors[0];
                }
            }
            finally
            {
                // cleanup
                if (Directory.Exists(fullTargetPathAndDirectoryName))
                {
                    Directory.Delete(fullTargetPathAndDirectoryName, true);
                }
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_Target_Directory_Exists()
        {
            // arrange
            string sourceDirectoryPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string targetDirectoryPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";

            string sourceDirectoryName = Guid.NewGuid().ToString();
            string targetDirectoryName = Guid.NewGuid().ToString();

            string fullSourcePathAndDirectoryName = sourceDirectoryPath + sourceDirectoryName;
            string fullTargetPathAndDirectoryName = targetDirectoryPath + targetDirectoryName;

            Directory.CreateDirectory(fullSourcePathAndDirectoryName);
            Directory.CreateDirectory($"{fullSourcePathAndDirectoryName}{Path.DirectorySeparatorChar}{sourceDirectoryName}{Path.DirectorySeparatorChar}");
            Directory.CreateDirectory(fullTargetPathAndDirectoryName);

            var options = new MoveDirectoryOptions();

            var context = new PipelineContext();
            context.Items.Add(options.SourceBaseDirectoryPathContextName, sourceDirectoryPath);
            context.Items.Add(options.SourceDirectoryContextName, sourceDirectoryName);
            context.Items.Add(options.TargetBaseDirectoryPathContextName, targetDirectoryPath);
            context.Items.Add(options.TargetDirectoryContextName, targetDirectoryName);

            var logger = new Mock<ILogger<MoveDirectoryStep>>().Object;

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);

            var next = mockNext.Object;

            var expectedMessage = Resources.DIRECTORY_RENAME_ISSUE(fullSourcePathAndDirectoryName, fullTargetPathAndDirectoryName);
            var expectedErrorCount = 1;

            try
            {
                using (var source = new MoveDirectoryStep(logger, options, next))
                {
                    // act
                    await source.InvokeAsync(context).ConfigureAwait(false);
                }

                // assert
                Assert.AreEqual(expectedErrorCount, context.Errors.Count, $"The expected error count should be 1 not {context.Errors.Count}.");
                Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
                Assert.AreEqual(expectedMessage, context.Errors[0].Message);
            }
            finally
            {
                // cleanup
                if (Directory.Exists(fullSourcePathAndDirectoryName))
                {
                    Directory.Delete(fullSourcePathAndDirectoryName, true);
                }

                if (Directory.Exists(fullTargetPathAndDirectoryName))
                {
                    Directory.Delete(fullTargetPathAndDirectoryName, true);
                }
            }
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Logger_Is_Null()
        {
            // arrange
            ILogger<MoveDirectoryStep> logger = null;
            var next = new Mock<IPipelineRequest>().Object;
            var options = new MoveDirectoryOptions();

            string expectedParamName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new MoveDirectoryStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Next_Is_Null()
        {
            // arrange
            var logger = new Mock<ILogger<MoveDirectoryStep>>().Object;
            IPipelineRequest next = null;
            var options = new MoveDirectoryOptions();

            string expectedParamName = nameof(next);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new MoveDirectoryStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Options_Is_Null()
        {
            // arrange
            var logger = new Mock<ILogger<MoveDirectoryStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;
            MoveDirectoryOptions options = null;

            string expectedParamName = nameof(options);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new MoveDirectoryStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public async Task Throws_ArgumentNullException_From_InvokeAsync_When_Context_Is_Null()
        {
            // arrange
            var logger = new Mock<ILogger<MoveDirectoryStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;
            var options = new MoveDirectoryOptions();

            IPipelineContext context = null;

            string expectedParamName = nameof(context);

            using (var sut = new MoveDirectoryStep(logger, options, next))
            {
                // act
                var result = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await sut.InvokeAsync(context));

                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedParamName, result.ParamName);
            }
        }

        [TestMethod]
        public async Task Throws_InvalidOperationException_From_InvokeAsync_When_SourceBaseDirectoryPathContextName_Value_Is_WhiteSpace()
        {
            // arrange
            string sourceDirectoryPath = $"   ";
            string targetDirectoryPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";

            string sourceDirectoryName = Guid.NewGuid().ToString();
            string targetDirectoryName = Guid.NewGuid().ToString();

            var options = new MoveDirectoryOptions();

            var context = new PipelineContext();
            context.Items.Add(options.SourceBaseDirectoryPathContextName, sourceDirectoryPath);
            context.Items.Add(options.SourceDirectoryContextName, sourceDirectoryName);
            context.Items.Add(options.TargetBaseDirectoryPathContextName, targetDirectoryPath);
            context.Items.Add(options.TargetDirectoryContextName, targetDirectoryName);

            var logger = new Mock<ILogger<MoveDirectoryStep>>().Object;

            var next = new Mock<IPipelineRequest>().Object;

            var expectedMessage = MyTrout.Pipelines.Resources.CONTEXT_VALUE_IS_WHITESPACE(CultureInfo.CurrentCulture, options.SourceBaseDirectoryPathContextName);
            var expectedErrorCount = 1;

            using (var sut = new MoveDirectoryStep(logger, options, next))
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
            string sourceDirectoryPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string targetDirectoryPath = sourceDirectoryPath;

            string sourceDirectoryName = "\r\n\t";
            string targetDirectoryName = Guid.NewGuid().ToString();

            var options = new MoveDirectoryOptions();

            var context = new PipelineContext();
            context.Items.Add(options.SourceBaseDirectoryPathContextName, sourceDirectoryPath);
            context.Items.Add(options.SourceDirectoryContextName, sourceDirectoryName);
            context.Items.Add(options.TargetBaseDirectoryPathContextName, targetDirectoryPath);
            context.Items.Add(options.TargetDirectoryContextName, targetDirectoryName);

            var logger = new Mock<ILogger<MoveDirectoryStep>>().Object;

            var next = new Mock<IPipelineRequest>().Object;

            var expectedMessage = MyTrout.Pipelines.Resources.CONTEXT_VALUE_IS_WHITESPACE(CultureInfo.CurrentCulture, options.SourceDirectoryContextName);
            var expectedErrorCount = 1;

            using (var sut = new MoveDirectoryStep(logger, options, next))
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
        public async Task Throws_InvalidOperationException_From_InvokeAsync_When_TargetBaseDirectoryPathContextName_Value_Is_WhiteSpace()
        {
            // arrange
            string sourceDirectoryPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string targetDirectoryPath = $"   ";

            string sourceDirectoryName = Guid.NewGuid().ToString();
            string targetDirectoryName = Guid.NewGuid().ToString();

            var options = new MoveDirectoryOptions();

            var context = new PipelineContext();
            context.Items.Add(options.SourceBaseDirectoryPathContextName, sourceDirectoryPath);
            context.Items.Add(options.SourceDirectoryContextName, sourceDirectoryName);
            context.Items.Add(options.TargetBaseDirectoryPathContextName, targetDirectoryPath);
            context.Items.Add(options.TargetDirectoryContextName, targetDirectoryName);

            var logger = new Mock<ILogger<MoveDirectoryStep>>().Object;

            var next = new Mock<IPipelineRequest>().Object;

            var expectedMessage = MyTrout.Pipelines.Resources.CONTEXT_VALUE_IS_WHITESPACE(CultureInfo.CurrentCulture, options.TargetBaseDirectoryPathContextName);
            var expectedErrorCount = 1;

            using (var sut = new MoveDirectoryStep(logger, options, next))
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
        public async Task Throws_InvalidOperationException_From_InvokeAsync_When_TargetDirectoryContextName_Value_Is_WhiteSpace()
        {
            // arrange
            string sourceDirectoryPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            string targetDirectoryPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";

            string sourceDirectoryName = Guid.NewGuid().ToString();
            string targetDirectoryName = "\r\n\t";

            var options = new MoveDirectoryOptions();

            var context = new PipelineContext();
            context.Items.Add(options.SourceBaseDirectoryPathContextName, sourceDirectoryPath);
            context.Items.Add(options.SourceDirectoryContextName, sourceDirectoryName);
            context.Items.Add(options.TargetBaseDirectoryPathContextName, targetDirectoryPath);
            context.Items.Add(options.TargetDirectoryContextName, targetDirectoryName);

            var logger = new Mock<ILogger<MoveDirectoryStep>>().Object;

            var next = new Mock<IPipelineRequest>().Object;

            var expectedMessage = MyTrout.Pipelines.Resources.CONTEXT_VALUE_IS_WHITESPACE(CultureInfo.CurrentCulture, options.TargetDirectoryContextName);
            var expectedErrorCount = 1;

            using (var sut = new MoveDirectoryStep(logger, options, next))
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