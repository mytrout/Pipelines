// <copyright file="MoveInputStreamToOutputStreamStepTests.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.Tests
{
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines.Core;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class MoveInputStreamToOutputStreamStepTests
    {
        [TestMethod]
        public void Constructs_MoveInputStreamToOutputStreamStep_Successfully()
        {
            // arrange
            ILogger<MoveInputStreamToOutputStreamStep> logger = new Mock<ILogger<MoveInputStreamToOutputStreamStep>>().Object;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            // act
            using (var result = new MoveInputStreamToOutputStreamStep(logger, next))
            {
                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(logger, result.Logger);
                Assert.AreEqual(next, result.Next);
            }
        }

        [TestMethod]
        public async Task Returns_Task_From_DisposeAsync()
        {
            // arrange
            ILogger<MoveInputStreamToOutputStreamStep> logger = new Mock<ILogger<MoveInputStreamToOutputStreamStep>>().Object;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            using (var source = new MoveInputStreamToOutputStreamStep(logger, next))
            {
                // act
                await source.DisposeAsync();

                // assert
                Assert.IsTrue(true);

                // No exceptions mean this worked appropriately.
            }
        }

        [TestMethod]
        public async Task Provides_OutputStream_In_InvokeAsync_To_Downstream_Callers()
        {
            // arrange
            using (var stream = new MemoryStream())
            {
                var context = new PipelineContext();
                context.Items.Add(PipelineContextConstants.INPUT_STREAM, stream);

                var logger = new Mock<ILogger<MoveInputStreamToOutputStreamStep>>().Object;
                var mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(context))
                                        .Callback(() => MoveInputStreamToOutputStreamStepTests.Validate_Context(context, stream))
                                        .Returns(Task.CompletedTask);
                var next = mockNext.Object;

                using (var source = new MoveInputStreamToOutputStreamStep(logger, next))
                {
                    // act
                    await source.InvokeAsync(context);

                    // assert
                    Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.INPUT_STREAM), "INPUT_STREAM should exist in the Pipeline context.");
                    Assert.IsFalse(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_STREAM), "OUTPUT_STREAM should not exist in the Pipeline context.");

                    Assert.AreEqual(stream, context.Items[PipelineContextConstants.INPUT_STREAM]);
                }
            }
        }

        [TestMethod]
        public async Task Returns_Context_With_No_Alterations_From_InvokeAsync_After_Execution()
        {
            // arrange
            int errorCount = 0;

            using (var stream = new MemoryStream())
            {
                var context = new PipelineContext();
                context.Items.Add(PipelineContextConstants.INPUT_STREAM, stream);
                var contextName = Guid.NewGuid().ToString();
                var contextValue = Guid.NewGuid();
                context.Items.Add(contextName, contextValue);

                int expectedItemsCount = 2;

                var logger = new Mock<ILogger<MoveInputStreamToOutputStreamStep>>().Object;
                var mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(context))
                                        .Callback(() => MoveInputStreamToOutputStreamStepTests.Validate_Context(context, stream))
                                        .Returns(Task.CompletedTask);
                var next = mockNext.Object;

                using (var source = new MoveInputStreamToOutputStreamStep(logger, next))
                {
                    // act
                    await source.InvokeAsync(context);

                    // assert
                    Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.INPUT_STREAM), "INPUT_STREAM should exist in the Pipeline context.");
                    Assert.IsFalse(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_STREAM), "OUTPUT_STREAM should not exist in the Pipeline context.");
                    Assert.AreEqual(errorCount, context.Errors.Count);
                    Assert.AreEqual(stream, context.Items[PipelineContextConstants.INPUT_STREAM]);

                    Assert.AreEqual(expectedItemsCount, context.Items.Count);
                    Assert.IsTrue(context.Items.ContainsKey(contextName), "context does contain a key named '{0}'.", contextName);
                    Assert.AreEqual(contextValue, context.Items[contextName], "context does contain a key named '{0}' with a value of '{1}'.", contextName, contextValue);
                }
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_Exception_Is_Thrown_In_Next()
        {
            // arrange
            using (var stream = new MemoryStream())
            {
                var context = new PipelineContext();
                context.Items.Add(PipelineContextConstants.INPUT_STREAM, stream);

                var exception = new InvalidTimeZoneException();

                ILogger<MoveInputStreamToOutputStreamStep> logger = new Mock<ILogger<MoveInputStreamToOutputStreamStep>>().Object;
                var mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(context))
                                        .Throws(exception);

                var next = mockNext.Object;

                int errorCount = 1;

                using (var source = new MoveInputStreamToOutputStreamStep(logger, next))
                {
                    // act
                    await source.InvokeAsync(context);

                    // assert
                    Assert.AreEqual(errorCount, context.Errors.Count);
                    Assert.AreEqual(exception, context.Errors[0]);
                    Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.INPUT_STREAM), "INPUT_STREAM should exist in the Pipeline context.");
                    Assert.IsFalse(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_STREAM), "OUTPUT_STREAM should not exist in the Pipeline context.");

                    Assert.AreEqual(stream, context.Items[PipelineContextConstants.INPUT_STREAM]);
                }
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_No_InputStream_In_PipelineContext()
        {
            // arrange
            var context = new PipelineContext();

            ILogger<MoveInputStreamToOutputStreamStep> logger = new Mock<ILogger<MoveInputStreamToOutputStreamStep>>().Object;
            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);

            var next = mockNext.Object;

            int errorCount = 1;

            string expectedMessage = Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, PipelineContextConstants.INPUT_STREAM);

            using (var source = new MoveInputStreamToOutputStreamStep(logger, next))
            {
                // act
                await source.InvokeAsync(context);

                // assert
                Assert.AreEqual(errorCount, context.Errors.Count);
                Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
                Assert.AreEqual(expectedMessage, context.Errors[0].Message);
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_InputStream_Is_Not_A_Stream_In_PipelineContext()
        {
            // arrange
            var context = new PipelineContext();
            context.Items.Add(PipelineContextConstants.INPUT_STREAM, "Not a Stream");

            ILogger<MoveInputStreamToOutputStreamStep> logger = new Mock<ILogger<MoveInputStreamToOutputStreamStep>>().Object;
            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                        .Returns(Task.CompletedTask);
            var next = mockNext.Object;

            int errorCount = 1;

            using (var source = new MoveInputStreamToOutputStreamStep(logger, next))
            {
                string expectedMessage = Resources.CONTEXT_VALUE_NOT_EXPECTED_TYPE(CultureInfo.CurrentCulture, PipelineContextConstants.INPUT_STREAM, typeof(Stream));

                // act
                await source.InvokeAsync(context);

                // assert
                Assert.AreEqual(errorCount, context.Errors.Count);
                Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
                Assert.AreEqual(expectedMessage, context.Errors[0].Message);
            }
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Logger_Is_Null()
        {
            // arrange
            ILogger<MoveInputStreamToOutputStreamStep> logger = null;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            string expectedParamName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new MoveInputStreamToOutputStreamStep(logger, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Next_Is_Null()
        {
            // arrange
            ILogger<MoveInputStreamToOutputStreamStep> logger = new Mock<ILogger<MoveInputStreamToOutputStreamStep>>().Object;
            IPipelineRequest next = null;

            string expectedParamName = nameof(next);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new MoveInputStreamToOutputStreamStep(logger, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public async Task Throws_ArgumentNullException_From_InvokeAsync_When_Context_Is_Null()
        {
            // arrange
            PipelineContext context = null;
            string expectedParamName = nameof(context);

            ILogger<MoveInputStreamToOutputStreamStep> logger = new Mock<ILogger<MoveInputStreamToOutputStreamStep>>().Object;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            using (var source = new MoveInputStreamToOutputStreamStep(logger, next))
            {
                // act
                var result = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => source.InvokeAsync(context));

                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedParamName, result.ParamName);
            }
        }

        private static void Validate_Context(PipelineContext context, Stream stream)
        {
            Assert.IsFalse(context.Items.ContainsKey(PipelineContextConstants.INPUT_STREAM), "INPUT_STREAM should not exist in the Pipeline context.");
            Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_STREAM), "OUTPUT_STREAM should exist in the Pipeline context.");

            Assert.AreEqual(stream, context.Items[PipelineContextConstants.OUTPUT_STREAM]);
        }
    }
}