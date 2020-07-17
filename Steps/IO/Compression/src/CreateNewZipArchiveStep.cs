// <copyright file="CreateNewZipArchiveStep.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2020 Chris Trout
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
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Threading.Tasks;

    /// <summary>
    /// Unzips a <see cref="Stream"/> and calls downstream once for each file in the zip archive.
    /// </summary>
    public class CreateNewZipArchiveStep : AbstractPipelineStep<CreateNewZipArchiveStep>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateNewZipArchiveStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public CreateNewZipArchiveStep(ILogger<CreateNewZipArchiveStep> logger, IPipelineRequest next)
            : base(logger, next)
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
            if (context.Items.TryGetValue(CompressionConstants.ZIP_ARCHIVE, out object previousArchive))
            {
                context.Items.Remove(CompressionConstants.ZIP_ARCHIVE);
            }

            if (context.Items.TryGetValue(PipelineContextConstants.OUTPUT_STREAM, out object previousOutputStream))
            {
                if (previousOutputStream is IAsyncDisposable)
                {
                    await (previousOutputStream as IAsyncDisposable).DisposeAsync().ConfigureAwait(false);
                }

                context.Items.Remove(PipelineContextConstants.OUTPUT_STREAM);
            }

            var outputStream = new MemoryStream();

            using (ZipArchive zipArchive = new ZipArchive(outputStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                try
                {
                    this.Logger.LogInformation(Resources.ZIP_ARCHIVE_OPENED(CultureInfo.CurrentCulture, nameof(CreateNewZipArchiveStep)));

                    context.Items.Add(CompressionConstants.ZIP_ARCHIVE, zipArchive);

                    this.Logger.LogDebug(Resources.ZIP_ARCHIVE_ADDED_TO_PIPELINE(CultureInfo.CurrentCulture));

                    await this.Next.InvokeAsync(context).ConfigureAwait(false);

                    context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, outputStream);
                }
                finally
                {
                    if (context.Items.ContainsKey(CompressionConstants.ZIP_ARCHIVE))
                    {
                        context.Items.Remove(CompressionConstants.ZIP_ARCHIVE);
                        this.Logger.LogDebug(Resources.ZIP_ARCHIVE_REMOVED_FROM_PIPELINE(CultureInfo.CurrentCulture));
                    }

                    if (previousArchive != null)
                    {
                        context.Items.Add(CompressionConstants.ZIP_ARCHIVE, previousArchive);
                    }
                }
            }
        }
    }
}
