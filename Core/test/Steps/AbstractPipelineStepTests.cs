// <copyright file="AbstractPipelineStepTests.cs" company="Chris Trout">
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
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class AbstractPipelineStepTests
    {
        [TestMethod]
        public void Constructs_AbstractPipelineStep_Successfully()
        {
            // arrange
            ILogger<SampleStep> logger = new Mock<ILogger<SampleStep>>().Object;
            using (IPipelineRequest next = new NoOpStep())
            {
                // act
                using (var result = new SampleStep(logger, next))
                {
                    // assert
                    Assert.IsNotNull(result);
                    Assert.AreEqual(logger, result.Logger);
                    Assert.AreEqual(next, result.Next);
                }
            }
        }

        [TestMethod]
        public void Constructs_AbstractPipelineStep_With_Options_Successfully()
        {
            // arrange
            ILogger<SampleWithOptionsStep> logger = new Mock<ILogger<SampleWithOptionsStep>>().Object;
            using (IPipelineRequest next = new NoOpStep())
            {
                var options = new SampleOptions();

                // act
                using (var result = new SampleWithOptionsStep(logger, options, next))
                {
                    // assert
                    Assert.IsNotNull(result);
                    Assert.AreEqual(logger, result.Logger);
                    Assert.AreEqual(next, result.NextItem);
                    Assert.AreEqual(options, result.Options);
                }
            }
        }

        [TestMethod]
        public async Task Returns_Task_From_DisposeAsync()
        {
            // arrange
            ILogger<SampleStep> logger = new Mock<ILogger<SampleStep>>().Object;
            using (IPipelineRequest next = new NoOpStep())
            {
                using (var source = new SampleStep(logger, next))
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
            ILogger<SampleStep> logger = new Mock<ILogger<SampleStep>>().Object;
            var context = new PipelineContext();
            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context)).Throws(new InvalidCastException());
            IPipelineRequest next = mockNext.Object;

            using (var step = new SampleStep(logger, next))
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
            ILogger<SampleStep> logger = new Mock<ILogger<SampleStep>>().Object;
            using (IPipelineRequest next = new NoOpStep())
            {
                var context = new PipelineContext();

                using (var step = new SampleStep(logger, next))
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
            ILogger<SampleStep> logger = null;

            using (IPipelineRequest next = new NoOpStep())
            {
                string expectedParamName = nameof(logger);

                // act
                var result = Assert.ThrowsException<ArgumentNullException>(() => new SampleStep(logger, next));

                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedParamName, result.ParamName);
            }
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Next_Parameter_Is_Null()
        {
            // arrange
            ILogger<SampleStep> logger = new Mock<ILogger<SampleStep>>().Object;
            IPipelineRequest next = null;
            string expectedParamName = nameof(next);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new SampleStep(logger, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Options_Parameter_Is_Null()
        {
            // arrange
            ILogger<SampleWithOptionsStep> logger = new Mock<ILogger<SampleWithOptionsStep>>().Object;
            using (IPipelineRequest next = new NoOpStep())
            {
                SampleOptions options = null;
                string expectedParamName = nameof(options);

                // act
                var result = Assert.ThrowsException<ArgumentNullException>(() => new SampleWithOptionsStep(logger, options, next));

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
            ILogger<SampleStep> logger = new Mock<ILogger<SampleStep>>().Object;
            using (IPipelineRequest next = new NoOpStep())
            {
                string expectedParamName = nameof(context);

                using (var step = new SampleStep(logger, next))
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
