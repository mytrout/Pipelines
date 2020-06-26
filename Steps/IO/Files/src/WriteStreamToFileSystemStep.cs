// <copyright file="WriteFileToFileSystemStep.cs" company="Chris Trout">
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
    using MyTrout.Pipelines;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Writes a file tp the File System from an output <see cref="Stream"/> in <see cref="PipelineContext"/>.
    /// </summary>
    public class WriteStreamToFileSystemStep : AbstractPipelineStep<WriteStreamToFileSystemStep, WriteStreamToFileSystemOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteStreamToFileSystemStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        public WriteStreamToFileSystemStep(ILogger<WriteStreamToFileSystemStep> logger, IPipelineRequest next, WriteStreamToFileSystemOptions options)
            : base(logger, options, next)
        {
            // no op
        }

        /// <summary>
        /// Reads a file from the configured file system location.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        /// <remarks><paramref name="context"/> is guaranteed to not be -<see langword="null" /> by the base class.</remarks>
        protected override async Task InvokeCoreAsync(IPipelineContext context)
        {
            if (!context.Items.ContainsKey(PipelineFileConstants.TARGET_FILE))
            {
                context.Errors.Add(new InvalidOperationException(Resources.KEY_DOES_NOT_EXIST_IN_CONTEXT(CultureInfo.CurrentCulture, PipelineFileConstants.TARGET_FILE)));
            }
            else
            {
                string workingFile = context.Items[PipelineFileConstants.TARGET_FILE] as string;

                if (string.IsNullOrWhiteSpace(workingFile))
                {
                    context.Errors.Add(new InvalidOperationException(Resources.VALUE_IS_WHITESPACE_IN_CONTEXT(CultureInfo.CurrentCulture, PipelineFileConstants.TARGET_FILE)));
                }
                else
                {
                    if (!Path.IsPathFullyQualified(workingFile))
                    {
                        workingFile = Path.Combine(this.Options.WriteFileBaseDirectory, workingFile);
                    }

                    workingFile = Path.GetFullPath(workingFile);

                    if (!workingFile.StartsWith(this.Options.WriteFileBaseDirectory, StringComparison.OrdinalIgnoreCase))
                    {
                        context.Errors.Add(new InvalidOperationException(Resources.PATH_TRAVERSAL_ISSUE(CultureInfo.CurrentCulture, this.Options.WriteFileBaseDirectory, workingFile)));
                    }
                    else
                    {
                        if (!context.Items.TryGetValue(PipelineContextConstants.OUTPUT_STREAM, out object workingItem))
                        {
                            context.Errors.Add(new InvalidOperationException(Resources.NO_OUTPUT_STREAM(CultureInfo.CurrentCulture)));
                        }
                        else
                        {
                            Stream workingStream = workingItem as Stream;

                            if (workingStream == null)
                            {
                                context.Errors.Add(new InvalidOperationException(Resources.INVALID_OUTPUT_STREAM(CultureInfo.CurrentCulture)));
                            }
                            else
                            {
                                using (FileStream outputStream = File.OpenWrite(workingFile))
                                {
                                    await workingStream.CopyToAsync(outputStream).ConfigureAwait(false);
                                }

                                await this.Next.InvokeAsync(context).ConfigureAwait(false);
                            }
                        }
                    }
                }
            }
        }
    }
}