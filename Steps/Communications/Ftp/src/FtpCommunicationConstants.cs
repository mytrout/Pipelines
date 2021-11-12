// <copyright file="FtpCommunicationConstants.cs" company="Chris Trout">
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
    using System.Net.Http;
    using WinSCP;

    /// <summary>
    /// Constants defined for <see cref="MyTrout.Pipelines.Core.PipelineContext" /> Ftp-related items across all Steps.
    /// </summary>
    public static class FtpCommunicationConstants
    {
        /// <summary>
        /// Indicates that this <see cref="MyTrout.Pipelines.Core.PipelineContext" /> item is the Host on which the FTP activities will take place.
        /// </summary>
        public static readonly string FTP_HOSTNAME = "PIPELINE_HOSTNAME";

        /// <summary>
        /// Indicates that this <see cref="MyTrout.Pipelines.Core.PipelineContext" /> item is the <see cref="Session"/> that is available to all steps in the pipeline.
        /// </summary>
        public static readonly string FTP_SESSION = "PIPELINE_FTP_SESSION";

        /// <summary>
        /// Indicates that this <see cref="MyTrout.Pipelines.Core.PipelineContext" /> item is the file name used as the destination (or target) file for delete, move, or write operations.
        /// </summary>
        public static readonly string FTP_TARGET_FILE = "PIPELINE_FTP_TARGET_FILE_NAME";

        /// <summary>
        /// Indicates that this <see cref="MyTrout.Pipelines.Core.PipelineContext" /> item is the directory to be listed.
        /// </summary>
        public static readonly string FTP_SOURCE_DIRECTORY = "PIPELINE_FTP_SOURCE_DIRECTORY";

        /// <summary>
        /// Indicates that this <see cref="MyTrout.Pipelines.Core.PipelineContext" /> item is the file name used as the source file for read operations.
        /// </summary>
        public static readonly string FTP_SOURCE_FILE = "PIPELINE_FTP_SOURCE_FILE_NAME";

    }
}
