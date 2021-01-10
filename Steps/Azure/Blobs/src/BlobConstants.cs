// <copyright file="BlobConstants.cs" company="Chris Trout">
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
    /// <summary>
    /// Constants defined for <see cref="MyTrout.Pipelines.Core.PipelineContext" /> items for file-based steps.
    /// </summary>
    public static class BlobConstants
    {
        /// <summary>
        /// Indicates that this <see cref="MyTrout.Pipelines.Core.PipelineContext" /> item is the blob used as the source container for read or move operations.
        /// </summary>
        public const string SOURCE_BLOB = "PIPELINE_SOURCE_BLOB";

        /// <summary>
        /// Indicates that this <see cref="MyTrout.Pipelines.Core.PipelineContext" /> item is the container from which blobs will be read..
        /// </summary>
        public const string SOURCE_CONTAINER_NAME = "PIPELINE_SOURCE_CONTAINER_NAME";

        /// <summary>
        /// Indicates that this <see cref="MyTrout.Pipelines.Core.PipelineContext" /> item is the blob used as the destination (or target) container for delete, move, or write operations.
        /// </summary>
        public const string TARGET_BLOB = "PIPELINE_TARGET_BLOB";

        /// <summary>
        /// Indicates that this <see cref="MyTrout.Pipelines.Core.PipelineContext" /> item is the container name to which blovs will be written.
        /// </summary>
        public const string TARGET_CONTAINER_NAME = "PIPELINE_TARGET_CONTAINER_NAME";
    }
}
