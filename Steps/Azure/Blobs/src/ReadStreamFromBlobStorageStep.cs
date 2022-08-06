// <copyright file="ReadStreamFromBlobStorageStep.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.Azure.Blobs
{
    using global::Azure.Storage.Blobs;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Reads a file from the File System and puts it into the <see cref="MyTrout.Pipelines.Core.PipelineContext" /> as an input <see cref="Stream"/>.
    /// </summary>
    public class ReadStreamFromBlobStorageStep : AbstractCachingPipelineStep<ReadStreamFromBlobStorageStep, ReadStreamFromBlobStorageOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadStreamFromBlobStorageStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public ReadStreamFromBlobStorageStep(ILogger<ReadStreamFromBlobStorageStep> logger, ReadStreamFromBlobStorageOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        /// <inheritdoc />
        public override IEnumerable<string> CachedItemNames => new List<string>() { PipelineContextConstants.INPUT_STREAM };

        /// <summary>
        /// Verifies that <see cref="ReadStreamFromBlobStorageOptions.SourceContainerNameContextName"/> and <see cref="ReadStreamFromBlobStorageOptions.SourceBlobContextName"/> exist in <see cref="IPipelineContext.Items"/>.
        /// </summary>
        /// <param name="context">The <paramref name="context"/> passed during pipeline execution.</param>
        /// <returns>A <see cref="Task" />.</returns>
        protected override Task InvokeBeforeCacheAsync(IPipelineContext context)
        {
            context.AssertStringIsNotWhiteSpace(this.Options.SourceContainerNameContextName);
            context.AssertStringIsNotWhiteSpace(this.Options.SourceBlobContextName);
            return base.InvokeBeforeCacheAsync(context);
        }

        /// <summary>
        /// Writes a file to the configured file system.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        /// <remarks><paramref name="context"/> is guaranteed to not be -<see langword="null" /> by the base class.</remarks>
        protected override async Task InvokeCachedCoreAsync(IPipelineContext context)
        {
            string connectionString = await this.Options.RetrieveConnectionStringAsync().ConfigureAwait(false);

            string sourceContainer = (context.Items[this.Options.SourceContainerNameContextName] as string)!;

            string sourceBlob = (context.Items[this.Options.SourceBlobContextName] as string)!;

            var client = new BlobContainerClient(connectionString, sourceContainer);

            if (await client.ExistsAsync().ConfigureAwait(false))
            {
                var blobClient = client.GetBlobClient(sourceBlob);
                if (await blobClient.ExistsAsync().ConfigureAwait(false))
                {
                    try
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            await blobClient.DownloadToAsync(stream).ConfigureAwait(false);
                            context.Items.Add(this.Options.InputStreamContextName, stream);
                            await this.Next.InvokeAsync(context).ConfigureAwait(false);
                        }
                    }
                    finally
                    {
                        // Clean-up after this step is completed.
                        context.Items.Remove(this.Options.InputStreamContextName);
                    }
                }
                else
                {
                    context.Errors.Add(new InvalidOperationException(Resources.BLOB_DOES_NOT_EXIST(CultureInfo.CurrentCulture, sourceContainer, sourceBlob)));
                }
            }
            else
            {
                context.Errors.Add(new InvalidOperationException(Resources.CONTAINER_DOES_NOT_EXIST(CultureInfo.CurrentCulture, sourceContainer)));
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}