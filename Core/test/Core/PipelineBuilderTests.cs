// <copyright file="PipelineBuilderTests.cs" company="Chris Trout">
// MIT License
//
// Copyright © 2019-2021 Chris Trout
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
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines.Samples.Tests;
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
        public void Fires_StepAdded_Event_From_AddStep_When_GenericTypeParam_Is_Supplied()
        {
            // arrange
            var expectedType = typeof(SampleStep1);

            var builder = new PipelineBuilder();
            builder.AddStepAddedEventHandler(
                    (object sender, StepAddedEventArgs e) =>
                    {
                        // Note: This section is executed after the // act while still in-operation.
                        // assert
                        Assert.AreEqual(builder, sender);
                        Assert.AreEqual(e.CurrentStep.StepType, expectedType);
                        Assert.IsNull(e.CurrentStep.StepContext);
                    });

            // act
            builder.AddStep<SampleStep1>();
        }

        [TestMethod]
        public void Fires_StepAdded_Event_From_AddStep_When_StepType_Is_Supplied()
        {
            // arrange
            var expectedType = typeof(SampleStep2);

            var builder = new PipelineBuilder();
            builder.AddStepAddedEventHandler(
                (object sender, StepAddedEventArgs e) =>
                {
                    // Note: This section is executed after the // act while still in-operation.
                    // assert
                    Assert.AreEqual(builder, sender);
                    Assert.AreEqual(e.CurrentStep.StepType, expectedType);
                    Assert.IsNull(e.CurrentStep.StepContext);
                });

            // act
            builder.AddStep(expectedType);
        }

        [TestMethod]
        public async Task Returns_Correct_Message_From_ConstructedPipeline()
        {
            // arrange
            var mockStepActivator = new Mock<IStepActivator>();
            mockStepActivator.Setup(x => x.CreateInstance(It.Is<StepWithContext>(x => x.StepType == typeof(SampleStep1)), It.IsAny<IPipelineRequest>())).Returns((StepWithContext step, IPipelineRequest next) => { return new SampleStep1(next); });
            mockStepActivator.Setup(x => x.CreateInstance(It.Is<StepWithContext>(x => x.StepType == typeof(SampleStep2)), It.IsAny<IPipelineRequest>())).Returns((StepWithContext step, IPipelineRequest next) => { return new SampleStep2(next); });
            mockStepActivator.Setup(x => x.CreateInstance(It.Is<StepWithContext>(x => x.StepType == typeof(SampleStep3)), It.IsAny<IPipelineRequest>())).Returns((StepWithContext step, IPipelineRequest next) => { return new SampleStep3(next); });
            IStepActivator stepActivator = mockStepActivator.Object;

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
            string expectedMessage = Resources.TYPE_MUST_BE_REFERENCE(CultureInfo.CurrentCulture, "StepType");

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
            string expectedMessage = Resources.TYPE_MUST_BE_REFERENCE(CultureInfo.CurrentCulture, "StepType");
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
            string expectedMessage = Resources.TYPE_MUST_IMPLEMENT_IPIPELINEREQUEST(CultureInfo.CurrentCulture, "StepType");

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
            string expectedMessage = Resources.TYPE_MUST_IMPLEMENT_IPIPELINEREQUEST(CultureInfo.CurrentCulture, "StepType");
            string stepContext = "Step-Context-1";

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => sut.AddStep(stepType, stepContext));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_Build_When_A_StepType_Is_Null()
        {
            // TO FUTURE DEVELOPER: THIS TEST REQUIRES KNOWLEDGE OF CLASS INTERNALS.
            //                      IT IS INTENTIONALLY BRITTLE TO GUARANTEE THAT ANY
            //                      VALIDATION ISSUES ARE CAPTURED.

            // arrange
            IStepActivator stepActivator = new Mock<IStepActivator>().Object;

            var sut = new PipelineBuilder()
                    .AddStep<SampleStep1>();

            // Force a null into the PipelineBuilder's Step
            PropertyInfo propertyInfo = typeof(PipelineBuilder).GetProperty("StepTypes", BindingFlags.NonPublic | BindingFlags.Instance);
            var stepStack = propertyInfo.GetMethod.Invoke(sut, null) as Stack<StepWithContext>;
            stepStack.Push(new StepWithContextUsingNullStep());

            string expectedMessage = Resources.NULL_MIDDLEWARE(CultureInfo.CurrentCulture);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => sut.Build(stepActivator));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
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

        [TestMethod]
        public void Throws_InvalidOperationException_From_Build_When_StepTypes_Return_Null()
        {
            // TO FUTURE DEVELOPER: THIS TEST REQUIRES KNOWLEDGE OF CLASS INTERNALS.
            //                      IT IS INTENTIONALLY BRITTLE TO GUARANTEE THAT ANY
            //                      VALIDATION ISSUES ARE CAPTURED.

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

        [TestCategory("Integration")]
        [TestMethod]
        public async Task Validates_StepWithContext_Values_Passed_To_StepActivator_Using_Each_AddStep_Overload()
        {
            // arrange
            var config = new ConfigurationBuilder().AddJsonFile("test.json").Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<IConfiguration>(_ => config);
            serviceCollection.AddLogging();
            serviceCollection.AddTransient<IStepActivator, StepActivator>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            IStepActivator stepActivator = serviceProvider.GetRequiredService<IStepActivator>();

            var sut = new PipelineBuilder()
                            .AddStep<SampleStep1>()
                            .AddStep(typeof(SampleStep2))
                            .AddStep<SampleWithOptionsStep>("leading-context")
                            .AddStep(new StepWithContext(typeof(SampleWithOptionsStep), typeof(SampleOptions), "trailing-context"))
                            .AddStep(new StepWithContext(typeof(SampleWithOptionsStep), typeof(SampleOptions), stepContext: "json-context", "last-context"))
                            .AddStep<SampleStep3>();

            var expectedMessage = "Sponge Bob leading-connection-string trailing-connection-string last-connection-string SquarePants";
            var pipelineContext = new PipelineContext();

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