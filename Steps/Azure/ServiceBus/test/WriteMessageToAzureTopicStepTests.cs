// <copyright file="WriteMessageToAzureStepTests.cs" company="Chris Trout">
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
    public class WriteMessageToAzureStepTests
    {
        [TestMethod]
        public void Constructs_WriteMessageToAzureStep_Successfully()
        {
            // arrange
            ILogger<WriteMessageToAzureStep> logger = new Mock<ILogger<WriteMessageToAzureStep>>().Object;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;
            var options = new WriteMessageToAzureOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                QueueOrTopicName = Tests.TestConstants.WriteToTopicName
            };

            // act
            var result = new WriteMessageToAzureStep(logger, options, next);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(logger, result.Logger);
            Assert.AreEqual(options, result.Options);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_Using_FileStream()
        {
            // arrange
            ILogger<WriteMessageToAzureStep> logger = new Mock<ILogger<WriteMessageToAzureStep>>().Object;

            Stream inputStream = new FileStream($"{ AppDomain.CurrentDomain.BaseDirectory}text.json", FileMode.Open, FileAccess.Read, FileShare.Read);

            int id = 1;
            string name = "name";
            bool isActive = true;

            var context = new PipelineContext();
            context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, inputStream);
            context.Items.Add(MessagingConstants.CORRELATION_ID, Guid.NewGuid());
            context.Items.Add("id", id);
            context.Items.Add("name", name);
            context.Items.Add("isActive", isActive);

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new WriteMessageToAzureOptions()
            {
                ApplicationProperties = new List<string>() { "id", "name", "isActive" },
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                QueueOrTopicName = Tests.TestConstants.WriteToTopicName
            };

            string subscriptionName = Tests.TestConstants.SubscriptionName;

            const int errorCount = 0;

            var source = new WriteMessageToAzureStep(logger, options, next);

            // act
            await source.InvokeAsync(context);

            // assert
            Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_STREAM));
            Assert.AreEqual(errorCount, context.Errors.Count);

            // cleanup
            ServiceBusReceiver receiver = source.ServiceBusClient.CreateReceiver(options.QueueOrTopicName, subscriptionName);
            try
            {
                var messages = await receiver.ReceiveMessagesAsync(1000).ConfigureAwait(false);

                foreach (var message in messages)
                {
                    Assert.AreEqual(id, message.ApplicationProperties["id"]);
                    Assert.AreEqual(name, message.ApplicationProperties["name"]);
                    Assert.AreEqual(isActive, message.ApplicationProperties["isActive"]);
                    await receiver.CompleteMessageAsync(message);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await source.DisposeAsync();
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_With_Correlation_From_Options()
        {
            // arrange
            ILogger<WriteMessageToAzureStep> logger = new Mock<ILogger<WriteMessageToAzureStep>>().Object;

            Stream inputStream = new FileStream($"{ AppDomain.CurrentDomain.BaseDirectory}text.json", FileMode.Open, FileAccess.Read, FileShare.Read);

            int id = 1;
            string name = "name";
            bool isActive = true;

            var context = new PipelineContext();
            context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, inputStream);
            context.Items.Add("id", id);
            context.Items.Add("name", name);
            context.Items.Add("isActive", isActive);

            Mock<IPipelineRequest> mockPipelineRequest = new Mock<IPipelineRequest>();
            mockPipelineRequest.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockPipelineRequest.Object;

            var options = new WriteMessageToAzureOptions()
            {
                ApplicationProperties = new List<string>() { "description" },
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                QueueOrTopicName = Tests.TestConstants.WriteToTopicName
            };

            string subscriptionName = Tests.TestConstants.SubscriptionName;

            const int errorCount = 0;

            var source = new WriteMessageToAzureStep(logger, options, next);

            // act
            await source.InvokeAsync(context);

            // assert
            Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_STREAM));
            Assert.AreEqual(errorCount, context.Errors.Count);

            // cleanup
            ServiceBusReceiver receiver = source.ServiceBusClient.CreateReceiver(options.QueueOrTopicName, subscriptionName);
            try
            {
                var messages = await receiver.ReceiveMessagesAsync(1000).ConfigureAwait(false);

                foreach (var message in messages)
                {
                    await receiver.CompleteMessageAsync(message);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await source.DisposeAsync();
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_CancellationToken_Is_Cancelled()
        {
            // arrange
            ILogger<WriteMessageToAzureStep> logger = new Mock<ILogger<WriteMessageToAzureStep>>().Object;

            Stream inputStream = new FileStream($"{ AppDomain.CurrentDomain.BaseDirectory}text.json", FileMode.Open, FileAccess.Read, FileShare.Read);

            var tokenSource = new CancellationTokenSource();
            tokenSource.Cancel();

            var context = new PipelineContext();
            context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, inputStream);
            context.CancellationToken = tokenSource.Token;

            var next = new Mock<IPipelineRequest>().Object;

            var options = new WriteMessageToAzureOptions()
            {
                AzureServiceBusConnectionString = Tests.TestConstants.AzureServiceBusConnectionString,
                QueueOrTopicName = Tests.TestConstants.WriteToTopicName
            };

            string subscriptionName = Tests.TestConstants.SubscriptionName;

            const int errorCount = 0;

            var source = new WriteMessageToAzureStep(logger, options, next);

            // act
            await source.InvokeAsync(context);

            // assert
            Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_STREAM));
            Assert.AreEqual(errorCount, context.Errors.Count);

            // cleanup
            ServiceBusReceiver receiver = source.ServiceBusClient.CreateReceiver(options.QueueOrTopicName, subscriptionName);
            try
            {
                var messages = await receiver.ReceiveMessagesAsync(1000).ConfigureAwait(false);

                foreach (var message in messages)
                {
                    await receiver.CompleteMessageAsync(message);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await source.DisposeAsync();
            }
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Logger_Is_Null()
        {
            // arrange
            ILogger<WriteMessageToAzureStep> logger = null;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;
            WriteMessageToAzureOptions options = new WriteMessageToAzureOptions();
            const string paramName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new WriteMessageToAzureStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(paramName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Next_Is_Null()
        {
            // arrange
            ILogger<WriteMessageToAzureStep> logger = new Mock<ILogger<WriteMessageToAzureStep>>().Object;
            IPipelineRequest next = null;
            WriteMessageToAzureOptions options = new WriteMessageToAzureOptions();
            const string paramName = nameof(next);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new WriteMessageToAzureStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(paramName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Options_Is_Null()
        {
            // arrange
            ILogger<WriteMessageToAzureStep> logger = new Mock<ILogger<WriteMessageToAzureStep>>().Object;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;
            WriteMessageToAzureOptions options = null;
            const string paramName = nameof(options);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new WriteMessageToAzureStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(paramName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentException_From_Constructor_When_TopicClient_Creation_Throws_Exception()
        {
            // arrange
            ILogger<WriteMessageToAzureStep> logger = new Mock<ILogger<WriteMessageToAzureStep>>().Object;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;
            
            var options = new WriteMessageToAzureOptions()
            {
                AzureServiceBusConnectionString = "something that isn't a connection string",
                QueueOrTopicName = Tests.TestConstants.MissingTopicName
            };
            
            const string message = "The connection string could not be parsed; either it was malformed or contains no well-known tokens.";

            // act
            var result = Assert.ThrowsException<FormatException>(() => new WriteMessageToAzureStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(message, result.Message);
        }
    }
}
