// <copyright file="ReadStreamFromFileSystemStep.cs" company="Chris Trout">
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
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Reads a file from the File System and puts it into the <see cref="MyTrout.Pipelines.Core.PipelineContext" /> as an input <see cref="Stream"/>.
    /// </summary>
    public class ReadStreamFromFileSystemStep : AbstractCachingPipelineStep<ReadStreamFromFileSystemStep, ReadStreamFromFileSystemOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadStreamFromFileSystemStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public ReadStreamFromFileSystemStep(ILogger<ReadStreamFromFileSystemStep> logger, ReadStreamFromFileSystemOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        /// <inheritdoc />
        public override IEnumerable<string> CachedItemNames => new List<string>() { this.Options.InputStreamContextName };

        /// <summary>
        /// Reads a file from the configured file system and puts it into the <see cref="MyTrout.Pipelines.Core.PipelineContext"/> named <see cref="ReadStreamFromFileSystemOptions.SourceFileContextName" />.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        /// <remarks><paramref name="context"/> is guaranteed to not be -<see langword="null" /> by the base class.</remarks>
        protected override async Task InvokeCachedCoreAsync(IPipelineContext context)
        {
            context.AssertFileNameParameterIsValid(this.Options.SourceFileContextName, this.Options.ReadFileBaseDirectory);

            string workingFile = (context.Items[this.Options.SourceFileContextName] as string)!;

            workingFile = workingFile.GetFullyQualifiedPath(this.Options.ReadFileBaseDirectory);

            if (!File.Exists(workingFile))
            {
                throw new InvalidOperationException(Resources.FILE_DOES_NOT_EXIST(CultureInfo.CurrentCulture, workingFile));
            }

            using (var inputStream = File.OpenRead(workingFile))
            {
                context.Items.Add(this.Options.InputStreamContextName, inputStream);

                try
                {
                    await this.Next.InvokeAsync(context).ConfigureAwait(false);
                }
                finally
                {
                    // Guarantees that INPUT_STREAM is removed prior to returning from this function, even when an exception is thrown in this.Next.InvokeAsync().
                    context.Items.Remove(this.Options.InputStreamContextName);
                }
            }
        }
    }
}