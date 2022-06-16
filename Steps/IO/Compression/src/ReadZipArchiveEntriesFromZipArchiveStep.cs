// <copyright file="ReadZipArchiveEntriesFromZipArchiveStep.cs" company="Chris Trout">
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
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO.Compression;
    using System.Threading.Tasks;

    /// <summary>
    /// Read <see cref="ZipArchiveEntry"/> values contains within the <see cref="ZipArchive"/> provided to this step.
    /// </summary>
    public class ReadZipArchiveEntriesFromZipArchiveStep : AbstractCachingPipelineStep<ReadZipArchiveEntriesFromZipArchiveStep, ReadZipArchiveEntriesFromZipArchiveOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadZipArchiveEntriesFromZipArchiveStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public ReadZipArchiveEntriesFromZipArchiveStep(ILogger<ReadZipArchiveEntriesFromZipArchiveStep> logger, ReadZipArchiveEntriesFromZipArchiveOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        /// <inheritdoc />
        public override IEnumerable<string> CachedItemNames => new List<string>() { this.Options.OutputStreamContextName, this.Options.ZipArchiveEntryNameContextName };

        /// <summary>
        /// Read <see cref="ZipArchiveEntry"/> values contains within the <see cref="ZipArchive"/> provided to this step.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        protected override async Task InvokeCachedCoreAsync(IPipelineContext context)
        {
            context.AssertZipArchiveIsReadable(this.Options.ZipArchiveContextName);

            this.Logger.LogDebug(Resources.INFO_VALIDATED(CultureInfo.CurrentCulture, nameof(ReadZipArchiveEntriesFromZipArchiveStep)));

            // Null-forgiving operator at the end of this line because context.AssertZipArchiveIsReadable ensures non-null ZipArchive-typed value.
            ZipArchive zipArchive = (context.Items[this.Options.ZipArchiveContextName] as ZipArchive)!;

            this.Logger.LogDebug(Resources.INFO_LOADED(CultureInfo.CurrentCulture, nameof(ReadZipArchiveEntriesFromZipArchiveStep)));

            foreach (var archiveEntry in zipArchive.Entries)
            {
                using (var outputStream = archiveEntry.Open())
                {
                    try
                    {
                        context.Items.Add(this.Options.ZipArchiveEntryNameContextName, archiveEntry.Name);
                        context.Items.Add(this.Options.OutputStreamContextName, outputStream);

                        await this.Next.InvokeAsync(context).ConfigureAwait(false);
                    }
                    finally
                    {
                        context.Items.Remove(this.Options.ZipArchiveEntryNameContextName);
                        context.Items.Remove(this.Options.OutputStreamContextName);
                    }
                }
            }
        }
    }
}