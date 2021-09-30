// <copyright file="HttpCommunicationConstants.cs" company="Chris Trout">
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
    using System.Net.Http;

    /// <summary>
    /// Constants defined for <see cref="MyTrout.Pipelines.Core.PipelineContext" /> Http-related items across all Steps.
    /// </summary>
    public static class HttpCommunicationConstants
    {
        /// <summary>
        /// Indicates that this <see cref="MyTrout.Pipelines.Core.PipelineContext" /> item is the <see cref="HttpClient"/> that is available to all steps in the pipeline.
        /// </summary>
        public static readonly string HTTP_CLIENT = "PIPELINE_HTTP_CLIENT";

        /// <summary>
        /// Indicates that this <see cref="MyTrout.Pipelines.Core.PipelineContext" /> item is the HTTP Status Code from the HTTP response.
        /// </summary>
        public static readonly string HTTP_STATUS_CODE = "PIPELINE_HTTP_STATUS_CODE";

        /// <summary>
        /// Indicates that this <see cref="MyTrout.Pipelines.Core.PipelineContext" /> item is a flag indicating wheter the HTTP Status code is considered successful from the HTTP response.
        /// </summary>
        public static readonly string HTTP_IS_SUCCESSFUL_STATUS_CODE = "PIPELINE_HTTP_IS_SUCCESSFUL_STATUS_CODE";

        /// <summary>
        /// Indicates that this <see cref="MyTrout.Pipelines.Core.PipelineContext" /> item is the Reason Phrase from the HTTP response.
        /// </summary>
        public static readonly string HTTP_REASON_PHRASE = "PIPELINE_HTTP_REASON_PHRASE";

        /// <summary>
        /// Indicates that this <see cref="MyTrout.Pipelines.Core.PipelineContext" /> item is the HTTP Headers from the HTTP response.
        /// </summary>
        public static readonly string HTTP_RESPONSE_HEADERS = "PIPELINE_HTTP_RESPONSE_HEADERS";

        /// <summary>
        /// Indicates that this <see cref="MyTrout.Pipelines.Core.PipelineContext" /> item is Trailing HTTP Headers from the HTTP response.
        /// </summary>
        public static readonly string HTTP_RESPONSE_TRAILING_HEADERS = "PIPELINE_HTTP_RESPONSE_TRAILING_HEADERS";
    }
}
