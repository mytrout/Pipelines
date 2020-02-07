// <copyright file="MiddlewareActivatorTests.cs" company="Chris Trout">
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
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Globalization;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class MiddlewareActivatorTests
    {
        [TestMethod]
        public void Constructs_MiddlewareActivator_Successfully()
        {
            // arrange
            ILogger<MiddlewareActivator> logger = new Mock<ILogger<MiddlewareActivator>>().Object;
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;

            // act
            var result = new MiddlewareActivator(logger, serviceProvider);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(serviceProvider, result.ServiceProvider);
        }

        [TestMethod]
        public void Returns_Null_From_InitializeMiddleware_When_Middleware_Cannot_Be_Constructed()
        {
            // arrange
            var mockLogger = new Mock<ILogger<MiddlewareActivator>>();
            ILogger<MiddlewareActivator> logger = mockLogger.Object;
            int expectedLogMessages = 1;
            string expectedArgument0 = "Information";
            string expectedArgument2 = "SampleWithConstructorParameter(IDictionary`2, PipelineRequest) failed to intialize properly.";
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
            Type middlewareType = typeof(SampleWithConstructorParameter);
            var pipelineRequest = new PipelineRequest(context => Task.CompletedTask);

            var source = new MiddlewareActivator(logger, serviceProvider);

            // act
            var result = source.CreateInstance(middlewareType, pipelineRequest);

            // assert
            Assert.IsNull(result, "Result should be null because SampleWithConstructorParameter should not be buildable under those circumstances.");
            Assert.AreEqual(expectedLogMessages, mockLogger.Invocations.Count, "Expected only 1 log message.");
            Assert.AreEqual(expectedArgument0, mockLogger.Invocations[0].Arguments[0].ToString(), "Argument 0 should be 'Information' representing the logging level.");
            Assert.AreEqual(expectedArgument2, mockLogger.Invocations[0].Arguments[2].ToString(), "Argument 2 should have the same exception message text.");
        }

        [TestMethod]
        public void Returns_Valid_Middleware_Instance_From_InitializeMiddleware()
        {
            // arrange
            ILogger<MiddlewareActivator> logger = new Mock<ILogger<MiddlewareActivator>>().Object;
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
            Type middlewareType = typeof(SampleWithInvokeAsyncMethod);
            var pipelineRequest = new PipelineRequest(context => Task.CompletedTask);

            var source = new MiddlewareActivator(logger, serviceProvider);

            // act
            var result = source.CreateInstance(middlewareType, pipelineRequest);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, middlewareType);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Logger_Is_Null()
        {
            // arrange
            ILogger<MiddlewareActivator> logger = null;
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
            string expectedParamName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new MiddlewareActivator(logger, serviceProvider));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_ServiceProvider_Is_Null()
        {
            // arrange
            ILogger<MiddlewareActivator> logger = new Mock<ILogger<MiddlewareActivator>>().Object;
            IServiceProvider serviceProvider = null;
            string expectedParamName = nameof(serviceProvider);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new MiddlewareActivator(logger, serviceProvider));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_InitializeMiddleware_When_MiddlewareType_Is_Null()
        {
            // arrange
            ILogger<MiddlewareActivator> logger = new Mock<ILogger<MiddlewareActivator>>().Object;
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
            Type middlewareType = null;
            var nextRequest = new PipelineRequest(context => Task.CompletedTask);

            var sut = new MiddlewareActivator(logger, serviceProvider);

            var expectedParamName = nameof(middlewareType);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => sut.CreateInstance(middlewareType, nextRequest));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_InitializeMiddleware_When_PipelineRequest_Is_Null()
        {
            // arrange
            ILogger<MiddlewareActivator> logger = new Mock<ILogger<MiddlewareActivator>>().Object;
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
            var middlewareType = typeof(M1);
            PipelineRequest nextRequest = null;

            var sut = new MiddlewareActivator(logger, serviceProvider);

            var expectedParamName = nameof(nextRequest);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => sut.CreateInstance(middlewareType, nextRequest));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_InitializeMiddleware_When_MiddlewareType_Does_Not_Have_InvokeAsync_Method()
        {
            // arrange
            ILogger<MiddlewareActivator> logger = new Mock<ILogger<MiddlewareActivator>>().Object;
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
            Type middlewareType = typeof(SampleWithoutInvokeAsyncMethod);
            var pipelineRequest = new PipelineRequest(context => Task.CompletedTask);

            var sut = new MiddlewareActivator(logger, serviceProvider);

            var expectedMessage = $"'{middlewareType.Name}' middleware does not contain a constructor that has a PipelineRequest parameter.";

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => sut.CreateInstance(middlewareType, pipelineRequest));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }
    }
}
