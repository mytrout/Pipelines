// <copyright file="DirectoryConstants.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2022 Chris Trout
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

namespace MyTrout.Pipelines.Steps.IO.Directories
{
    /*
     *  IMPORTANT NOTE: As long as this class only contains compiler-generated functionality, it requires no unit tests.
     */

    /// <summary>
    /// Constants defined for <see cref="MyTrout.Pipelines.Core.PipelineContext" /> items for directory-based steps.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static class DirectoryConstants
    {
        /// <summary>
        /// Indicates that this <see cref="MyTrout.Pipelines.Core.PipelineContext"/> item is the base directory for directory operations.
        /// </summary>
        public const string BASE_DIRECTORY = "PIPELINE_BASE_DIRECTORY";

        /// <summary>
        /// Indicates that this <see cref="MyTrout.Pipelines.Core.PipelineContext"/> item is the file search pattern for the enumerate directory operation.
        /// </summary>
        public const string FILE_SEARCH_PATTERN = "PIPELINE_FILE_SEARCH_PATTERN";

        /// <summary>
        /// Indicates that this <see cref="MyTrout.Pipelines.Core.PipelineContext"/> item is the source's base directory for directory operations.
        /// </summary>
        public const string SOURCE_BASE_DIRECTORY = "PIPELINE_SOURCE_BASE_DIRECTORY";

        /// <summary>
        /// Indicates that this <see cref="MyTrout.Pipelines.Core.PipelineContext" /> item is the source directory for any directory operations.
        /// </summary>
        public const string SOURCE_DIRECTORY_NAME = "PIPELINE_SOURCE_DIRECTORY_NAME";

        /// <summary>
        /// Indicates that this <see cref="MyTrout.Pipelines.Core.PipelineContext"/> item is the target's base directory for directory operations.
        /// </summary>
        public const string TARGET_BASE_DIRECTORY = "PIPELINE_TARGET_BASE_DIRECTORY";

        /// <summary>
        /// Indicates that this <see cref="MyTrout.Pipelines.Core.PipelineContext" /> item is the target directory for any directory operations.
        /// </summary>
        public const string TARGET_DIRECTORY_NAME = "PIPELINE_TARGET_DIRECTORY_NAME";
    }
}
