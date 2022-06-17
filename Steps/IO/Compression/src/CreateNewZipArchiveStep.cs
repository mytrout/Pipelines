// <copyright file="CreateNewZipArchiveStep.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2020-2022 Chris Trout
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
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Threading.Tasks;

    /// <summary>
    /// Unzips a <see cref="System.IO.Stream"/> and calls downstream once for each file in the zip archive.
    /// </summary>
    public class CreateNewZipArchiveStep : AbstractCachingPipelineStep<CreateNewZipArchiveStep, CreateNewZipArchiveOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateNewZipArchiveStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public CreateNewZipArchiveStep(ILogger<CreateNewZipArchiveStep> logger, CreateNewZipArchiveOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        /// <inheritdoc />
        public override IEnumerable<string> CachedItemNames => new List<string>() { this.Options.ZipArchiveContextName, this.Options.OutputStreamContextName };

        /// <summary>
        /// Unzips the stream and calls the downstream caller once for each file in the zip archive.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        protected override async Task InvokeCachedCoreAsync(IPipelineContext context)
        {
            using (var outputStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(outputStream, ZipArchiveMode.Create, leaveOpen: true))
                {
                    try
                    {
                        this.Logger.LogInformation(Resources.ZIP_ARCHIVE_OPENED(CultureInfo.CurrentCulture, nameof(CreateNewZipArchiveStep)));

                        context.Items.Add(this.Options.OutputStreamContextName, outputStream);
                        context.Items.Add(this.Options.ZipArchiveContextName, zipArchive);

                        this.Logger.LogDebug(Resources.ZIP_ARCHIVE_ADDED_TO_PIPELINE(CultureInfo.CurrentCulture));

                        await this.Next.InvokeAsync(context).ConfigureAwait(false);
                    }
                    finally
                    {
                        context.Items.Remove(this.Options.OutputStreamContextName);
                        context.Items.Remove(this.Options.ZipArchiveContextName);
                    }
                }
            }
        }
    }
}
