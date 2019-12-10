// <copyright file="PipelineMiddlewareInitializerTests.cs" company="Chris Trout">
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

namespace Cross.Pipelines.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Threading.Tasks;

    [TestClass]
    public class PipelineMiddlewareInitializerTests
    {
        [TestMethod]
        public void Builds_Middleware_Instance_From_InitializeMiddleware()
        {
            // arrange
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
            Type middlewareType = typeof(SampleWithInvokeAsyncMethod);
            var pipelineRequest = new PipelineRequest(context => Task.CompletedTask);

            var source = new PipelineMiddlewareInitializer(serviceProvider);

            // act
            var result = source.InitializeMiddleware(middlewareType, pipelineRequest);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, middlewareType);
        }

        [TestMethod]
        public void Constructs_PipelineMiddlewareInitializer_Successfully()
        {
            // arrange
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;

            // act
            var result = new PipelineMiddlewareInitializer(serviceProvider);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(serviceProvider, result.ServiceProvider);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_ServiceProvider_Is_Null()
        {
            // arrange
            IServiceProvider serviceProvider = null;
            string expectedParamName = nameof(serviceProvider);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new PipelineMiddlewareInitializer(serviceProvider));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_InitializeMiddleware_When_MiddlewareType_Is_Null()
        {
            // arrange
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
            Type middlewareType = null;
            var pipelineRequest = new PipelineRequest(context => Task.CompletedTask);

            var sut = new PipelineMiddlewareInitializer(serviceProvider);

            var expectedParamName = nameof(middlewareType);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => sut.InitializeMiddleware(middlewareType, pipelineRequest));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_InitializeMiddleware_When_MiddlewareType_Does_Not_Have_InvokeAsync_Method()
        {
            // arrange
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
            Type middlewareType = typeof(SampleWithoutInvokeAsyncMethod);
            var pipelineRequest = new PipelineRequest(context => Task.CompletedTask);

            var sut = new PipelineMiddlewareInitializer(serviceProvider);

            var expectedMessage = $"'{middlewareType.Name}' middleware does not contain a constructor that has a PipelineRequest parameter.";

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => sut.InitializeMiddleware(middlewareType, pipelineRequest));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }
    }
}
