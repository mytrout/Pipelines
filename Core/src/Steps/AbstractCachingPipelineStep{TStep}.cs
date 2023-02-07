// <copyright file="AbstractCachingPipelineStep{TStep}.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2021-2023 Chris Trout
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
// TO FUTURE DEVELOPERS: Remove this warning disable when the obsolete methods are moved.
#pragma warning disable CS0618
    using Microsoft.Extensions.Logging;
    using System;
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
        /// <param name="predicates">The predicate that is evaluated by the step.</param>
        protected AbstractCachingPipelineStep(ILogger<TStep> logger, IPipelineRequest next, ExecutionPredicates? predicates = null)
            : base(logger, next, predicates)
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
        /// Restores the cached items back to the <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The <paramref name="context"/> passed during pipeline execution.</param>
        /// <returns>A <see cref="Task" />.</returns>
        protected override async Task AfterNextStepAsync(IPipelineContext context)
        {
            foreach (var cacheItemName in this.CachedItems.Keys)
            {
                context.Items.Remove(cacheItemName);
                context.Items.Add(cacheItemName, this.CachedItems[cacheItemName]);
                this.CachedItems.Remove(cacheItemName);
                this.Logger.LogDebug("Unloaded CacheItemName '{cacheItemName}' from the cached pipeline step.", cacheItemName);
            }

            // TO FUTURE DEVELOPERS: Remove this call after the next breaking change when the obsolete methods are removed.
            await this.InvokeAfterCacheAsync(context).ConfigureAwait(false);

            await base.AfterNextStepAsync(context).ConfigureAwait(false);
        }

        /// <summary>
        /// Caches <paramref name="context"/>.<see cref="IPipelineContext.Items">Items</see>.
        /// </summary>
        /// <param name="context">The <paramref name="context"/> passed during pipeline execution.</param>
        /// <returns>A <see cref="Task" />.</returns>
        protected override async Task BeforeNextStepAsync(IPipelineContext context)
        {
            await base.BeforeNextStepAsync(context).ConfigureAwait(false);

            // TO FUTURE DEVELOPERS: Remove this call after the next breaking change when the obsolete methods are removed.
            await this.InvokeBeforeCacheAsync(context).ConfigureAwait(false);

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
        }

        /// <summary>
        /// Executes code after the caching for this step is invoked.
        /// </summary>
        /// <param name="context">The <paramref name="context"/> passed during pipeline execution.</param>
        /// <returns>A <see cref="Task" />.</returns>
        [Obsolete("Use the AfterNextStepAsync() method to define behaviors that occur after the Next Step.")]
        protected virtual Task InvokeAfterCacheAsync(IPipelineContext context) => Task.CompletedTask;

        /// <summary>
        /// Executes code before the caching for this step is invoked.
        /// </summary>
        /// <param name="context">The <paramref name="context"/> passed during pipeline execution.</param>
        /// <returns>A <see cref="Task" />.</returns>
        [Obsolete("Use the BeforeNextStepAsync() method to define behaviors that occur before the Next Step.")]
        protected virtual Task InvokeBeforeCacheAsync(IPipelineContext context) => Task.CompletedTask;

        /// <summary>
        /// Invoke a step after caching items and restoring them after execution.
        /// </summary>
        /// <param name="context">The <paramref name="context"/> passed during pipeline execution.</param>
        /// <returns>A <see cref="Task" />.</returns>
        [Obsolete("Use the BeforeNextStepAsync() and AfterNextStepAsync() methods to define behaviors that occur around the Next Step.  This abstract method should implement 'await this.CallNextStepAsync(context);' in the implementation body until the next breaking change.")]
        protected abstract Task InvokeCachedCoreAsync(IPipelineContext context);

        // TO FUTURE DEVELOPERS: Remove this override when the obsolete methodsa removed, as it will no longer be required.

        /// <summary>
        /// Caches <paramref name="context"/>.<see cref="IPipelineContext.Items">Items</see>, Executes the caching, passes the execution to <see cref="InvokeCachedCoreAsync(IPipelineContext)"/>, and then restores the item from cache.
        /// </summary>
        /// <param name="context">The <paramref name="context"/> passed during pipeline execution.</param>
        /// <returns>A <see cref="Task" />.</returns>
        protected override async Task InvokeCoreAsync(IPipelineContext context)
        {
            await this.InvokeCachedCoreAsync(context).ConfigureAwait(false);
        }
    }
#pragma warning restore CS0618
}
