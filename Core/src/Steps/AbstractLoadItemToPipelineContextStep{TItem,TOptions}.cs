// <copyright file="AbstractLoadItemToPipelineContextStep{TItem,TOptions}.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2022 Chris Trout
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
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Loads the properties from <typeparamref name="TItem"/> into <see cref="IPipelineContext.Items"/>.
    /// </summary>
    /// <typeparam name="TItem">The object from which new properties will be loaded into <see cref="IPipelineContext.Items"/>.</typeparam>
    /// <typeparam name="TOptions">The Options through which the <see cref="BuildContextNameDelegate" /> will be configured.</typeparam>
    public abstract class AbstractLoadItemToPipelineContextStep<TItem, TOptions> : AbstractCachingPipelineStep<AbstractLoadItemToPipelineContextStep<TItem, TOptions>, TOptions>
        where TItem : class
        where TOptions : IContextNameBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractLoadItemToPipelineContextStep{TItem,TOptions}" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="options">THe options to configure this pipeline.</param>
        /// <param name="next">The next step in the pipeline.</param>
        /// <param name="predicates">The predicate that is evaluated by the step.</param>
        protected AbstractLoadItemToPipelineContextStep(ILogger<AbstractLoadItemToPipelineContextStep<TItem, TOptions>> logger, TOptions options, IPipelineRequest next, ExecutionPredicates? predicates = null)
            : base(logger, options, next, predicates)
        {
            // no op
        }

        /// <inheritdoc />
        public override IEnumerable<string> CachedItemNames => typeof(TItem).GetProperties().Select(x => this.Options.BuildContextName(typeof(TItem).Name, x.Name));

        /// <summary>
        /// Loads the properties from <typeparamref name="TItem"/> in the <see cref="IPipelineContext.Items"/>.
        /// </summary>
        /// <param name="context">The <see cref="IPipelineContext">context</see> passed during pipeline execution.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        protected override async Task InvokeCachedCoreAsync(IPipelineContext context)
        {
            try
            {
                TItem valueSource = this.RetrieveItemToLoad(context);

                foreach (var itemProperty in typeof(TItem).GetProperties())
                {
                    string contextName = this.Options.BuildContextName(typeof(TItem).Name, itemProperty.Name);

                    // Item1 is the PipelineContext Key and Item2 is the Getter method to invoke to get the value.
                    context.Items.Add(contextName, itemProperty.GetValue(valueSource)!);
                }

                await this.Next.InvokeAsync(context).ConfigureAwait(false);
            }
            finally
            {
                foreach (var itemProperty in typeof(TItem).GetProperties())
                {
                    string contextName = this.Options.BuildContextName(typeof(TItem).Name, itemProperty.Name);
                    context.Items.Remove(contextName);
                }
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves the item from which to load values.
        /// </summary>
        /// <param name="context">The <paramref name="context"/> passed during pipeline execution.</param>
        /// <returns>The object from which to load values.</returns>
        protected abstract TItem RetrieveItemToLoad(IPipelineContext context);
    }
}
