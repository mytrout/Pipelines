// <copyright file="WriteMessageToAzureServiceTopicStepTests.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.Azure.ServiceBus.Tests
{
    using global::Azure.Messaging.ServiceBus;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Core;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage]
    [TestClass]
    public class WriteMessageToAzureTopicStepTests
    {
        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs, PipelineContext context)
        {
            context.Errors.Add(exceptionReceivedEventArgs.Exception);
            return Task.CompletedTask;
        }

        [TestMethod]
        public void Constructs_WriteMessageToAzureServiceTopicStep_Successfully()
        {
            // arrange
            ILogger<WriteMessageToAzureTopicStep> logger = new Mock<ILogger<WriteMessageToAzureTopicStep>>().Object;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;
            var options = new WriteMessageToAzureTopicOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                TopicName = Tests.TestConstants.WriteToTopicName
            };

            // act
            var result = new WriteMessageToAzureTopicStep(logger, options, next);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(logger, result.Logger);
            Assert.AreEqual(options, result.Options);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Message_Needs_To_Be_Sent()
        {
            // arrange
            ILogger<WriteMessageToAzureTopicStep> logger = new Mock<ILogger<WriteMessageToAzureTopicStep>>().Object;

            Stream inputStream = new MemoryStream(Encoding.UTF8.GetBytes("Hello Woeld."));
            Guid correlationId = Guid.NewGuid();
            bool isActive = true;

            var context = new PipelineContext();
            context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, inputStream);
            context.Items.Add(WriteMessageToAzureTopicStep.CORRELATION_ID, correlationId.ToString());
            context.Items.Add("IsActive", isActive);

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new WriteMessageToAzureTopicOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                TopicName = Tests.TestConstants.WriteToTopicName,
                ApplicationProperties = new List<string>()
                                        {
                                            "IsActive",
                                            "ID"
                                        }
            };

            string subscriptionName = Tests.TestConstants.SubscriptionName;

            const int errorCount = 0;

            var source = new WriteMessageToAzureTopicStep(logger, options, next);

            // act
            await source.InvokeAsync(context);

            await source.DisposeAsync();

            // assert
            Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_STREAM));
            Assert.AreEqual(errorCount, context.Errors.Count);

            // cleanup
            try
            {
                var subscriptionClient = new SubscriptionClient(
                                            options.AzureServiceBusConnectionString,
                                            options.TopicName,
                                            subscriptionName,
                                            ReceiveMode.PeekLock,
                                            RetryPolicy.Default);

                // The lambda allows the Pipeline context to be passed in while preserving the standard method signature.
                var messageHandlerOptions = new MessageHandlerOptions((ExceptionReceivedEventArgs args) =>
                                                    WriteMessageToAzureTopicStepTests.ExceptionReceivedHandler(args, context))
                {
                    // Allow 1 concurrent call to simplify pipeline processing.
                    MaxConcurrentCalls = 1,
                    // Handle the message completion manually within the ProcessMessageAsync() call.
                    AutoComplete = false
                };

                subscriptionClient.RegisterMessageHandler(
                            async (ServiceBusMessage message, CancellationToken token) =>
                            {
                                Assert.AreEqual(isActive, message.UserProperties["IsActive"]);
                                Assert.AreEqual(correlationId, message.UserProperties[WriteMessageToAzureTopicStep.CORRELATION_ID]);
                                await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken).ConfigureAwait(false);
                            },
                            messageHandlerOptions);

                // Allow the message to be read from the subscription prior to ending the execution of this method.
                await Task.Delay(500);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_Using_FileStream()
        {
            // arrange
            ILogger<WriteMessageToAzureTopicStep> logger = new Mock<ILogger<WriteMessageToAzureTopicStep>>().Object;

            Stream inputStream = new FileStream($"{ AppDomain.CurrentDomain.BaseDirectory}text.json", FileMode.Open, FileAccess.Read, FileShare.Read);

            var context = new PipelineContext();
            context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, inputStream);

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new WriteMessageToAzureTopicOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                TopicName = Tests.TestConstants.WriteToTopicName
            };

            string subscriptionName = Tests.TestConstants.SubscriptionName;

            const int errorCount = 0;

            var source = new WriteMessageToAzureTopicStep(logger, options, next);

            // act
            await source.InvokeAsync(context);

            await source.DisposeAsync();

            // assert
            Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_STREAM));
            Assert.AreEqual(errorCount, context.Errors.Count);

            // cleanup
            try
            {
                var subscriptionClient = new SubscriptionClient(
                                            options.AzureServiceBusConnectionString,
                                            options.TopicName,
                                            subscriptionName,
                                            ReceiveMode.PeekLock,
                                            RetryPolicy.Default);

                // The lambda allows the Pipeline context to be passed in while preserving the standard method signature.
                var messageHandlerOptions = new MessageHandlerOptions((ExceptionReceivedEventArgs args) =>
                                                    WriteMessageToAzureTopicStepTests.ExceptionReceivedHandler(args, context))
                {
                    // Allow 1 concurrent call to simplify pipeline processing.
                    MaxConcurrentCalls = 1,
                    // Handle the message completion manually within the ProcessMessageAsync() call.
                    AutoComplete = false
                };

                subscriptionClient.RegisterMessageHandler(
                            async (ServiceBusMessage message, CancellationToken token) =>
                            {
                                await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken).ConfigureAwait(false);
                            },
                            messageHandlerOptions);

                // Allow the message to be read from the subscription prior to ending the execution of this method.
                await Task.Delay(500);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_No_SendMessage_Is_Supplied_In_PipelineContext()
        {
            // arrange
            ILogger<WriteMessageToAzureTopicStep> logger = new Mock<ILogger<WriteMessageToAzureTopicStep>>().Object;

            var context = new PipelineContext();

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new WriteMessageToAzureTopicOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                TopicName = Tests.TestConstants.WriteToTopicName
            };

            const int errorCount = 1;

            var source = new WriteMessageToAzureTopicStep(logger, options, next);

            string expectedMessage = MyTrout.Pipelines.Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, PipelineContextConstants.OUTPUT_STREAM);
            // act
            await source.InvokeAsync(context);

            // assert
            Assert.IsFalse(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_STREAM));
            Assert.AreEqual(errorCount, context.Errors.Count);
            Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidOperationException));
            Assert.AreEqual(expectedMessage, context.Errors[0].Message);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Options_Is_Null()
        {
            // arrange
            ILogger<WriteMessageToAzureTopicStep> logger = new Mock<ILogger<WriteMessageToAzureTopicStep>>().Object;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;
            WriteMessageToAzureTopicOptions options = null;
            const string paramName = nameof(options);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new WriteMessageToAzureTopicStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(paramName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentException_From_Constructor_When_TopicClient_Creation_Throws_Exception()
        {
            // arrange
            ILogger<WriteMessageToAzureTopicStep> logger = new Mock<ILogger<WriteMessageToAzureTopicStep>>().Object;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;
            
            var options = new WriteMessageToAzureTopicOptions()
            {
                AzureServiceBusConnectionString = "something that isn't a connection string",
                TopicName = Tests.TestConstants.MissingTopicName
            };
            
            const string paramName = "connectionString";

            // act
            var result = Assert.ThrowsException<ArgumentException>(() => new WriteMessageToAzureTopicStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(paramName, result.ParamName);
        }
    }
}
