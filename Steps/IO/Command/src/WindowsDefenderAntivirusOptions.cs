// <copyright file="WindowsDefenderAntivirusOptions.cs" company="Chris Trout">
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
    /// Provides the options to run anti-virus using the Windows Defender client.
    /// </summary>
    public class WindowsDefenderAntivirusOptions : ExecuteCommandOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsDefenderAntivirusOptions"/> class.
        /// </summary>
        public WindowsDefenderAntivirusOptions()
            : base()
        {
            this.Arguments = "-Scan -ScanType 3 -File \"{0}\"";
            this.CommandString  = "C:\\Program Files\\Windows Defender\\MpCmdRun.exe"; 
            this.ExpectedResult = "Failed with hr = 0x80004005";
            this.IncludeFileNameTransformInArguments = true;
        }
    }
}
