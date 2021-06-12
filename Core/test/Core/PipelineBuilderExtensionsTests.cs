// <copyright file="PipelineBuilderExtensionsTests.cs" company="Chris Trout">
// MIT License
//
// Copyright © 2021 Chris Trout
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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines.Samples.Tests;
    using System;
    using System.Globalization;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class PipelineBuilderExtensionsTests
    {
        [TestMethod]
        public void Adds_Correct_Step_To_PipelineBuilder_From_AddStep_T_With_No_Parameters()
        {
            // arrange
            var expectedStepType = typeof(SampleStep1);

            var builder = new PipelineBuilder();
            builder.AddStepAddedEventHandler(
                    (object sender, StepAddedEventArgs e) =>
                    {
                        // Note: This section is executed after the // act while still in-operation.
                        // assert
                        Assert.AreEqual(builder, sender);
                        Assert.AreEqual(e.CurrentStep.StepType, expectedStepType);
                        Assert.IsNull(e.CurrentStep.StepContext);
                        Assert.IsNull(e.CurrentStep.StepDependencyType);
                    });

            // act
            builder.AddStep<SampleStep1>();
        }

        [TestMethod]
        public void Adds_Correct_Step_To_PipelineBuilder_From_AddStep_T_With_StepContext()
        {
            // arrange
            var expectedStepContext = Guid.NewGuid().ToString("n", CultureInfo.InvariantCulture);
            var expectedStepType = typeof(SampleStep1);

            var builder = new PipelineBuilder();
            builder.AddStepAddedEventHandler(
                    (object sender, StepAddedEventArgs e) =>
                    {
                        // Note: This section is executed after the // act while still in-operation.
                        // assert
                        Assert.AreEqual(builder, sender);
                        Assert.AreEqual(expectedStepType, e.CurrentStep.StepType);
                        Assert.AreEqual(expectedStepContext, e.CurrentStep.StepContext);
                        Assert.IsNull(e.CurrentStep.StepDependencyType);
                    });

            // act
            builder.AddStep<SampleStep1>(expectedStepContext);
        }

        [TestMethod]
        public void Adds_Correct_Step_To_PipelineBuilder_From_AddStep_T_With_DependencyType_And_StepContext()
        {
            // arrange
            var expectedStepContext = Guid.NewGuid().ToString("n", CultureInfo.InvariantCulture);
            var expectedStepType = typeof(SampleStep1);
            var expectedDependencyType = typeof(SampleOptions);

            var builder = new PipelineBuilder();
            builder.AddStepAddedEventHandler(
                    (object sender, StepAddedEventArgs e) =>
                    {
                        // Note: This section is executed after the // act while still in-operation.
                        // assert
                        Assert.AreEqual(builder, sender);
                        Assert.AreEqual(expectedStepType, e.CurrentStep.StepType);
                        Assert.AreEqual(expectedStepContext, e.CurrentStep.StepContext);
                        Assert.AreEqual(expectedDependencyType, e.CurrentStep.StepDependencyType);
                    });

            // act
            builder.AddStep<SampleStep1>(expectedDependencyType, expectedStepContext);
        }

        [TestMethod]
        public void Adds_Correct_Step_To_PipelineBuilder_From_AddStep_T_With_Generic_DependencyType_And_StepContext()
        {
            // arrange
            var expectedStepContext = Guid.NewGuid().ToString("n", CultureInfo.InvariantCulture);
            var expectedStepType = typeof(SampleStep1);
            var expectedDependencyType = typeof(SampleOptions);

            var builder = new PipelineBuilder();
            builder.AddStepAddedEventHandler(
                    (object sender, StepAddedEventArgs e) =>
                    {
                        // Note: This section is executed after the // act while still in-operation.
                        // assert
                        Assert.AreEqual(builder, sender);
                        Assert.AreEqual(expectedStepType, e.CurrentStep.StepType);
                        Assert.AreEqual(expectedStepContext, e.CurrentStep.StepContext);
                        Assert.AreEqual(expectedDependencyType, e.CurrentStep.StepDependencyType);
                    });

            // act
            builder.AddStep<SampleStep1, SampleOptions>(expectedStepContext);
        }

        [TestMethod]
        public async Task Returns_Correct_Message_From_ConstructedPipeline_Using_AddStep_T_With_No_Parameters()
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
    }
}