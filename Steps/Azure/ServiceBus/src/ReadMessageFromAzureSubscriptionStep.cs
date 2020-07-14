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
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Reads a single message from an Azure Service Bus subscription.
    /// </summary>
    public class ReadMessageFromAzureSubscriptionStep : AbstractPipelineStep<ReadMessageFromAzureSubscriptionStep, ReadMessageFromAzureSubscriptionOptions>
    {
        private Func<ILogger<ReadMessageFromAzureSubscriptionStep>, ISubscriptionClient, Message, IPipelineContext, CancellationToken[], Task<bool>> evaluateHandler = ReadMessageFromAzureSubscriptionStep.EvaluateCancellationOfMessageAsync;

        private DateTimeOffset lastMessageProcessedAt;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadMessageFromAzureSubscriptionStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        public ReadMessageFromAzureSubscriptionStep(ILogger<ReadMessageFromAzureSubscriptionStep> logger, IPipelineRequest next, ReadMessageFromAzureSubscriptionOptions options)
            : base(logger, options, next)
        {
            this.Logger.LogDebug($"Building connection to Azure Service Bus subscription '{this.Options.SubscriptionName}' on topic '{this.Options.TopicName}'.");

            this.SubscriptionClient = new SubscriptionClient(
                                            this.Options.AzureServiceBusConnectionString,
                                            this.Options.TopicName,
                                            this.Options.SubscriptionName,
                                            ReceiveMode.PeekLock,
                                            RetryPolicy.Default);
        }

        /// <summary>
        /// Gets or sets the function to evaluate cancellation tokens in the ProcessMessagesAsync method.
        /// </summary>
        /// <remarks>
        /// This property is used for unit testing only and is not available during the normal pipeline building process.
        /// </remarks>
        public Func<ILogger<ReadMessageFromAzureSubscriptionStep>, ISubscriptionClient, Message, IPipelineContext, CancellationToken[], Task<bool>> EvaluateCancellationTokenHandler
        {
            get
            {
                return this.evaluateHandler;
            }
            set
            {
                this.evaluateHandler = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// Gets the subscription client for Azure Service Bus.
        /// </summary>
        private ISubscriptionClient SubscriptionClient { get; }

        /// <summary>
        /// Evaluate whether a message should be cancelled.
        /// </summary>
        /// <param name="logger">The logger to report the cancellation.</param>
        /// <param name="subscriptionClient">The Azure <see cref="SubscriptionClient"/> from which the message was received.</param>
        /// <param name="message">The <see cref="Message"/> to be cancelled.</param>
        /// <param name="context">The context f0r the currently executing pipeline.</param>
        /// <param name="tokens">The <see cref="CancellationToken"/> to be checked for cancellation.</param>
        /// <returns></returns>
        public static async Task<bool> EvaluateCancellationOfMessageAsync(ILogger<ReadMessageFromAzureSubscriptionStep> logger, ISubscriptionClient subscriptionClient, Message message, IPipelineContext context, params CancellationToken[] tokens)
        {
            logger.AssertParameterIsNotNull(nameof(logger));
            subscriptionClient.AssertParameterIsNotNull(nameof(subscriptionClient));
            message.AssertParameterIsNotNull(nameof(message));
            context.AssertParameterIsNotNull(nameof(context));
            tokens.AssertParameterIsNotNull(nameof(tokens));

            bool result = tokens.Any(x => x.IsCancellationRequested);

            try
            {
                if (result)
                {
                    logger.LogDebug(Resources.CANCELLATION_REQUESTED(CultureInfo.CurrentCulture, nameof(ReadMessageFromAzureSubscriptionStep)));
                    await subscriptionClient.AbandonAsync(message.SystemProperties.LockToken).ConfigureAwait(false);
                }
            }
            catch (Exception exc)
            {
                context.Errors.Add(exc);
            }

            return await Task.FromResult(result).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exceptionReceivedEventArgs"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs, IPipelineContext context)
        {
            exceptionReceivedEventArgs.AssertParameterIsNotNull(nameof(exceptionReceivedEventArgs));
            context.AssertParameterIsNotNull(nameof(context));

            context.Errors.Add(exceptionReceivedEventArgs.Exception);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Dispose of any unmanaged resources.
        /// </summary>
        /// <returns>A completed <see cref="ValueTask"/>.</returns>
        public async override ValueTask DisposeAsync()
        {
            await this.SubscriptionClient.CloseAsync().ConfigureAwait(false);
            await base.DisposeAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Reads a single message from an Azure Service Bus subscription.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        protected override async Task InvokeCoreAsync(IPipelineContext context)
        {
            context.AssertParameterIsNotNull(nameof(context));

            this.lastMessageProcessedAt = DateTimeOffset.UtcNow;

            // The lambda allows the Pipeline context to be passed in while preserving the standard method signature.
            var messageHandlerOptions = new MessageHandlerOptions((ExceptionReceivedEventArgs args) =>
                                                ReadMessageFromAzureSubscriptionStep.ExceptionReceivedHandler(args, context))
            {
                // Allow 1 concurrent call to simplify pipeline processing.
                MaxConcurrentCalls = 1,
                // Handle the message completion manually within the ProcessMessageAsync() call.
                AutoComplete = false
            };

            this.Logger.LogDebug("Hooking up the messaging processing handler and exception handler.");

            this.SubscriptionClient.RegisterMessageHandler(
                        (Message message, CancellationToken token) =>
                            this.ProcessMessagesAsync(message, context, token),
                        messageHandlerOptions);

            // Check periodically to determine if this process should stop checking for messages.
            while (DateTimeOffset.UtcNow.Subtract(this.lastMessageProcessedAt) <= this.Options.TimeToWaitForNewMessage)
            {
                this.Logger.LogDebug($"Entering wait state for {this.Options.TimeToWaitForNewMessage} to see if additional messages can be read.");

                await Task.Delay(this.Options.TimeToWaitBetweenMessageChecks, context.CancellationToken).ConfigureAwait(false);
            }
        }

        private async Task ProcessMessagesAsync(Message message, IPipelineContext context, CancellationToken token)
        {
            // If this method returns true, then cancellation has been requested.
            if (await this.EvaluateCancellationTokenHandler.Invoke(
                                            this.Logger,
                                            this.SubscriptionClient,
                                            message,
                                            context,
                                            new CancellationToken[2]
                                            {
                                                token,
                                                context.CancellationToken
                                            }).ConfigureAwait(false))
            {
                // Forces the Message wait loop to exit at the next check, if the cancellationToken hasn't forced and exit.
                this.lastMessageProcessedAt = DateTimeOffset.MinValue;
                return;
            }

            this.lastMessageProcessedAt = DateTimeOffset.UtcNow;

            int errorCount = context.Errors.Count;

            context.Items.TryGetValue(PipelineContextConstants.INPUT_STREAM, out var previousMessage);

            bool configuredTheCorrelationId = false;

            // Set up the CorrelationId, if it doesn't exist.
            if (!context.Items.ContainsKey(WriteMessageToAzureTopicStep.CORRELATION_ID))
            {
                context.Items.Add(WriteMessageToAzureTopicStep.CORRELATION_ID, message.CorrelationId);
                configuredTheCorrelationId = true;
            }

            try
            {
                if (previousMessage != null)
                {
                    context.Items.Remove(PipelineContextConstants.INPUT_STREAM);
                }

                // Add the values in UserProperties to the context.
                foreach (var key in this.Options.UserProperties)
                {
                    if (message.UserProperties.ContainsKey(key))
                    {
                        context.Items.Add(key, message.UserProperties[key]);
                    }
                }

                // Load inputStream and pass it into the InvokeAsync().
                using (var inputStream = new MemoryStream(message.Body))
                {
                    context.Items.Add(PipelineContextConstants.INPUT_STREAM, inputStream);

                    await this.Next.InvokeAsync(context).ConfigureAwait(false);

                    context.Items.Remove(PipelineContextConstants.INPUT_STREAM);
                }

                // Remove the values in UserProperties from the context.
                foreach (var key in this.Options.UserProperties)
                {
                    if (message.UserProperties.ContainsKey(key))
                    {
                        context.Items.Remove(key);
                    }
                }

                if (context.Errors.Count > errorCount)
                {
                    // Abandon the message because it did not process correctly.
                    await this.SubscriptionClient.AbandonAsync(message.SystemProperties.LockToken).ConfigureAwait(false);
                }
                else
                {
                    // Complete the message so that it is not received again.
                    await this.SubscriptionClient.CompleteAsync(message.SystemProperties.LockToken).ConfigureAwait(false);
                }
            }
            catch (Exception exc)
            {
                context.Errors.Add(exc);
            }
            finally
            {
                if (configuredTheCorrelationId)
                {
                    context.Items.Remove(WriteMessageToAzureTopicStep.CORRELATION_ID);
                }

                if(previousMessage != null)
                {
                    if (context.Items.ContainsKey(PipelineContextConstants.INPUT_STREAM))
                    {
                        context.Items.Remove(PipelineContextConstants.INPUT_STREAM);
                    }

                    context.Items.Add(PipelineContextConstants.INPUT_STREAM, previousMessage);
                }
            }
        }
    }
}
