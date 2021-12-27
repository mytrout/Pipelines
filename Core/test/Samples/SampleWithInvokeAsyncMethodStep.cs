// <copyright file="SampleWithInvokeAsyncMethodStep.cs" company="Chris Trout">
// MIT License
//
// Copyright © 2019-2021 Chris Trout
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

namespace MyTrout.Pipelines.Samples.Tests
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class SampleWithInvokeAsyncMethodStep : IPipelineRequest
    {
        public SampleWithInvokeAsyncMethodStep(ILogger<SampleWithInvokeAsyncMethodStep> logger, IPipelineRequest next)
        {
            this.Logger = logger;
            this.Next = next;
        }

        private ILogger<SampleWithInvokeAsyncMethodStep> Logger { get; }

        private IPipelineRequest Next { get; }

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

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        public Task InvokeAsync(IPipelineContext context)
        {
            return this.Next.InvokeAsync(context);
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
    }
}
