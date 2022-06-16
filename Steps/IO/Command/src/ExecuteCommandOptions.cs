// <copyright file="ExecuteCommandOptions.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2021-2022 Chris Trout
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

namespace MyTrout.Pipelines.Steps.IO.Command
{
    using MyTrout.Pipelines.Steps.IO.Files;

    /// <summary>
    /// Provides the options for the <see cref="ExecuteCommandStep"/>.
    /// </summary>
    public class ExecuteCommandOptions
    {
        /// <summary>
        /// Gets or sets the arguments for the command line utility.
        /// </summary>
        public string Arguments { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Pipeline Context Name for a Command Line Status.
        /// </summary>
        public string CommandLineStatusContextName { get; set; } = CommandLineConstants.COMMAND_LINE_STATUS;

        /// <summary>
        /// Gets or sets the Pipeline Context Name for a Command Line Failed Status.
        /// </summary>
        public string CommandLineFailedStatusContextName { get; set; } = CommandLineConstants.COMMAND_LINE_FAILED;

        /// <summary>
        /// Gets or sets the Pipeline Context Name for a Command Line Successful Status.
        /// </summary>
        public string CommandLineSuccessStatusContextName { get; set; } = CommandLineConstants.COMMAND_LINE_SUCCESS;

        /// <summary>
        /// Gets or sets the full path and file name to execute the command line utility.
        /// </summary>
        public virtual string CommandString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the console result text from the command line utility.
        /// </summary>
        public string ExpectedResult { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Arguments"/> has a replaceable parameter for <see cref="TargetFileContextName"/>.
        /// </summary>
        public bool IncludeFileNameTransformInArguments { get; set; } = false;

        /// <summary>
        /// Gets or sets the command line's target file for execution.
        /// </summary>
        public string TargetFileContextName { get; set; } = FileConstants.TARGET_FILE;
    }
}
