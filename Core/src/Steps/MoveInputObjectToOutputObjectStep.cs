// <copyright file="MoveInputObjectToOutputObjectStep.cs" company="Chris Trout">
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
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Move the Input Object to Output Object in the pipeline.
    /// </summary>
    public class MoveInputObjectToOutputObjectStep : RenameContextItemStep
    {
        private static readonly RenameContextItemOptions DefaultOptions = new()
        {
            RenameValues = { { PipelineContextConstants.INPUT_OBJECT, PipelineContextConstants.OUTPUT_OBJECT } },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveInputObjectToOutputObjectStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public MoveInputObjectToOutputObjectStep(ILogger<MoveInputObjectToOutputObjectStep> logger, IPipelineRequest next)
            : base(logger, MoveInputObjectToOutputObjectStep.DefaultOptions, next)
        {
            // no op
        }

        /// <summary>
        /// Executes code before the caching for this step is invoked.
        /// </summary>
        /// <param name="context">The <paramref name="context"/> passed during pipeline execution.</param>
        /// <returns>A <see cref="Task" />.</returns>
        protected override Task InvokeBeforeCacheAsync(IPipelineContext context)
        {
            context.AssertValueExists(PipelineContextConstants.INPUT_OBJECT);

            return base.InvokeBeforeCacheAsync(context);
        }
    }
}
