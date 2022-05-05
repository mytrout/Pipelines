// <copyright file="LoadValuesFromContextObjectToPipelineContextStep{TObject}.cs" company="Chris Trout">
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
    using MyTrout.Pipelines;
    using System.Threading.Tasks;

    /// <summary>
    /// Loads the properties for <typeparamref name="TObject"/> named <see cref="LoadValuesFromContextObjectToPipelineContextOptions.InputObjectContextName"/> in <see cref="IPipelineContext.Items"/> to the context.
    /// </summary>
    /// <typeparam name="TObject">The object from which new properties will be loaded into <see cref="IPipelineContext.Items"/>.</typeparam>
    public class LoadValuesFromContextObjectToPipelineContextStep<TObject> : AbstractLoadItemToPipelineContextStep<TObject, LoadValuesFromContextObjectToPipelineContextOptions>
        where TObject : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadValuesFromContextObjectToPipelineContextStep{TObject}" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        /// /// <param name="options">THe options to configure this pipeline.</param>
        protected LoadValuesFromContextObjectToPipelineContextStep(ILogger<LoadValuesFromContextObjectToPipelineContextStep<TObject>> logger, LoadValuesFromContextObjectToPipelineContextOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        /// <summary>
        /// Validates that the input object exists before the caching for this step is invoked.
        /// </summary>
        /// <param name="context">The <paramref name="context"/> passed during pipeline execution.</param>
        /// <returns>A <see cref="Task" />.</returns>
        protected override Task InvokeBeforeCacheAsync(IPipelineContext context)
        {
            context.AssertValueIsValid<TObject>(this.Options.InputObjectContextName);
            return base.InvokeBeforeCacheAsync(context);
        }

        /// <summary>
        /// Retrieve item from <see cref="IPipelineContext.Items"/>.
        /// </summary>
        /// <param name="context">The <paramref name="context"/> passed during pipeline execution.</param>
        /// <returns>The object from which to load values.</returns>
        protected override TObject RetrieveItemToLoad(IPipelineContext context)
        {
            return (context.Items[this.Options.InputObjectContextName] as TObject)!;
        }
    }
}
