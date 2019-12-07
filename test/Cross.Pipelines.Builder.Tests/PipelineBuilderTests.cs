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

namespace Cross.Pipelines.Builder.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Threading.Tasks;

    [TestClass]
    public class PipelineBuilderTests
    {
        [TestMethod]
        public void Constructs_PipelineBuilder_Successfully()
        {
            // arrange
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
            string expectedMethodName = "InvokeAsync";

            // act
            var result = new PipelineBuilder(serviceProvider);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMethodName, result.MethodName);
            Assert.AreEqual(serviceProvider, result.ServiceProvider);
        }

        [TestMethod]
        public void Returns_Correct_PipelineRequest_Delegate_From_Build_Method()
        {
            // arrange
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
            var sut = new PipelineBuilder(serviceProvider).AddMiddleware<SampleWithInvokeAsyncMethod>();

            var expectedTargetType = typeof(SampleWithInvokeAsyncMethod);

            // act
            var result = sut.Build();

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedTargetType, result.Target.GetType());
        }

        [TestMethod]
        public async Task Returns_Correct_Message_From_ConstructedPipeline()
        {
            // arrange
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
            var pipeline = new PipelineBuilder(serviceProvider)
                                .AddMiddleware<M1>()
                                .AddMiddleware<M2>()
                                .AddMiddleware<M3>()
                                .Build();

            var expectedMessage = "Sponge Bob SquarePants";
            var pipelineContext = new PipelineContext("Constructed Pipeline");

            // act
            await pipeline.Invoke(pipelineContext).ConfigureAwait(false);

            // assert
            Assert.IsTrue(pipelineContext.Items.ContainsKey("Message"), "No 'Message' item was added to the PipelineContext.");
            Assert.AreEqual(expectedMessage, pipelineContext.Items["Message"]);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AddMiddleware_When_MiddlewareType_Is_A_Value_Type()
        {
            // arrange
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
            var sut = new PipelineBuilder(serviceProvider);
            Type middlewareType = typeof(int);
            string expectedMessage = "middlewareType must be a reference type.";

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => sut.AddMiddleware(middlewareType));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AddMiddleware_When_MiddlewareType_Does_Not_Contain_InvokeAsync_Method()
        {
            // arrange
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
            var sut = new PipelineBuilder(serviceProvider);
            Type middlewareType = typeof(SampleWithoutInvokeAsyncMethod);
            string expectedMessage = "Middleware must contain an InvokeAsync method with one parameter of type 'PipelineContext'.";

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => sut.AddMiddleware(middlewareType));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AddMiddleware_When_Middleware_InvokeAsync_Method_Has_Wrong_ParameterType()
        {
            // arrange
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
            var sut = new PipelineBuilder(serviceProvider);
            Type middlewareType = typeof(SampleWithWrongParameterType);
            string expectedMessage = "Middleware must contain an InvokeAsync method with one parameter of type 'PipelineContext'.";

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => sut.AddMiddleware(middlewareType));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AddMiddleware_When_Middleware_InvokeAsync_Method_Has_Too_Many_Parameters()
        {
            // arrange
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
            var sut = new PipelineBuilder(serviceProvider);
            Type middlewareType = typeof(SampleWithTooManyParameters);
            string expectedMessage = "Middleware must contain an InvokeAsync method with one parameter of type 'PipelineContext'.";

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => sut.AddMiddleware(middlewareType));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AddMiddleware_When_Middleware_InvokeAsync_Method_No_Parameters()
        {
            // arrange
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
            var sut = new PipelineBuilder(serviceProvider);
            Type middlewareType = typeof(SampleWithNoParameters);
            string expectedMessage = "Middleware must contain an InvokeAsync method with one parameter of type 'PipelineContext'.";

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => sut.AddMiddleware(middlewareType));

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
    public class SampleWithoutInvokeAsyncMethod
    {
        public Task Invoke(PipelineContext context)
        {
            return Task.CompletedTask;
        }
    }

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

    public class M2 : M1
    {
        public M2(PipelineRequest next)
            : base(next)
        {
            // no op
        }

        protected override string Key => "Bob";
    }

    public class M3 : M1
    {
        public M3(PipelineRequest next)
            : base(next)
        {
            // no op
        }

        protected override string Key => "SquarePants";
    }

    public class SampleWithWrongParameterType
    {
        public Task InvokeAsync(string context)
        {
            return Task.CompletedTask;
        }
    }

    public class SampleWithTooManyParameters
    {
        public Task InvokeAsync(PipelineContext context, string item2)
        {
            return Task.CompletedTask;
        }
    }

    public class SampleWithNoParameters
    {
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
