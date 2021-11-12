// <copyright file="LoginToFtpStep.cs" company="Chris Trout">
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
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using WinSCP;

    public class LoginToFtpStep : AbstractPipelineStep<LoginToFtpStep, SessionOptions>
    {
        public LoginToFtpStep(ILogger<LoginToFtpStep> logger, SessionOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        protected override async Task InvokeCoreAsync(IPipelineContext context)
        {
            var session = new Session();

            try
            {
                session.Open(this.Options);

                var hostName = await Dns.GetHostEntryAsync(this.Options.HostName);
                var ipAddressList = string.Join<IPAddress>(",", hostName.AddressList);
                context.Items.Add(FtpCommunicationConstants.FTP_HOSTNAME, this.Options.HostName);

                this.Logger.LogInformation("Session opened for Host: {HostName}, IP Address: {ipAddressList}, Port: {Port}, UserName: {UserName}, Protocol: {Protocol}, FTPMode: {FtpMode},  FtpSecure: {FtpSecure}", this.Options.HostName, ipAddressList, this.Options.PortNumber, this.Options.UserName, this.Options.Protocol, this.Options.FtpMode, this.Options.FtpSecure);

                context.Items.Add(FtpCommunicationConstants.FTP_SESSION, session);

                await this.Next.InvokeAsync(context).ConfigureAwait(false);
            }
            finally
            {
                context.Items.Remove(FtpCommunicationConstants.FTP_HOSTNAME);

                if (session != null)
                {
                    var strb = new StringBuilder();

                    foreach (var outputItem in session.Output.Where(x => x is not null))
                    {
                        strb.AppendLine(outputItem);
                    }

                    session.Close();
                    this.Logger.LogInformation("Session closted for Host: {HostName}", this.Options.HostName);
                    session.Dispose();
                }
            }
        }
    }
}
