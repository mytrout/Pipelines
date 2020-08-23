// <copyright file="ReadMessageFromAzureSubscriptionStepTests.cs" company="Chris Trout">
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
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Core;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class ReadMessageFromAzureSubscriptionStepTests
    {
        [TestMethod]
        public async Task Abandons_Message_Lock_From_InvokeAsync_When_Exception_Is_Added_To_Pipeline_Context()
        {
            // arrange
            ILogger<ReadMessageFromAzureSubscriptionStep> logger = new Mock<ILogger<ReadMessageFromAzureSubscriptionStep>>().Object;

            PipelineContext context = new PipelineContext();

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context))
                                        .Callback(() => context.Errors.Add(new InvalidTimeZoneException()))
                                        .Returns(Task.CompletedTask);
            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureSubscriptionOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                TopicName = Tests.TestConstants.AbandonMessageTopicName,
                SubscriptionName = Tests.TestConstants.SubscriptionName,
                TimeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 3),
                TimeToWaitForNewMessage = new TimeSpan(0, 0, 3),
            };

            // sending message to be read.
            ITopicClient topicClient = new TopicClient(options.AzureServiceBusConnectionString, options.TopicName, RetryPolicy.Default);
            byte[] messageBody = Encoding.UTF8.GetBytes("Are you there?");

            Message message = new Message(messageBody);

            await topicClient.SendAsync(message).ConfigureAwait(false);
            await topicClient.CloseAsync().ConfigureAwait(false);

            var source = new ReadMessageFromAzureSubscriptionStep(logger, next, options);

            // act
            await source.InvokeAsync(context);

            // assert
            Assert.AreEqual(1, context.Errors.Count);
            Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidTimeZoneException));

            // cleanup
            await source.DisposeAsync();

            try
            {
                var subscriptionClient = new SubscriptionClient(
                                            options.AzureServiceBusConnectionString,
                                            options.TopicName,
                                            options.SubscriptionName,
                                            ReceiveMode.PeekLock,
                                            RetryPolicy.Default);

                // The lambda allows the Pipeline context to be passed in while preserving the standard method signature.
                var messageHandlerOptions = new MessageHandlerOptions((ExceptionReceivedEventArgs args) => { return Task.CompletedTask; })
                {
                    // Allow 1 concurrent call to simplify pipeline processing.
                    MaxConcurrentCalls = 1,
                    // Handle the message completion manually within the ProcessMessageAsync() call.
                    AutoComplete = false
                };

                subscriptionClient.RegisterMessageHandler(
                            async (Message message, CancellationToken token) =>
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
        public async Task Abandons_Message_Lock_From_InvokeAsync_When_Exception_Is_Thrown_By_InvokeAsync()
        {
            // arrange
            ILogger<ReadMessageFromAzureSubscriptionStep> logger = new Mock<ILogger<ReadMessageFromAzureSubscriptionStep>>().Object;

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("Previous Message"));

            PipelineContext context = new PipelineContext();
            context.Items.Add(PipelineContextConstants.INPUT_STREAM, stream);

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Throws((new InvalidTimeZoneException()));
            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureSubscriptionOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                TopicName = Tests.TestConstants.AbandonMessageThrownTopicName,
                SubscriptionName = Tests.TestConstants.SubscriptionName,
                TimeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 3),
                TimeToWaitForNewMessage = new TimeSpan(0, 0, 3),
            };

            // sending message to be read.
            ITopicClient topicClient = new TopicClient(options.AzureServiceBusConnectionString, options.TopicName, RetryPolicy.Default);
            byte[] messageBody = Encoding.UTF8.GetBytes("Are you there?");

            Message message = new Message(messageBody);

            await topicClient.SendAsync(message).ConfigureAwait(false);
            await topicClient.CloseAsync().ConfigureAwait(false);

            var source = new ReadMessageFromAzureSubscriptionStep(logger, next, options);

            // act
            await source.InvokeAsync(context);

            // assert
            Assert.AreEqual(1, context.Errors.Count);
            Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidTimeZoneException));
            Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.INPUT_STREAM));
            Assert.AreEqual(stream, context.Items[PipelineContextConstants.INPUT_STREAM]);

            // cleanup
            await source.DisposeAsync();

            try
            {
                var subscriptionClient = new SubscriptionClient(
                                            options.AzureServiceBusConnectionString,
                                            options.TopicName,
                                            options.SubscriptionName,
                                            ReceiveMode.PeekLock,
                                            RetryPolicy.Default);

                // The lambda allows the Pipeline context to be passed in while preserving the standard method signature.
                var messageHandlerOptions = new MessageHandlerOptions((ExceptionReceivedEventArgs args) => { return Task.CompletedTask; })
                {
                    // Allow 1 concurrent call to simplify pipeline processing.
                    MaxConcurrentCalls = 1,
                    // Handle the message completion manually within the ProcessMessageAsync() call.
                    AutoComplete = false
                };

                subscriptionClient.RegisterMessageHandler(
                            async (Message message, CancellationToken token) =>
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
        public async Task Cancels_Message_Processing_From_InvokeAsync_When_EvaluateCancellationOfMessageAsync_Returns_True()
        {
            // arrange
            ILogger<ReadMessageFromAzureSubscriptionStep> logger = new Mock<ILogger<ReadMessageFromAzureSubscriptionStep>>().Object;

            PipelineContext context = new PipelineContext();

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureSubscriptionOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                TopicName = Tests.TestConstants.EvaluateTokenTopicName,
                SubscriptionName = Tests.TestConstants.SubscriptionName,
                TimeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 3),
                TimeToWaitForNewMessage = new TimeSpan(0, 0, 3),
            };

            // sending message to be read.
            ITopicClient topicClient = new TopicClient(options.AzureServiceBusConnectionString, options.TopicName, RetryPolicy.Default);
            byte[] messageBody = Encoding.UTF8.GetBytes("Are you there?");

            Message message = new Message(messageBody);

            await topicClient.SendAsync(message).ConfigureAwait(false);
            await topicClient.CloseAsync().ConfigureAwait(false);

            var source = new ReadMessageFromAzureSubscriptionStep(logger, next, options)
            {
                // Allows PipelineContext to be passed in so that the delegate
                EvaluateCancellationTokenHandler =
                    delegate (ILogger<ReadMessageFromAzureSubscriptionStep> logger, ISubscriptionClient subscriptionClient, Message message, IPipelineContext context, CancellationToken[] tokens)
                    {
                        context.Items.Add("TEST_HANDLER_VERIED", true);
                        return Task.FromResult(true);
                    }
            };

            // act
            await source.InvokeAsync(context);

            // assert
            Assert.IsTrue(context.Items.ContainsKey("TEST_HANDLER_VERIED"), "context.Items does not contain the expected key: 'TEST_HANDLER_VERIED'");

            // cleanup
            await source.DisposeAsync();

            try
            {
                var subscriptionClient = new SubscriptionClient(
                                            options.AzureServiceBusConnectionString,
                                            options.TopicName,
                                            options.SubscriptionName,
                                            ReceiveMode.PeekLock,
                                            RetryPolicy.Default);

                // The lambda allows the Pipeline context to be passed in while preserving the standard method signature.
                var messageHandlerOptions = new MessageHandlerOptions((ExceptionReceivedEventArgs args) => { return Task.CompletedTask; })
                {
                    // Allow 1 concurrent call to simplify pipeline processing.
                    MaxConcurrentCalls = 1,
                    // Handle the message completion manually within the ProcessMessageAsync() call.
                    AutoComplete = false
                };

                subscriptionClient.RegisterMessageHandler(
                            async (Message message, CancellationToken token) =>
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
        public async Task Cancels_Message_Processing_From_EvaluateCancellationOfMessageAsync_When_CancellationToken_Is_Cancelled()
        {
            // arrange
            bool didTheMethodGetCalled = false;

            ILogger<ReadMessageFromAzureSubscriptionStep> logger = new Mock<ILogger<ReadMessageFromAzureSubscriptionStep>>().Object;

            PipelineContext context = new PipelineContext();

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureSubscriptionOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                TopicName = Tests.TestConstants.CancellationDuringEvaluateTopicName,
                SubscriptionName = Tests.TestConstants.SubscriptionName,
                TimeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 3),
                TimeToWaitForNewMessage = new TimeSpan(0, 0, 3),
            };

            // sending message to be read.
            ITopicClient topicClient = new TopicClient(options.AzureServiceBusConnectionString, options.TopicName, RetryPolicy.Default);
            byte[] messageBody = Encoding.UTF8.GetBytes("Are you there?");

            Message message = new Message(messageBody);

            await topicClient.SendAsync(message).ConfigureAwait(false);
            await topicClient.CloseAsync().ConfigureAwait(false);


            var subscriptionClient = new SubscriptionClient(
                                        options.AzureServiceBusConnectionString,
                                        options.TopicName,
                                        options.SubscriptionName,
                                        ReceiveMode.PeekLock,
                                        RetryPolicy.Default);

            // The lambda allows the Pipeline context to be passed in while preserving the standard method signature.
            var messageHandlerOptions = new MessageHandlerOptions((ExceptionReceivedEventArgs args) =>
                            ReadMessageFromAzureSubscriptionStep.ExceptionReceivedHandlerAsync(args, context))
            {
                // Allow 1 concurrent call to simplify pipeline processing.
                MaxConcurrentCalls = 1,
                // Handle the message completion manually within the ProcessMessageAsync() call.
                AutoComplete = false
            };

            CancellationTokenSource cancelledTokenSource = new CancellationTokenSource();
            cancelledTokenSource.Cancel();

            subscriptionClient.RegisterMessageHandler(
                        async (Message message, CancellationToken token) =>
                        {
                            int originalDeliveryCount = message.SystemProperties.DeliveryCount;
                            // ************************************
                            // ATTENTION: ACT AND ASSERT ARE HERE.
                            // ************************************
                            // act
                            bool result = await ReadMessageFromAzureSubscriptionStep.EvaluateCancellationOfMessageAsync(logger, subscriptionClient, message, context, token, cancelledTokenSource.Token);

                            // assert
                            Assert.IsTrue(result, "result should be true and message should be abandoned.");

                            didTheMethodGetCalled = true;
                            return;
                        },
                        messageHandlerOptions);

            // Allow the message to be read from the subscription prior to ending the execution of this method.
            while (!didTheMethodGetCalled)
            {
                await Task.Delay(500);
            }

            // assert
            Assert.AreEqual(0, context.Errors.Count, "No errors should be reported by this execution.");

            // cleanup
            await subscriptionClient.CloseAsync().ConfigureAwait(false);

            subscriptionClient = new SubscriptionClient(
                                        options.AzureServiceBusConnectionString,
                                        options.TopicName,
                                        options.SubscriptionName,
                                        ReceiveMode.PeekLock,
                                        RetryPolicy.Default);

            // Read the existing message from the topic.
            subscriptionClient.RegisterMessageHandler(
                        async (Message message, CancellationToken token) =>
                        {
                            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken).ConfigureAwait(false);
                        },
                        messageHandlerOptions);

            // Allow the message to be read from the subscription prior to ending the execution of this method.
            await Task.Delay(500);
        }

        [TestMethod]
        public async Task Constructs_ReadMessageFromAzureSubscriptionStep_Successfully()
        {
            // arrange
            ILogger<ReadMessageFromAzureSubscriptionStep> logger = new Mock<ILogger<ReadMessageFromAzureSubscriptionStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;
            var options = new ReadMessageFromAzureSubscriptionOptions()
            {
                AzureServiceBusConnectionString = TestConstants.AzureServiceBusConnectionString,
                TopicName = TestConstants.MissingTopicName,
                SubscriptionName = TestConstants.MissingSubscriptionName
            };

            // act
            var result = new ReadMessageFromAzureSubscriptionStep(logger, next, options);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(logger, result.Logger);
            Assert.AreEqual(options, result.Options);

            // cleanup
            await result.DisposeAsync();
        }

        [TestMethod]
        public async Task Returns_True_From_EvaluateCancellationOfMessageAsync_When_CancellationToken_Is_Cancelled()
        {
            // arrange
            ILogger<ReadMessageFromAzureSubscriptionStep> logger = new Mock<ILogger<ReadMessageFromAzureSubscriptionStep>>().Object;

            PipelineContext context = new PipelineContext();

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureSubscriptionOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                TopicName = Tests.TestConstants.CancellationTokenTopicName,
                SubscriptionName = Tests.TestConstants.CancellationTokenTopicName,
                TimeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 3),
                TimeToWaitForNewMessage = new TimeSpan(0, 0, 3)
            };

            // sending message to be read.
            ITopicClient topicClient = new TopicClient(options.AzureServiceBusConnectionString, options.TopicName, RetryPolicy.Default);
            byte[] messageBody = Encoding.UTF8.GetBytes("Are you there?");

            Message message = new Message(messageBody);

            await topicClient.SendAsync(message).ConfigureAwait(false);
            await topicClient.CloseAsync().ConfigureAwait(false);

            var source = new ReadMessageFromAzureSubscriptionStep(logger, next, options);

            // act
            await source.InvokeAsync(context);

            // cleanup
            await source.DisposeAsync();

            try
            {
                var subscriptionClient = new SubscriptionClient(
                                            options.AzureServiceBusConnectionString,
                                            options.TopicName,
                                            options.SubscriptionName,
                                            ReceiveMode.PeekLock,
                                            RetryPolicy.Default);

                // The lambda allows the Pipeline context to be passed in while preserving the standard method signature.
                var messageHandlerOptions = new MessageHandlerOptions((ExceptionReceivedEventArgs args) => { return Task.CompletedTask; })
                {
                    // Allow 1 concurrent call to simplify pipeline processing.
                    MaxConcurrentCalls = 1,
                    // Handle the message completion manually within the ProcessMessageAsync() call.
                    AutoComplete = false
                };

                subscriptionClient.RegisterMessageHandler(
                            async (Message message, CancellationToken token) =>
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
        public async Task Returns_Pipeline_Error_From_InvokeAsync_When_Exception_Is_Thrown_While_Message_Lock_Expires_With_A_Cancelled_Token()
        {
            // arrange
            bool isAsyncTestingCompleted = false;

            ILogger<ReadMessageFromAzureSubscriptionStep> logger = new Mock<ILogger<ReadMessageFromAzureSubscriptionStep>>().Object;

            PipelineContext context = new PipelineContext();

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureSubscriptionOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                TopicName = Tests.TestConstants.FastReadLockExpiryTopicName,
                SubscriptionName = Tests.TestConstants.SubscriptionName,
                TimeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 3),
                TimeToWaitForNewMessage = new TimeSpan(0, 0, 3),
            };

            // sending message to be read.
            ITopicClient topicClient = new TopicClient(options.AzureServiceBusConnectionString, options.TopicName, RetryPolicy.Default);
            byte[] messageBody = Encoding.UTF8.GetBytes("Are you there?");

            Message message = new Message(messageBody);

            await topicClient.SendAsync(message).ConfigureAwait(false);
            await topicClient.CloseAsync().ConfigureAwait(false);

            var source = new ReadMessageFromAzureSubscriptionStep(logger, next, options)
            {
                EvaluateCancellationTokenHandler =
                    async delegate (ILogger<ReadMessageFromAzureSubscriptionStep> logger, ISubscriptionClient subscriptionClient, Message message, IPipelineContext context, CancellationToken[] tokens)
                    {
                        // Add a Cancelled Token to enable cancellation testing.
                        List<CancellationToken> workingTokens = new List<CancellationToken>(tokens);
                        CancellationTokenSource tokenSource = new CancellationTokenSource();
                        tokenSource.Cancel();
                        workingTokens.Add(tokenSource.Token);

                        // Force an exception when the method attempts to abandon the message.
                        await subscriptionClient.DeadLetterAsync(message.SystemProperties.LockToken);
                        
                        bool result = await ReadMessageFromAzureSubscriptionStep.EvaluateCancellationOfMessageAsync(logger, subscriptionClient, message, context, workingTokens.ToArray());
                        Assert.IsTrue(result, "result from ReadMessageFromAzureSubscriptionStep should be true.");
                        isAsyncTestingCompleted = true;
                        return result;
                    }
            };

            // act
            await source.InvokeAsync(context);
            
            // delay while async processing runs.
            while (!isAsyncTestingCompleted)
            {
                await Task.Delay(1000);
            }

            // assert
            Assert.AreEqual(1, context.Errors.Count);
            Assert.IsInstanceOfType(context.Errors[0], typeof(ServiceBusException));

            // cleanup
            await source.DisposeAsync();
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_Exception_Is_Thrown()
        {
            // arrange
            ILogger<ReadMessageFromAzureSubscriptionStep> logger = new Mock<ILogger<ReadMessageFromAzureSubscriptionStep>>().Object;

            PipelineContext context = new PipelineContext();

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureSubscriptionOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                TopicName = Tests.TestConstants.DeadMessageTopicName,
                SubscriptionName = Tests.TestConstants.MissingSubscriptionName,
                TimeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 3),
                TimeToWaitForNewMessage = new TimeSpan(0, 0, 3)
            };

            // sending message to be read.
            ITopicClient topicClient = new TopicClient(options.AzureServiceBusConnectionString, options.TopicName, RetryPolicy.Default);
            byte[] messageBody = Encoding.UTF8.GetBytes("Are you there?");

            Message message = new Message(messageBody);

            await topicClient.SendAsync(message).ConfigureAwait(false);
            await topicClient.CloseAsync().ConfigureAwait(false);

            var source = new ReadMessageFromAzureSubscriptionStep(logger, next, options);

            // act
            await source.InvokeAsync(context);

            // assert
            Assert.IsTrue(context.Errors.Count > 0, $"context.Errors.Count must be greater than zero: Actual Value {context.Errors.Count}.");
            Assert.IsInstanceOfType(context.Errors[0], typeof(ServiceBusException));

            // cleanup
            await source.DisposeAsync();
            // Because the DeadMessageTopicName has no subscriptions, all messages are immediately dropped.
        }

        [TestMethod]
        public async Task Returns_PipelineContext_InputStream_From_InvokeAsync_When_Message_Is_Received()
        {
            // arrange
            const int errorCount = 0;

            ILogger<ReadMessageFromAzureSubscriptionStep> logger = new Mock<ILogger<ReadMessageFromAzureSubscriptionStep>>().Object;

            PipelineContext context = new PipelineContext();
            context.Items.Add("TESTING_ERROR_COUNT", errorCount);

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Callback(() => this.VerifyValues(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureSubscriptionOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                TopicName = Tests.TestConstants.ReadFromTopicName,
                SubscriptionName = Tests.TestConstants.SubscriptionName,
                TimeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 3),
                TimeToWaitForNewMessage = new TimeSpan(0, 0, 3),
                UserProperties = new List<string>()
                                        {
                                            "IsActive",
                                            "Name"
                                        }
            };

            // sending message to be read.
            ITopicClient topicClient = new TopicClient(options.AzureServiceBusConnectionString, options.TopicName, RetryPolicy.Default);
            byte[] messageBody = Encoding.UTF8.GetBytes("Are you there?");
            context.Items.Add("TESTING_MESSAGE_BODY", messageBody);

            Message message = new Message(messageBody);
            message.UserProperties.Add("IsActive", false);
            message.UserProperties.Add("Name", "My-Mom");

            await topicClient.SendAsync(message).ConfigureAwait(false);
            await topicClient.CloseAsync().ConfigureAwait(false);

            var source = new ReadMessageFromAzureSubscriptionStep(logger, next, options);

            // act
            await source.InvokeAsync(context);

            // assert
            if (context.Errors.Count > 0)
            {
                throw context.Errors[0];
            }
            Assert.IsTrue(context.Items.TryGetValue("TESTING_VERIFY_VALUES_CALLED", out object result));
            Assert.IsTrue((bool)result);
            Assert.AreEqual(errorCount, context.Errors.Count);

            // Additional asserts were handled in the InvokeAsync method.

            // cleanup
            await source.DisposeAsync();
        }

        [TestMethod]
        public async Task Returns_PipelineContext_InputStream_From_InvokeAsync_When_PreviousMessage_Was_Received()
        {
            // arrange
            const int errorCount = 0;
            ILogger<ReadMessageFromAzureSubscriptionStep> logger = new Mock<ILogger<ReadMessageFromAzureSubscriptionStep>>().Object;

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("Previous Message"));

            PipelineContext context = new PipelineContext();
            context.Items.Add("TESTING_ERROR_COUNT", errorCount);
            context.Items.Add(PipelineContextConstants.INPUT_STREAM, stream);

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Callback(() => this.VerifyValues(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureSubscriptionOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                TopicName = Tests.TestConstants.PreviousMessageTopicName,
                SubscriptionName = Tests.TestConstants.SubscriptionName,
                TimeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 3),
                TimeToWaitForNewMessage = new TimeSpan(0, 0, 3),
                UserProperties = new List<string>()
                                        {
                                            "IsActive",
                                            "Name"
                                        }
            };

            // sending message to be read.
            ITopicClient topicClient = new TopicClient(options.AzureServiceBusConnectionString, options.TopicName, RetryPolicy.Default);
            byte[] messageBody = Encoding.UTF8.GetBytes("Are you there?");
            context.Items.Add("TESTING_MESSAGE_BODY", messageBody);

            Message message = new Message(messageBody);
            message.UserProperties.Add("IsActive", false);
            message.UserProperties.Add("Name", "My-Mom");

            await topicClient.SendAsync(message).ConfigureAwait(false);
            await topicClient.CloseAsync().ConfigureAwait(false);

            var source = new ReadMessageFromAzureSubscriptionStep(logger, next, options);

            // act
            await source.InvokeAsync(context);


            // assert
            if (context.Errors.Count > 0)
            {
                throw context.Errors[0];
            }
            Assert.IsTrue(context.Items.TryGetValue("TESTING_VERIFY_VALUES_CALLED", out object result));
            Assert.IsTrue((bool)result);
            Assert.AreEqual(context.Items[PipelineContextConstants.INPUT_STREAM], stream);

            // cleanup
            await source.DisposeAsync();
        }

        [TestMethod]
        public void Sets_EvaluateCancellationTokenHandler_To_The_Provided_Value()
        {
            // arrange
            ILogger<ReadMessageFromAzureSubscriptionStep> logger = new Mock<ILogger<ReadMessageFromAzureSubscriptionStep>>().Object;

            PipelineContext context = new PipelineContext();

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureSubscriptionOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                TopicName = Tests.TestConstants.MissingTopicName,
                SubscriptionName = Tests.TestConstants.MissingSubscriptionName,
                TimeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 3)
            };

            var source = new ReadMessageFromAzureSubscriptionStep(logger, next, options);

            Func<ILogger<ReadMessageFromAzureSubscriptionStep>, ISubscriptionClient, Message, IPipelineContext, CancellationToken[], Task<bool>> value =
                delegate (ILogger<ReadMessageFromAzureSubscriptionStep> logger, ISubscriptionClient subscriptionClient, Message message, IPipelineContext context, CancellationToken[] tokens)
                {
                    return Task.FromResult(true);
                };

            // act
            source.EvaluateCancellationTokenHandler = value;
                    

            // assert
            Assert.AreEqual(value, source.EvaluateCancellationTokenHandler);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Logger_Is_Null()
        {
            // arrange
            ILogger<ReadMessageFromAzureSubscriptionStep> logger = null;

            PipelineContext context = new PipelineContext();

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            var next = mockPipelineRequest.Object;

            ReadMessageFromAzureSubscriptionOptions options = new ReadMessageFromAzureSubscriptionOptions();
            const string paramName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new ReadMessageFromAzureSubscriptionStep(logger, next, options));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(paramName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Next_Is_Null()
        {
            // arrange
            ILogger<ReadMessageFromAzureSubscriptionStep> logger = new Mock<ILogger<ReadMessageFromAzureSubscriptionStep>>().Object;

            PipelineContext context = new PipelineContext();

            IPipelineRequest next = null;

            ReadMessageFromAzureSubscriptionOptions options = new ReadMessageFromAzureSubscriptionOptions();
            const string paramName = nameof(next);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new ReadMessageFromAzureSubscriptionStep(logger, next, options));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(paramName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Options_Is_Null()
        {
            // arrange
            ILogger<ReadMessageFromAzureSubscriptionStep> logger = new Mock<ILogger<ReadMessageFromAzureSubscriptionStep>>().Object;

            PipelineContext context = new PipelineContext();

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            var next = mockPipelineRequest.Object;

            ReadMessageFromAzureSubscriptionOptions options = null;
            const string paramName = nameof(options);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new ReadMessageFromAzureSubscriptionStep(logger, next, options));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(paramName, result.ParamName);
        }

        [TestMethod]
        public async Task Throws_ArgumentNullException_From_EvaluateCancellationOfMessageAsync_When_Token_Is_Null()
        {
            // arrange
            ILogger<ReadMessageFromAzureSubscriptionStep> logger = new Mock<ILogger<ReadMessageFromAzureSubscriptionStep>>().Object;
            ISubscriptionClient subscriptionClient = new Mock<ISubscriptionClient>().Object;
            Message message = new Message();
            PipelineContext context = null;
            CancellationToken[] tokens = new CancellationToken[1] { default };

            const string paramName = nameof(context);

            // act
            var result = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await ReadMessageFromAzureSubscriptionStep.EvaluateCancellationOfMessageAsync(logger, subscriptionClient, message, context, tokens));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(paramName, result.ParamName);
        }

        [TestMethod]
        public async Task Throws_ArgumentNullException_From_EvaluateCancellationOfMessageAsync_When_Logger_Is_Null()
        {
            // arrange
            ILogger<ReadMessageFromAzureSubscriptionStep> logger = null;
            Message message = new Message();
            ISubscriptionClient subscriptionClient = new Mock<ISubscriptionClient>().Object;
            PipelineContext context = new PipelineContext();
            CancellationToken[] tokens = new CancellationToken[1] { default };

            
            const string paramName = nameof(logger);
            // act
            var result = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await ReadMessageFromAzureSubscriptionStep.EvaluateCancellationOfMessageAsync(logger, subscriptionClient, message, context, tokens));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(paramName, result.ParamName);
        }

        [TestMethod]
        public async Task Throws_ArgumentNullException_From_EvaluateCancellationOfMessageAsync_When_Message_Is_Null()
        {
            // arrange
            ILogger<ReadMessageFromAzureSubscriptionStep> logger = new Mock<ILogger<ReadMessageFromAzureSubscriptionStep>>().Object;
            Message message = null;
            ISubscriptionClient subscriptionClient = new Mock<ISubscriptionClient>().Object;
            PipelineContext context = new PipelineContext();
            CancellationToken[] tokens = new CancellationToken[1] { default };

            const string paramName = nameof(message);
            // act
            var result = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await ReadMessageFromAzureSubscriptionStep.EvaluateCancellationOfMessageAsync(logger, subscriptionClient, message, context, tokens));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(paramName, result.ParamName);
        }

        [TestMethod]
        public async Task Throws_ArgumentNullException_From_EvaluateCancellationOfMessageAsync_When_SubscriptionClient_Is_Null()
        {
            // arrange
            ILogger<ReadMessageFromAzureSubscriptionStep> logger = new Mock<ILogger<ReadMessageFromAzureSubscriptionStep>>().Object;
            Message message = new Message();
            ISubscriptionClient subscriptionClient = null;
            PipelineContext context = new PipelineContext();
            CancellationToken[] tokens = new CancellationToken[1] { default };

            const string paramName = nameof(subscriptionClient);

            // act
            var result = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await ReadMessageFromAzureSubscriptionStep.EvaluateCancellationOfMessageAsync(logger, subscriptionClient, message, context, tokens));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(paramName, result.ParamName);
        }

        [TestMethod]
        public async Task Throws_ArgumentNullException_From_EvaluateCancellationOfMessageAsync_When_Context_Is_Null()
        {
            // arrange
            ILogger<ReadMessageFromAzureSubscriptionStep> logger = new Mock<ILogger<ReadMessageFromAzureSubscriptionStep>>().Object;
            ISubscriptionClient subscriptionClient = new Mock<ISubscriptionClient>().Object;
            Message message = new Message();
            PipelineContext context = new PipelineContext();
            CancellationToken[] tokens = null;

            const string paramName = nameof(tokens);

            // act
            var result = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await ReadMessageFromAzureSubscriptionStep.EvaluateCancellationOfMessageAsync(logger, subscriptionClient, message, context, tokens));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(paramName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_EvaluateCancellationTokenHandler_Setter_When_Value_Is_Null()
        {
            // arrange
            ILogger<ReadMessageFromAzureSubscriptionStep> logger = new Mock<ILogger<ReadMessageFromAzureSubscriptionStep>>().Object;

            PipelineContext context = new PipelineContext();

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureSubscriptionOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                TopicName = Tests.TestConstants.MissingTopicName,
                SubscriptionName = Tests.TestConstants.MissingSubscriptionName,
                TimeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 3)
            };

            var source = new ReadMessageFromAzureSubscriptionStep(logger, next, options);

            const string paramName = "value";

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => source.EvaluateCancellationTokenHandler = null);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(paramName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_ExceptionReceivedHandler_When_Context_Is_Null()
        {
            // arrange
            ILogger<ReadMessageFromAzureSubscriptionStep> logger = new Mock<ILogger<ReadMessageFromAzureSubscriptionStep>>().Object;

            PipelineContext context = null;

            ExceptionReceivedEventArgs exceptionReceivedEventArgs = new ExceptionReceivedEventArgs(new InvalidTimeZoneException(), "action", "endpoint", "entityName", "clientId");

            string paramName = nameof(context);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => ReadMessageFromAzureSubscriptionStep.ExceptionReceivedHandlerAsync(exceptionReceivedEventArgs, context));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(paramName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_ExceptionReceivedHandler_When_ExceptionReceivedEventArgs_Is_Null()
        {
            // arrange
            ILogger<ReadMessageFromAzureSubscriptionStep> logger = new Mock<ILogger<ReadMessageFromAzureSubscriptionStep>>().Object;

            PipelineContext context = new PipelineContext();

            ExceptionReceivedEventArgs exceptionReceivedEventArgs = null;

            string paramName = nameof(exceptionReceivedEventArgs);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => ReadMessageFromAzureSubscriptionStep.ExceptionReceivedHandlerAsync(exceptionReceivedEventArgs, context));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(paramName, result.ParamName);
        }

        private void VerifyValues(PipelineContext context)
        {
            context.Items.Add("TESTING_VERIFY_VALUES_CALLED", true);
            byte[] messageBody = context.Items["TESTING_MESSAGE_BODY"] as byte[];
            int errorCount = (int)context.Items["TESTING_ERROR_COUNT"];

            Assert.IsTrue(context.Items.TryGetValue(PipelineContextConstants.INPUT_STREAM, out object result));

            byte[] messageBody2 = (result as MemoryStream).ToArray();
            string messageBodyExpected = Encoding.UTF8.GetString(messageBody);
            string messageBodyActual = Encoding.UTF8.GetString(messageBody2);

            Assert.AreEqual(messageBodyExpected, messageBodyActual);
            Assert.AreEqual(errorCount, context.Errors.Count);
            Assert.AreEqual(false, context.Items["IsActive"]);
            Assert.AreEqual("My-Mom", context.Items["Name"]);
        }
    }
}
