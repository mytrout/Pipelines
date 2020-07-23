// <copyright file="ReadMessageFromAzureSubscriptionOptions.cs" company="Chris Trout">
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
namespace MyTrout.Pipelines.Steps.Azure.ServiceBus
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides user-configurable options for the <see cref="ReadMessageFromAzureSubscriptionStep" /> step.
    /// </summary>
    public class ReadMessageFromAzureSubscriptionOptions
    {
        /// <summary>
        /// Gets or sets the connection string for Azure Service Bus.
        /// </summary>
        public string AzureServiceBusConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the topic to which the subscription belongs.
        /// </summary>
        public string TopicName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the subscription from which to read messages.
        /// </summary>
        public string SubscriptionName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the time this step will wait for a new message until it times out.
        /// </summary>
        public TimeSpan TimeToWaitForNewMessage { get; set; } = new TimeSpan(0, 1, 0);

        /// <summary>
        /// Gets or sets the time to wait between message checks.
        /// </summary>
        public TimeSpan TimeToWaitBetweenMessageChecks { get; set; } = new TimeSpan(0, 0, 2);

        /// <summary>
        /// Gets or sets UserProperty names that will be added to the Message from <see cref="MyTrout.Pipelines.Core.PipelineContext" />, if they are available.
        /// </summary>
        public IEnumerable<string> UserProperties { get; set; } = new List<string>();
    }
}
