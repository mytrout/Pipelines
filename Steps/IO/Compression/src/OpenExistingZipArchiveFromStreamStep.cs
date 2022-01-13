﻿// <copyright file="OpenExistingZipArchiveFromStreamStep.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2020-2021 Chris Trout
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

namespace MyTrout.Pipelines.Steps.IO.Compression
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Threading.Tasks;

    /// <summary>
    /// Unzips a <see cref="Stream"/> and calls downstream once for each file in the zip archive.
    /// </summary>
    public class OpenExistingZipArchiveFromStreamStep : AbstractPipelineStep<OpenExistingZipArchiveFromStreamStep, OpenExistingZipArchiveFromStreamOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenExistingZipArchiveFromStreamStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="options">THe options to configure this pipeline.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public OpenExistingZipArchiveFromStreamStep(ILogger<OpenExistingZipArchiveFromStreamStep> logger, OpenExistingZipArchiveFromStreamOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        /// <summary>
        /// Unzips the stream and calls the downstream caller once for each file in the zip archive.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        protected override async Task InvokeCoreAsync(IPipelineContext context)
        {
            context.AssertValueIsValid<Stream>(PipelineContextConstants.INPUT_STREAM);

            // Null-forgiving operator at the end of this line because context.AssertValueIsValid ensures non-null Stream-typed value.
            Stream archiveStream = (context.Items[PipelineContextConstants.INPUT_STREAM] as Stream)!;

            try
            {
                using (var zipArchive = new ZipArchive(archiveStream, this.Options.ZipArchiveMode, leaveOpen: true))
                {
                    context.Items.Add(CompressionConstants.ZIP_ARCHIVE, zipArchive);

                    await this.Next.InvokeAsync(context).ConfigureAwait(false);
                }
            }
            finally
            {
                if (context.Items.ContainsKey(CompressionConstants.ZIP_ARCHIVE))
                {
                    context.Items.Remove(CompressionConstants.ZIP_ARCHIVE);
                }
            }
        }
    }
}
