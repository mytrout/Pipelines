// <copyright file="ReadZipArchiveEntriesFromZipArchiveStep.cs" company="Chris Trout">
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
    /// Read <see cref="ZipArchiveEntry"/> values contains within the <see cref="ZipArchive"/> provided to this step.
    /// </summary>
    public class ReadZipArchiveEntriesFromZipArchiveStep : AbstractPipelineStep<ReadZipArchiveEntriesFromZipArchiveStep>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadZipArchiveEntriesFromZipArchiveStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public ReadZipArchiveEntriesFromZipArchiveStep(ILogger<ReadZipArchiveEntriesFromZipArchiveStep> logger, IPipelineRequest next)
            : base(logger, next)
        {
            // no op
        }

        /// <summary>
        /// Read <see cref="ZipArchiveEntry"/> values contains within the <see cref="ZipArchive"/> provided to this step.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        protected override async Task InvokeCoreAsync(IPipelineContext context)
        {
            context.AssertZipArchiveIsReadable(CompressionConstants.ZIP_ARCHIVE);

            this.Logger.LogDebug(Resources.INFO_VALIDATED(CultureInfo.CurrentCulture, nameof(ReadZipArchiveEntriesFromZipArchiveStep)));

#pragma warning disable CS8600 // AssertValueIsValid guarantees that this value is not null.
            ZipArchive zipArchive = context.Items[CompressionConstants.ZIP_ARCHIVE] as ZipArchive;
#pragma warning restore CS8600

            this.Logger.LogDebug(Resources.INFO_LOADED(CultureInfo.CurrentCulture, nameof(ReadZipArchiveEntriesFromZipArchiveStep)));

            if (context.Items.TryGetValue(PipelineContextConstants.OUTPUT_STREAM, out object? previousOutputStream))
            {
                context.Items.Remove(PipelineContextConstants.OUTPUT_STREAM);
            }

            try
            {
#pragma warning disable CS8602 // AssertValueIsValid guarantees that this value is not null.
                foreach (var archiveEntry in zipArchive.Entries)
#pragma warning restore CS8602
                {
                    using (var outputStream = archiveEntry.Open())
                    {
                        context.Items.Add(CompressionConstants.ZIP_ARCHIVE_ENTRY_NAME, archiveEntry.Name);
                        context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, outputStream);

                        await this.Next.InvokeAsync(context).ConfigureAwait(false);

                        context.Items.Remove(CompressionConstants.ZIP_ARCHIVE_ENTRY_NAME);
                        context.Items.Remove(PipelineContextConstants.OUTPUT_STREAM);
                    }
                }
            }
            finally
            {
                if (context.Items.ContainsKey(CompressionConstants.ZIP_ARCHIVE_ENTRY_NAME))
                {
                    context.Items.Remove(CompressionConstants.ZIP_ARCHIVE_ENTRY_NAME);
                }

                if (context.Items.ContainsKey(PipelineContextConstants.OUTPUT_STREAM))
                {
                    context.Items.Remove(PipelineContextConstants.OUTPUT_STREAM);
                }

                if (previousOutputStream != null)
                {
                    context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, previousOutputStream);
                }
            }
        }
    }
}