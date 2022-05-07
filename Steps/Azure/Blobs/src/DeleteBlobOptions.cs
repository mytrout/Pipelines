// <copyright file="DeleteBlobOptions.cs" company="Chris Trout">
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
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides caller-configurable options to change the behavior of <see cref="DeleteBlobStep"/>.
    /// </summary>
    public class DeleteBlobOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteBlobOptions"/> class.
        /// </summary>
        public DeleteBlobOptions()
        {
            this.RetrieveConnectionStringAsync = this.RetrieveConnectionStringCoreAsync;
        }

        /// <summary>
        /// Gets or sets a value used to connect to Azure Blob Storage.
        /// </summary>
        public string DeleteBlobConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating when the blob deletion should be invoked.
        /// </summary>
        public ExecutionTimings ExecutionTiming { get; set; } = ExecutionTimings.After;

        /// <summary>
        /// Gets or sets the name of the of blob to delete.
        /// </summary>
        public string TargetBlobContextName { get; set; } = BlobConstants.TARGET_BLOB;

        /// <summary>
        /// Gets or sets the name of the container containing the blob to delete.
        /// </summary>
        public string TargetContainerNameContextName { get; set; } = BlobConstants.TARGET_CONTAINER_NAME;

        /// <summary>
        /// Gets or sets a caller-defined function to retrieve the Connection String.
        /// </summary>
        public Func<Task<string>> RetrieveConnectionStringAsync { get; set; }

        /// <summary>
        /// Retrieves a connection string using a caller-defined methodology.
        /// </summary>
        /// <returns>A Connection String or <see cref="string.Empty"/>.</returns>
        protected Task<string> RetrieveConnectionStringCoreAsync()
        {
            return Task.FromResult(this.DeleteBlobConnectionString);
        }
    }
}
