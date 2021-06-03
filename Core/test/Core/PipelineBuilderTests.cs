// <copyright file="PipelineBuilderTests.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Core.Tests
{
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines.Samples.Tests;
    using MyTrout.Pipelines.Steps;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class PipelineBuilderTests
    {
        [TestMethod]
        public void Does_Not_Throw_NullReferenceException_From_AddStep_When_StepContext_And_GenericTypeParam_Are_Supplied_And_And_There_Are_No_StepAdded_Event_Listeners()
        {
            // arrange
            var stepContext = "context-55";

            var builder = new PipelineBuilder();

            try
            {
                // act
                builder.AddStep<SampleStep2>(stepContext);
            }
            catch (Exception exc)
            {
                // assert
                Assert.Fail(exc.ToString());
            }
        }

        [TestMethod]
        public void Does_Not_Throw_NullReferenceException_From_AddStep_When_StepContext_And_StepType_Are_Supplied_And_There_Are_No_StepAdded_Event_Listeners()
        {
            // arrange
            var stepContext = "context-56";
            var expectedStepType = typeof(SampleStep2);

            var builder = new PipelineBuilder();

            try
            {
                // act
                builder.AddStep(expectedStepType, stepContext);
            }
            catch (Exception exc)
            {
                // assert
                Assert.Fail(exc.ToString());
            }
        }

        [TestMethod]
        public void Fires_StepAdded_Event_From_AddStep_When_GenericTypeParam_Is_Supplied()
        {
            // arrange
            var expectedType = typeof(SampleStep1);

            var builder = new PipelineBuilder();
            builder.StepAdded += (object sender, StepAddedEventArgs e) =>
                {
                    // Note: This section is executed after the // act while still in-operation.
                    // assert
                        Assert.AreEqual(builder, sender);
                        Assert.AreEqual(e.CurrentStep.StepType, expectedType);
                        Assert.IsNull(e.CurrentStep.StepContext);
                };

            // act
            builder.AddStep<SampleStep1>();
        }

        [TestMethod]
        public void Fires_StepAdded_Event_From_AddStep_When_StepType_And_StepContext_Are_Supplied()
        {
            // arrange
            var expectedStepContext = "context-482983";
            var expectedStepType = typeof(SampleStep2);

            var builder = new PipelineBuilder();
            builder.StepAdded += (object sender, StepAddedEventArgs e) =>
            {
                // Note: This section is executed after the // act while still in-operation.
                // assert
                Assert.AreEqual(builder, sender);
                Assert.AreEqual(e.CurrentStep.StepType, expectedStepType);
                Assert.AreEqual(e.CurrentStep.StepContext, expectedStepContext);
            };

            // act
            builder.AddStep(expectedStepType, expectedStepContext);
        }

        [TestMethod]
        public void Fires_StepAdded_Event_From_AddStep_When_StepType_Is_Supplied()
        {
            // arrange
            var expectedType = typeof(SampleStep2);

            var builder = new PipelineBuilder();
            builder.StepAdded += (object sender, StepAddedEventArgs e) =>
            {
                // Note: This section is executed after the // act while still in-operation.
                // assert
                Assert.AreEqual(builder, sender);
                Assert.AreEqual(e.CurrentStep.StepType, expectedType);
                Assert.IsNull(e.CurrentStep.StepContext);
            };

            // act
            builder.AddStep(expectedType);
        }

        [TestMethod]
        public void Returns_Correct_PipelineRequest_Delegate_From_Build_Method()
        {
            // arrange
            var nextRequest = new NoOpStep();
            var logger = new Mock<ILogger<SampleWithInvokeAsyncMethodStep>>().Object;

            var mockStepActivator = new Mock<IStepActivator>();
            mockStepActivator.Setup(x => x.CreateInstance(It.IsAny<StepWithContext>(), It.IsAny<IPipelineRequest>()))
                                            .Returns(new SampleWithInvokeAsyncMethodStep(logger, nextRequest));
            IStepActivator stepActivator = mockStepActivator.Object;

            var sut = new PipelineBuilder().AddStep<SampleWithInvokeAsyncMethodStep>();

            var expectedTargetType = typeof(SampleWithInvokeAsyncMethodStep);

            // act
            var result = sut.Build(stepActivator);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, expectedTargetType);
        }

        [TestMethod]
        public async Task Returns_Correct_Message_From_ConstructedPipeline()
        {
            // arrange
            ILogger<StepActivator> logger = new Mock<ILogger<StepActivator>>().Object;
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
            IStepActivator stepActivator = new StepActivator(logger, serviceProvider);

            var pipeline = new PipelineBuilder()
                                        .AddStep<SampleStep1>()
                                        .AddStep<SampleStep2>()
                                        .AddStep<SampleStep3>()
                                        .Build(stepActivator);

            var expectedMessage = "Sponge Bob SquarePants";
            var pipelineContext = new PipelineContext();

            // act
            await pipeline.InvokeAsync(pipelineContext).ConfigureAwait(false);

            // assert
            Assert.IsTrue(pipelineContext.Items.ContainsKey("Message"), "No 'Message' item was added to the PipelineContext.");
            Assert.AreEqual(expectedMessage, pipelineContext.Items["Message"]);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Build_When_StepActivator_Is_Null()
        {
            // arrange
            IStepActivator stepActivator = null;

            var sut = new PipelineBuilder()
                    .AddStep<SampleStep1>();

            string expectedParamName = nameof(stepActivator);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => sut.Build(stepActivator));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AddStep_When_StepType_Is_A_Value_Type()
        {
            // arrange
            var sut = new PipelineBuilder();
            Type stepType = typeof(int);
            string expectedMessage = Resources.TYPE_MUST_BE_REFERENCE(CultureInfo.CurrentCulture, nameof(stepType));

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => sut.AddStep(stepType));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AddStep_When_StepType_Is_A_Value_Type_And_StepContext_Is_Specified()
        {
            // arrange
            var sut = new PipelineBuilder();
            Type stepType = typeof(int);
            string expectedMessage = Resources.TYPE_MUST_BE_REFERENCE(CultureInfo.CurrentCulture, nameof(stepType));
            string stepContext = "Step-Context-1";

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => sut.AddStep(stepType, stepContext));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AddStep_When_StepType_Does_Not_Implement_IPipelineBuilder()
        {
            // arrange
            var sut = new PipelineBuilder();
            Type stepType = typeof(SampleWithoutIPipelineRequestStep);
            string expectedMessage = Resources.TYPE_MUST_IMPLEMENT_IPIPELINEREQUEST(CultureInfo.CurrentCulture, nameof(stepType));

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => sut.AddStep(stepType));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AddStep_When_StepType_Does_Not_Implement_IPipelineBuilder_And_StepContext_Is_Specified()
        {
            // arrange
            var sut = new PipelineBuilder();
            Type stepType = typeof(SampleWithoutIPipelineRequestStep);
            string expectedMessage = Resources.TYPE_MUST_IMPLEMENT_IPIPELINEREQUEST(CultureInfo.CurrentCulture, nameof(stepType));
            string stepContext = "Step-Context-1";

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => sut.AddStep(stepType, stepContext));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AddStep_When_StepType_Is_Null()
        {
            // arrange
            var sut = new PipelineBuilder();
            Type stepType = null;
            string expectedParamName = nameof(stepType);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => sut.AddStep(stepType));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_Build_When_StepActivator_Returns_Null()
        {
            // arrange
            var mockStepActivator = new Mock<IStepActivator>();
            mockStepActivator.Setup(x => x.CreateInstance(It.IsAny<StepWithContext>(), It.IsAny<IPipelineRequest>())).Returns(null);
            IStepActivator stepActivator = mockStepActivator.Object;

            var sut = new PipelineBuilder()
                    .AddStep<SampleStep1>();

            string expectedMessage = Resources.TYPE_MUST_IMPLEMENT_IPIPELINEREQUEST(CultureInfo.CurrentCulture, nameof(SampleStep1));

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => sut.Build(stepActivator));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        // TO FUTURE DEVELOPER: THIS TEST REQUIRES KNOWLEDGE OF CLASS INTERNALS.
        //                      IT IS INTENTIONALLY BRITTLE TO GUARANTEE THAT ANY
        //                      VALIDATION ISSUES ARE CAPTURED.
        [TestMethod]
        public void Throws_InvalidOperationException_From_Build_When_StepTypes_Return_Null()
        {
            // arrange
            var mockStepActivator = new Mock<IStepActivator>();
            mockStepActivator.Setup(x => x.CreateInstance(It.IsAny<StepWithContext>(), It.IsAny<IPipelineRequest>())).Returns(new object());
            IStepActivator stepActivator = mockStepActivator.Object;

            var sut = new PipelineBuilder()
                    .AddStep<SampleStep1>();

            // Force a null into the PipelineBuilder's Step
            PropertyInfo propertyInfo = typeof(PipelineBuilder).GetProperty("StepTypes", BindingFlags.NonPublic | BindingFlags.Instance);
            var stack = propertyInfo.GetMethod.Invoke(sut, null) as Stack<StepWithContext>;
            stack.Push(null);

            string expectedMessage = Resources.NULL_MIDDLEWARE(CultureInfo.CurrentCulture);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => sut.Build(stepActivator));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public async Task Validates_StepWithContext_Values_Passed_To_StepActivator_Using_Each_AddStep_OVerload()
        {
            // arrange
            var logger = new Mock<ILogger<StepActivator>>().Object;

            Dictionary<string, SampleOptions> options = new Dictionary<string, SampleOptions>()
            {
                { "leading-context", new SampleOptions("leading-context") },
                { "trailing-context", new SampleOptions("trailing-context") }
            };

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IDictionary<string, SampleOptions>))).Returns(options);
            var serviceProvider = mockServiceProvider.Object;

            IStepActivator stepActivator = new StepActivator(logger, serviceProvider);

            var sut = new PipelineBuilder()
                            .AddStep<SampleStep1>()
                            .AddStep(typeof(SampleStep2))
                            .AddStep<SampleWithOptionsStep>("leading-context")
                            .AddStep<SampleWithOptionsStep>("trailing-context")
                            .AddStep<SampleStep3>();

            var expectedMessage = "Sponge Bob leading-context trailing-context SquarePants";
            PipelineContext pipelineContext = new PipelineContext();

            // act
            IPipelineRequest result = sut.Build(stepActivator);

            // assert
            await result.InvokeAsync(pipelineContext).ConfigureAwait(false);

            // assert
            Assert.IsTrue(pipelineContext.Items.ContainsKey("Message"), "No 'Message' item was added to the PipelineContext.");
            Assert.AreEqual(expectedMessage, pipelineContext.Items["Message"]);
        }
    }
}