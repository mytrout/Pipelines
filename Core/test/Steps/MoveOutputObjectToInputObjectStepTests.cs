// <copyright file="MoveOutputObjectToInputObjectStepTests.cs" company="Chris Trout">
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
    public class MoveOutputObjectToInputObjectStepTests
    {
        [TestMethod]
        public void Constructs_MoveOutputObjectToInputObjectStep_Successfully()
        {
            // arrange
            ILogger<MoveOutputObjectToInputObjectStep> logger = new Mock<ILogger<MoveOutputObjectToInputObjectStep>>().Object;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            // act
            using (var result = new MoveOutputObjectToInputObjectStep(logger, next))
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
            ILogger<MoveOutputObjectToInputObjectStep> logger = new Mock<ILogger<MoveOutputObjectToInputObjectStep>>().Object;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            using (var source = new MoveOutputObjectToInputObjectStep(logger, next))
            {
                // act
                await source.DisposeAsync();

                // assert
                Assert.IsTrue(true);

                // No exceptions mean this worked appropriately.
            }
        }

        [TestMethod]
        public async Task Provides_InputObject_In_InvokeAsync_To_DownObject_Callers()
        {
            // arrange
            using (var Object = new MemoryStream())
            {
                var context = new PipelineContext();
                context.Items.Add(PipelineContextConstants.OUTPUT_OBJECT, Object);

                ILogger<MoveOutputObjectToInputObjectStep> logger = new Mock<ILogger<MoveOutputObjectToInputObjectStep>>().Object;
                var mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(context))
                                        .Callback(() => MoveOutputObjectToInputObjectStepTests.Validate_Context(context, Object))
                                        .Returns(Task.CompletedTask);
                var next = mockNext.Object;

                using (var source = new MoveOutputObjectToInputObjectStep(logger, next))
                {
                    // act
                    await source.InvokeAsync(context);

                    // assert
                    Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_OBJECT), "OUTPUT_OBJECT should exist in the Pipeline context.");
                    Assert.IsFalse(context.Items.ContainsKey(PipelineContextConstants.INPUT_OBJECT), "INPUT_OBJECT should not exist in the Pipeline context.");

                    Assert.AreEqual(Object, context.Items[PipelineContextConstants.OUTPUT_OBJECT]);
                }
            }
        }

        [TestMethod]
        public async Task Returns_Context_With_No_Alterations_From_InvokeAsync_After_Execution()
        {
            // arrange
            int errorCount = 0;
            using (var Object = new MemoryStream())
            {
                var context = new PipelineContext();
                context.Items.Add(PipelineContextConstants.OUTPUT_OBJECT, Object);
                var contextName = Guid.NewGuid().ToString();
                var contextValue = Guid.NewGuid();
                context.Items.Add(contextName, contextValue);

                int expectedItemsCount = 2;

                ILogger<MoveOutputObjectToInputObjectStep> logger = new Mock<ILogger<MoveOutputObjectToInputObjectStep>>().Object;
                var mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(context))
                                        .Callback(() => MoveOutputObjectToInputObjectStepTests.Validate_Context(context, Object))
                                        .Returns(Task.CompletedTask);
                var next = mockNext.Object;

                using (var source = new MoveOutputObjectToInputObjectStep(logger, next))
                {
                    // act
                    await source.InvokeAsync(context);

                    // assert
                    Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_OBJECT), "OUTPUT_OBJECT should exist in the Pipeline context.");
                    Assert.IsFalse(context.Items.ContainsKey(PipelineContextConstants.INPUT_OBJECT), "INPUT_OBJECT should not exist in the Pipeline context.");

                    Assert.AreEqual(Object, context.Items[PipelineContextConstants.OUTPUT_OBJECT]);

                    Assert.AreEqual(errorCount, context.Errors.Count);

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
            using (var Object = new MemoryStream())
            {
                var context = new PipelineContext();
                context.Items.Add(PipelineContextConstants.OUTPUT_OBJECT, Object);

                var exception = new InvalidTimeZoneException();

                ILogger<MoveOutputObjectToInputObjectStep> logger = new Mock<ILogger<MoveOutputObjectToInputObjectStep>>().Object;
                var mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(context))
                                        .Throws(exception);

                var next = mockNext.Object;

                int errorCount = 1;

                using (var source = new MoveOutputObjectToInputObjectStep(logger, next))
                {
                    // act
                    await source.InvokeAsync(context);

                    // assert
                    Assert.AreEqual(errorCount, context.Errors.Count);
                    Assert.AreEqual(exception, context.Errors[0]);
                    Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_OBJECT), "OUTPUT_OBJECT should exist in the Pipeline context.");
                    Assert.IsFalse(context.Items.ContainsKey(PipelineContextConstants.INPUT_OBJECT), "INPUT_OBJECT should not exist in the Pipeline context.");

                    Assert.AreEqual(Object, context.Items[PipelineContextConstants.OUTPUT_OBJECT]);
                }
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_No_OutputObject_In_PipelineContext()
        {
            // arrange
            var context = new PipelineContext();

            ILogger<MoveOutputObjectToInputObjectStep> logger = new Mock<ILogger<MoveOutputObjectToInputObjectStep>>().Object;
            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);

            var next = mockNext.Object;

            int errorCount = 1;

            using (var source = new MoveOutputObjectToInputObjectStep(logger, next))
            {
                // act
                await source.InvokeAsync(context);

                // assert
                Assert.AreEqual(errorCount, context.Errors.Count);
                Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
                Assert.AreEqual(Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, PipelineContextConstants.OUTPUT_OBJECT), context.Errors[0].Message);
            }
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Logger_Is_Null()
        {
            // arrange
            ILogger<MoveOutputObjectToInputObjectStep> logger = null;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            string expectedParamName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new MoveOutputObjectToInputObjectStep(logger, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Next_Is_Null()
        {
            // arrange
            ILogger<MoveOutputObjectToInputObjectStep> logger = new Mock<ILogger<MoveOutputObjectToInputObjectStep>>().Object;
            IPipelineRequest next = null;

            string expectedParamName = nameof(next);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new MoveOutputObjectToInputObjectStep(logger, next));

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

            ILogger<MoveOutputObjectToInputObjectStep> logger = new Mock<ILogger<MoveOutputObjectToInputObjectStep>>().Object;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            using (var source = new MoveOutputObjectToInputObjectStep(logger, next))
            {
                // act
                var result = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => source.InvokeAsync(context));

                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedParamName, result.ParamName);
            }
        }

        private static void Validate_Context(PipelineContext context, Object Object)
        {
            Assert.IsFalse(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_OBJECT), "OUTPUT_OBJECT should not exist in the Pipeline context.");
            Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.INPUT_OBJECT), "INPUT_OBJECT should exist in the Pipeline context.");

            Assert.AreEqual(Object, context.Items[PipelineContextConstants.INPUT_OBJECT]);
        }
    }
}