// <copyright file="ReadMessageFromAzureStepTests.cs" company="Chris Trout">
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
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class ReadMessageFromAzureStepTests
    {
        [TestMethod]
        public async Task Abandons_Message_Lock_From_InvokeAsync_When_Exception_Is_Added_To_Pipeline_Context()
        {
            // arrange
            ILogger<ReadMessageFromAzureStep> logger = new Mock<ILogger<ReadMessageFromAzureStep>>().Object;

            PipelineContext context = new PipelineContext();

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context))
                                        .Callback(() => context.Errors.Add(new InvalidTimeZoneException()))
                                        .Returns(Task.CompletedTask);
            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                BatchSize = 1,
                EntityPath = $"{Tests.TestConstants.AbandonMessageTopicName}/{Tests.TestConstants.SubscriptionName}",
                TimeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 3),
                TimeToWaitForNewMessage = new TimeSpan(0, 0, 3)
            };

            // sending message to be read.
            var client = new ServiceBusClient(options.AzureServiceBusConnectionString);
            var sender = client.CreateSender(options.ReadEntity.TopicName);

            byte[] messageBody = Encoding.UTF8.GetBytes("Are you there?");

            ServiceBusMessage message = new ServiceBusMessage(messageBody);

            await sender.SendMessageAsync(message).ConfigureAwait(false);
            await sender.CloseAsync().ConfigureAwait(false);

            var source = new ReadMessageFromAzureStep(logger, options, next);

            // act
            await source.InvokeAsync(context);

            // assert
            Assert.AreEqual(1, context.Errors.Count);
            Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidTimeZoneException));

            // cleanup
            await source.DisposeAsync();

            ServiceBusReceiver receiver = client.CreateReceiver(options.ReadEntity.TopicName, options.ReadEntity.SubscriptionName);

            try
            {
                var receivedMessage = await receiver.ReceiveMessageAsync().ConfigureAwait(false);
                await receiver.CompleteMessageAsync(receivedMessage);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await receiver.DisposeAsync().ConfigureAwait(false);
                await client.DisposeAsync().ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task Abandons_Message_Lock_From_InvokeAsync_When_Exception_Is_Thrown_By_InvokeAsync()
        {
            // arrange
            ILogger<ReadMessageFromAzureStep> logger = new Mock<ILogger<ReadMessageFromAzureStep>>().Object;

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("Previous Message"));

            PipelineContext context = new PipelineContext();
            context.Items.Add(PipelineContextConstants.INPUT_STREAM, stream);

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Throws((new InvalidTimeZoneException()));
            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                EntityPath = $"{Tests.TestConstants.AbandonMessageThrownTopicName}/{Tests.TestConstants.SubscriptionName}",
                TimeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 3),
                TimeToWaitForNewMessage = new TimeSpan(0, 0, 3),
            };

            // sending message to be read.
            var client = new ServiceBusClient(options.AzureServiceBusConnectionString);
            var sender = client.CreateSender(options.ReadEntity.TopicName);

            byte[] messageBody = Encoding.UTF8.GetBytes("Are you there?");

            ServiceBusMessage message = new ServiceBusMessage(messageBody);

            await sender.SendMessageAsync(message).ConfigureAwait(false);
            await client.DisposeAsync().ConfigureAwait(false);

            var source = new ReadMessageFromAzureStep(logger, options, next);

            // act
            await source.InvokeAsync(context);

            // assert
            Assert.AreEqual(1, context.Errors.Count);
            Assert.IsInstanceOfType(context.Errors[0], typeof(InvalidTimeZoneException));
            Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.INPUT_STREAM));
            Assert.AreEqual(stream, context.Items[PipelineContextConstants.INPUT_STREAM]);

            // cleanup
            await source.DisposeAsync();

            var receiver = client.CreateReceiver(options.ReadEntity.TopicName, options.ReadEntity.SubscriptionName);

            try
            {
                var receivedMessage = await receiver.ReceiveMessageAsync();
                await receiver.CompleteMessageAsync(receivedMessage);    
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await receiver.CloseAsync();
                await client.DisposeAsync();
            }
        }

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
        public async Task Returns_Pipeline_Error_From_InvokeAsync_When_Exception_Is_Thrown_While_Message_Lock_Expires_With_A_Cancelled_Token()
        {
            // arrange
            bool isAsyncTestingCompleted = false;

            ILogger<ReadMessageFromAzureStep> logger = new Mock<ILogger<ReadMessageFromAzureStep>>().Object;

            PipelineContext context = new PipelineContext();

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                EntityPath = "${Tests.TestConstants.FastReadLockExpiryTopicName}/{Tests.TestConstants.SubscriptionName}",
                TimeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 3),
                TimeToWaitForNewMessage = new TimeSpan(0, 0, 3),
            };

            // sending message to be read.
            var client = new ServiceBusClient(options.AzureServiceBusConnectionString);
            var sender = client.CreateSender(options.ReadEntity.TopicName);
            byte[] messageBody = Encoding.UTF8.GetBytes("Are you there?");

            ServiceBusMessage message = new ServiceBusMessage(messageBody);

            await sender.SendMessageAsync(message).ConfigureAwait(false);
            await sender.CloseAsync().ConfigureAwait(false);

            var source = new ReadMessageFromAzureStep(logger, options, next);

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
            ILogger<ReadMessageFromAzureStep> logger = new Mock<ILogger<ReadMessageFromAzureStep>>().Object;

            PipelineContext context = new PipelineContext();

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                EntityPath = $"{Tests.TestConstants.DeadMessageTopicName}/{Tests.TestConstants.MissingSubscriptionName}",
                TimeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 3),
                TimeToWaitForNewMessage = new TimeSpan(0, 0, 3)
            };

            // sending message to be read.
            var client = new ServiceBusClient(options.AzureServiceBusConnectionString);
            var sender = client.CreateSender(options.ReadEntity.TopicName);

            byte[] messageBody = Encoding.UTF8.GetBytes("Are you there?");

            ServiceBusMessage message = new ServiceBusMessage(messageBody);

            await sender.SendMessageAsync(message).ConfigureAwait(false);
            await sender.CloseAsync().ConfigureAwait(false);

            var source = new ReadMessageFromAzureStep(logger, options, next);

            // act
            await source.InvokeAsync(context);

            // assert
            Assert.IsTrue(context.Errors.Count > 0, $"context.Errors.Count must be greater than zero: Actual Value {context.Errors.Count}.");
            Assert.IsInstanceOfType(context.Errors[0], typeof(ServiceBusException));

            // cleanup
            await source.DisposeAsync();
            await client.DisposeAsync();
        }

        [TestMethod]
        public async Task Returns_PipelineContext_InputStream_From_InvokeAsync_When_Message_Is_Received()
        {
            // arrange
            const int errorCount = 0;

            ILogger<ReadMessageFromAzureStep> logger = new Mock<ILogger<ReadMessageFromAzureStep>>().Object;

            PipelineContext context = new PipelineContext();
            context.Items.Add("TESTING_ERROR_COUNT", errorCount);

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Callback(() => ReadMessageFromAzureStepTests.VerifyValues(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                EntityPath= $"{Tests.TestConstants.ReadFromTopicName}/{Tests.TestConstants.SubscriptionName}",
                TimeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 3),
                TimeToWaitForNewMessage = new TimeSpan(0, 0, 3),
                ApplicationProperties = new List<string>()
                                        {
                                            "IsActive",
                                            "Name"
                                        }
            };

            // sending message to be read.
            var client = new ServiceBusClient(options.AzureServiceBusConnectionString);
            var sender = client.CreateSender(options.ReadEntity.TopicName);

            byte[] messageBody = Encoding.UTF8.GetBytes("Are you there?");
            context.Items.Add("TESTING_MESSAGE_BODY", messageBody);

            ServiceBusMessage message = new ServiceBusMessage(messageBody);
            message.ApplicationProperties.Add("IsActive", false);
            message.ApplicationProperties.Add("Name", "My-Mom");

            await sender.SendMessageAsync(message).ConfigureAwait(false);
            await sender.CloseAsync().ConfigureAwait(false);

            var source = new ReadMessageFromAzureStep(logger, options, next);

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
            await client.DisposeAsync();
        }

        [TestMethod]
        public async Task Returns_PipelineContext_InputStream_From_InvokeAsync_When_PreviousMessage_Was_Received()
        {
            // arrange
            const int errorCount = 0;
            ILogger<ReadMessageFromAzureStep> logger = new Mock<ILogger<ReadMessageFromAzureStep>>().Object;

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("Previous Message"));

            PipelineContext context = new PipelineContext();
            context.Items.Add("TESTING_ERROR_COUNT", errorCount);
            context.Items.Add(PipelineContextConstants.INPUT_STREAM, stream);

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Callback(() => ReadMessageFromAzureStepTests.VerifyValues(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new ReadMessageFromAzureOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                EntityPath = $"{Tests.TestConstants.PreviousMessageTopicName}/{Tests.TestConstants.SubscriptionName}",
                TimeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 3),
                TimeToWaitForNewMessage = new TimeSpan(0, 0, 3),
                ApplicationProperties = new List<string>()
                                        {
                                            "IsActive",
                                            "Name"
                                        }
            };

            // sending message to be read.
            var client = new ServiceBusClient(options.AzureServiceBusConnectionString);
            var sender = client.CreateSender(options.ReadEntity.TopicName);
            byte[] messageBody = Encoding.UTF8.GetBytes("Are you there?");
            context.Items.Add("TESTING_MESSAGE_BODY", messageBody);

            ServiceBusMessage message = new ServiceBusMessage(messageBody);
            message.ApplicationProperties.Add("IsActive", false);
            message.ApplicationProperties.Add("Name", "My-Mom");

            await sender.SendMessageAsync(message).ConfigureAwait(false);
            await sender.CloseAsync().ConfigureAwait(false);

            var source = new ReadMessageFromAzureStep(logger, options, next);

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
            await client.DisposeAsync();
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Logger_Is_Null()
        {
            // arrange
            ILogger<ReadMessageFromAzureStep> logger = null;

            PipelineContext context = new PipelineContext();

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            var next = mockPipelineRequest.Object;

            ReadMessageFromAzureOptions options = new ReadMessageFromAzureOptions();
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

            PipelineContext context = new PipelineContext();

            IPipelineRequest next = null;

            ReadMessageFromAzureOptions options = new ReadMessageFromAzureOptions();
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

            PipelineContext context = new PipelineContext();

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
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

        private static void VerifyValues(PipelineContext context)
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
