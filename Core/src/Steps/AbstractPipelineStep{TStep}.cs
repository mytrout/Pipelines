// <copyright file="AbstractPipelineStep{TStep}.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2019-2021 Chris Trout
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
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a canonical implementation of a Pipeline Step which handles parameter validation.
    /// </summary>
    /// <typeparam name="TStep">The class which implements this abstract class.</typeparam>
    public abstract class AbstractPipelineStep<TStep> : IPipelineRequest
            where TStep : class, IPipelineRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractPipelineStep{TStep}" /> class with the requested parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        protected AbstractPipelineStep(ILogger<TStep> logger, IPipelineRequest next)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.Next = next ?? throw new ArgumentNullException(nameof(next));
        }

        /// <summary>
        /// Gets the logger for this step in the pipeline.
        /// </summary>
        public ILogger<TStep> Logger { get; }

        /// <summary>
        /// Gets the next step in the pipeline.
        /// </summary>
        public IPipelineRequest Next { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of any disposable resources for this instance.
        /// </summary>
        /// <returns>A completed <see cref="ValueTask" />.</returns>
        /// <remarks>Developers who need to dispose of unmanaged resources should override this method.</remarks>
        public async ValueTask DisposeAsync()
        {
            // Perform async cleanup.
            await this.DisposeCoreAsync();

            // Dispose of synchronous unmanaged resources.
            this.Dispose(false);

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize

            // Suppress finalization.
            GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        }

        /// <summary>
        /// Invokes a step in the pipeline.
        /// </summary>
        /// <param name="context">The <see cref="IPipelineContext">context</see> passed during pipeline execution.</param>
        /// <returns>A <see cref="Task" />.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <see langword="null" />.</exception>
        public async Task InvokeAsync(IPipelineContext context)
        {
            context.AssertParameterIsNotNull(nameof(context));

            try
            {
                await this.InvokeCoreAsync(context).ConfigureAwait(false);
            }
            catch (Exception exc)
            {
                context.Errors.Add(exc);
            }
        }

        /// <summary>
        /// Disposes of any disposable resources for this instance.
        /// </summary>
        /// <param name="disposing">A flag indicating whether this instance is already being disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            // no op
        }

        /// <summary>
        /// Disposes of any asynchronous disposable resources for this instance.
        /// </summary>
        /// <returns>A completed <see cref="ValueTask" />.</returns>
        protected virtual ValueTask DisposeCoreAsync()
        {
            return default;
        }

        /// <summary>
        /// Provides the implementation for this step.
        /// </summary>
        /// <param name="context">The <see cref="IPipelineContext">context</see> passed during pipeline execution.</param>
        /// <returns>A <see cref="Task" />.</returns>
        protected abstract Task InvokeCoreAsync(IPipelineContext context);
    }
}