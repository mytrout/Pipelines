// <copyright file="ReadMessageFromAzureOptions.cs" company="Chris Trout">
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
namespace MyTrout.Pipelines.Steps.Azure.ServiceBus
{
    using global::Azure.Messaging.ServiceBus;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides user-configurable options for the <see cref="ReadMessageFromAzureStep" /> step.
    /// </summary>
    public class ReadMessageFromAzureOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadMessageFromAzureOptions"/> class.
        /// </summary>
        /// <remarks>Initializes the <see cref="RetrieveConnectionString"/> to the <see cref="RetrieveConnectionStringCore"/> method, by default.</remarks>
        public ReadMessageFromAzureOptions()
        {
            this.RetrieveConnectionString = this.RetrieveConnectionStringCore;
        }

        /// <summary>
        /// Gets or sets the connection string for Azure Service Bus.
        /// </summary>
        public string AzureServiceBusConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the number of messages to download for each request to Azure Service Bus.
        /// </summary>
        public int BatchSize { get; set; } = 10;

        /// <summary>
        /// Gets or sets the number of times a message will be delivered before being dead-lettered.
        /// </summary>
        public int DeliveryAttemptsBeforeDeadLetter { get; set; } = 5;

        /// <summary>
        /// Gets or sets the entity path from which messages will be read.
        /// </summary>
        /// <remarks>Queues should be just the name of the queue. Subscriptions should be in the '{topic}/{subscription}' format using a forward slash between the topic name and the subscription name.</remarks>
        public string EntityPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets a <see cref="ServiceBusReadEntity"/> that represents either a queue or subscription configuration used for reading messages.
        /// </summary>
        public ServiceBusReadEntity ReadEntity
        {
            get
            {
                return new ServiceBusReadEntity(this.EntityPath);
            }
        }

        /// <summary>
        /// Gets or sets the prefix for keys of <see cref="ServiceBusReceivedMessage.ApplicationProperties"/> when copied to <see cref="IPipelineContext.Items"/>.
        /// </summary>
        public string MessageContextItemsPrefix { get; set; } = "MSG_";

        /// <summary>
        /// Gets or sets the time this step will wait for a new message until it times out.
        /// </summary>
        public TimeSpan TimeToWaitForNewMessage { get; set; } = new TimeSpan(0, 1, 0);

        /// <summary>
        /// Gets or sets the time to wait between message checks.
        /// </summary>
        public TimeSpan TimeToWaitBetweenMessageChecks { get; set; } = new TimeSpan(0, 0, 2);

        /// <summary>
        /// Gets or sets a user-defined function to retrieve the Connection String.
        /// </summary>
        public Func<string> RetrieveConnectionString { get; set; }

        /// <summary>
        /// Performs any special handling of the <see cref="AzureServiceBusConnectionString"/> before the <see cref="ReadMessageFromAzureStep"/> uses the value.
        /// </summary>
        /// <returns>A valid Azure ServiceBus connection string.</returns>
        protected virtual string RetrieveConnectionStringCore()
        {
            return this.AzureServiceBusConnectionString;
        }
    }
}
