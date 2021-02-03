// <copyright file="MoveFileStep.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2019-2020 Chris Trout
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
    using System;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Moves a file from one location to another.
    /// </summary>
    public class MoveFileStep : AbstractPipelineStep<MoveFileStep, MoveFileOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveFileStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        public MoveFileStep(ILogger<MoveFileStep> logger, IPipelineRequest next, MoveFileOptions options)
            : base(logger, options, next)
        {
            // no op
        }

        /// <summary>
        /// Moves file from one location to another.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        /// <remarks><paramref name="context"/> is guaranteed to not be -<see langword="null" /> by the base class.</remarks>
        protected override async Task InvokeCoreAsync(IPipelineContext context)
        {
            if (this.Options.ExecutionTimings.HasFlag(ExecutionTimings.Before))
            {
                await MoveFileStep.MoveFileAsync(context, this.Options);
            }

            await this.Next.InvokeAsync(context).ConfigureAwait(false);

            if (this.Options.ExecutionTimings.HasFlag(ExecutionTimings.After))
            {
                await MoveFileStep.MoveFileAsync(context, this.Options);
            }
        }

        private static Task MoveFileAsync(IPipelineContext context, MoveFileOptions options)
        {
            context.AssertFileNameParameterIsValid(FileConstants.SOURCE_FILE, options.MoveSourceFileBaseDirectory);
            context.AssertFileNameParameterIsValid(FileConstants.TARGET_FILE, options.MoveTargetFileBaseDirectory);

#pragma warning disable CS8600, CS8604 // AssertFileNameParameterIsValid guarantees that SOURCE_FILE and TARGET_FILE is not null.

            string sourceFile = context.Items[FileConstants.SOURCE_FILE] as string;

            sourceFile = sourceFile.GetFullyQualifiedPath(options.MoveSourceFileBaseDirectory);

            if (!File.Exists(sourceFile))
            {
                throw new InvalidOperationException(Resources.FILE_DOES_NOT_EXIST(CultureInfo.CurrentCulture, sourceFile));
            }

            string targetFile = context.Items[FileConstants.TARGET_FILE] as string;
            targetFile = targetFile.GetFullyQualifiedPath(options.MoveTargetFileBaseDirectory);

            if (File.Exists(targetFile))
            {
                throw new InvalidOperationException(Resources.FILE_ALREADY_EXISTS(CultureInfo.CurrentCulture, targetFile));
            }

            string workingPath = Path.GetDirectoryName(targetFile);

            if (!Directory.Exists(workingPath))
            {
                Directory.CreateDirectory(workingPath);
            }

            File.Move(sourceFile, targetFile);

#pragma warning restore CS8600, CS8604

            return Task.CompletedTask;
        }
    }
}
