// <copyright file="SendFtpRequestStep.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.Communications.Ftp
{
    using Microsoft.Extensions.Logging;
    using MyTrout.Pipelines;
    using System.IO;
    using System.Threading.Tasks;
    using WinSCP;

    /// <summary>
    /// Sends an HTTP Request to an endpoint with the specified URI, Headers, Query String Parameters, and/or Content payload and returns an HTTP Response.
    /// </summary>
    public class SendStreamToFtpStep : AbstractPipelineStep<SendStreamToFtpStep, TransferOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendStreamToFtpStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public SendStreamToFtpStep(ILogger<SendStreamToFtpStep> logger, TransferOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        /// <summary>
        /// Send Stream to FTP Endpoint with specified configuration.
        /// </summary>
        /// <param name="context">The <see cref="IPipelineContext">context</see> passed during pipeline execution.</param>
        /// <returns>A <see cref="Task" />.</returns>
        protected override async Task InvokeCoreAsync(IPipelineContext context)
        {
            context.AssertValueIsValid<Session>(FtpCommunicationConstants.FTP_SESSION);
            context.AssertValueIsValid<Stream>(PipelineContextConstants.OUTPUT_STREAM);
            context.AssertStringIsNotWhiteSpace(FtpCommunicationConstants.FTP_HOSTNAME);
            context.AssertStringIsNotWhiteSpace(FtpCommunicationConstants.FTP_TARGET_FILE);

            var session = (context.Items[FtpCommunicationConstants.FTP_SESSION] as Session)!;
            var hostName = (context.Items[FtpCommunicationConstants.FTP_HOSTNAME] as string)!;
            string targetPathAndFileName = (context.Items[FtpCommunicationConstants.FTP_TARGET_FILE] as string)!;

            Stream stream = (context.Items[PipelineContextConstants.OUTPUT_STREAM] as Stream)!;

            session.PutFile(stream, targetPathAndFileName, this.Options);

            this.Logger.LogInformation("File '{targetPathAndFileName}' is sent to {hostName}.", targetPathAndFileName, hostName);

            await this.Next.InvokeAsync(context).ConfigureAwait(false);
        }
    }
}
