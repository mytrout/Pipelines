// <copyright file="ListFilesInDirectoryFromFtpStep.cs" company="Chris Trout">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using WinSCP;

    public class ListFilesInDirectoryFromFtpStep : AbstractCachingPipelineStep<ListFilesInDirectoryFromFtpStep, ListFilesInDirectoryFromFtpOptions>
    {
        public ListFilesInDirectoryFromFtpStep(ILogger<ListFilesInDirectoryFromFtpStep> logger, ListFilesInDirectoryFromFtpOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        /// <inheritdoc />
        public override IEnumerable<string> CachedItemNames => new List<string>() { FtpCommunicationConstants.FTP_SOURCE_FILE };

        protected override async Task InvokeCachedCoreAsync(IPipelineContext context)
        {
            context.AssertValueIsValid<Session>(FtpCommunicationConstants.FTP_SESSION);
            context.AssertStringIsNotWhiteSpace(FtpCommunicationConstants.FTP_HOSTNAME);
            context.AssertValueIsValid<string>(FtpCommunicationConstants.FTP_SOURCE_DIRECTORY);

            var session = (context.Items[FtpCommunicationConstants.FTP_SESSION] as Session)!;
            var hostName = (context.Items[FtpCommunicationConstants.FTP_HOSTNAME] as string)!;
            string sourcePath = (context.Items[FtpCommunicationConstants.FTP_SOURCE_DIRECTORY] as string)!;

            try
            {
                var result = session.ListDirectory(sourcePath);

                foreach (RemoteFileInfo remoteFile in result.Files.Where(x => this.Options.Filter?.Invoke(x) ?? true))
                {
                    this.Logger.LogInformation("Listing file '{FullName}' retrieved from {hostName} to next step.", remoteFile.FullName, hostName);

                    if (context.Items.ContainsKey(FtpCommunicationConstants.FTP_SOURCE_FILE))
                    {
                        context.Items.Remove(FtpCommunicationConstants.FTP_SOURCE_FILE);
                    }

                    context.Items.Add(FtpCommunicationConstants.FTP_SOURCE_FILE, remoteFile.FullName);

                    await this.Next.InvokeAsync(context).ConfigureAwait(false);
                }
            }
            finally
            {
                context.Items.Remove(FtpCommunicationConstants.FTP_SOURCE_FILE);
            }
        }
    }
}
