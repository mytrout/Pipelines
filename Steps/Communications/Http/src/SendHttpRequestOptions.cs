// <copyright file="SendHttpRequestOptions.cs" company="Chris Trout">
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
        /// Gets or sets the HTTP Headers from <see cref="IPipelineContext.Items"/> that need to be set for the <see cref="HttpEndpoint" /> to work.
        /// </summary>
        public IEnumerable<string> HeaderNames { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the HTTP endpoint to which the stream will be uploaded.
        /// </summary>
        public Uri HttpEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the method for the Htto Endpoint call.
        /// </summary>
        public HttpMethod Method { get; set; } = HttpMethod.Post;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets a value whether the <see cref="PipelineContextConstants.OUTPUT_STREAM" /> contained in <see cref="IPipelineContext.Items"/> should be submitted as <see cref="HttpContent"/> to the <see cref="HttpEndpoint"/>.
        /// </summary>
        public bool UploadInputStreamAsContent { get; set; } = false;

        /// <summary>
        /// Gets or sets <see cref="HttpRequestOptions"/> to be set on the call.
        /// </summary>
        public HttpRequestOptions RequestOptions { get; set; } = new HttpRequestOptions();
    }
}
