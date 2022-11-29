// <copyright file="SendHttpRequestStep.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2021-2022 Chris Trout
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
    using System.Net.Http.Headers;
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
        /// <param name="options">Step-specific options for altering behavior.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public SendHttpRequestStep(ILogger<SendHttpRequestStep> logger, SendHttpRequestOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        /// <inheritdoc />
        public override IEnumerable<string> CachedItemNames => new List<string>()
                                                                {
                                                                    this.Options.HttpStatusCodeContextName,
                                                                    this.Options.IsSuccessfulStatusCodeContextName,
                                                                    this.Options.HttpReasonPhraseContextName,
                                                                    this.Options.HttpResponseHeadersContextName,
                                                                    this.Options.HttpResponseTrailingHeadersContextName,
                                                                    this.Options.OutputStreamContextName
                                                                };

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
                    using (var response = await this.Options.HttpClient.SendAsync(request))
                    {
                        using (var responseStream = new MemoryStream())
                        {
                            context.Items.Add(this.Options.HttpStatusCodeContextName, response.StatusCode);
                            context.Items.Add(this.Options.IsSuccessfulStatusCodeContextName, response.IsSuccessStatusCode);
                            context.Items.Add(this.Options.HttpReasonPhraseContextName, response.ReasonPhrase ?? string.Empty);
                            context.Items.Add(this.Options.HttpResponseHeadersContextName, response.Headers);
                            context.Items.Add(this.Options.HttpResponseTrailingHeadersContextName, response.TrailingHeaders);

                            // https://github.com/dotnet/runtime/pull/35910
                            // Removed the response.Content != null check because the upgrade from .NET 3.1 to .NET 5.0 makes
                            // response.Content into a non-null guaranteed value.
                            await response.Content.CopyToAsync(responseStream).ConfigureAwait(false);
                            responseStream.Position = 0;
                            context.Items.Add(this.Options.OutputStreamContextName, responseStream);

                            await this.Next.InvokeAsync(context).ConfigureAwait(false);
                        }
                    }
                }
            }
            finally
            {
                context.Items.Remove(this.Options.HttpStatusCodeContextName);
                context.Items.Remove(this.Options.IsSuccessfulStatusCodeContextName);
                context.Items.Remove(this.Options.HttpReasonPhraseContextName);
                context.Items.Remove(this.Options.HttpResponseHeadersContextName);
                context.Items.Remove(this.Options.HttpResponseTrailingHeadersContextName);
                context.Items.Remove(this.Options.OutputStreamContextName);
            }
        }

        private HttpRequestMessage CreateHttpRequestMessageFromContext(IPipelineContext context)
        {
            // Replace any parameters that are marked for replacement values.
            string uriString = this.Options.HttpEndpoint.AbsoluteUri;

            foreach (var key in this.Options.HttpEndpointReplacementKeys)
            {
                context.AssertValueExists(key);
                uriString = uriString.Replace(key, context.Items[key].ToString());
            }

            var result = new HttpRequestMessage(new HttpMethod(this.Options.HttpMethod), new Uri(uriString));

            // Add any headers from Pipeline Context to the HTTP Request to allow security headers or anything required to be able to be processed.
            foreach (var headerName in this.Options.HeaderNames)
            {
                if (!context.Items.ContainsKey(headerName) || context.Items[headerName] is null)
                {
                    this.Logger.LogWarning("Header Name {headerName} does not exist or the value is null in PipelineContext.Items", headerName);
                }
                else
                {
                    result.Headers.Add(headerName, context.Items[headerName].ToString());
                }
            }

            foreach (var httpOptionItem in this.Options.RequestOptions)
            {
                result.Options.Set<object>(new HttpRequestOptionsKey<object>(httpOptionItem.Key), httpOptionItem.Value!);
            }

            if (this.Options.UploadInputStreamAsContent)
            {
                context.AssertValueIsValid<Stream>(this.Options.InputStreamContextName);

                // Used the null-forgiving operator '!' because context.AssertValueIsValid guarantees it is a non-null value.
                result.Content = new StreamContent((context.Items[this.Options.InputStreamContextName] as Stream)!);

                result.Content.Headers.ContentType = new MediaTypeHeaderValue(this.Options.ContentTypeHeaderValue);
            }

            return result;
        }
    }
}