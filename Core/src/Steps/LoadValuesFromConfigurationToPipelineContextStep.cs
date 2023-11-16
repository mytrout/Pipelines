// <copyright file="LoadValuesFromConfigurationToPipelineContextStep.cs" company="Chris Trout">
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
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using MyTrout.Pipelines;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Loads values from <see cref="IConfiguration"/> into <see cref="IPipelineContext.Items"/>.
    /// </summary>
    public class LoadValuesFromConfigurationToPipelineContextStep : AbstractCachingPipelineStep<LoadValuesFromConfigurationToPipelineContextStep, LoadValuesFromConfigurationToPipelineContextOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadValuesFromConfigurationToPipelineContextStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        /// /// <param name="options">THe options to configure this pipeline.</param>
        public LoadValuesFromConfigurationToPipelineContextStep(ILogger<LoadValuesFromConfigurationToPipelineContextStep> logger, LoadValuesFromConfigurationToPipelineContextOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        /// <inheritdoc />
        public override IEnumerable<string> CachedItemNames => this.Options.ConfigurationNames;

        /// <summary>
        /// Loads values from <see cref="IConfiguration"/> into the <see cref="IPipelineContext.Items"/>.
        /// </summary>
        /// <param name="context">The <see cref="IPipelineContext">context</see> passed during pipeline execution.</param>
        /// <returns>A <see cref="Task" />.</returns>
        protected override async Task InvokeCachedCoreAsync(IPipelineContext context)
        {
            try
            {
                foreach (var itemName in this.Options.ConfigurationNames)
                {
                    context.Items.Add(itemName, this.Options.Configuration.GetValue<object>(itemName)!);
                }

                await this.Next.InvokeAsync(context);
            }
            finally
            {
                foreach (var itemName in this.Options.ConfigurationNames)
                {
                    context.Items.Remove(itemName);
                }
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
