// <copyright file="WriteStreamToBlobStorageStep.cs" company="Chris Trout">
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
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Writes a file tp the File System from an output <see cref="Stream"/> in <see cref="MyTrout.Pipelines.Core.PipelineContext"/>.
    /// </summary>
    public class WriteStreamToBlobStorageStep : AbstractPipelineStep<WriteStreamToBlobStorageStep, WriteStreamToBlobStorageOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteStreamToBlobStorageStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public WriteStreamToBlobStorageStep(ILogger<WriteStreamToBlobStorageStep> logger, WriteStreamToBlobStorageOptions options, IPipelineRequest next)
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

            context.AssertStringIsNotWhiteSpace(this.Options.TargetContainerNameContextName);
            context.AssertStringIsNotWhiteSpace(this.Options.TargetBlobContextName);
            context.AssertValueIsValid<Stream>(this.Options.OutputStreamContextName);

#pragma warning disable CS8600 // Assert~ methods guarantees non-null values.

            string targetContainer = context.Items[this.Options.TargetContainerNameContextName] as string;

            string targetBlob = context.Items[this.Options.TargetBlobContextName] as string;

#pragma warning restore CS8600

            string connectionString = await this.Options.RetrieveConnectionStringAsync().ConfigureAwait(false);

            var client = new BlobContainerClient(connectionString, targetContainer);

            await client.CreateIfNotExistsAsync().ConfigureAwait(false);

            var blobClient = client.GetBlobClient(targetBlob);

            if (await blobClient.ExistsAsync().ConfigureAwait(false))
            {
                context.Errors.Add(new InvalidOperationException(Resources.BLOB_ALREADY_EXISTS(CultureInfo.CurrentCulture, targetBlob, targetContainer)));
            }
            else
            {
                var outputStream = context.Items[this.Options.OutputStreamContextName] as Stream;
                await blobClient.UploadAsync(outputStream).ConfigureAwait(false);
            }
        }
    }
}