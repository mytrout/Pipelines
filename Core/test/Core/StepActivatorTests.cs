// <copyright file="StepActivatorTests.cs" company="Chris Trout">
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
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines.Samples.Tests;
    using MyTrout.Pipelines.Steps;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Security.Cryptography.X509Certificates;

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
            Assert.AreEqual(logger, result.Logger);
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

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IDictionary<object, string>))).Returns(null as object);
            var serviceProvider = mockServiceProvider.Object;

            Type stepType = typeof(SampleWithConstructorParameterStep);

            using (var pipelineRequest = new NoOpStep())
            {
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
            using (var pipelineRequest = new NoOpStep())
            {
                string stepContext = null;

                var source = new StepActivator(logger, serviceProvider);

                // act
                var result = source.CreateInstance(new StepWithContext(stepType, stepContext), pipelineRequest);

                // assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, stepType);
            }
        }

        [TestMethod]
        public void Returns_Valid_Step_Instance_From_CreateInstance_When_Options_Uses_FromServices_Attribute()
        {
            // arrange
            ILogger<StepActivator> logger = new Mock<ILogger<StepActivator>>().Object;

            var config = new ConfigurationBuilder().AddJsonFile("test-type.json").Build();

            var contextNameBuilder = new Mock<IContextNameBuilder>().Object;

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IConfiguration))).Returns(config);
            mockServiceProvider.Setup(x => x.GetService(typeof(IContextNameBuilder))).Returns(contextNameBuilder);

            IServiceProvider serviceProvider = mockServiceProvider.Object;

            Type stepType = typeof(SampleWithFromServicesAttributeStep);
            using (var pipelineRequest = new NoOpStep())
            {
                string stepContext = null;

                var source = new StepActivator(logger, serviceProvider);

                // act
                var result = source.CreateInstance(new StepWithContext(stepType, stepContext), pipelineRequest);

                // assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, stepType);

                var typedResult = result as SampleWithFromServicesAttributeStep;
                Assert.IsNotNull(typedResult, "result is not SampleWithFromServicesAttributeOptionsStep.");
                Assert.IsNotNull(typedResult.Options.ContextNameBuilder, "Options.ContextNameBuilder is null.");
            }
        }

        [TestMethod]
        public void Returns_Valid_Step_Instance_From_CreateInstance_When_Options_Uses_FromServices_Attribute_And_Default_Property_Value_Is_Supplied()
        {
            // arrange
            ILogger<StepActivator> logger = new Mock<ILogger<StepActivator>>().Object;

            var config = new ConfigurationBuilder().AddJsonFile("test-type.json").Build();

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IConfiguration))).Returns(config);
            IServiceProvider serviceProvider = mockServiceProvider.Object;

            Type stepType = typeof(SampleWithFromServicesAttributeAndDefaultPropertyValueStep);
            using (var pipelineRequest = new NoOpStep())
            {
                string stepContext = null;

                var source = new StepActivator(logger, serviceProvider);

                // act
                var result = source.CreateInstance(new StepWithContext(stepType, stepContext), pipelineRequest);

                // assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, stepType);

                var typedResult = result as SampleWithFromServicesAttributeAndDefaultPropertyValueStep;
                Assert.IsNotNull(typedResult, "result is not SampleWithFromServicesAttributeAndDefaultPropertyValueStep.");
                Assert.IsNotNull(typedResult.Options.ContextNameBuilder, "Options.ContextNameBuilder is null.");
            }
        }

        [TestMethod]
        public void Returns_Valid_Step_Instance_From_CreateInstance_When_StepWithContext_Containing_ConfigKeys_Is_Used()
        {
            // arrange
            string configContext = "last-context";
            string stepContext = "leading-context";
            var config = new ConfigurationBuilder().AddJsonFile("test.json").Build();

            var mockLogger = new Mock<ILogger<StepActivator>>();
            var logger = mockLogger.Object;

            ILogger<SampleWithOptionsStep> stepLogger = new Mock<ILogger<SampleWithOptionsStep>>().Object;
            int expectedLogMessages = 0;

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(ILogger<SampleWithOptionsStep>))).Returns(stepLogger);
            mockServiceProvider.Setup(x => x.GetService(typeof(IConfiguration))).Returns(config);
            var serviceProvider = mockServiceProvider.Object;

            using (var pipelineRequest = new NoOpStep())
            {
                var source = new StepActivator(logger, serviceProvider);
                var step = new StepWithContext(typeof(SampleWithOptionsStep), typeof(SampleOptions), stepContext: stepContext, configContext);

                // act
                var result = source.CreateInstance(step, pipelineRequest);

                // assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(SampleWithOptionsStep));
                Assert.AreEqual("last-connection-string", (result as SampleWithOptionsStep).Options.ConnectionString);
                Assert.AreEqual(expectedLogMessages, mockLogger.Invocations.Count, "Expected only 1 log message.");
            }
        }

        [TestMethod]
        public void Returns_Valid_Step_Instance_From_CreateInstance_When_StepWithContext_Containing_Whitespace_ConfigKeys_Is_Used()
        {
            // arrange
            string configContext = "last-context";
            string stepContext = "leading-context";
            string whitespaceContext = "    ";

            var config = new ConfigurationBuilder().AddJsonFile("test.json").Build();

            var mockLogger = new Mock<ILogger<StepActivator>>();
            var logger = mockLogger.Object;

            ILogger<SampleWithOptionsStep> stepLogger = new Mock<ILogger<SampleWithOptionsStep>>().Object;
            int expectedLogMessages = 0;

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(ILogger<SampleWithOptionsStep>))).Returns(stepLogger);
            mockServiceProvider.Setup(x => x.GetService(typeof(IConfiguration))).Returns(config);
            var serviceProvider = mockServiceProvider.Object;

            using (var pipelineRequest = new NoOpStep())
            {
                var source = new StepActivator(logger, serviceProvider);
                var step = new StepWithContext(typeof(SampleWithOptionsStep), typeof(SampleOptions), stepContext: stepContext, whitespaceContext, configContext);

                // act
                var result = source.CreateInstance(step, pipelineRequest);

                // assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(SampleWithOptionsStep));
                Assert.AreEqual("last-connection-string", (result as SampleWithOptionsStep).Options.ConnectionString);
                Assert.AreEqual(expectedLogMessages, mockLogger.Invocations.Count, "Expected only 1 log message.");
            }
        }

        [TestMethod]
        public void Returns_Valid_Step_Instance_From_CreateInstance_When_TypeContext_Is_Used()
        {
            // arrange
            var config = new ConfigurationBuilder().AddJsonFile("test-type.json").Build();

            var mockLogger = new Mock<ILogger<StepActivator>>();
            var logger = mockLogger.Object;

            ILogger<SampleWithOptionsStep> stepLogger = new Mock<ILogger<SampleWithOptionsStep>>().Object;
            int expectedLogMessages = 0;

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(ILogger<SampleWithOptionsStep>))).Returns(stepLogger);
            mockServiceProvider.Setup(x => x.GetService(typeof(IConfiguration))).Returns(config);
            var serviceProvider = mockServiceProvider.Object;

            using (var pipelineRequest = new NoOpStep())
            {
                var source = new StepActivator(logger, serviceProvider);

                var step = new StepWithContext(typeof(SampleWithOptionsStep), typeof(SampleOptions), null);

                // act
                var result = source.CreateInstance(step, pipelineRequest);

                // assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(SampleWithOptionsStep));
                Assert.AreEqual("sample-options-connection-string", (result as SampleWithOptionsStep).Options.ConnectionString);
                Assert.AreEqual(expectedLogMessages, mockLogger.Invocations.Count, "Expected only 1 log message.");
            }
        }

        [TestMethod]
        public void Returns_Valid_Step_Instance_From_CreateInstance_When_StepWithFactory_Is_Used()
        {
            // arrange
            var options = new SampleOptions("special-connection-string-step-with-implementation-factory");

            Func<IServiceProvider, SampleOptions> implementationFactory = (IServiceProvider services) =>
                                        {
                                            return options;
                                        };

            ILogger<StepActivator> logger = new Mock<ILogger<StepActivator>>().Object;
            ILogger<SampleWithOptionsStep> stepLogger = new Mock<ILogger<SampleWithOptionsStep>>().Object;

            IConfiguration config = new Mock<IConfiguration>().Object;

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IConfiguration))).Returns(config);
            mockServiceProvider.Setup(x => x.GetService(typeof(ILogger<SampleWithOptionsStep>))).Returns(stepLogger);
            var serviceProvider = mockServiceProvider.Object;

            using (var pipelineRequest = new NoOpStep())
            {
                var source = new StepActivator(logger, serviceProvider);

                // act
                var result = source.CreateInstance(new StepWithFactory<SampleWithOptionsStep, SampleOptions>("first-context", implementationFactory), pipelineRequest);

                // assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(SampleWithOptionsStep));
                Assert.AreEqual(options, (result as SampleWithOptionsStep).Options);
                Assert.AreEqual(options.ConnectionString, (result as SampleWithOptionsStep).Options.ConnectionString);
            }
        }

        [TestMethod]
        public void Returns_Valid_Step_Instance_From_CreateInstance_When_StepWithInstance_Is_Used()
        {
            // arrange
            var options = new SampleOptions("special-connection-string-step-with-instance");

            ILogger<StepActivator> logger = new Mock<ILogger<StepActivator>>().Object;
            ILogger<SampleWithOptionsStep> stepLogger = new Mock<ILogger<SampleWithOptionsStep>>().Object;

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(ILogger<SampleWithOptionsStep>))).Returns(stepLogger);
            var serviceProvider = mockServiceProvider.Object;

            using (var pipelineRequest = new NoOpStep())
            {
                var source = new StepActivator(logger, serviceProvider);

                // act
                var result = source.CreateInstance(new StepWithInstance<SampleWithOptionsStep, SampleOptions>("first-context", options), pipelineRequest);

                // assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(SampleWithOptionsStep));
                Assert.AreEqual(options, (result as SampleWithOptionsStep).Options);
                Assert.AreEqual(options.ConnectionString, (result as SampleWithOptionsStep).Options.ConnectionString);
            }
        }

        [TestMethod]
        public void Returns_Valid_Step_Instance_And_Logger_Warning_From_CreateInstance_When_StepWithContext_Containing_ConfigKeys_With_Missing_Key_Is_Used()
        {
            // arrange
            string missingContext = "missing-context";
            string configContext = "last-context";
            string stepContext = "leading-context";
            var config = new ConfigurationBuilder().AddJsonFile("test.json").Build();

            var mockLogger = new Mock<ILogger<StepActivator>>();
            var logger = mockLogger.Object;

            ILogger<SampleWithOptionsStep> stepLogger = new Mock<ILogger<SampleWithOptionsStep>>().Object;
            int expectedLogMessages = 1;
            string expectedArgument0 = "Warning";
            string expectedArgument2 = MyTrout.Pipelines.Resources.MISSING_CONFIGKEY(CultureInfo.CurrentCulture, missingContext, nameof(SampleOptions), nameof(SampleWithOptionsStep), stepContext);

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(ILogger<SampleWithOptionsStep>))).Returns(stepLogger);
            mockServiceProvider.Setup(x => x.GetService(typeof(IConfiguration))).Returns(config);
            var serviceProvider = mockServiceProvider.Object;

            using (var pipelineRequest = new NoOpStep())
            {
                var source = new StepActivator(logger, serviceProvider);
                var step = new StepWithContext(typeof(SampleWithOptionsStep), typeof(SampleOptions), stepContext, configContext, missingContext);

                // act
                var result = source.CreateInstance(step, pipelineRequest);

                // assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(SampleWithOptionsStep));
                Assert.AreEqual("last-connection-string", (result as SampleWithOptionsStep).Options.ConnectionString);
                Assert.AreEqual(expectedLogMessages, mockLogger.Invocations.Count, "Expected only 1 log message.");
                Assert.AreEqual(expectedArgument0, mockLogger.Invocations[0].Arguments[0].ToString(), "Argument 0 should be 'Warning' representing the logging level.");
                Assert.AreEqual(expectedArgument2, mockLogger.Invocations[0].Arguments[2].ToString(), "Argument 2 should have the same exception message text.");
            }
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
            using (var nextRequest = new NoOpStep())
            {
                string stepContext = null;

                var sut = new StepActivator(logger, serviceProvider);

                var expectedParamName = nameof(stepType);

                // act
                var result = Assert.ThrowsException<ArgumentNullException>(() => sut.CreateInstance(new StepWithContext(stepType, stepContext), nextRequest));

                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedParamName, result.ParamName);
            }
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
        public void Throws_InvalidOperationException_From_CreateInstance_When_IConfiguration_Returns_Null_For_StepContext_Config()
        {
            // arrange
            string stepContext = "context-was-here";

            ILogger<StepActivator> logger = new Mock<ILogger<StepActivator>>().Object;
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection(stepContext)).Returns(null as IConfigurationSection);
            var config = mockConfig.Object;

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IConfiguration))).Returns(config);
            var serviceProvider = mockServiceProvider.Object;

            Type stepType = typeof(SampleWithOptionsStep);

            using (var pipelineRequest = new NoOpStep())
            {
                var sut = new StepActivator(logger, serviceProvider);

                var expectedMessage = Resources.STEP_CONTEXT_NOT_FOUND(CultureInfo.CurrentCulture, typeof(SampleOptions).Name, stepType.Name);

                // act
                var result = Assert.ThrowsException<InvalidOperationException>(() => sut.CreateInstance(new StepWithContext(stepType, stepContext), pipelineRequest));

                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedMessage, result.Message);
            }
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_CreateInstance_When_StepType_Does_Not_Have_InvokeAsync_Method()
        {
            // arrange
            ILogger<StepActivator> logger = new Mock<ILogger<StepActivator>>().Object;
            ILogger<SampleWithoutNextInConstructorStep> stepLogger = new Mock<ILogger<SampleWithoutNextInConstructorStep>>().Object;
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(ILogger<SampleWithoutNextInConstructorStep>))).Returns(stepLogger);

            var serviceProvider = mockServiceProvider.Object;
            Type stepType = typeof(SampleWithoutNextInConstructorStep);

            using (var pipelineRequest = new NoOpStep())
            {
                string stepContext = null;

                var sut = new StepActivator(logger, serviceProvider);

                var expectedMessage = $"'{stepType.Name}' step does not contain a constructor that has a PipelineRequest parameter.";

                // act
                var result = Assert.ThrowsException<InvalidOperationException>(() => sut.CreateInstance(new StepWithContext(stepType, stepContext), pipelineRequest));

                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedMessage, result.Message);
            }
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_CreateInstance_When_FromServices_Property_Does_Not_Have_A_Setter()
        {
            // arrange
            ILogger<StepActivator> logger = new Mock<ILogger<StepActivator>>().Object;

            var config = new ConfigurationBuilder().AddJsonFile("test-type.json").Build();

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IConfiguration))).Returns(config);
            IServiceProvider serviceProvider = mockServiceProvider.Object;

            Type stepType = typeof(SampleWithFromServicesAttributeWithNoSetterStep);
            using (var pipelineRequest = new NoOpStep())
            {
                string stepContext = null;

                var sut = new StepActivator(logger, serviceProvider);

                var expectedMessage = Resources.FROMSERVICES_PROPERTY_MUST_HAVE_SETTER(
                                                    CultureInfo.InvariantCulture,
                                                    nameof(SampleWithFromServicesAttributeWithNoSetterOptions.Configuration),
                                                    nameof(SampleWithFromServicesAttributeWithNoSetterOptions),
                                                    nameof(SampleWithFromServicesAttributeWithNoSetterStep));

                // act
                var result = Assert.ThrowsException<InvalidOperationException>(() => sut.CreateInstance(new StepWithContext(stepType, stepContext), pipelineRequest));

                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedMessage, result.Message);
            }
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_CreateInstance_When_FromServices_Dependency_Injected_Property_Does_Not_Exist_In_Service_Provider()
        {
            // arrange
            ILogger<StepActivator> logger = new Mock<ILogger<StepActivator>>().Object;

            var config = new ConfigurationBuilder().AddJsonFile("test-type.json").Build();

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IConfiguration))).Returns(config);
            IServiceProvider serviceProvider = mockServiceProvider.Object;

            Type stepType = typeof(SampleWithFromServicesAttributeStep);
            using (var pipelineRequest = new NoOpStep())
            {
                string stepContext = null;

                var sut = new StepActivator(logger, serviceProvider);

                var expectedMessage = Resources.SERVICEPROVIDER_LACKS_PARAMETER(
                                                    CultureInfo.InvariantCulture,
                                                    nameof(SampleWithFromServicesAttributeOptions),
                                                    nameof(SampleWithFromServicesAttributeOptions.ContextNameBuilder),
                                                    nameof(SampleWithFromServicesAttributeStep));

                // act
                var result = Assert.ThrowsException<InvalidOperationException>(() => sut.CreateInstance(new StepWithContext(stepType, stepContext), pipelineRequest));

                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedMessage, result.Message);
            }
        }
    }
}