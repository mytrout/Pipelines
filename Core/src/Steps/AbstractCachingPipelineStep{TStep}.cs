// <copyright file="AbstractCachingPipelineStep{TStep}.cs" company="Chris Trout">
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
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a canonical implementation of a Pipeline Step which handles caching and restoring <see cref="IPipelineContext.Items"/> values while executing a step.
    /// </summary>
    /// <typeparam name="TStep">The class which implements this abstract class.</typeparam>
    public abstract class AbstractCachingPipelineStep<TStep> : AbstractPipelineStep<TStep>
                where TStep : class, IPipelineRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractCachingPipelineStep{TStep}" /> class with the requested parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        protected AbstractCachingPipelineStep(ILogger<TStep> logger, IPipelineRequest next)
            : base(logger, next)
        {
            // no op
        }

        /// <summary>
        /// Gets the names of the <see cref="IPipelineContext.Items"/> that should be cached.
        /// </summary>
        public abstract IEnumerable<string> CachedItemNames { get; }

        /// <summary>
        /// Gets the items that have been cached by this step.
        /// </summary>
        protected Dictionary<string, object> CachedItems { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Executes code after the caching for this step is invoked.
        /// </summary>
        /// <param name="context">The <paramref name="context"/> passed during pipeline execution.</param>
        /// <returns>A <see cref="Task" />.</returns>
        protected virtual Task InvokeAfterCacheAsync(IPipelineContext context)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Executes code before the caching for this step is invoked.
        /// </summary>
        /// <param name="context">The <paramref name="context"/> passed during pipeline execution.</param>
        /// <returns>A <see cref="Task" />.</returns>
        protected virtual Task InvokeBeforeCacheAsync(IPipelineContext context)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Invoke a step after caching items and restoring them after execution.
        /// </summary>
        /// <param name="context">The <paramref name="context"/> passed during pipeline execution.</param>
        /// <returns>A <see cref="Task" />.</returns>
        protected abstract Task InvokeCachedCoreAsync(IPipelineContext context);

        /// <summary>
        /// Caches <paramref name="context"/>.<see cref="IPipelineContext.Items">Items</see>, Executes the caching, passes the execution to <see cref="InvokeCachedCoreAsync(IPipelineContext)"/>, and then restores the item from cache.
        /// </summary>
        /// <param name="context">The <paramref name="context"/> passed during pipeline execution.</param>
        /// <returns>A <see cref="Task" />.</returns>
        protected sealed override async Task InvokeCoreAsync(IPipelineContext context)
        {
            await this.InvokeBeforeCacheAsync(context).ConfigureAwait(false);

            try
            {
                foreach (var cachedItemName in this.CachedItemNames)
                {
                    // While previous can never be used as a null value
                    // (because the key must exist to get to any usage)
                    // the compiler null-checking requires that it be nullable.
                    if (context.Items.TryGetValue(cachedItemName, out object? previous))
                    {
                        this.CachedItems.Add(cachedItemName, previous);
                        context.Items.Remove(cachedItemName);
                        this.Logger.LogDebug("Loaded '{cacheItemName}' into the cached pipeline step", cachedItemName);
                    }
                    else
                    {
                        this.Logger.LogDebug("CachedItemName '{cachedItemName}' does not exist in the Pipelines.Context.", cachedItemName);
                    }
                }

                this.Logger.LogDebug("Loaded {Count} cache items before step execution.", this.CachedItems.Count);

                await this.InvokeCachedCoreAsync(context).ConfigureAwait(false);
            }
            finally
            {
                foreach (var cacheItemName in this.CachedItems.Keys)
                {
                    context.Items.Remove(cacheItemName);
                    context.Items.Add(cacheItemName, this.CachedItems[cacheItemName]);
                    this.CachedItems.Remove(cacheItemName);
                    this.Logger.LogDebug("Unloaded CacheItemName '{cacheItemName}' from the cached pipeline step.", cacheItemName);
                }
            }

            await this.InvokeAfterCacheAsync(context).ConfigureAwait(false);
        }
    }
}
