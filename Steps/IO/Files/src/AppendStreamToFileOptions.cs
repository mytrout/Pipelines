// <copyright file="AppendStreamToFileOptions.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.IO.Files
{
    /*
     *  IMPORTANT NOTE: As long as this class only contains compiler-generated functionality, it requires no unit tests.
     */

    /// <summary>
    /// Provides caller-configurable options to change the behavior of <see cref="AppendStreamToFileStep"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class AppendStreamToFileOptions
    {
        /// <summary>
        /// Gets or sets the base directory to which the application should write the file.
        /// </summary>
        public string AppendFileBaseDirectory { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the timing of when the <see cref="AppendStreamToFileStep"/> will be executed.
        /// </summary>
        public ExecutionTimings ExecutionTimings { get; set; } = ExecutionTimings.After;

        /// <summary>
        /// Gets or sets the name used for the writing to the output stream in the Pipeline Context.
        /// </summary>
        public string OutputStreamContextName { get; set; } = PipelineContextConstants.OUTPUT_STREAM;

        /// <summary>
        /// Gets or sets the name used to which to write the file from the Pipeline Context.
        /// </summary>
        public string TargetFileContextName { get; set; } = FileConstants.TARGET_FILE;
    }
}
