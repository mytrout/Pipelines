// <copyright file="ReadMessageFromAzureSubscriptionStep.cs" company="Chris Trout">
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
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Reads a single message from an Azure Service Bus subscription.
    /// </summary>
    public class ReadMessageFromAzureSubscriptionStep : AbstractPipelineStep<ReadMessageFromAzureSubscriptionStep>
    {
        private DateTimeOffset lastMessageProcessedAt;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadMessageFromAzureSubscriptionStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public ReadMessageFromAzureSubscriptionStep(ILogger<ReadMessageFromAzureSubscriptionStep> logger, PipelineRequest next, ReadMessageFromAzureSubscriptionOptions options)
            : base(logger, next)
        {
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Gets the options to alter behavior or provide configuration for the <see cref="ReadMessageFromAzureSubscriptionStep"/> class.
        /// </summary>
        public ReadMessageFromAzureSubscriptionOptions Options { get; }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs, PipelineContext context)
        {
            context.Errors.Add(exceptionReceivedEventArgs.Exception);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Reads a single message from an Azure Service Bus subscription.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        protected override async Task InvokeAsyncCore(PipelineContext context)
        {
            this.lastMessageProcessedAt = DateTimeOffset.UtcNow;

            ISubscriptionClient subscriptionClient = null;

            try
            {
                this.Logger.LogDebug($"Building connection to Azure Service Bus subscription '{this.Options.SubscriptionName}' on topic '{this.Options.TopicName}'.");

                subscriptionClient = new SubscriptionClient(
                                            this.Options.AzureServiceBusConnectionString,
                                            this.Options.TopicName,
                                            this.Options.SubscriptionName,
                                            ReceiveMode.PeekLock,
                                            RetryPolicy.Default);

                // The lambda allows the Pipeline context to be passed in while preserving the standard method signature.
                var messageHandlerOptions = new MessageHandlerOptions(
                                                (ExceptionReceivedEventArgs args) =>
                                                    ReadMessageFromAzureSubscriptionStep.ExceptionReceivedHandler(args, context))
                {
                    // Allow 1 concurrent call to simplify pipeline processing.
                    MaxConcurrentCalls = 1,
                    // Handle the message completion manually within the ProcessMessageAsync() call.
                    AutoComplete = false
                };

                this.Logger.LogDebug("Hooking up the messaging processing handler and exception handler.");

                subscriptionClient.RegisterMessageHandler(
                            (Message message, CancellationToken token) =>
                                this.ProcessMessagesAsync(message, token, subscriptionClient, context),
                            messageHandlerOptions);

                // Check periodically to determine if this process should stop checking for messages.
                while (DateTimeOffset.UtcNow.Subtract(this.lastMessageProcessedAt) >= this.Options.TimeToWaitForNewMessage)
                {
                    this.Logger.LogDebug($"Entering wait state for {this.Options.TimeToWaitForNewMessage} to see if additional messages can be read.");

                    await Task.Delay(this.Options.TimeToWaitBetweenMessageChecks, context.CancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception exc)
            {
                this.Logger.LogError(exc, $"Exception thrown in '{nameof(ReadMessageFromAzureSubscriptionStep)}'.");
                context.Errors.Add(exc);
            }
            finally
            {
                await subscriptionClient.CloseAsync().ConfigureAwait(false);
            }
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token, ISubscriptionClient subscriptionClient, PipelineContext context)
        {
            if (token.IsCancellationRequested || context.CancellationToken.IsCancellationRequested)
            {
                this.Logger.LogDebug($"Cancellation requested while executing {nameof(ReadMessageFromAzureSubscriptionStep)}.");

                // Guarantees that the next periodic check in InvokeAsyncCore will fail.
                this.lastMessageProcessedAt = DateTimeOffset.MinValue;

                await subscriptionClient.AbandonAsync(message.SystemProperties.LockToken).ConfigureAwait(false);

                return;
            }

            this.lastMessageProcessedAt = DateTimeOffset.UtcNow;

            int errorCount = context.Errors.Count;

            context.Items.TryGetValue(Constants.ReceivedMessage, out var previousMessage);

            try
            {
                if (previousMessage != null)
                {
                    context.Items.Remove(Constants.ReceivedMessage);
                }

                context.Items.Add(Constants.ReceivedMessage, message);

                await this.Next.Invoke(context).ConfigureAwait(false);

                context.Items.Remove(Constants.ReceivedMessage);

                if (context.Errors.Count > errorCount)
                {
                    // Abandon the message because it did not process correctly.
                    await subscriptionClient.AbandonAsync(message.SystemProperties.LockToken).ConfigureAwait(false);
                }
                else
                {
                    // Complete the message so that it is not received again.
                    await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken).ConfigureAwait(false);
                }
            }
            catch (Exception exc)
            {
                context.Errors.Add(exc);
            }
            finally
            {
                if(previousMessage != null)
                {
                    if (context.Items.ContainsKey(Constants.ReceivedMessage))
                    {
                        context.Items.Remove(Constants.ReceivedMessage);
                    }

                    context.Items.Add(Constants.ReceivedMessage, previousMessage);
                }
            }
        }
    }
}
