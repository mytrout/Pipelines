﻿// <copyright file="ReadFileFromFileSystemStep.cs" company="Chris Trout">
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
    /// Reads a file from the File System and puts it into the <see cref="PipelineContext" /> as an input <see cref="Stream"/>.
    /// </summary>
    public class ReadStreamFromFileSystemStep : AbstractPipelineStep<ReadStreamFromFileSystemStep, ReadStreamFromFileSystemOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadStreamFromFileSystemStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        public ReadStreamFromFileSystemStep(ILogger<ReadStreamFromFileSystemStep> logger, IPipelineRequest next, ReadStreamFromFileSystemOptions options)
            : base(logger, options, next)
        {
            // no op
        }

        /// <summary>
        /// Writes a file to the configured file system.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        /// <remarks><paramref name="context"/> is guaranteed to not be -<see langword="null" /> by the base class.</remarks>
        protected override async Task InvokeCoreAsync(IPipelineContext context)
        {
            if (!context.Items.ContainsKey(PipelineFileConstants.SOURCE_FILE))
            {
                context.Errors.Add(new InvalidOperationException(Resources.KEY_DOES_NOT_EXIST_IN_CONTEXT(CultureInfo.CurrentCulture, PipelineFileConstants.SOURCE_FILE)));
            }
            else
            {
                string workingFile = context.Items[PipelineFileConstants.SOURCE_FILE] as string;

                if (string.IsNullOrWhiteSpace(workingFile))
                {
                    context.Errors.Add(new InvalidOperationException(Resources.VALUE_IS_WHITESPACE_IN_CONTEXT(CultureInfo.CurrentCulture, PipelineFileConstants.SOURCE_FILE)));
                }
                else
                {
                    if (!Path.IsPathFullyQualified(workingFile))
                    {
                        workingFile = Path.Combine(this.Options.ReadFileBaseDirectory, workingFile);
                    }

                    workingFile = Path.GetFullPath(workingFile);

                    if (!workingFile.StartsWith(this.Options.ReadFileBaseDirectory, StringComparison.Ordinal))
                    {
                        context.Errors.Add(new InvalidOperationException(Resources.PATH_TRAVERSAL_ISSUE(CultureInfo.CurrentCulture, this.Options.ReadFileBaseDirectory, workingFile)));
                    }
                    else
                    {
                        if (context.Items.TryGetValue(PipelineContextConstants.INPUT_STREAM, out object previous))
                        {
                            context.Items.Remove(PipelineContextConstants.INPUT_STREAM);
                        }

                        try
                        {
                            using (var inputStream = File.OpenRead(workingFile))
                            {
                                context.Items.Add(PipelineContextConstants.INPUT_STREAM, inputStream);

                                await this.Next.InvokeAsync(context).ConfigureAwait(false);

                                context.Items.Remove(PipelineContextConstants.INPUT_STREAM);
                            }
                        }
                        catch (Exception exc)
                        {
                            context.Errors.Add(exc);
                        }
                        finally
                        {
                            if (context.Items.ContainsKey(PipelineContextConstants.INPUT_STREAM))
                            {
                                context.Items.Remove(PipelineContextConstants.INPUT_STREAM);
                            }

                            if (previous != null)
                            {
                                context.Items.Add(PipelineContextConstants.INPUT_STREAM, previous);
                            }
                        }
                    }
                }
            }
        }
    }
}