// <copyright file="AbstractCachingPipelineStep{TStep,TOptions}.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps
{
    using Microsoft.Extensions.Logging;
    using System;

    /// <summary>
    /// Provides a canonical implementation of a Pipeline Step which handles caching and restoring <see cref="IPipelineContext.Items"/> values while executing a step.
    /// </summary>
    /// <typeparam name="TStep">The class which implements this abstract class.</typeparam>
    /// <typeparam name="TOptions">The class which provides configuration for this class.</typeparam>
    public abstract class AbstractCachingPipelineStep<TStep, TOptions> : AbstractCachingPipelineStep<TStep>
                where TStep : class, IPipelineRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractCachingPipelineStep{TStep, TOptions}" /> class with the requested parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        /// <param name="options">THe options to configure this pipeline step.</param>
        protected AbstractCachingPipelineStep(ILogger<TStep> logger, TOptions options, IPipelineRequest next)
            : base(logger, next)
        {
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Gets the options that configure this pipeline step.
        /// </summary>
        public TOptions Options { get; init; }
    }
}
