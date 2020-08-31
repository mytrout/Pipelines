// <copyright file="DeleteBlobStep.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.Azure.Blobs
{
    using global::Azure.Storage.Blobs;
    using global::Azure.Storage.Blobs.Models;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;

    /// <summary>
    /// Deletes a blob located in Azure Blob Storage.
    /// </summary>
    public class DeleteBlobStep : AbstractPipelineStep<DeleteBlobStep, DeleteBlobOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteBlobStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        public DeleteBlobStep(ILogger<DeleteBlobStep> logger, IPipelineRequest next, DeleteBlobOptions options)
            : base(logger, options, next)
        {
            // no op
        }

        /// <summary>
        /// Deletes a blob located in Azure Blob Storage.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        /// <remarks><paramref name="context"/> is guaranteed to not be <see langword="null" /> by the base class.</remarks>
        protected override async Task InvokeCoreAsync(IPipelineContext context)
        {
            BlobContainerClient client = null;
            string connectionString = await this.Options.RetrieveConnectionStringAsync().ConfigureAwait(false);

            if (this.Options.ExecutionTiming.HasFlag(DeleteBlobTimings.Before))
            {
                await DeleteBlobStep.DeleteBlobAsync(context, client, connectionString).ConfigureAwait(false);
            }

            await this.Next.InvokeAsync(context).ConfigureAwait(false);

            if (this.Options.ExecutionTiming.HasFlag(DeleteBlobTimings.After))
            {
                await DeleteBlobStep.DeleteBlobAsync(context, client, connectionString).ConfigureAwait(false);
            }
        }

        private static async Task DeleteBlobAsync(IPipelineContext context, BlobContainerClient client, string connectionString)
        {
            context.AssertStringIsNotWhiteSpace(BlobConstants.TARGET_CONTAINER_NAME);
            context.AssertStringIsNotWhiteSpace(BlobConstants.TARGET_BLOB);

            string targetContainer = context.Items[BlobConstants.TARGET_CONTAINER_NAME] as string;
            string targetBlob = context.Items[BlobConstants.TARGET_BLOB] as string;

            if (client == null)
            {
                client = new BlobContainerClient(connectionString, targetContainer);
            }

            if (await client.ExistsAsync().ConfigureAwait(false))
            {
                await client.DeleteBlobIfExistsAsync(targetBlob, snapshotsOption: DeleteSnapshotsOption.IncludeSnapshots).ConfigureAwait(false);
            }
        }
    }
}
