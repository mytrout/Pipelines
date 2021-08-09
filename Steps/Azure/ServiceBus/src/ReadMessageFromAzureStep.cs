// <copyright file="ReadMessageFromAzureStep.cs" company="Chris Trout">
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
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Reads a batch of messages from an Azure Service Bus queue or subscription and passes them to the next step in the pipeline one at a time.
    /// </summary>
    public class ReadMessageFromAzureStep : AbstractPipelineStep<ReadMessageFromAzureStep, ReadMessageFromAzureOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadMessageFromAzureStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public ReadMessageFromAzureStep(ILogger<ReadMessageFromAzureStep> logger, ReadMessageFromAzureOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            ServiceBusReadEntity entity = options.ReadEntity;

            this.ServiceBusClient = new ServiceBusClient(this.Options.RetrieveConnectionString());

            switch (entity.Kind)
            {
                case ServiceBusReadEntityKind.Queue:
                    {
                        this.Logger.LogDebug($"Building connection to Azure Service Bus queue '{entity.QueueName}'.");
                        this.ServiceBusReceiver = this.ServiceBusClient.CreateReceiver(entity.QueueName);
                        break;
                    }

                case ServiceBusReadEntityKind.Subscription:
                    {
                        this.Logger.LogDebug($"Building connection to Azure Service Bus subscription '{entity.SubscriptionName} on topic '{entity.TopicName}'.");
                        this.ServiceBusReceiver = this.ServiceBusClient.CreateReceiver(entity.TopicName, entity.SubscriptionName);
                        break;
                    }

                default:
                    {
                        throw new InvalidOperationException(Resources.ENTITY_PATH_INVALID(CultureInfo.CurrentCulture));
                    }
            }
        }

        /// <summary>
        /// Gets the Azure Service Bus client.
        /// </summary>
        public ServiceBusClient ServiceBusClient { get; init; }

        /// <summary>
        /// Gets or sets the Azure Service Bus message receiver.
        /// </summary>
        public ServiceBusReceiver ServiceBusReceiver { get; protected set; }

        /// <summary>
        /// Dispose of any unmanaged resources.
        /// </summary>
        /// <returns>A completed <see cref="ValueTask"/>.</returns>
        public async override ValueTask DisposeAsync()
        {
            if (this.ServiceBusClient != null)
            {
                await this.ServiceBusReceiver.CloseAsync().ConfigureAwait(false);
            }

            if (this.ServiceBusReceiver != null)
            {
                await this.ServiceBusReceiver.DisposeAsync().ConfigureAwait(false);
            }

            await base.DisposeAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Reads a batch of messages from an Azure Service Bus subscription and processes them one at time.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <see langword="null" />.</exception>
        protected override async Task InvokeCoreAsync(IPipelineContext context)
        {
            context.AssertParameterIsNotNull(nameof(context));

            context.Items.TryGetValue(PipelineContextConstants.INPUT_STREAM, out var previousInputStream);

            // Cache the previousInputStream if prior steps created it.
            if (previousInputStream != null)
            {
                context.Items.Remove(PipelineContextConstants.INPUT_STREAM);
            }

            try
            {
                DateTimeOffset lastMessageProcessedAt = DateTimeOffset.UtcNow;

                while (DateTimeOffset.UtcNow.Subtract(lastMessageProcessedAt) <= this.Options.TimeToWaitForNewMessage)
                {
                    var messages = await this.ServiceBusReceiver.ReceiveMessagesAsync(this.Options.BatchSize, maxWaitTime: this.Options.TimeToWaitForNewMessage, cancellationToken: context.CancellationToken).ConfigureAwait(false);

                    foreach (var message in messages)
                    {
                        await this.ProcessMessageAsync(context, message, context.CancellationToken).ConfigureAwait(false);
                        lastMessageProcessedAt = DateTimeOffset.UtcNow;
                    }

                    if (context.CancellationToken.IsCancellationRequested)
                    {
                        // Forces to a UTC version of MinValue.
                        lastMessageProcessedAt = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
                    }
                }
            }
            finally
            {
                // Restore the previousInputStream if prior steps created it.
                if (previousInputStream != null)
                {
                    context.Items.Add(PipelineContextConstants.INPUT_STREAM, previousInputStream);
                }
            }
        }

        /// <summary>
        /// Evaluate whether a message should be cancelled.
        /// </summary>
        /// <param name="context">The context f0r the currently executing pipeline.</param>
        /// <param name="message">The <see cref="ServiceBusMessage"/> to be cancelled.</param>
        /// <param name="tokens">The <see cref="CancellationToken"/> to be checked for cancellation.</param>
        /// <returns><see langword="true"/> if the message was cancelled; otherwise <see langword="false"/>.</returns>
        protected virtual async Task<bool> EvaluateCancellationOfMessageAsync(IPipelineContext context, ServiceBusReceivedMessage message,  params CancellationToken[] tokens)
        {
            message.AssertParameterIsNotNull(nameof(message));
            context.AssertParameterIsNotNull(nameof(context));
            tokens.AssertParameterIsNotNull(nameof(tokens));

            bool result = tokens.Any(x => x.IsCancellationRequested);

            try
            {
                if (result)
                {
                    this.Logger.LogDebug(Resources.CANCELLATION_REQUESTED(CultureInfo.CurrentCulture, nameof(ReadMessageFromAzureStep)));
                    await this.ServiceBusReceiver.AbandonMessageAsync(message).ConfigureAwait(false);
                }
            }
            catch (Exception exc)
            {
                context.Errors.Add(exc);
            }

            return await Task.FromResult(result).ConfigureAwait(false);
        }

        /// <summary>
        /// Process the message into the <paramref name="context"/> after it is read.
        /// </summary>
        /// <param name="context">The context f0r the currently executing pipeline.</param>
        /// <param name="message">The <see cref="ServiceBusMessage"/> to be cancelled.</param>
        /// <param name="token">The <see cref="CancellationToken"/> to be checked for cancellation.</param>
        /// <returns>A completed <see cref="Task"/>.</returns>
        protected virtual async Task ProcessMessageAsync(IPipelineContext context, ServiceBusReceivedMessage message, CancellationToken token)
        {
            if (await this.EvaluateCancellationOfMessageAsync(context, message, token, context.CancellationToken).ConfigureAwait(false))
            {
                return;
            }

            int errorCount = context.Errors.Count;

            bool configuredTheCorrelationId = false;

            // Set up the CorrelationId, if it doesn't exist.
            if (!context.Items.ContainsKey(MessagingConstants.CORRELATION_ID))
            {
                context.Items.Add(MessagingConstants.CORRELATION_ID, message.CorrelationId);
                configuredTheCorrelationId = true;
            }

            try
            {
                // Add the values in ApplicationProperties to the context.
                foreach (var key in this.Options.ApplicationProperties)
                {
                    if (message.ApplicationProperties.ContainsKey(key))
                    {
                        context.Items.Add(key, message.ApplicationProperties[key]);
                    }
                }

                // Load inputStream and pass it into the InvokeAsync().
                using (var inputStream = message.Body.ToStream())
                {
                    context.Items.Add(PipelineContextConstants.INPUT_STREAM, inputStream);

                    await this.Next.InvokeAsync(context).ConfigureAwait(false);
                }
            }
            finally
            {
                context.Items.Remove(PipelineContextConstants.INPUT_STREAM);

                // Remove the CorrelationId if it was configured in this step.
                if (configuredTheCorrelationId)
                {
                    context.Items.Remove(MessagingConstants.CORRELATION_ID);
                }
                
                // Remove the values in ApplicationProperties from the context.
                foreach (var key in this.Options.ApplicationProperties)
                {
                    if (message.ApplicationProperties.ContainsKey(key))
                    {
                        context.Items.Remove(key);
                    }
                }

                if (context.Errors.Count > errorCount)
                {
                    if (message.DeliveryCount >= this.Options.DeliveryAttemptsBeforeDeadLetter)
                    {
                        // Deadletter the message because it has been attempted too may times.
                        await this.ServiceBusReceiver.DeadLetterMessageAsync(message, cancellationToken: context.CancellationToken);
                    }
                    else
                    {
                        // Abandon the message because it did not process correctly.
                        await this.ServiceBusReceiver.AbandonMessageAsync(message, cancellationToken: token).ConfigureAwait(false);
                    }
                }
                else
                {
                    // Complete the message so that it is not received again.
                    await this.ServiceBusReceiver.CompleteMessageAsync(message, cancellationToken: token).ConfigureAwait(false);
                }
            }
        }
    }
}
