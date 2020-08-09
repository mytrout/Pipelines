// <copyright file="IPipelineContext.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// Provides the context that is passed between step implenentation in the pipeline.
    /// </summary>
    public interface IPipelineContext
    {
        /// <summary>
        /// Gets or sets the token used by callers to cancel the pipeline execution.
        /// </summary>
        CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IConfiguration"/> that can be optionally used by steps in the pipeline.
        /// </summary>
        IConfiguration? Configuration { get; set; }

        /// <summary>
        /// Gets a correlation value that can be used to correlate log entries and reporting data.
        /// </summary>
        Guid CorrelationId { get; }

        /// <summary>
        /// Gets a list of non-fatal exceptions reported by the pipeline.
        /// </summary>
        IList<Exception> Errors { get; }

        /// <summary>
        /// Gets a value indicating whether or not <see cref="Configuration"/> is available.
        /// </summary>
        bool IsConfigurationAvailable { get; }

        /// <summary>
        /// Gets context items which are passed between step instances during execution.
        /// </summary>
        IDictionary<string, object> Items { get; }

        /// <summary>
        /// Gets a UTC timestamp representing when this pipeline started its execution.
        /// </summary>
        DateTimeOffset Timestamp { get; }
    }
}
