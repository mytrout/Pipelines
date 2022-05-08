// <copyright file="WriteMessageToAzureOptions.cs" company="Chris Trout">
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

    /// <summary>
    /// Provides user-configurable options for the <see cref="WriteMessageToAzureStep" /> step.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class WriteMessageToAzureOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteMessageToAzureOptions"/> class.
        /// </summary>
        /// <remarks>Initializes the <see cref="RetrieveConnectionString"/> to the <see cref="RetrieveConnectionStringCore"/> method, by default.</remarks>
        public WriteMessageToAzureOptions()
        {
            this.RetrieveConnectionString = this.RetrieveConnectionStringCore;
        }

        /// <summary>
        /// Gets or sets the connection string for Azure Service Bus.
        /// </summary>
        public string AzureServiceBusConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the context name to read a correlation id from <see cref="IPipelineContext.Items"/>.
        /// </summary>
        public string CorrelationIdContextName { get; set; } = MessagingConstants.CORRELATION_ID;

        /// <summary>
        /// Gets or sets the prefix for keys in <see cref="IPipelineContext.Items"/> that should be copied to <see cref="ServiceBusMessage.ApplicationProperties"/>.
        /// </summary>
        public string MessageContextItemsPrefix { get; set; } = "MSG_";

        /// <summary>
        /// Gets or sets the context name to read an output <see cref="System.IO.Stream"/> from <see cref="IPipelineContext.Items"/>.
        /// </summary>
        public string OutputStreamContextName { get; set; } = PipelineContextConstants.OUTPUT_STREAM;

        /// <summary>
        /// Gets or sets the Queue or Topic Name to which the message will be written.
        /// </summary>
        public string QueueOrTopicName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a user-defined function to retrieve the Connection String.
        /// </summary>
        public Func<string> RetrieveConnectionString { get; set; }

        /// <summary>
        /// Performs any special handling of the <see cref="AzureServiceBusConnectionString"/> before the <see cref="WriteMessageToAzureStep"/> uses the value.
        /// </summary>
        /// <returns>A valid Azure ServiceBus connection string.</returns>
        protected virtual string RetrieveConnectionStringCore()
        {
            return this.AzureServiceBusConnectionString;
        }
    }
}
