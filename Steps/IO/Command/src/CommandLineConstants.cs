// <copyright file="CommandLineConstants.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2021 Chris Trout
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
    /// <summary>
    /// Constants defined for <see cref="MyTrout.Pipelines.Core.PipelineContext" /> items for the command line steps.
    /// </summary>
    public static class CommandLineConstants
    {
#pragma warning disable SA1310
        /// <summary>
        /// Indicates that this <see cref="MyTrout.Pipelines.Core.PipelineContext" /> item is the status after the command line utility executes.
        /// </summary>
        public const string COMMAND_LINE_STATUS = "CommandLineStatus";

        /// <summary>
        /// Indicates that the command line utility was successfully.
        /// </summary>
        public const string COMMAND_LINE_SUCCESS = "Success";

        /// <summary>
        /// Indicates that the command line utility failed.
        /// </summary>
        public const string COMMAND_LINE_FAILED = "Failed";

#pragma warning restore SA1310
    }
}
