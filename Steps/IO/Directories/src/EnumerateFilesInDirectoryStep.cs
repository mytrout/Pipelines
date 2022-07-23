// <copyright file="EnumerateFilesInDirectoryStep.cs" company="Chris Trout">
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
    using MyTrout.Pipelines;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Enumerates matching files from a directory on the file system.
    /// </summary>
    public class EnumerateFilesInDirectoryStep : AbstractCachingPipelineStep<EnumerateFilesInDirectoryStep, EnumerateFilesInDirectoryOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerateFilesInDirectoryStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public EnumerateFilesInDirectoryStep(ILogger<EnumerateFilesInDirectoryStep> logger, EnumerateFilesInDirectoryOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        /// <inheritdoc />
        public override IEnumerable<string> CachedItemNames => new List<string>() { this.Options.TargetBaseDirectoryPathContextName, this.Options.TargetFileContextName };

        /// <summary>
        /// Guarantees that all Context Names are valid prior to execution of caching.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task"/>.</returns>
        protected override Task InvokeBeforeCacheAsync(IPipelineContext context)
        {
            context.AssertStringIsNotWhiteSpace(this.Options.FileSearchPatternContextName);
            context.AssertStringIsNotWhiteSpace(this.Options.SourceBaseDirectoryPathContextName);
            context.AssertStringIsNotWhiteSpace(this.Options.SourceDirectoryContextName);

            return base.InvokeBeforeCacheAsync(context);
        }

        /// <summary>
        /// Enumerates matching file in the source directory location.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        /// <remarks><paramref name="context"/> is guaranteed to not be -<see langword="null" /> by the base class.</remarks>
        protected override async Task InvokeCachedCoreAsync(IPipelineContext context)
        {
            string fileSearchPattern = (context.Items[this.Options.FileSearchPatternContextName] as string)!;
            string sourceBasePath = (context.Items[this.Options.SourceBaseDirectoryPathContextName] as string)!;
            string sourceDirectoryName = (context.Items[this.Options.SourceDirectoryContextName] as string)!;

            string sourcePath = Path.Combine(sourceBasePath, sourceDirectoryName);

            foreach (var fileName in Directory.EnumerateFiles(sourcePath, fileSearchPattern))
            {
                this.Logger.LogInformation("Process {FileName} from {SearchDirectory} with the following pattern {FileSearchPattern}.", fileName, sourcePath, fileSearchPattern);

                try
                {
                    context.Items.Add(this.Options.TargetBaseDirectoryPathContextName, Path.GetDirectoryName(fileName)!);
                    context.Items.Add(this.Options.TargetFileContextName, Path.GetFileName(fileName)!);
                    await this.Next.InvokeAsync(context);
                }
                finally
                {
                    context.Items.Remove(this.Options.TargetBaseDirectoryPathContextName);
                    context.Items.Remove(this.Options.TargetFileContextName);
                }
            }
        }
    }
}
