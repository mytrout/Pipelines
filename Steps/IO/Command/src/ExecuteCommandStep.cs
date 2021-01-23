// <copyright file="ExecuteCommandStep.cs" company="Chris Trout">
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
    using Microsoft.Extensions.Logging;
    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Steps;
    using MyTrout.Pipelines.Steps.IO.Files;
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading.Tasks;

    /// <summary>
    /// Runs a command line utility with arguments.
    /// </summary>
    public class ExecuteCommandStep : AbstractPipelineStep<ExecuteCommandStep, ExecuteCommandOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteCommandStep"/> class.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public ExecuteCommandStep(ILogger<ExecuteCommandStep> logger, ExecuteCommandOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        /// <summary>
        /// Runs a command line utility and captures the output.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        protected async override Task InvokeCoreAsync(IPipelineContext context)
        {
            var arguments = this.Options.Arguments;

            if (this.Options.IncludeFileNameTransformInArguments)
            {
                context.AssertStringIsNotWhiteSpace(FileConstants.TARGET_FILE);

                var fileName = context.Items[FileConstants.TARGET_FILE] as string;
                arguments = string.Format(CultureInfo.CurrentCulture, this.Options.Arguments, fileName);
            }

            using (Process process = new Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.FileName = this.Options.CommandString;

                process.StartInfo.Arguments = arguments;
                process.Start();

                await process.WaitForExitAsync(context.CancellationToken).ConfigureAwait(false);

                var exceptionOutput = await process.StandardError.ReadToEndAsync().ConfigureAwait(false);

                var output = await process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);

                if (exceptionOutput.Length > 0)
                {
                    this.Logger.LogInformation(exceptionOutput);
                    context.Errors.Add(new InvalidOperationException(exceptionOutput));
                }
                else
                {
                    this.Logger.LogInformation(output);
                    var status = output.Contains(this.Options.ExpectedResult) ? CommandLineConstants.COMMAND_LINE_SUCCESS : CommandLineConstants.COMMAND_LINE_FAILED;
                    context.Items.Add(CommandLineConstants.COMMAND_LINE_STATUS, status);
                }
            }

            await this.Next.InvokeAsync(context).ConfigureAwait(false);
        }
    }
}
