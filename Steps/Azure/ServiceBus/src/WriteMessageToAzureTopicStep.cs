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
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Writes a single message to an Azure Service Bus topic.
    /// </summary>
    public class WriteMessageToAzureTopicStep : AbstractPipelineStep<WriteMessageToAzureTopicStep, WriteMessageToAzureTopicOptions>
    {
        /// <summary>
        /// Gets the Correlation ID of the message.
        /// </summary>
        public const string CORRELATION_ID = "CorrelationId";
        
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteMessageToAzureTopicStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        public WriteMessageToAzureTopicStep(ILogger<WriteMessageToAzureTopicStep> logger, WriteMessageToAzureTopicOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            this.Logger.LogDebug($"Building connection to Azure Service Bus topic '{this.Options.TopicName}'.");

            this.TopicClient = new TopicClient(this.Options.AzureServiceBusConnectionString, this.Options.TopicName, RetryPolicy.Default);
        }

        private ITopicClient TopicClient { get;  }

        /// <summary>
        /// Dispose of the <see cref="TopicClient"/>.
        /// </summary>
        /// <returns>A completed <see cref="ValueTask"/>.</returns>
        public override async ValueTask DisposeAsync()
        {
            await this.TopicClient.CloseAsync().ConfigureAwait(false);
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

            if (context.Items.ContainsKey(PipelineContextConstants.OUTPUT_STREAM)
                    && context.Items[PipelineContextConstants.OUTPUT_STREAM] is Stream)
            {
                Message message = this.ConstructMessage(context);

                this.Logger.LogDebug($"Sending message '{message.MessageId}' to Azure Service Bus topic '{this.Options.TopicName}'.");

                await this.TopicClient.SendAsync(message).ConfigureAwait(false);

                this.Logger.LogDebug($"Sent message successfully '{message.MessageId}' to Azure Service Bus topic '{this.Options.TopicName}'.");

                await this.Next.InvokeAsync(context).ConfigureAwait(false);
            }
            else
            {
                Exception exc = new ServiceBusException(false, Resources.NO_MESSAGE_TO_SEND_IN_CONTEXT(CultureInfo.CurrentCulture));
                this.Logger.LogError(exc, $"Exception created in '{nameof(WriteMessageToAzureTopicStep)}'.");
                context.Errors.Add(exc);
            }
        }

        /// <summary>
        /// Converts a <see cref="System.IO.Stream" /> into a byte array.
        /// </summary>
        /// <param name="inputStream"><see cref="System.IO.Stream" /> to be converted.</param>
        /// <returns>A byte array.</returns>
        /// <remarks>
        /// <para>
        ///     This method has a short-cut if the <paramref name="inputStream"/> is a <see cref="MemoryStream" /> to avoid the copy operation.
        /// </para>
        /// <para>
        /// <paramref name="inputStream"/> cannot be <see langword="null" />.
        /// </para>
        /// </remarks>
        private static byte[] ConvertStreamToByteArray(Stream inputStream)
        {
            inputStream.Position = 0;

            // The implementation of this method has been altered to guarantee that memoryStrem is never null.
            // This boolean check guarantees that the incoming inputStream is copied into a MemoryStream, only if needed.
            bool isMemoryStream = inputStream is MemoryStream;

            MemoryStream memoryStream = (inputStream as MemoryStream) ?? new MemoryStream();

            try
            {
                if(!isMemoryStream)
                {
                    inputStream.CopyToAsync(memoryStream);
                }

                return memoryStream.ToArray();
            }
            finally
            {
                memoryStream.Close();
            }
        }

        /// <summary>
        /// Constructs an Azure Message with the values from the <paramref name="context"/> based on the user-configured options.
        /// </summary>
        /// <param name="context">The <see cref="IPipelineContext" /> for the currently executing pipeline.</param>
        /// <returns>An Azure <see cref="Message" />.</returns>
        private Message ConstructMessage(IPipelineContext context)
        {
            var inputStream = context.Items[PipelineContextConstants.OUTPUT_STREAM] as Stream;

#pragma warning disable CS8604 // Possible null reference argument.
            byte[] messageBody = WriteMessageToAzureTopicStep.ConvertStreamToByteArray(inputStream);
#pragma warning restore CS8604 // Possible null reference argument.

            Message result = new Message(messageBody);

            if (context.Items.ContainsKey(WriteMessageToAzureTopicStep.CORRELATION_ID))
            {
                result.CorrelationId = context.Items[WriteMessageToAzureTopicStep.CORRELATION_ID].ToString();
            }
            else
            {
                result.CorrelationId = context.CorrelationId.ToString();
            }

            // Sets UserProperties aka Filter Values on the Message, if they are available in the context.
            foreach (var userProperty in this.Options.UserProperties)
            {
                if (context.Items.ContainsKey(userProperty))
                {
                    result.UserProperties.Add(userProperty, context.Items[userProperty]);
                }
                else
                {
                    this.Logger.LogDebug("IPipelineContext: '{result.CorrelationId}' did not contain the user property: '{userProperty}'", result.CorrelationId, userProperty);
                }
            }

            return result;
        }
    }
}
