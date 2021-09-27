// <copyright file="RenameContextItemStepTests.cs" company="Chris Trout">
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
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class RenameContextItemStepTests
    {
        [TestMethod]
        public void Constructs_RenameContextItemStep_Successfully()
        {
            // arrange
            ILogger<RenameContextItemStep> logger = new Mock<ILogger<RenameContextItemStep>>().Object;
            var options = new RenameContextItemOptions();

            using (IPipelineRequest next = new NoOpStep())
            {
                // act
                using (var result = new RenameContextItemStep(logger, options, next))
                {
                    // assert
                    Assert.IsNotNull(result);
                    Assert.AreEqual(logger, result.Logger);
                    Assert.AreEqual(options, result.Options);
                    Assert.AreEqual(next, result.Next);
                }
            }
        }

        [TestMethod]
        public async Task Renames_CacheItems_And_Restores_Them_During_Step_Execution()
        {
            // arrange
            ILogger<RenameContextItemStep> logger = new Mock<ILogger<RenameContextItemStep>>().Object;

            string originalKey1 = "PIPELINE_VALUE_1";
            string originalKey2 = "PIPELINE_VALUE_2";
            string originalKey3 = "PIPELINE_VALUE_3";

            string expectedKey1 = "PIPELINE_RENAMED_VALUE_1";
            string expectedKey2 = "PIPELINE_RENAMED_VALUE_2";
            string expectedKey3 = "PIPELINE_RENAMED_VALUE_3";

            var options = new RenameContextItemOptions()
            {
                RenameValues = new Dictionary<string, string>()
                {
                    { originalKey1, expectedKey1 },
                    { originalKey2, expectedKey2 },
                    { originalKey3, expectedKey3 }
                }
            };

            string expectedValue1 = "Something to believe in.";
            string expectedValue2 = "Poison";

            var mockRequest = new Mock<IPipelineRequest>();
            mockRequest.Setup(x => x.InvokeAsync(It.IsAny<IPipelineContext>())).Callback((IPipelineContext context) =>
            {
                // assert - Stage 1
                Assert.IsFalse(context.Items.ContainsKey(originalKey1), "originalKey1 still exists after it should be renamed.");
                Assert.IsFalse(context.Items.ContainsKey(originalKey2), "originalKey2 still exists after it should be renamed.");
                Assert.IsTrue(context.Items.ContainsKey(expectedKey1), "expectedKey1 does not exist after originalKey1 should be renamed.");
                Assert.IsTrue(context.Items.ContainsKey(expectedKey2), "expectedKey2 does not exist after originalKey2 should be renamed.");
                Assert.AreEqual(expectedValue1, context.Items[expectedKey1], "expectedValue1 should be the same as the originalValue1.");
                Assert.AreEqual(expectedValue2, context.Items[expectedKey2], "expectedValue2 should be the same as the originalValue2.");
            });

            using (IPipelineRequest next = mockRequest.Object)
            {
                // act
                using (var step = new RenameContextItemStep(logger, options, next))
                {
                    var context = new PipelineContext();
                    context.Items.Add(originalKey1, expectedValue1);
                    context.Items.Add(originalKey2, expectedValue2);

                    await step.InvokeAsync(context);

                    // assert - Stage 2
                    Assert.IsTrue(context.Items.ContainsKey(originalKey1), "expectedKey1 still exists after originalKey1 should be restored.");
                    Assert.IsTrue(context.Items.ContainsKey(originalKey2), "expectedKey2 still exists after originalKey2 should be restored.");
                    Assert.IsFalse(context.Items.ContainsKey(expectedKey1), "expectedKey1 still exists after originalKey1 should be restpred.");
                    Assert.IsFalse(context.Items.ContainsKey(expectedKey2), "expectedKey2 still exists after originalKey2 should be restored.");
                    Assert.AreEqual(expectedValue1, context.Items[originalKey1], "originalValue1 should be restored.");
                    Assert.AreEqual(expectedValue2, context.Items[originalKey2], "originalValie2 should be restored.");
                }
            }
        }

        [TestMethod]
        public async Task Returns_Task_From_DisposeAsync()
        {
            // arr
            ILogger<RenameContextItemStep> logger = new Mock<ILogger<RenameContextItemStep>>().Object;
            var options = new RenameContextItemOptions();

            using (IPipelineRequest next = new NoOpStep())
            {
                using (var source = new RenameContextItemStep(logger, options, next))
                {
                    // act
                    await source.DisposeAsync();

                    // assert
                    Assert.IsTrue(true);

                    // No exceptions mean this worked appropriately.
                }
            }
        }

        [TestMethod]
        public async Task Returns_Context_Errors_From_InvokeAsync_When_Exception_Is_Thrown_In_InvokeCoreAsync()
        {
            // arrange
            ILogger<RenameContextItemStep> logger = new Mock<ILogger<RenameContextItemStep>>().Object;
            var context = new PipelineContext();
            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context)).Throws(new InvalidCastException());
            IPipelineRequest next = mockNext.Object;
            var options = new RenameContextItemOptions();

            using (var step = new RenameContextItemStep(logger, options, next))
            {
                int errorCount = 1;

                // act
                await step.InvokeAsync(context);

                // assert
                Assert.AreEqual(errorCount, context.Errors.Count);
                Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidCastException));
            }
        }

        [TestMethod]
        public async Task Returns_Without_Error_From_InvokeAsync()
        {
            // arrange
            int errorCount = 0;
            ILogger<RenameContextItemStep> logger = new Mock<ILogger<RenameContextItemStep>>().Object;
            var options = new RenameContextItemOptions();

            using (IPipelineRequest next = new NoOpStep())
            {
                var context = new PipelineContext();

                using (var step = new RenameContextItemStep(logger, options, next))
                {
                    // act
                    await step.InvokeAsync(context);

                    // assert
                    Assert.AreEqual(errorCount, context.Errors.Count);
                }
            }
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Logger_Parameter_Is_Null()
        {
            // arrange
            ILogger<RenameContextItemStep> logger = null;
            var options = new RenameContextItemOptions();

            using (IPipelineRequest next = new NoOpStep())
            {
                string expectedParamName = nameof(logger);

                // act
                var result = Assert.ThrowsException<ArgumentNullException>(() => new RenameContextItemStep(logger, options, next));

                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedParamName, result.ParamName);
            }
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Next_Parameter_Is_Null()
        {
            // arrange
            ILogger<RenameContextItemStep> logger = new Mock<ILogger<RenameContextItemStep>>().Object;
            var options = new RenameContextItemOptions();
            IPipelineRequest next = null;
            string expectedParamName = nameof(next);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new RenameContextItemStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Options_Parameter_Is_Null()
        {
            // arrange
            ILogger<RenameContextItemStep> logger = new Mock<ILogger<RenameContextItemStep>>().Object;

            using (IPipelineRequest next = new NoOpStep())
            {
                RenameContextItemOptions options = null;
                string expectedParamName = nameof(options);

                // act
                var result = Assert.ThrowsException<ArgumentNullException>(() => new RenameContextItemStep(logger, options, next));

                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedParamName, result.ParamName);
            }
        }

        [TestMethod]
        public async Task Throws_ArgumentNullException_From_InvokeAsync_When_Context_Is_Null()
        {
            // arrange
            PipelineContext context = null;
            ILogger<RenameContextItemStep> logger = new Mock<ILogger<RenameContextItemStep>>().Object;
            var options = new RenameContextItemOptions();

            using (IPipelineRequest next = new NoOpStep())
            {
                string expectedParamName = nameof(context);

                using (var step = new RenameContextItemStep(logger, options, next))
                {
                    // act
                    var result = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => step.InvokeAsync(context));

                    // assert
                    Assert.IsNotNull(result);
                    Assert.AreEqual(expectedParamName, result.ParamName);
                }
            }
        }
    }
}
