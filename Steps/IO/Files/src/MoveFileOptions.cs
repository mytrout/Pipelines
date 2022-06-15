// <copyright file="MoveFileOptions.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2019-2022 Chris Trout
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

namespace MyTrout.Pipelines.Steps.IO.Files
{
    /*
     *  IMPORTANT NOTE: As long as this class only contains compiler-generated functionality, it requires no unit tests.
     */

    /// <summary>
    /// Provides caller-configurable options to change the behavior of <see cref="MoveFileStep"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class MoveFileOptions
    {
        /// <summary>
        /// Gets or sets the timing of when the <see cref="MoveFileStep"/> will be executed.
        /// </summary>
        public ExecutionTimings ExecutionTimings { get; set; } = ExecutionTimings.After;

        /// <summary>
        /// Gets or sets a base directory for the source file name.
        /// </summary>
        public string MoveSourceFileBaseDirectory { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a base directory for the target file name.
        /// </summary>
        public string MoveTargetFileBaseDirectory { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name used to read the file from the Pipeline Context.
        /// </summary>
        public string SourceFileContextName { get; set; } = FileConstants.SOURCE_FILE;

        /// <summary>
        /// Gets or sets the name used to which to write the file from the Pipeline Context.
        /// </summary>
        public string TargetFileContextName { get; set; } = FileConstants.TARGET_FILE;
    }
}
