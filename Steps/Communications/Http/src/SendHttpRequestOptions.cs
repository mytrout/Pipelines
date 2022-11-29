﻿// <copyright file="SendHttpRequestOptions.cs" company="Chris Trout">
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
    using Microsoft.Extensions.Configuration;
    using MyTrout.Pipelines.Core;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    /*
     *  IMPORTANT NOTE: As long as this class only contains compiler-generated functionality, it requires no unit tests.
     */

    /// <summary>
    /// Provides caller-configurable options to change the behavior of <see cref="SendHttpRequestOptions"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class SendHttpRequestOptions
    {
        /// <summary>
        /// Gets or sets the HTTP Content-Type Content Header that is specified when <see cref="UploadInputStreamAsContent"/> is <see langword="true"/>.
        /// </summary>
        public string ContentTypeHeaderValue { get; set; } = "application/json";

        /// <summary>
        /// Gets or sets the HTTP Headers from <see cref="IPipelineContext.Items"/> that need to be set for the <see cref="HttpEndpoint" /> to work.
        /// </summary>
        public List<string> HeaderNames { get; set; } = new List<string>();

#pragma warning disable CS8618 // HttpClient and HttpEndpoint should always be set in options.
        /// <summary>
        /// Gets or sets the <see cref="HttpClient"/> used to make an http request.
        /// </summary>
        [FromServices]
        public HttpClient HttpClient { get; set; }

        /// <summary>
        /// Gets or sets the HTTP endpoint to which the stream will be uploaded.
        /// </summary>
        public Uri HttpEndpoint { get; set; }
#pragma warning restore CS8618

        /// <summary>
        /// Gets or sets the Status Code Context Name used for <see cref="IPipelineContext.Items"/>.
        /// </summary>
        public string HttpStatusCodeContextName { get; set; } = HttpCommunicationConstants.HTTP_STATUS_CODE;

        /// <summary>
        /// Gets or sets the Reason Phrase Context Name used for <see cref="IPipelineContext.Items"/>.
        /// </summary>
        public string HttpReasonPhraseContextName { get; set; } = HttpCommunicationConstants.HTTP_REASON_PHRASE;

        /// <summary>
        /// Gets or sets the Response Headers Context Name used for <see cref="IPipelineContext.Items"/>.
        /// </summary>
        public string HttpResponseHeadersContextName { get; set; } = HttpCommunicationConstants.HTTP_RESPONSE_HEADERS;

        /// <summary>
        /// Gets or sets the Response Trailing Headers Context Name used for <see cref="IPipelineContext.Items"/>.
        /// </summary>
        public string HttpResponseTrailingHeadersContextName { get; set; } = HttpCommunicationConstants.HTTP_RESPONSE_TRAILING_HEADERS;

        /// <summary>
        /// Gets or sets the Input Stream Context Name used for <see cref="IPipelineContext.Items"/>.
        /// </summary>
        public string InputStreamContextName { get; set; } = PipelineContextConstants.INPUT_STREAM;

        /// <summary>
        /// Gets or sets the Is Status Code Successful Context Name used for <see cref="IPipelineContext.Items"/>.
        /// </summary>
        public string IsSuccessfulStatusCodeContextName { get; set; } = HttpCommunicationConstants.HTTP_IS_SUCCESSFUL_STATUS_CODE;

        /// <summary>
        /// Gets or sets the HTTP Method value from <see cref="IConfiguration"/>.
        /// </summary>
        public string HttpMethod { get; set; } = "POST";

        /// <summary>
        /// Gets or sets a list of values that will be replaced in the <see cref="HttpEndpoint"/> from the <see cref="IPipelineContext"/>.
        /// </summary>
        public List<string> HttpEndpointReplacementKeys { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the Output Stream Context Name used for <see cref="IPipelineContext.Items"/>.
        /// </summary>
        public string OutputStreamContextName { get; set; } = PipelineContextConstants.OUTPUT_STREAM;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets a value whether the <see cref="InputStreamContextName" /> contained in <see cref="IPipelineContext.Items"/> should be submitted as <see cref="HttpContent"/> to the <see cref="HttpEndpoint"/>.
        /// </summary>
        public bool UploadInputStreamAsContent { get; set; } = false;

        /// <summary>
        /// Gets or sets <see cref="HttpRequestOptions"/> to be set on the call.
        /// </summary>
        public HttpRequestOptions RequestOptions { get; set; } = new HttpRequestOptions();
    }
}
