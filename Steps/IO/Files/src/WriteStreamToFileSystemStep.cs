// <copyright file="WriteStreamToFileSystemStep.cs" company="Chris Trout">
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
    /// Writes a file tp the File System from an output <see cref="Stream"/> in <see cref="MyTrout.Pipelines.Core.PipelineContext"/>.
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
            await this.Next.InvokeAsync(context).ConfigureAwait(false);

            context.AssertFileNameParameterIsValid(FileConstants.TARGET_FILE, this.Options.WriteFileBaseDirectory);
            context.AssertValueIsValid<Stream>(PipelineContextConstants.OUTPUT_STREAM);

            string workingFile = context.Items[FileConstants.TARGET_FILE] as string;

            workingFile = workingFile.GetFullyQualifiedPath(this.Options.WriteFileBaseDirectory);

            if (File.Exists(workingFile))
            {
                throw new InvalidOperationException(Resources.FILE_ALREADY_EXISTS(CultureInfo.CurrentCulture, workingFile));
            }

            Stream workingStream = context.Items[PipelineContextConstants.OUTPUT_STREAM] as Stream;

            using (FileStream outputStream = File.OpenWrite(workingFile))
            {
                await workingStream.CopyToAsync(outputStream).ConfigureAwait(false);
            }
        }
    }
}