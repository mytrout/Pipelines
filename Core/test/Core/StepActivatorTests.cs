// <copyright file="StepActivatorTests.cs" company="Chris Trout">
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

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class StepActivatorTests
    {
        [TestMethod]
        public void Constructs_StepActivator_Successfully()
        {
            // arrange
            ILogger<StepActivator> logger = new Mock<ILogger<StepActivator>>().Object;
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;

            // act
            var result = new StepActivator(logger, serviceProvider);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(serviceProvider, result.ServiceProvider);
        }

        [TestMethod]
        public void Returns_Null_From_CreateInstance_When_Step_Cannot_Be_Constructed()
        {
            // arrange
            var mockLogger = new Mock<ILogger<StepActivator>>();
            ILogger<StepActivator> logger = mockLogger.Object;
            int expectedLogMessages = 1;
            string expectedArgument0 = "Information";
            string expectedArgument2 = "SampleWithConstructorParameterStep(IDictionary`2, IPipelineRequest) failed to intialize properly due to one of the following reasons: no IPipelineRequest parameter was found, an injected parameter was null, StepContext was null, or StepContext did not contain a non-null value for the appropriate context.";
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
            Type stepType = typeof(SampleWithConstructorParameterStep);
            var pipelineRequest = new NoOpStep();
            string stepContext = null;

            var source = new StepActivator(logger, serviceProvider);

            // act
            var result = source.CreateInstance(new StepWithContext(stepType, stepContext), pipelineRequest);

            // assert
            Assert.IsNull(result, "Result should be null because SampleWithConstructorParameterStep should not be buildable under those circumstances.");
            Assert.AreEqual(expectedLogMessages, mockLogger.Invocations.Count, "Expected only 1 log message.");
            Assert.AreEqual(expectedArgument0, mockLogger.Invocations[0].Arguments[0].ToString(), "Argument 0 should be 'Information' representing the logging level.");
            Assert.AreEqual(expectedArgument2, mockLogger.Invocations[0].Arguments[2].ToString(), "Argument 2 should have the same exception message text.");
        }

        [TestMethod]
        public void Returns_Valid_Step_Instance_From_CreateInstance()
        {
            // arrange
            ILogger<StepActivator> logger = new Mock<ILogger<StepActivator>>().Object;
            ILogger<SampleWithLoggerStep> stepLogger = new Mock<ILogger<SampleWithLoggerStep>>().Object;

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(ILogger<SampleWithLoggerStep>))).Returns(stepLogger);
            IServiceProvider serviceProvider = mockServiceProvider.Object;

            Type stepType = typeof(SampleWithLoggerStep);
            var pipelineRequest = new NoOpStep();
            string stepContext = null;

            var source = new StepActivator(logger, serviceProvider);

            // act
            var result = source.CreateInstance(new StepWithContext(stepType, stepContext), pipelineRequest);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, stepType);
        }

        [TestMethod]
        public void Returns_Valid_Step_Instance_From_CreateInstance_When_Multiple_Contexts_Are_Defined()
        {
            // arrange
            var items = new Dictionary<string, SampleOptions>()
            {
                { "first-context", new SampleOptions("first-context") },
                { "last-context", new SampleOptions("last-context") }
            };

            ILogger<StepActivator> logger = new Mock<ILogger<StepActivator>>().Object;
            Mock<IServiceProvider> mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IDictionary<string, SampleOptions>)))
                                .Returns(items);
            var serviceProvider = mockServiceProvider.Object;

            var pipelineRequest = new NoOpStep();

            var source = new StepActivator(logger, serviceProvider);

            // act
            var result = source.CreateInstance(new StepWithContext(typeof(SampleWithOptionsStep), "first-context"), pipelineRequest);

            // assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Logger_Is_Null()
        {
            // arrange
            ILogger<StepActivator> logger = null;
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
            string expectedParamName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new StepActivator(logger, serviceProvider));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_ServiceProvider_Is_Null()
        {
            // arrange
            ILogger<StepActivator> logger = new Mock<ILogger<StepActivator>>().Object;
            IServiceProvider serviceProvider = null;
            string expectedParamName = nameof(serviceProvider);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new StepActivator(logger, serviceProvider));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_CreateInstance_When_StepType_Is_Null()
        {
            // arrange
            ILogger<StepActivator> logger = new Mock<ILogger<StepActivator>>().Object;
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
            Type stepType = null;
            var nextRequest = new NoOpStep();
            string stepContext = null;

            var sut = new StepActivator(logger, serviceProvider);

            var expectedParamName = nameof(stepType);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => sut.CreateInstance(new StepWithContext(stepType, stepContext), nextRequest));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_CreateInstance_When_PipelineRequest_Is_Null()
        {
            // arrange
            ILogger<StepActivator> logger = new Mock<ILogger<StepActivator>>().Object;
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
            var stepType = typeof(SampleStep1);
            IPipelineRequest nextRequest = null;
            string stepContext = null;

            var sut = new StepActivator(logger, serviceProvider);

            var expectedParamName = nameof(nextRequest);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => sut.CreateInstance(new StepWithContext(stepType, stepContext), nextRequest));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_CreateInstance_When_StepType_Does_Not_Have_InvokeAsync_Method()
        {
            // arrange
            ILogger<StepActivator> logger = new Mock<ILogger<StepActivator>>().Object;
            IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;
            Type stepType = typeof(SampleWithoutNextInConstructorStep);
            var pipelineRequest = new NoOpStep();
            string stepContext = null;

            var sut = new StepActivator(logger, serviceProvider);

            var expectedMessage = $"'{stepType.Name}' step does not contain a constructor that has a PipelineRequest parameter.";

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => sut.CreateInstance(new StepWithContext(stepType, stepContext), pipelineRequest));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_CreateInstance_When_Contexts_Is_Not_Defined()
        {
            // arrange
            var items = new Dictionary<string, SampleOptions>()
            {
                { "first-context", new SampleOptions("first-context") },
                { "last-context", new SampleOptions("last-context") }
            };

            ILogger<StepActivator> logger = new Mock<ILogger<StepActivator>>().Object;
            Mock<IServiceProvider> mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IDictionary<string, SampleOptions>)))
                                .Returns(items);
            var serviceProvider = mockServiceProvider.Object;

            var pipelineRequest = new NoOpStep();

            var source = new StepActivator(logger, serviceProvider);
            string expectedMessage = Resources.STEP_CONTEXT_NOT_FOUND(CultureInfo.CurrentCulture, typeof(SampleOptions).Name, typeof(SampleWithOptionsStep).Name);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => source.CreateInstance(new StepWithContext(typeof(SampleWithOptionsStep), "second-context"), pipelineRequest));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_CreateInstance_When_Context_Is_Null()
        {
            // arrange
            ILogger<StepActivator> logger = new Mock<ILogger<StepActivator>>().Object;
            Mock<IServiceProvider> mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IDictionary<string, SampleOptions>)))
                                .Returns(null);
            var serviceProvider = mockServiceProvider.Object;

            var pipelineRequest = new NoOpStep();

            var source = new StepActivator(logger, serviceProvider);
            string expectedMessage = Resources.STEP_CONTEXT_NOT_FOUND(CultureInfo.CurrentCulture, typeof(SampleOptions).Name, typeof(SampleWithOptionsStep).Name);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => source.CreateInstance(new StepWithContext(typeof(SampleWithOptionsStep), "second-context"), pipelineRequest));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }
    }
}