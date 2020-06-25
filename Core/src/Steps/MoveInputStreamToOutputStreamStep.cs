// <copyright file="MoveInputStreamToOutputStreamStep.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2019-2020 Chris Trout
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
namespace MyTrout.Pipelines.Steps
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Move the Input Stream to Output Stream in the pipeline.
    /// </summary>
    public class MoveInputStreamToOutputStreamStep : AbstractPipelineStep<MoveInputStreamToOutputStreamStep>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveInputStreamToOutputStreamStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public MoveInputStreamToOutputStreamStep(ILogger<MoveInputStreamToOutputStreamStep> logger, IPipelineRequest next)
            : base(logger, next)
        {
            // no op
        }

        /// <summary>
        /// Copy the Input Stream to Output Stream in the pipeline.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        protected override async Task InvokeCoreAsync(IPipelineContext context)
        {
            context.AssertParameterIsNotNull(nameof(context));

            if (!context.Items.TryGetValue(PipelineContextConstants.INPUT_STREAM, out object workingValue))
            {
                context.Errors.Add(new InvalidOperationException(Resources.INPUT_STREAM_DOES_NOT_EXIST(CultureInfo.CurrentCulture)));
            }
            else
            {
                Stream workingStream = workingValue as Stream;

                if (workingStream == null)
                {
                    context.Errors.Add(new InvalidOperationException(Resources.INPUT_STREAM_IS_NOT_TYPE_STREAM(CultureInfo.CurrentCulture)));
                }
                else
                {
                    try
                    {
                        context.Items.Remove(PipelineContextConstants.INPUT_STREAM);

                        context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, workingStream);

                        await this.Next.InvokeAsync(context).ConfigureAwait(false);
                    }
                    catch (Exception exc)
                    {
                        context.Errors.Add(exc);
                    }
                    finally
                    {
                        context.Items.Remove(PipelineContextConstants.OUTPUT_STREAM);

                        context.Items.Add(PipelineContextConstants.INPUT_STREAM, workingStream);
                    }
                }
            }
        }
    }
}
