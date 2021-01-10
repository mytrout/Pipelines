// <copyright file="ReadStreamFromBlobStorageStep.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.Azure.Blobs
{
    using global::Azure.Storage.Blobs;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Reads a file from the File System and puts it into the <see cref="MyTrout.Pipelines.Core.PipelineContext" /> as an input <see cref="Stream"/>.
    /// </summary>
    public class ReadStreamFromBlobStorageStep : AbstractPipelineStep<ReadStreamFromBlobStorageStep, ReadStreamFromBlobStorageOptions>
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

        /// <summary>
        /// Writes a file to the configured file system.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        /// <remarks><paramref name="context"/> is guaranteed to not be -<see langword="null" /> by the base class.</remarks>
        protected override async Task InvokeCoreAsync(IPipelineContext context)
        {
            context.AssertStringIsNotWhiteSpace(BlobConstants.SOURCE_CONTAINER_NAME);
            context.AssertStringIsNotWhiteSpace(BlobConstants.SOURCE_BLOB);

            string connectionString = await this.Options.RetrieveConnectionStringAsync().ConfigureAwait(false);

#pragma warning disable CS8600 // Assert~ methods guarantee non-null values.

            string sourceContainer = context.Items[BlobConstants.SOURCE_CONTAINER_NAME] as string;

            string sourceBlob = context.Items[BlobConstants.SOURCE_BLOB] as string;

#pragma warning restore CS8600

            var client = new BlobContainerClient(connectionString, sourceContainer);

            if (await client.ExistsAsync().ConfigureAwait(false))
            {
                var blobClient = client.GetBlobClient(sourceBlob);
                if (await blobClient.ExistsAsync().ConfigureAwait(false))
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        await blobClient.DownloadToAsync(stream).ConfigureAwait(false);
                        context.Items.Add(PipelineContextConstants.INPUT_STREAM, stream);
                        await this.Next.InvokeAsync(context).ConfigureAwait(false);
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
        }
    }
}