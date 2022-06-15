// <copyright file="DeleteFileStep.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2019-2022 Chris Trout
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

namespace MyTrout.Pipelines.Steps.IO.Files
{
    using Microsoft.Extensions.Logging;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Deletes a file located on the file system.
    /// </summary>
    public class DeleteFileStep : AbstractPipelineStep<DeleteFileStep, DeleteFileOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteFileStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public DeleteFileStep(ILogger<DeleteFileStep> logger, DeleteFileOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        /// <summary>
        /// Deletes a file from the specified location.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        /// <remarks><paramref name="context"/> is guaranteed to not be -<see langword="null" /> by the base class.</remarks>
        protected override async Task InvokeCoreAsync(IPipelineContext context)
        {
            if (this.Options.ExecutionTimings.HasFlag(ExecutionTimings.Before))
            {
                await DeleteFileStep.DeleteFileAsync(context, this.Options).ConfigureAwait(false);
            }

            await this.Next.InvokeAsync(context).ConfigureAwait(false);

            if (this.Options.ExecutionTimings.HasFlag(ExecutionTimings.After))
            {
                await DeleteFileStep.DeleteFileAsync(context, this.Options).ConfigureAwait(false);
            }
        }

        private static Task DeleteFileAsync(IPipelineContext context, DeleteFileOptions options)
        {
            context.AssertFileNameParameterIsValid(options.TargetFileContextName, options.DeleteFileBaseDirectory);

            string targetFile = (context.Items[options.TargetFileContextName] as string)!;

            targetFile = targetFile.GetFullyQualifiedPath(options.DeleteFileBaseDirectory);

            File.Delete(targetFile);

            return Task.CompletedTask;
        }
    }
}
