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

namespace MyTrout.Pipelines.Tests
{
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class AbstractPipelineStepTests
    {
        public static Task NextAsync(PipelineContext context)
        {
            return Task.CompletedTask;
        }

        [TestMethod]
        public void Constructs_AbstractPipelineStep_Successfully()
        {
            // arrange
            ILogger<SampleStep> logger = new Mock<ILogger<SampleStep>>().Object;
            PipelineRequest next = AbstractPipelineStepTests.NextAsync;

            // act
            SampleStep result = new SampleStep(logger, next);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(logger, result.Logger);
            Assert.AreEqual(AbstractPipelineStepTests.NextAsync, result.NextItem);
        }

        [TestMethod]
        public void Constructs_AbstractPipelineStep_With_Options_Successfully()
        {
            // arrange
            ILogger<SampleStep> logger = new Mock<ILogger<SampleStep>>().Object;
            PipelineRequest next = AbstractPipelineStepTests.NextAsync;
            SampleStepOptions options = new SampleStepOptions();

            // act
            SampleStepWithOptions result = new SampleStepWithOptions(logger, options, next);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(logger, result.Logger);
            Assert.AreEqual(AbstractPipelineStepTests.NextAsync, result.NextItem);
            Assert.AreEqual(options, result.Options);
        }

#pragma warning disable VSTHRD200 // Suppressed because the member name is the suffix of the test method name.
        [TestMethod]
        public void Returns_Task_From_InvokeAsync()
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
        {
            // arrange
            ILogger<SampleStep> logger = new Mock<ILogger<SampleStep>>().Object;
            PipelineRequest next = AbstractPipelineStepTests.NextAsync;
            PipelineContext context = new PipelineContext();

            var step = new SampleStep(logger, next);

            // act
            var result = step.InvokeAsync(context);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(Task.CompletedTask, result);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Logger_Parameter_Is_Null()
        {
            // arrange
            ILogger<SampleStep> logger = null;
            PipelineRequest next = AbstractPipelineStepTests.NextAsync;
            string expectedParamName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new SampleStep(logger, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Next_Parameter_Is_Null()
        {
            // arrange
            ILogger<SampleStep> logger = new Mock<ILogger<SampleStep>>().Object;
            PipelineRequest next = null;
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
            ILogger<SampleStep> logger = new Mock<ILogger<SampleStep>>().Object;
            PipelineRequest next = AbstractPipelineStepTests.NextAsync;
            SampleStepOptions options = null;
            string expectedParamName = nameof(options);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new SampleStepWithOptions(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

#pragma warning disable VSTHRD200 // Test method name should reflect what it is testing, not Async.
        [TestMethod]
        public async Task Throws_ArgumentNullException_From_InvokeAsync_When_Context_Is_Null()
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
        {
            // arrange
            PipelineContext context = null;
            ILogger<SampleStep> logger = new Mock<ILogger<SampleStep>>().Object;
            PipelineRequest next = AbstractPipelineStepTests.NextAsync;
            string expectedParamName = nameof(context);

            var step = new SampleStep(logger, next);

            // act
            var result = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => step.InvokeAsync(context));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }
    }

    // Allow testing class here because it is used by no other class in the tests.
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable CA1801 // Remove unused parameter
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class SampleStep : AbstractPipelineStep<SampleStep>
    {
        public SampleStep(ILogger<SampleStep> logger, PipelineRequest next)
            : base(logger, next)
        {
            // no op
        }

        public PipelineRequest NextItem
        {
            get
            {
                return this.Next;
            }
        }

        protected override Task InvokeAsyncCore(PipelineContext context)
        {
            return Task.CompletedTask;
        }
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class SampleStepWithOptions : AbstractPipelineStep<SampleStep, SampleStepOptions>
    {
        public SampleStepWithOptions(ILogger<SampleStep> logger, SampleStepOptions options, PipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        public PipelineRequest NextItem
        {
            get
            {
                return this.Next;
            }
        }

        protected override Task InvokeAsyncCore(PipelineContext context)
        {
            return Task.CompletedTask;
        }
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class SampleStepOptions
    {
        public string Option1 { get; set; }
    }
#pragma warning restore CA1801 // Remove unused parameter
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CA1822 // Mark members as static
#pragma warning restore SA1402 // File may only contain a single type
}
