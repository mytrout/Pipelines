// <copyright file="EnumerateItemsInCollectionStep{TParent,TObject}.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2022-2023 Chris Trout
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
    using MyTrout.Pipelines;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Loads a <typeparamref name="TObject"/> into <see cref="IPipelineContext.Items"/> from <typeparamref name="TParent"/>.
    /// </summary>
    /// <typeparam name="TParent">The object with the <see cref="IEnumerable{TObject}"/> implementation.</typeparam>
    /// <typeparam name="TObject">Tbe object which is the result of the iteration.</typeparam>
    public class EnumerateItemsInCollectionStep<TParent, TObject> : AbstractCachingPipelineStep<EnumerateItemsInCollectionStep<TParent, TObject>, EnumerateItemsInCollectionOptions>
        where TParent : class, IEnumerable<TObject>
        where TObject : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerateItemsInCollectionStep{TParent,TObject}" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        /// /// <param name="options">THe options to configure this pipeline.</param>
        public EnumerateItemsInCollectionStep(ILogger<EnumerateItemsInCollectionStep<TParent, TObject>> logger, EnumerateItemsInCollectionOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        /// <inheritdoc />
        public override IEnumerable<string> CachedItemNames => new List<string>() { this.Options.OutputObjectContextName };

        /// <summary>
        /// Validates that the input object exists before the caching for this step is invoked.
        /// </summary>
        /// <param name="context">The <paramref name="context"/> passed during pipeline execution.</param>
        /// <returns>A <see cref="Task" />.</returns>
        protected async override Task BeforeNextStepAsync(IPipelineContext context)
        {
            await base.BeforeNextStepAsync(context).ConfigureAwait(false);

            // TO FUTURE DEVELOPERS: Remove this call after the next breaking change when the obsolete methods are removed.
#pragma warning disable CS0618
            await this.InvokeBeforeCacheAsync(context).ConfigureAwait(false);
#pragma warning restore CS0618

            context.AssertValueIsValid<TParent>(this.Options.InputObjectContextName);
        }

        // TO FUTURE DEVELOPERS: When the obsolete methods are removed, this method name changes to InvokeCoreAsync().
#pragma warning disable CS0672
        /// <summary>
        /// Loads a <typeparamref name="TObject"/> into <see cref="IPipelineContext.Items"/> from <typeparamref name="TParent"/>.
        /// </summary>
        /// <param name="context">The <see cref="IPipelineContext">context</see> passed during pipeline execution.</param>
        /// <returns>A <see cref="Task" />.</returns>
        protected override async Task InvokeCachedCoreAsync(IPipelineContext context)
        {
            IEnumerable<TObject> collection = (context.Items[this.Options.InputObjectContextName] as TParent)!;

            this.Logger.LogDebug("Found {count} objects in the {InputObjectContextName} collection.", collection.Count(), this.Options.InputObjectContextName);

            foreach (TObject item in collection)
            {
                try
                {
                    context.Items.Add(this.Options.OutputObjectContextName, item);
                    await this.Next.InvokeAsync(context).ConfigureAwait(false);
                }
                finally
                {
                    context.Items.Remove(this.Options.OutputObjectContextName);
                }
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }
#pragma warning restore CS0672
    }
}
