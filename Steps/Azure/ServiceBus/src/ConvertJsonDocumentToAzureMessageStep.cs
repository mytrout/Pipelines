// <copyright file="ConvertJsonDocumentToAzureMessageStep.cs" company="Chris Trout">
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
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class ConvertJsonDocumentToAzureMessageStep : AbstractPipelineStep<ConvertJsonDocumentToAzureMessageStep>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertJsonDocumentToAzureMessageStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public ConvertJsonDocumentToAzureMessageStep(ILogger<ConvertJsonDocumentToAzureMessageStep> logger, PipelineRequest next, ConvertJsonDocumentToAzureMessageOptions options)
            : base(logger, next)
        {
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Gets the options to alter behavior or provide configuration for the <see cref="ConvertJsonDocumentToAzureMessageStep"/> class.
        /// </summary>
        public ConvertJsonDocumentToAzureMessageOptions Options { get; }

        protected override async Task InvokeAsyncCore(PipelineContext context)
        {
            context.Items.TryGetValue(Constants.SendMessage, out var previousMessage);

            context.Items.TryGetValue(this.Options.JsonDocumentKey, out object jsonObject);

            try
            {
                if (previousMessage != null)
                {
                    context.Items.Remove(Constants.SendMessage);
                }

                if (!(jsonObject is JsonDocument))
                {
                    context.Errors.Add(new ApplicationException($"'{this.Options.JsonDocumentKey}' did not contain a valid JSON document in the pipeline context."));
                }
                else
                {
                    JsonDocument document = jsonObject as JsonDocument;

                    Message message = new Message();

                    foreach (string userPropertyName in this.Options.UserPropertyNames)
                    {
                        if (document.RootElement.TryGetProperty(userPropertyName, out JsonElement jsonElement))
                        {
                            message.UserProperties.Add(userPropertyName, jsonElement.GetRawText());
                        }
                        else
                        {
                            // no op
                        }
                    }

                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (Utf8JsonWriter writer = new Utf8JsonWriter(stream))
                        {
                            document.WriteTo(writer);
                            stream.Position = 0;
                            message.Body = stream.ToArray();
                        }
                    }

                    context.Items.Add(Constants.SendMessage, message);
                }
            }
            catch (Exception exc)
            {
                this.Logger.LogError(exc, $"Exception thrown in '{nameof(ConvertJsonDocumentToAzureMessageStep)}'.");
                context.Errors.Add(exc);
            }
            finally
            {
                if (previousMessage != null)
                {
                    context.Items.Remove(Constants.SendMessage);
                }
            }
        }
    }
}
