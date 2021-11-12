// <copyright file="ReadStreamFromFtpStep.cs" company="Chris Trout">
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
    using System.IO;
    using System.Threading.Tasks;
    using WinSCP;

    public class ReadStreamFromFtpStep : AbstractPipelineStep<ReadStreamFromFtpStep, TransferOptions>
    {
        public ReadStreamFromFtpStep(ILogger<ReadStreamFromFtpStep> logger, TransferOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        protected override async Task InvokeCoreAsync(IPipelineContext context)
        {
            context.AssertValueIsValid<Session>(FtpCommunicationConstants.FTP_SESSION);
            context.AssertStringIsNotWhiteSpace(FtpCommunicationConstants.FTP_HOSTNAME);
            context.AssertStringIsNotWhiteSpace(FtpCommunicationConstants.FTP_SOURCE_FILE);

            var session = (context.Items[FtpCommunicationConstants.FTP_SESSION] as Session)!;
            var hostName = (context.Items[FtpCommunicationConstants.FTP_HOSTNAME] as string)!;
            string sourcePathAndFileName = (context.Items[FtpCommunicationConstants.FTP_SOURCE_FILE] as string)!;

            Stream? stream = null;
            try
            {
                stream = session.GetFile(sourcePathAndFileName, this.Options);

                this.Logger.LogInformation("File '{sourcePathAndFileName}' retrieved from {hostName}.", sourcePathAndFileName, hostName);

                context.Items.Add(PipelineContextConstants.INPUT_STREAM, stream);

                await this.Next.InvokeAsync(context).ConfigureAwait(false);
            }
            finally
            {
                context.Items.Remove(PipelineContextConstants.INPUT_STREAM);

                stream?.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
