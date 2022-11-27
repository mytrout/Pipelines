// <copyright file="RenameContextItemStep.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a step that caches and renames one or more <see cref="IPipelineContext.Items"/> before executing the next step.
    /// </summary>
    public class RenameContextItemStep : AbstractCachingPipelineStep<RenameContextItemStep, RenameContextItemOptions>
    {
        /// <summary>
        /// Gets the item names that need to be cached.
        /// </summary>
        private readonly IEnumerable<string> cachedItemsPrivate;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenameContextItemStep" /> class with the requested parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        /// <param name="options">The options to configure this pipeline.</param>
        public RenameContextItemStep(ILogger<RenameContextItemStep> logger, RenameContextItemOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // Determines if the collision avoidance capabilities are turned on.
            if (string.Compare(
                    Environment.GetEnvironmentVariable(
                                options.PreventCollisionsWithRenamedValueNamesEnvironmentVariableName,
                                options.EnvironmentVariableTarget),
                    "true",
                    true) == 0)
            {
                // Provides a unique list of the combination of keys and values.
                this.cachedItemsPrivate = this.Options.RenameValues.Keys.Union(this.Options.RenameValues.Values);
            }
            else
            {
                this.cachedItemsPrivate = this.Options.RenameValues.Keys;
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// The original implementation using only RenameValues.Keys failed because sometimes the destination name
        /// also existed in the <see cref="IPipelineContext"/>.
        /// Unpredictable behavior 
        /// </remarks>
        public override IEnumerable<string> CachedItemNames => this.cachedItemsPrivate;

        /// <summary>
        /// Rename <see cref="IPipelineContext.Items"/> values and then execute the pipeline step.
        /// </summary>
        /// <param name="context">The <see cref="IPipelineContext">context</see> passed during pipeline execution.</param>
        /// <returns>A <see cref="Task" />.</returns>
        protected override async Task InvokeCachedCoreAsync(IPipelineContext context)
        {
            try
            {
                int renamedCount = 0;
                foreach (var contextKey in this.Options.RenameValues.Keys)
                {
                    string renamedKey = this.Options.RenameValues[contextKey];

                    if (this.CachedItems.TryGetValue(contextKey, out object? renamedValue))
                    {
                        // Removal of the contextKey is unnecessary because the AbstractCachingPipelineStep handles it.
                        context.Items.Add(renamedKey, renamedValue);
                        renamedCount++;
                        this.Logger.LogDebug("Loaded '{renamedKey}' into Pipeline.Items.", renamedKey);
                    }
                    else
                    {
                        this.Logger.LogWarning("PipelineContext.Items did not contain an item named '{contextKey}' that can be renamed to '{renamedKey}'.", contextKey, renamedKey);
                    }
                }

                int failedCount = this.Options.RenameValues.Count - renamedCount;

                this.Logger.LogDebug("{SuccessfulCount} PipelineContext.Items were renamed successfully. {FailedCount} items were removed without being renamed.", renamedCount, failedCount);

                await this.Next.InvokeAsync(context);
            }
            finally
            {
                foreach (var renamedKey in this.Options.RenameValues.Values)
                {
                    // Restoration of the original contextKey is unnecessary because the AbstractCachingPipelineStep handles it.
                    // Remove does not throw an exception if the key doesn't exist.
                    if (context.Items.Remove(renamedKey))
                    {
                        this.Logger.LogDebug("Removed '{RenamedKey}' from Pipeline.Items.", renamedKey);
                    }
                }
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
