// <copyright file="UpdateZipArchiveEntryStep.cs" company="Chris Trout">
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
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Threading.Tasks;

    /// <summary>
    /// Adds a <see cref="ZipArchiveEntry"/> to the <see cref="ZipArchive"/> provided by a previous step.
    /// </summary>
    public class UpdateZipArchiveEntryStep : AbstractPipelineStep<UpdateZipArchiveEntryStep>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateZipArchiveEntryStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public UpdateZipArchiveEntryStep(ILogger<UpdateZipArchiveEntryStep> logger, IPipelineRequest next)
            : base(logger, next)
        {
            // no op
        }

        /// <summary>
        /// Add a <see cref="ZipArchiveEntry"/> to the <see cref="ZipArchive"/> from the <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        protected override async Task InvokeCoreAsync(IPipelineContext context)
        {
            await this.Next.InvokeAsync(context).ConfigureAwait(false);

            context.AssertValueIsValid<Stream>(PipelineContextConstants.OUTPUT_STREAM);
            context.AssertZipArchiveIsUpdatable(CompressionConstants.ZIP_ARCHIVE);
            context.AssertStringIsNotWhiteSpace(CompressionConstants.ZIP_ARCHIVE_ENTRY_NAME);

            this.Logger.LogDebug(Resources.INFO_VALIDATED(CultureInfo.CurrentCulture, nameof(UpdateZipArchiveEntryStep)));

            var outputStream = context.Items[PipelineContextConstants.OUTPUT_STREAM] as Stream;
            var zipArchive = context.Items[CompressionConstants.ZIP_ARCHIVE] as ZipArchive;

#pragma warning disable CS8600 // AssertValueIsValid guarantees that this value is not null.
            string zipEntryFileName = context.Items[CompressionConstants.ZIP_ARCHIVE_ENTRY_NAME] as string;
#pragma warning restore CS8600

            this.Logger.LogDebug(Resources.INFO_LOADED(CultureInfo.CurrentCulture, nameof(UpdateZipArchiveEntryStep), zipEntryFileName));

#pragma warning disable CS8602 // AssertValueIsValid guarantees that this value is not null.
            var archiveEntry = zipArchive.GetEntry(zipEntryFileName);
#pragma warning restore CS8602

            using (var archiveStream = archiveEntry.Open())
            {
#pragma warning disable CS8602 // AssertValueIsValid guarantees that this value is not null.
                await outputStream.CopyToAsync(archiveStream).ConfigureAwait(false);
#pragma warning restore CS8602

                this.Logger.LogInformation(Resources.ZIP_ARCHIVE_ENTRY_UPDATED(CultureInfo.CurrentCulture, zipEntryFileName));
            }
        }
    }
}
