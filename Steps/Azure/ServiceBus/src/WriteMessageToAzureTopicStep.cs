// <copyright file="WriteMessageToAzureTopicStep.cs" company="Chris Trout">
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
    using System.Threading.Tasks;

    /// <summary>
    /// Writes a single message to an Azure Service Bus topic.
    /// </summary>
    public class WriteMessageToAzureTopicStep : AbstractPipelineStep<WriteMessageToAzureTopicStep>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteMessageToAzureTopicStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public WriteMessageToAzureTopicStep(ILogger<WriteMessageToAzureTopicStep> logger, PipelineRequest next, WriteMessageToAzureTopicOptions options)
            : base(logger, next)
        {
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Gets the options to alter behavior or provide configuration for the <see cref="ConvertJsonDocumentToAzureMessageStep"/> class.
        /// </summary>
        public WriteMessageToAzureTopicOptions Options { get; }

        /// <summary>
        /// Writes a single message to an Azure Service Bus topic.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Logging messages do not need to be localized.")]
        protected override async Task InvokeAsyncCore(PipelineContext context)
        {
            if (context.Items.TryGetValue(Constants.SendMessage, out var messageObject)
                    && messageObject is Message)
            {
                Message message = messageObject as Message;

                ITopicClient topicClient = null;

                try
                {
                    this.Logger.LogDebug($"Building connection to Azure Service Bus topic '{this.Options.TopicName}'.");

                    topicClient = new TopicClient(this.Options.AzureServiceBusConnectionString, this.Options.TopicName, RetryPolicy.Default);

                    this.Logger.LogDebug($"Sending message '{message.MessageId}' to Azure Service Bus topic '{this.Options.TopicName}'.");

                    await topicClient.SendAsync(message).ConfigureAwait(false);

                    this.Logger.LogDebug($"Sent message successfully '{message.MessageId}' to Azure Service Bus topic '{this.Options.TopicName}'.");

                    await this.Next(context).ConfigureAwait(false);

                    context.Items.Remove(Constants.SendMessage);
                }
                catch (Exception exc)
                {
                    this.Logger.LogError(exc, $"Exception thrown in '{nameof(WriteMessageToAzureTopicStep)}'.");
                    context.Errors.Add(exc);
                }
                finally
                {
                    topicClient?.CloseAsync();
                }
            }
            else
            {
                Exception exc = new ServiceBusException(false, Resources.NO_MESSAGE_TO_SEND_IN_CONTEXT(CultureInfo.CurrentCulture));
                this.Logger.LogError(exc, $"Exception created in '{nameof(WriteMessageToAzureTopicStep)}'.");
                context.Errors.Add(exc);
            }
        }
    }
}
