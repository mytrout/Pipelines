// <copyright file="Constants.cs" company="Chris Trout">
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

using System;
using System.Diagnostics.CodeAnalysis;

namespace MyTrout.Pipelines.Steps.Azure.ServiceBus.Tests
{
    [ExcludeFromCodeCoverage]
    public static class TestConstants
    {
        /// <summary>
        /// A subscription name that is used for all topics that deliver messages.
        /// </summary>
        public const string SubscriptionName = "test-read-from-subscription";

        /// <summary>
        /// A subscription that has not been provisioned within the containing topic.
        /// </summary>
        /// <remarks>
        /// This subscription name is also used for test methods that don't send/receive messages, such as <see cref="ArgumentNullException"/> tests.
        /// </remarks>
        public const string MissingSubscriptionName = "missing-subscription";

        public const string AbandonMessageThrownTopicName = "test-read-from-topic-abandon-message-thrown";

        public const string AbandonMessageTopicName = "test-read-from-topic-abandon-message";

        public const string CancellationTokenTopicName = "test-read-from-topic-cancellation-token";
        
        public const string CancellationDuringEvaluateTopicName = "test-read-from-topic-cancel-evaluate";

        public const string DeadMessageTopicName = "test-write-to-topic-dead-message";

        public const string EvaluateTokenTopicName = "test-read-from-topic-evalute-token";

        public const string FastReadLockExpiryTopicName = "test-read-from-topic-fast-read-lock-expiry";

        public const string PreviousMessageTopicName = "test-read-from-topic-previous-message";

        public const string InputStreamTopicName = "test-read-from-topic-input-stream";

        /// <summary>
        /// A topic that has not been provisioned within this Service Bus.
        /// </summary>
        /// <remarks>
        /// This topic name is also used for test methods that don't send/receive messages, such as <see cref="ArgumentNullException"/> tests.
        /// </remarks>
        public const string MissingTopicName = "missing-topic";

        /// <summary>
        /// A topic that is used for the standard ReadMessageFrom~ test.
        /// </summary>
        public const string ReadFromTopicName = "test-read-from-topic";
        
        /// <summary>
        /// A topic that is used for the standard WriteMessageTo~ test.
        /// </summary>
        public const string WriteToTopicName = "test-write-to-topic";

        /// <summary>
        /// Reads the ASB_CONNECTION_STRING User Environment variable (can be set through launchsettings.json) to feed the connection string.
        /// </summary>
        public static readonly string AzureServiceBusConnectionString = Environment.GetEnvironmentVariable("PIPELINE_TEST_AZURE_SERVICE_BUS_CONNECTION_STRING", EnvironmentVariableTarget.Machine);

        /// <summary>
        /// An Azure Service Bus ConnectionString that will cause an exception to be thrown by <see cref="TopicClient"/> and <see cref="SubscriptionClient"/>, if used.
        /// </summary>C:\MyTrout.Pipelines\Steps\Azure\ServiceBus\test\Constants.cs
        public const string InvalidAzureServiceBusConnectionString = "SharedAccessKey=8qi3zSXL2pZVMO5nhK7Wz1rIjxZZxiJvrwD9i0HItCc=";
    }
}
