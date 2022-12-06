﻿// <copyright file="WriteMessageToAzureStep.cs" company="Chris Trout">
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
    using Microsoft.Extensions.Logging;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Writes a single message to an Azure Service Bus queue or topic.
    /// </summary>
    public class WriteMessageToAzureStep : AbstractPipelineStep<WriteMessageToAzureStep, WriteMessageToAzureOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteMessageToAzureStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public WriteMessageToAzureStep(ILogger<WriteMessageToAzureStep> logger, WriteMessageToAzureOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            this.ServiceBusClient = new ServiceBusClient(options.RetrieveConnectionString.Invoke());
            this.ServiceBusSender = this.ServiceBusClient.CreateSender(options.QueueOrTopicName);
        }

        /// <summary>
        /// Gets the subscription client for Azure Service Bus.
        /// </summary>
        public ServiceBusClient ServiceBusClient { get; init; }

        /// <summary>
        /// Gets the service bus sender for Azure Service Bus.
        /// </summary>
        public ServiceBusSender ServiceBusSender { get; init; }

        /// <summary>
        /// Dispose of any unmanaged resources.
        /// </summary>
        /// <returns>A completed <see cref="ValueTask"/>.</returns>
        protected async override ValueTask DisposeCoreAsync()
        {
            if (this.ServiceBusSender != null)
            {
                await this.ServiceBusSender.DisposeAsync().ConfigureAwait(false);
            }

            if (this.ServiceBusClient != null)
            {
                await this.ServiceBusClient.DisposeAsync().ConfigureAwait(false);
            }

            await base.DisposeCoreAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Writes a single message to an Azure Service Bus topic.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        protected override async Task InvokeCoreAsync(IPipelineContext context)
        {
            context.AssertParameterIsNotNull(nameof(context));

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            await this.Next.InvokeAsync(context).ConfigureAwait(false);

            context.AssertValueIsValid<Stream>(this.Options.OutputStreamContextName);

            ServiceBusMessage message = await this.ConstructMessageAsync(context).ConfigureAwait(false);

            this.Logger.LogDebug($"Sending message '{message.MessageId}' to Azure Service Bus topic '{this.Options.QueueOrTopicName}'.");

            await this.ServiceBusSender.SendMessageAsync(message, cancellationToken: context.CancellationToken).ConfigureAwait(false);

            this.Logger.LogDebug($"Sent message successfully '{message.MessageId}' to Azure Service Bus topic '{this.Options.QueueOrTopicName}'.");
        }

        /// <summary>
        /// Constructs an Azure Message with the values from the <paramref name="context"/> based on the user-configured options.
        /// </summary>
        /// <param name="context">The <see cref="IPipelineContext" /> for the currently executing pipeline.</param>
        /// <returns>An Azure <see cref="ServiceBusMessage" />.</returns>
        private async Task<ServiceBusMessage> ConstructMessageAsync(IPipelineContext context)
        {
            context.AssertValueIsValid<Stream>(this.Options.OutputStreamContextName);

            var outputStream = context.Items[this.Options.OutputStreamContextName] as Stream;

            var messageBody = await BinaryData.FromStreamAsync(outputStream!);

            var result = new ServiceBusMessage(messageBody);

#pragma warning disable CS8600

            // workingCorrelationId can be null, if TryGetValue returns false.
            // The if statement handles it, so this is a false positive on CS8600.
            if (context.Items.TryGetValue(this.Options.CorrelationIdContextName, out object workingCorrelationId))
            {
                result.CorrelationId = workingCorrelationId.ToString();
            }
            else
            {
                result.CorrelationId = context.CorrelationId.ToString();
            }

#pragma warning restore CS8600

            // Sets UserProperties aka Filter Values on the Message, if they are available in the context.
            foreach (var userProperty in context.Items.Keys.Where(x => x.StartsWith(this.Options.MessageContextItemsPrefix)))
            {
                result.ApplicationProperties.Add(userProperty.Substring(this.Options.MessageContextItemsPrefix.Length), context.Items[userProperty]);
            }

            return result;
        }
    }
}
