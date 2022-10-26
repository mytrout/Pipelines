// <copyright file="ReadStreamFromBlobStorageOptions.cs" company="Chris Trout">
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
    /// Provides caller-configurable options to change the behavior of <see cref="ReadStreamFromBlobStorageStep"/>.
    /// </summary>
    public class ReadStreamFromBlobStorageOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadStreamFromBlobStorageOptions"/> class.
        /// </summary>
        public ReadStreamFromBlobStorageOptions()
        {
            this.RetrieveConnectionStringAsync = this.RetrieveConnectionStringCoreAsync;
        }

        /// <summary>
        /// Gets or sets the context name for <see cref="IPipelineContext.Items"/> where the input <see cref="System.IO.Stream"/> will be loaded.
        /// </summary>
        public string InputStreamContextName { get; set; } = PipelineContextConstants.INPUT_STREAM;

        /// <summary>
        /// Gets or sets a value used to connect to Azure Blob Storage.
        /// </summary>
        public string ReadBlobStorageConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a user-defined function to retrieve the Connection String.
        /// </summary>
        public Func<Task<string>> RetrieveConnectionStringAsync { get; set; }

        /// <summary>
        /// Gets or sets the context name for the source blob.
        /// </summary>
        public string SourceBlobContextName { get; set; } = BlobConstants.SOURCE_BLOB;

        /// <summary>
        /// Gets or sets the context name for container containing the source blob.
        /// </summary>
        public string SourceContainerNameContextName { get; set; } = BlobConstants.SOURCE_CONTAINER_NAME;

        /// <summary>
        /// Retrieves a connection string using a caller-defined methodology.
        /// </summary>
        /// <returns>A Connection String or <see cref="string.Empty"/>.</returns>
        protected Task<string> RetrieveConnectionStringCoreAsync()
        {
            return Task.FromResult(this.ReadBlobStorageConnectionString);
        }
    }
}
