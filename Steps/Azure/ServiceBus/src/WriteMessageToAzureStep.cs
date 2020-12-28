// <copyright file="WriteMessageToAzureStep.cs" company="Chris Trout">
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
    using global::Azure.Messaging.ServiceBus;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Globalization;
    using System.IO;
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
        /// <param name="next">The next step in the pipeline.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
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
        public async override ValueTask DisposeAsync()
        {
            if (this.ServiceBusSender != null)
            {
                await this.ServiceBusSender.CloseAsync().ConfigureAwait(false);
            }

            if (this.ServiceBusClient != null)
            {
                await this.ServiceBusClient.DisposeAsync().ConfigureAwait(false);
            }

            await base.DisposeAsync().ConfigureAwait(false);
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

            context.AssertValueIsValid<Stream>(PipelineContextConstants.OUTPUT_STREAM);

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
            context.AssertValueIsValid<Stream>(PipelineContextConstants.OUTPUT_STREAM);

            var outputStream = context.Items[PipelineContextConstants.OUTPUT_STREAM] as Stream;

#pragma warning disable CS8604 // context.AssertValueIsValue(OUTPUT_STREAM) ensures that outputStream is not null when converted to a Stream.

            var messageBody = await BinaryData.FromStreamAsync(outputStream);

#pragma warning restore CS8604

            ServiceBusMessage result = new ServiceBusMessage(messageBody);

            if (context.Items.ContainsKey(MessagingConstants.CORRELATION_ID))
            {
                result.CorrelationId = context.Items[MessagingConstants.CORRELATION_ID].ToString();
            }
            else
            {
                result.CorrelationId = context.CorrelationId.ToString();
            }

            // Sets UserProperties aka Filter Values on the Message, if they are available in the context.
            foreach (var userProperty in this.Options.ApplicationProperties)
            {
                if (context.Items.ContainsKey(userProperty))
                {
                    result.ApplicationProperties.Add(userProperty, context.Items[userProperty]);
                }
                else
                {
                    this.Logger.LogDebug(Resources.USER_PROPERTY_MISSING(CultureInfo.CurrentCulture, result.CorrelationId, userProperty));
                }
            }

            return result;
        }
    }
}
