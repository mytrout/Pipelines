// <copyright file="ReadMessageFromAzureStepTests.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2019-2022 Chris Trout
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
    using MyTrout.Pipelines.Steps.Azure.ServiceBus;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class ReadMessageFromAzureStepTests
    {
        [TestMethod]
        public async Task Constructs_ReadMessageFromAzureStep_Successfully()
        {
            // arrange
            ILogger<ReadMessageFromAzureStep> logger = new Mock<ILogger<ReadMessageFromAzureStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;
            var options = new ReadMessageFromAzureOptions()
            {
                AzureServiceBusConnectionString = TestConstants.AzureServiceBusConnectionString,
                EntityPath = "${TestConstants.MissingTopicName}/{TestConstants.MissingSubscriptionName}"
            };

            // act
            var result = new ReadMessageFromAzureStep(logger, options, next);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(logger, result.Logger);
            Assert.AreEqual(options, result.Options);

            // cleanup
            await result.DisposeAsync();
        }

        [TestMethod]
        public async Task Disposes_ReadMessageFromAzureStep_Successfully()
        {
            // arrange
            ILogger<ReadMessageFromAzureStep> logger = new Mock<ILogger<ReadMessageFromAzureStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;
            var options = new ReadMessageFromAzureOptions()
            {
                AzureServiceBusConnectionString = TestConstants.AzureServiceBusConnectionString,
                EntityPath = "${TestConstants.MissingTopicName}/{TestConstants.MissingSubscriptionName}"
            };

            
            var sut = new TestOverrideReadMessageFromAzureDisposeStep(logger, options, next);

            // act
            await sut.DisposeAsync();

            // assert
            // No exception means that everything went well.
            Assert.IsTrue(true);

        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_And_Abandoned_Message_From_InvokeAsync()
        {
            // arrange
            ILogger<ReadMessageFromAzureStep> logger = new Mock<ILogger<ReadMessageFromAzureStep>>().Object;

            var context = new PipelineContext();

            string messageValue = "Are you there?";
            var mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context))
                                .Callback(() =>
                                {
                                    context.Errors.Add(new InvalidDataException());
                                    var tokenSource = new CancellationTokenSource();
                                    tokenSource.Cancel();
                                    context.CancellationToken = tokenSource.Token;

                                })
                                .Returns(Task.CompletedTask);

            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                DeliveryAttemptsBeforeDeadLetter = 2,
                EntityPath = $"{Tests.TestConstants.AbandonMessageTopicName}/{Tests.TestConstants.SubscriptionName}",
                TimeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 0, 0, 100),
                TimeToWaitForNewMessage = new TimeSpan(0, 0, 1, 0, 0)
            };


            // sending message to be read.
            var client = new ServiceBusClient(options.AzureServiceBusConnectionString);
            var sender = client.CreateSender(options.ReadEntity.TopicName);

            byte[] messageBody = Encoding.UTF8.GetBytes(messageValue);

            var message = new ServiceBusMessage(messageBody);

            await sender.SendMessageAsync(message).ConfigureAwait(false);
            await sender.CloseAsync().ConfigureAwait(false);

            var source = new ReadMessageFromAzureStep(logger, options, next);

            // act
            await source.InvokeAsync(context);

            // assert
            Assert.IsTrue(context.Errors.Any());
            Assert.AreEqual(typeof(InvalidDataException), context.Errors[0].GetType());

            // cleanup
            var receiver = client.CreateReceiver(Tests.TestConstants.AbandonMessageTopicName, $"{Tests.TestConstants.SubscriptionName}/ /$DeadLetterQueue");
            var receivedMessages = await receiver.ReceiveMessagesAsync(1000).ConfigureAwait(false);
            await Task.WhenAll(receivedMessages.Select(x => receiver.CompleteMessageAsync(x)));

            await receiver.CloseAsync();
            await source.DisposeAsync();
            await client.DisposeAsync();
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_And_Deadletter_Message_From_InvokeAsync_When_Single_Delivery_Attempt_Is_Requested()
        {
            // arrange
            ILogger<ReadMessageFromAzureStep> logger = new Mock<ILogger<ReadMessageFromAzureStep>>().Object;

            var context = new PipelineContext();

            string messageValue = "Are you there?";
            var mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context))
                                .Callback(() =>
                                {
                                    context.Errors.Add(new InvalidDataException());

                                })
                                .Returns(Task.CompletedTask);

            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                DeliveryAttemptsBeforeDeadLetter = 1,
                EntityPath = $"{Tests.TestConstants.DeadMessageTopicName}/{Tests.TestConstants.SubscriptionName}",
                TimeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 0, 0, 50),
                TimeToWaitForNewMessage = new TimeSpan(0, 0, 0, 10, 0)
            };


            // sending message to be read.
            var client = new ServiceBusClient(options.AzureServiceBusConnectionString);
            var sender = client.CreateSender(options.ReadEntity.TopicName);

            byte[] messageBody = Encoding.UTF8.GetBytes(messageValue);

            var message = new ServiceBusMessage(messageBody);

            await sender.SendMessageAsync(message).ConfigureAwait(false);
            await sender.CloseAsync().ConfigureAwait(false);

            var source = new ReadMessageFromAzureStep(logger, options, next);

            // act
            await source.InvokeAsync(context);

            // assert
            Assert.IsTrue(context.Errors.Any());
            Assert.AreEqual(typeof(InvalidDataException), context.Errors[0].GetType());

            // cleanup
            var receiver = client.CreateReceiver(Tests.TestConstants.CancellationTokenTopicName, $"{Tests.TestConstants.SubscriptionName}/ /$DeadLetterQueue");
            var receivedMessages = await receiver.ReceiveMessagesAsync(1000).ConfigureAwait(false);
            await Task.WhenAll(receivedMessages.Select(x => receiver.CompleteMessageAsync(x)));

            await receiver.CloseAsync();
            await source.DisposeAsync();
            await client.DisposeAsync();
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Cancellation_Token_Is_Set()
        {
            // arrange
            ILogger<ReadMessageFromAzureStep> logger = new Mock<ILogger<ReadMessageFromAzureStep>>().Object;

            var context = new PipelineContext();

            string messageValue = "Are you there?";
            var mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context))
                                .Callback(() =>
                                {
                                    var tokenSource = new CancellationTokenSource();
                                    tokenSource.Cancel();
                                    context.CancellationToken = tokenSource.Token;
                                })
                                .Returns(Task.CompletedTask);

            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                EntityPath = Tests.TestConstants.QueueName,
                TimeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 0, 0, 100),
                TimeToWaitForNewMessage = new TimeSpan(0, 0, 1, 0, 0)
            };

            // sending message to be read.
            var client = new ServiceBusClient(options.AzureServiceBusConnectionString);
            var sender = client.CreateSender(options.ReadEntity.QueueName);

            byte[] messageBody = Encoding.UTF8.GetBytes(messageValue);

            var message = new ServiceBusMessage(messageBody);

            await sender.SendMessageAsync(message).ConfigureAwait(false);
            await sender.SendMessageAsync(message).ConfigureAwait(false);
            await sender.CloseAsync().ConfigureAwait(false);

            var source = new ReadMessageFromAzureStep(logger, options, next);

            // act
            await source.InvokeAsync(context);

            // assert
            Assert.IsTrue(context.CancellationToken.IsCancellationRequested);

            // cleanup
            var receiver = client.CreateReceiver(options.ReadEntity.QueueName);
            var receivedMessages = await receiver.ReceiveMessagesAsync(1000).ConfigureAwait(false);
            await Task.WhenAll(receivedMessages.Select(x => receiver.CompleteMessageAsync(x)));

            await source.DisposeAsync();
            await receiver.CloseAsync();
            await client.DisposeAsync();
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Exception_Is_Thrown_During_Cancellation_Token_Evaluation()
        {
            // arrange
            ILogger<ReadMessageFromAzureStep> logger = new Mock<ILogger<ReadMessageFromAzureStep>>().Object;

            var context = new PipelineContext();

            string messageValue = "Are you there?";
            var mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context))
                                .Callback(() =>
                                {
                                    var tokenSource = new CancellationTokenSource();
                                    tokenSource.Cancel();
                                    context.CancellationToken = tokenSource.Token;
                                })
                                .Returns(Task.CompletedTask);

            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                EntityPath = Tests.TestConstants.QueueName,
                TimeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 0, 0, 100),
                TimeToWaitForNewMessage = new TimeSpan(0, 0, 1, 0, 0)
            };

            // sending message to be read.
            var client = new ServiceBusClient(options.AzureServiceBusConnectionString);
            var sender = client.CreateSender(options.ReadEntity.QueueName);

            byte[] messageBody = Encoding.UTF8.GetBytes(messageValue);

            var message = new ServiceBusMessage(messageBody);

            await sender.SendMessageAsync(message).ConfigureAwait(false);
            await sender.SendMessageAsync(message).ConfigureAwait(false);
            await sender.CloseAsync().ConfigureAwait(false);

            var source = new TestOverrideEvaluateCancellationTokenStep(logger, options, next);

            // act
            await source.InvokeAsync(context);

            // assert
            Assert.IsTrue(context.CancellationToken.IsCancellationRequested);
            Assert.IsTrue(context.Errors.Count > 1);
            Assert.IsInstanceOfType(context.Errors[0], typeof(ServiceBusException));

            // cleanup
            var receiver = client.CreateReceiver(options.ReadEntity.QueueName);
            var receivedMessages = await receiver.ReceiveMessagesAsync(1000).ConfigureAwait(false);
            await Task.WhenAll(receivedMessages.Select(x => receiver.CompleteMessageAsync(x)));

            await source.DisposeAsync();
            await receiver.CloseAsync();
            await client.DisposeAsync();
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Message_Is_Read_From_Queue()
        {
            // arrange
            ILogger<ReadMessageFromAzureStep> logger = new Mock<ILogger<ReadMessageFromAzureStep>>().Object;

            int keyId = 1;
            string name = "Name";

            var previousStream = new MemoryStream();
            var context = new PipelineContext();
            context.Items.Add(PipelineContextConstants.INPUT_STREAM, previousStream);

            string messageValue = "Are you there?";
            var mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context))
                                .Callback(() =>
                                {
                                    // assert
                                    using (var reader = new StreamReader(context.Items[PipelineContextConstants.INPUT_STREAM] as Stream))
                                    {
                                        Assert.AreEqual(keyId, context.Items["MSG_KeyId"]);
                                        Assert.AreEqual(name, context.Items["MSG_Name"]);
                                        Assert.AreEqual(messageValue, reader.ReadToEnd());
                                    }

                                })
                                .Returns(Task.CompletedTask);

            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                EntityPath = Tests.TestConstants.QueueName,
                TimeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 0, 0, 50),
                TimeToWaitForNewMessage = new TimeSpan(0, 0, 1, 0, 0)
            };


            // sending message to be read.
            var client = new ServiceBusClient(options.AzureServiceBusConnectionString);
            var sender = client.CreateSender(options.ReadEntity.QueueName);

            byte[] messageBody = Encoding.UTF8.GetBytes(messageValue);

            var message = new ServiceBusMessage(messageBody);
            message.ApplicationProperties.Add("KeyId", keyId);
            message.ApplicationProperties.Add("Name", name);

            await sender.SendMessageAsync(message).ConfigureAwait(false);
            await sender.CloseAsync().ConfigureAwait(false);

            var source = new ReadMessageFromAzureStep(logger, options, next);

            // act
            await source.InvokeAsync(context);

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            // cleanup
            await source.DisposeAsync();
            await client.DisposeAsync();
            previousStream.Close();
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Message_Is_Read_From_Subscription_With_PreExisting_CorrelationId()
        {
            // arrange
            ILogger<ReadMessageFromAzureStep> logger = new Mock<ILogger<ReadMessageFromAzureStep>>().Object;

            int keyId = 1;
            string name = "Name";
            Guid correlationid = Guid.NewGuid();

            var previousStream = new MemoryStream();
            var context = new PipelineContext();
            context.Items.Add(PipelineContextConstants.INPUT_STREAM, previousStream);
            context.Items.Add(MessagingConstants.CORRELATION_ID, correlationid);

            string messageValue = "Are you there?";
            var mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context))
                                .Callback(() => 
                                {
                                    // assert
                                    using (var reader = new StreamReader(context.Items[PipelineContextConstants.INPUT_STREAM] as Stream))
                                    {
                                        Assert.AreEqual(keyId, context.Items["CHANGED_KeyId"]);
                                        Assert.AreEqual(name, context.Items["CHANGED_Name"]);
                                        Assert.AreEqual(messageValue, reader.ReadToEnd());
                                    }
                                    
                                })
                                .Returns(Task.CompletedTask);

            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                EntityPath = $"{Tests.TestConstants.ReadFromTopicName}/{Tests.TestConstants.SubscriptionName}",
                MessageContextItemsPrefix = "CHANGED_",
                TimeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 0, 0, 50),
                TimeToWaitForNewMessage = new TimeSpan(0, 0, 1, 0, 0)
            };

           
            // sending message to be read.
            var client = new ServiceBusClient(options.AzureServiceBusConnectionString);
            var sender = client.CreateSender(options.ReadEntity.TopicName);

            byte[] messageBody = Encoding.UTF8.GetBytes(messageValue);

            var message = new ServiceBusMessage(messageBody);
            message.ApplicationProperties.Add("KeyId", keyId);
            message.ApplicationProperties.Add("Name", name);

            await sender.SendMessageAsync(message).ConfigureAwait(false);
            await sender.CloseAsync().ConfigureAwait(false);

            var source = new ReadMessageFromAzureStep(logger, options, next);

            // act
            await source.InvokeAsync(context);

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }
            Assert.IsTrue(context.Items.ContainsKey(MessagingConstants.CORRELATION_ID), "CORRELATION_ID should exist in PipelineContext.Items and does not.");
            Assert.AreEqual(correlationid, context.Items[MessagingConstants.CORRELATION_ID]);

            // cleanup
            await source.DisposeAsync();
            await client.DisposeAsync();
            previousStream.Close();
        }

        [TestMethod]
        public async Task Returns_PipelineContext_With_No_Alterations_From_InvokeAsync_After_Execution()
        {
            // arrange
            ILogger<ReadMessageFromAzureStep> logger = new Mock<ILogger<ReadMessageFromAzureStep>>().Object;

            int keyId = 1;
            string name = "Name";

            var previousStream = new MemoryStream();
            var context = new PipelineContext();
            context.Items.Add(PipelineContextConstants.INPUT_STREAM, previousStream);
            int expectedItemCount = context.Items.Count;

            string messageValue = "Are you there?";
            var mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);

            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                EntityPath = Tests.TestConstants.QueueName,
                TimeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 0, 0, 50),
                TimeToWaitForNewMessage = new TimeSpan(0, 0, 1, 0, 0)
            };


            // sending message to be read.
            var client = new ServiceBusClient(options.AzureServiceBusConnectionString);
            var sender = client.CreateSender(options.ReadEntity.QueueName);

            byte[] messageBody = Encoding.UTF8.GetBytes(messageValue);

            var message = new ServiceBusMessage(messageBody);
            message.ApplicationProperties.Add("KeyId", keyId);
            message.ApplicationProperties.Add("Name", name);

            await sender.SendMessageAsync(message).ConfigureAwait(false);
            await sender.CloseAsync().ConfigureAwait(false);

            var source = new ReadMessageFromAzureStep(logger, options, next);

            // act
            await source.InvokeAsync(context);

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }
            Assert.AreEqual(expectedItemCount, context.Items.Count);
            Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.INPUT_STREAM), "INPUT_STREAM does not exist in PipelineContext.Items after execution.");
            Assert.AreEqual(previousStream, context.Items[PipelineContextConstants.INPUT_STREAM], "INPUT_STREAM value does not match expected value after execution.");

            // cleanup
            await source.DisposeAsync();
            await client.DisposeAsync();
            previousStream.Close();
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Logger_Is_Null()
        {
            // arrange
            ILogger<ReadMessageFromAzureStep> logger = null;

            var context = new PipelineContext();

            var mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            var next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureOptions();
            const string paramName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new ReadMessageFromAzureStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(paramName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Next_Is_Null()
        {
            // arrange
            ILogger<ReadMessageFromAzureStep> logger = new Mock<ILogger<ReadMessageFromAzureStep>>().Object;

            var context = new PipelineContext();

            IPipelineRequest next = null;

            var options = new ReadMessageFromAzureOptions();
            const string paramName = nameof(next);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new ReadMessageFromAzureStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(paramName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Options_Is_Null()
        {
            // arrange
            ILogger<ReadMessageFromAzureStep> logger = new Mock<ILogger<ReadMessageFromAzureStep>>().Object;

            var context = new PipelineContext();

            var mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            var next = mockPipelineRequest.Object;

            ReadMessageFromAzureOptions options = null;
            const string paramName = nameof(options);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new ReadMessageFromAzureStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(paramName, result.ParamName);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_Constructor_When_ServiceBusReadEntityKind_Is_Unknown()
        {
            // arrange
            ILogger<ReadMessageFromAzureStep> logger = new Mock<ILogger<ReadMessageFromAzureStep>>().Object;

            var context = new PipelineContext();

            var mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context))
                                .Returns(Task.CompletedTask);

            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                EntityPath = "Topic/Queue/SubQueue"
            };

            string message = ServiceBus.Resources.ENTITY_PATH_INVALID(CultureInfo.CurrentCulture);
            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => new ReadMessageFromAzureStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(message, result.Message);
        }
    }
}
