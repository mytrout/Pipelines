// <copyright file="EnumerateFilesInDirectoryOptions.cs" company="Chris Trout">
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
    /// Provides caller-configurable options to change the behavior of <see cref="EnumerateFilesInDirectoryStep"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class EnumerateFilesInDirectoryOptions
    {
        /// <summary>
        /// Gets or sets the pattern used to match file names during the enumeration.
        /// </summary>
        public string FileSearchPatternContextName { get; set; } = DirectoryConstants.FILE_SEARCH_PATTERN;

        /// <summary>
        /// Gets or sets a base directory used to prevent directory-based path traversal issues.
        /// </summary>
        public string SourceBaseDirectoryPathContextName { get; set; } = DirectoryConstants.SOURCE_BASE_DIRECTORY;

        /// <summary>
        /// Gets or sets the context name used to specify the directory to search.
        /// </summary>
        public string SourceDirectoryContextName { get; set; } = DirectoryConstants.SOURCE_DIRECTORY_NAME;

        /// <summary>
        /// Gets or sets the base directory of the target file.
        /// </summary>
        public string TargetDirectoryContextName { get; set; } = DirectoryConstants.TARGET_BASE_DIRECTORY;

        /// <summary>
        /// Gets or sets the full path and file name of the target file.
        /// </summary>
        public string TargetFileContextName { get; set; } = Files.FileConstants.TARGET_FILE;

        /// <summary>
        /// Gets or sets the file name of the target file with the file extension.
        /// </summary>
        public string TargetFileNameWithExtensionContextName { get; set; } = DirectoryConstants.TARGET_FILE_NAME_WITH_EXTENSION;

        /// <summary>
        /// Gets or sets the file name of the target file without the file extension.
        /// </summary>
        public string TargetFileNameWithoutExtensionContextName { get; set; } = DirectoryConstants.TARGET_FILE_NAME_WO_EXTENSION;
    }
}
