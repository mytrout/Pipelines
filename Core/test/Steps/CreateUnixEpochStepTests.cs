// <copyright file="CreateUnixEpochStepTests.cs" company="Chris Trout">
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
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CreateUnixEpochStepTests
    {
        [TestMethod]
        public void Constructs_CreateUnixEpochStep_Successfully()
        {
            // arrange
            ILogger<CreateUnixEpochStep> logger = new Mock<ILogger<CreateUnixEpochStep>>().Object;
            CreateUnixEpochOptions options = new();
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            // act
            using (var result = new CreateUnixEpochStep(logger, options, next))
            {
                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(logger, result.Logger);
                Assert.AreEqual(next, result.Next);
            }
        }

        [TestMethod]
        public async Task Returns_A_Unix_Epoch_In_Seconds_In_Pipeline_Context()
        {
            // arrange
            PipelineContext context = new();
            long minimumEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            await Task.Delay(1000);

            ILogger<CreateUnixEpochStep> logger = new Mock<ILogger<CreateUnixEpochStep>>().Object;
            CreateUnixEpochOptions options = new();
            Mock<IPipelineRequest> mockNext = new();
            mockNext.Setup(x => x.InvokeAsync(It.IsAny<IPipelineContext>())).Callback(() =>
            {
                // assert
                Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.UNIX_EPOCH), "UNIX_EPOCH item does not exist in context.");

                // Cannot use Task.Delay here.
                System.Threading.Thread.Sleep(1000);

                long maximumEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                long testEpoch = (long)context.Items[PipelineContextConstants.UNIX_EPOCH];
                Assert.IsTrue(testEpoch > minimumEpoch, "UNIX_EPOCH '{0}' must be greater than our minimumEpoch '{1}'.", testEpoch, minimumEpoch);
                Assert.IsTrue(testEpoch < maximumEpoch, "UNIX_EPOCH '{0}' must be less than our maximumEpoch '{1}'.", testEpoch, maximumEpoch);
            });

            var next = mockNext.Object;

            using (var sut = new CreateUnixEpochStep(logger, options, next))
            {
                // act
                await sut.InvokeAsync(context);

                // assert
                if (context.Errors.Count > 0)
                {
                    // Because the exception is surfaced if there is one.
                    Assert.IsTrue(context.Errors.Count == 0, context.Errors[0].ToString());
                }
            }
        }

        [TestMethod]
        public async Task Returns_A_Unix_Epoch_In_Milliseconds_In_Pipeline_Context()
        {
            // arrange
            PipelineContext context = new();
            long minimumEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            await Task.Delay(7);

            ILogger<CreateUnixEpochStep> logger = new Mock<ILogger<CreateUnixEpochStep>>().Object;

            CreateUnixEpochOptions options = new()
            {
                EpochKind = UnixEpochKind.InMilliseconds
            };

            Mock<IPipelineRequest> mockNext = new();

            mockNext.Setup(x => x.InvokeAsync(It.IsAny<IPipelineContext>())).Callback(() =>
            {
                // assert
                Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.UNIX_EPOCH), "UNIX_EPOCH item does not exist in context.");

                // Cannot use Task.Delay here.
                System.Threading.Thread.Sleep(7);

                long maximumEpoch = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                long testEpoch = (long)context.Items[PipelineContextConstants.UNIX_EPOCH];
                Assert.IsTrue(testEpoch > minimumEpoch, "UNIX_EPOCH '{0}' must be greater than our minimumEpoch '{1}'.", testEpoch, minimumEpoch);
                Assert.IsTrue(testEpoch < maximumEpoch, "UNIX_EPOCH '{0}' must be less than our maximumEpoch '{1}'.", testEpoch, maximumEpoch);
            });

            var next = mockNext.Object;

            using (var sut = new CreateUnixEpochStep(logger, options, next))
            {
                // act
                await sut.InvokeAsync(context);

                // assert
                if (context.Errors.Count > 0)
                {
                    // Because the exception is thrown if there is one.
                    Assert.IsTrue(context.Errors.Count == 0, context.Errors[0].ToString());
                }
            }
        }

        [TestMethod]
        public async Task Returns_Context_With_No_Alterations_From_InvokeAsync_After_Execution()
        {
            // arrange
            int errorCount = 0;
            ILogger<CreateUnixEpochStep> logger = new Mock<ILogger<CreateUnixEpochStep>>().Object;
            CreateUnixEpochOptions options = new();
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            using (var sut = new CreateUnixEpochStep(logger, options, next))
            {
                var context = new PipelineContext();
                var contextName = Guid.NewGuid().ToString();
                var contextValue = Guid.NewGuid();
                context.Items.Add(contextName, contextValue);

                var expectedItemsCount = 1;

                // act
                await sut.InvokeAsync(context);

                // assert
                Assert.AreEqual(errorCount, context.Errors.Count);
                Assert.AreEqual(expectedItemsCount, context.Items.Count);
                Assert.IsTrue(context.Items.ContainsKey(contextName), "context does contain a key named '{0}'.", contextName);
                Assert.AreEqual(contextValue, context.Items[contextName], "context does contain a key named '{0}' with a value of '{1}'.", contextName, contextValue);
            }
        }

        [TestMethod]
        public async Task Returns_Task_From_DisposeAsync()
        {
            // arrange
            ILogger<CreateUnixEpochStep> logger = new Mock<ILogger<CreateUnixEpochStep>>().Object;
            CreateUnixEpochOptions options = new();
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            using (var source = new CreateUnixEpochStep(logger, options, next))
            {
                // act
                await source.DisposeAsync();

                // assert
                Assert.IsTrue(true);

                // No exceptions mean this worked appropriately.
            }
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Logger_Is_Null()
        {
            // arrange
            ILogger<CreateUnixEpochStep> logger = null;
            CreateUnixEpochOptions options = new();
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            string expectedParamName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new CreateUnixEpochStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Next_Is_Null()
        {
            // arrange
            ILogger<CreateUnixEpochStep> logger = new Mock<ILogger<CreateUnixEpochStep>>().Object;
            CreateUnixEpochOptions options = new();
            IPipelineRequest next = null;

            string expectedParamName = nameof(next);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new CreateUnixEpochStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Options_Is_Null()
        {
            // arrange
            ILogger<CreateUnixEpochStep> logger = new Mock<ILogger<CreateUnixEpochStep>>().Object;
            CreateUnixEpochOptions options = null;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            string expectedParamName = nameof(options);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new CreateUnixEpochStep(logger, options, next));

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

            ILogger<CreateUnixEpochStep> logger = new Mock<ILogger<CreateUnixEpochStep>>().Object;
            CreateUnixEpochOptions options = new();
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            using (var source = new CreateUnixEpochStep(logger, options, next))
            {
                // act
                var result = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => source.InvokeAsync(context));

                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedParamName, result.ParamName);
            }
        }
    }
}