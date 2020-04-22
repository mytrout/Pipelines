// <copyright file="PipelineBuilderTests.cs" company="Chris Trout">
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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class PipelineBuilderTests
    {
        [TestMethod]
        public void Constructs_PipelineBuilder_Successfully()
        {
            // arrange
            string expectedMethodName = "InvokeAsync";

            // act
            var result = new PipelineBuilder();

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMethodName, result.MethodName);
        }

        [TestMethod]
        public void Returns_Correct_PipelineRequest_Delegate_From_Build_Method()
        {
            // arrange
            var nextRequest = new PipelineRequest(context => Task.CompletedTask);
            var mockStepActivator = new Mock<IStepActivator>();
            mockStepActivator.Setup(x => x.CreateInstance(It.IsAny<Type>(), It.IsAny<PipelineRequest>()))
                                            .Returns(new SampleWithInvokeAsyncMethod(nextRequest));
            IStepActivator stepActivator = mockStepActivator.Object;

            var sut = new PipelineBuilder().AddStep<SampleWithInvokeAsyncMethod>();

            var expectedTargetType = typeof(SampleWithInvokeAsyncMethod);

            // act
            var result = sut.Build(stepActivator);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedTargetType, result.Target.GetType());
        }

        [TestMethod]
        public async Task Returns_Correct_Message_From_ConstructedPipeline()
        {
            // arrange
            ILogger<StepActivator> logger = new Mock<ILogger<StepActivator>>().Object;
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
            IStepActivator stepActivator = new StepActivator(logger, serviceProvider);

            var pipeline = new PipelineBuilder()
                                        .AddStep<M1>()
                                        .AddStep<M2>()
                                        .AddStep<M3>()
                                        .Build(stepActivator);

            var expectedMessage = "Sponge Bob SquarePants";
            var pipelineContext = new PipelineContext();

            // act
            await pipeline.Invoke(pipelineContext).ConfigureAwait(false);

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
                    .AddStep<M1>();

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
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
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
        public void Throws_InvalidOperationException_From_AddStep_When_StepType_Does_Not_Contain_InvokeAsync_Method()
        {
            // arrange
            var sut = new PipelineBuilder();
            Type stepType = typeof(SampleWithoutInvokeAsyncMethod);
            string expectedMessage = Resources.METHOD_NOT_FOUND(CultureInfo.CurrentCulture, stepType.Name, sut.MethodName);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => sut.AddStep(stepType));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AddStep_When_Step_InvokeAsync_Method_Has_Wrong_ParameterType()
        {
            // arrange
            var sut = new PipelineBuilder();
            Type stepType = typeof(SampleWithWrongParameterType);
            string expectedMessage = Resources.METHOD_NOT_FOUND(CultureInfo.CurrentCulture, stepType.Name, sut.MethodName);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => sut.AddStep(stepType));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AddStep_When_Step_InvokeAsync_Method_Has_Too_Many_Parameters()
        {
            // arrange
            var sut = new PipelineBuilder();
            Type stepType = typeof(SampleWithTooManyParameters);
            string expectedMessage = Resources.METHOD_NOT_FOUND(CultureInfo.CurrentCulture, stepType.Name, sut.MethodName);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => sut.AddStep(stepType));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AddStep_When_Step_InvokeAsync_Method_No_Parameters()
        {
            // arrange
            var sut = new PipelineBuilder();
            Type stepType = typeof(SampleWithNoParameters);
            string expectedMessage = Resources.METHOD_NOT_FOUND(CultureInfo.CurrentCulture, stepType.Name, sut.MethodName);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => sut.AddStep(stepType));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_Build_When_StepActivator_Returns_Null()
        {
            // arrange
            var mockStepActivator = new Mock<IStepActivator>();
            mockStepActivator.Setup(x => x.CreateInstance(It.IsAny<Type>(), It.IsAny<PipelineRequest>())).Returns(null);
            IStepActivator stepActivator = mockStepActivator.Object;

            var sut = new PipelineBuilder()
                    .AddStep<M1>();

            string expectedMessage = Resources.SERVICEPROVIDER_LACKS_PARAMETER(CultureInfo.CurrentCulture, typeof(M1).Name);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => sut.Build(stepActivator));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_Build_When_StepActivator_Returns_Type_Without_InvokeAsync_Method()
        {
            // arrange
            var mockStepActivator = new Mock<IStepActivator>();
            mockStepActivator.Setup(x => x.CreateInstance(It.IsAny<Type>(), It.IsAny<PipelineRequest>())).Returns(new object());
            IStepActivator stepActivator = mockStepActivator.Object;

            var sut = new PipelineBuilder()
                    .AddStep<M1>();

            string expectedMessage = Resources.METHOD_NOT_FOUND(CultureInfo.CurrentCulture, "Object", sut.MethodName);

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
            mockStepActivator.Setup(x => x.CreateInstance(It.IsAny<Type>(), It.IsAny<PipelineRequest>())).Returns(new object());
            IStepActivator stepActivator = mockStepActivator.Object;

            var sut = new PipelineBuilder()
                    .AddStep<M1>();

            // Force a null into the PipelineBuilder's Step
            PropertyInfo propertyInfo = typeof(PipelineBuilder).GetProperty("StepTypes", BindingFlags.NonPublic | BindingFlags.Instance);
            var stack = propertyInfo.GetMethod.Invoke(sut, null) as Stack<Type>;
            stack.Push(null);

            string expectedMessage = Resources.NULL_MIDDLEWARE(CultureInfo.CurrentCulture);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => sut.Build(stepActivator));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }
    }

    // Allow multiple testing classes here because there are so many options to test.
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable CA1801 // Remove unused parameter
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class SampleWithoutInvokeAsyncMethod
    {
        public Task Invoke(PipelineContext context)
        {
            return Task.CompletedTask;
        }
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class SampleWithInvokeAsyncMethod
    {
        public SampleWithInvokeAsyncMethod(PipelineRequest next)
        {
            this.Next = next;
        }

        private PipelineRequest Next { get; }

        public Task InvokeAsync(PipelineContext context)
        {
            return this.Next.Invoke(context);
        }
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class M1
    {
        private readonly PipelineRequest next;

        public M1(PipelineRequest next) => this.next = next;

        protected virtual string Key => "Sponge";

        public Task InvokeAsync(PipelineContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Items.ContainsKey("Message"))
            {
                context.Items["Message"] += $" {this.Key}";
            }
            else
            {
                context.Items.Add("Message", this.Key);
            }

            return this.next(context);
        }
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class M2 : M1
    {
        public M2(PipelineRequest next)
            : base(next)
        {
            // no op
        }

        protected override string Key => "Bob";
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class M3 : M1
    {
        public M3(PipelineRequest next)
            : base(next)
        {
            // no op
        }

        protected override string Key => "SquarePants";
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class SampleWithWrongParameterType
    {
        public Task InvokeAsync(string context)
        {
            return Task.CompletedTask;
        }
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class SampleWithTooManyParameters
    {
        public Task InvokeAsync(PipelineContext context, string item2)
        {
            return Task.CompletedTask;
        }
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class SampleWithNoParameters
    {
        public Task InvokeAsync()
        {
            return Task.CompletedTask;
        }
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class SampleWithConstructorParameter
    {
        public SampleWithConstructorParameter(IDictionary<object, string> weirdParameter, PipelineRequest next)
        {
            this.WeirdParameter = weirdParameter ?? throw new ArgumentNullException(nameof(weirdParameter));
        }

        public PipelineRequest Next { get; }

        public IDictionary<object, string> WeirdParameter { get; }

        public Task InvokeAsync()
        {
            return Task.CompletedTask;
        }
    }
#pragma warning restore CA1801 // Remove unused parameter
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CA1822 // Mark members as static
#pragma warning restore SA1402 // File may only contain a single type

}