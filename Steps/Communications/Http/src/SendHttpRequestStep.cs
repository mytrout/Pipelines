// <copyright file="SendHttpRequestStep.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2021 Chris Trout
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

namespace MyTrout.Pipelines.Steps.Communications.Http
{
    using Microsoft.Extensions.Logging;
    using MyTrout.Pipelines;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Sends an HTTP Request to an endpoint with the specified URI, Headers, Query String Parameters, and/or Content payload and returns an HTTP Response.
    /// </summary>
    public class SendHttpRequestStep : AbstractCachingPipelineStep<SendHttpRequestStep, SendHttpRequestOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendHttpRequestStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="httpClient">An <see cref="HttpClient"/>.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public SendHttpRequestStep(ILogger<SendHttpRequestStep> logger, HttpClient httpClient, SendHttpRequestOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            this.HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <inheritdoc />
        public override IEnumerable<string> CachedItemNames => new List<string>() { PipelineContextConstants.INPUT_STREAM };

        /// <summary>
        /// Gets the <see cref="HttpClient" /> necessary to execute HTTP requests.
        /// </summary>
        public HttpClient HttpClient { get; init; }

        /// <summary>
        /// Send Http Request to an Endpoint with specified Headers, Query Strings,etc.
        /// </summary>
        /// <param name="context">The <see cref="IPipelineContext">context</see> passed during pipeline execution.</param>
        /// <returns>A <see cref="Task" />.</returns>
        protected override async Task InvokeCachedCoreAsync(IPipelineContext context)
        {
            // try...finally is used because the context.Items starting with "HTTP_"
            // need to be removed in the exception flow before passing control to callers' finally blocks.
            try
            {
                using (var request = this.CreateHttpRequestMessageFromContext(context))
                {
                    using (var response = await this.HttpClient.SendAsync(request))
                    {
                        using (var responseStream = new MemoryStream())
                        {
                            context.Items.Add(HttpCommunicationConstants.HTTP_STATUS_CODE, response.StatusCode);
                            context.Items.Add(HttpCommunicationConstants.HTTP_IS_SUCCESSFUL_STATUS_CODE, response.IsSuccessStatusCode);
                            context.Items.Add(HttpCommunicationConstants.HTTP_REASON_PHRASE, response.ReasonPhrase ?? string.Empty);
                            context.Items.Add(HttpCommunicationConstants.HTTP_RESPONSE_HEADERS, response.Headers);
                            context.Items.Add(HttpCommunicationConstants.HTTP_RESPONSE_TRAILING_HEADERS, response.TrailingHeaders);

                            if (response.Content != null)
                            {
                                await response.Content.CopyToAsync(responseStream).ConfigureAwait(false);
                                responseStream.Position = 0;
                                context.Items.Add(PipelineContextConstants.INPUT_STREAM, responseStream);
                            }

                            await this.Next.InvokeAsync(context).ConfigureAwait(false);
                        }
                    }
                }
            }
            finally
            {
                context.Items.Remove(HttpCommunicationConstants.HTTP_STATUS_CODE);
                context.Items.Remove(HttpCommunicationConstants.HTTP_IS_SUCCESSFUL_STATUS_CODE);
                context.Items.Remove(HttpCommunicationConstants.HTTP_REASON_PHRASE);
                context.Items.Remove(HttpCommunicationConstants.HTTP_RESPONSE_HEADERS);
                context.Items.Remove(HttpCommunicationConstants.HTTP_RESPONSE_TRAILING_HEADERS);
            }
        }

        private HttpRequestMessage CreateHttpRequestMessageFromContext(IPipelineContext context)
        {
            var result = new HttpRequestMessage(this.Options.Method, this.Options.HttpEndpoint);

            // Add any headers from Pipeline Context to the HTTP Request to allow security headers or anything required to be able to be processed.
            foreach (var headerName in this.Options.HeaderNames)
            {
                if (!context.Items.ContainsKey(headerName) || context.Items[headerName] is null)
                {
                    this.Logger.LogWarning("Header Name {headerName} does not exist or the value is null in PipelineContext.Items", headerName);
                }
                else
                {
                    result.Headers.Add(headerName, context.Items[headerName] as string);
                }
            }

            foreach (var httpOptionItem in this.Options.RequestOptions)
            {
                result.Options.Set<object>(new HttpRequestOptionsKey<object>(httpOptionItem.Key), httpOptionItem.Value!);
            }

            if (this.Options.UploadInputStreamAsContent)
            {
                context.AssertValueIsValid<Stream>(PipelineContextConstants.OUTPUT_STREAM);

                // Used the null-forgiving operator '!' because context.AssertValueIsValid guarantees it is a non-null value.
                result.Content = new StreamContent((context.Items[PipelineContextConstants.OUTPUT_STREAM] as Stream) !);
            }

            return result;
        }
    }
}
