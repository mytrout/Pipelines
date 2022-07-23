// <copyright file="MoveDirectoryStep.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.IO.Directories
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Moves or renames a directory on the file system.
    /// </summary>
    public class MoveDirectoryStep : AbstractPipelineStep<MoveDirectoryStep, MoveDirectoryOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveDirectoryStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public MoveDirectoryStep(ILogger<MoveDirectoryStep> logger, MoveDirectoryOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        /// <summary>
        /// Moves or renames the source directory to the target directory.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        /// <remarks><paramref name="context"/> is guaranteed to not be -<see langword="null" /> by the base class.</remarks>
        protected override async Task InvokeCoreAsync(IPipelineContext context)
        {
            context.AssertStringIsNotWhiteSpace(this.Options.SourceBaseDirectoryPathContextName);
            context.AssertStringIsNotWhiteSpace(this.Options.SourceDirectoryContextName);
            context.AssertStringIsNotWhiteSpace(this.Options.TargetBaseDirectoryPathContextName);
            context.AssertStringIsNotWhiteSpace(this.Options.TargetDirectoryContextName);

            string sourceBasePath = (context.Items[this.Options.SourceBaseDirectoryPathContextName] as string)!;
            string sourceDirectory = (context.Items[this.Options.SourceDirectoryContextName] as string)!;

            string targetBasePath = (context.Items[this.Options.TargetBaseDirectoryPathContextName] as string)!;
            string targetDirectory = (context.Items[this.Options.TargetDirectoryContextName] as string)!;

            string sourcePath = Path.Combine(sourceBasePath, sourceDirectory);
            string targetPath = Path.Combine(targetBasePath, targetDirectory);

            if (Directory.Exists(targetPath))
            {
                context.Errors.Add(new InvalidOperationException(Resources.DIRECTORY_RENAME_ISSUE(sourcePath, targetPath)));
            }
            else
            {
                Directory.Move(sourcePath, targetPath);
                await this.Next.InvokeAsync(context);
            }
        }
    }
}
