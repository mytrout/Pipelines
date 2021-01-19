// <copyright file="NortonAntivirusOptionsTests.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.IO.Command.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class WindowsDefenderAntivirusOptionsTests
    {
        [TestMethod]
        public void Constructs_WindowsDefenderAntivirusOptions_Successfully()
        {
            // arrange
            var arguments = "-Scan -ScanType 3 -File \"{0}\"";
            var commandString = "C:\\Program Files\\Windows Defender\\MpCmdRun.exe";
            var expectedResult = "Failed with hr = 0x80004005";
            var includeFileNameTransformInArguments = true;

            // act
            var result = new WindowsDefenderAntivirusOptions();

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(arguments, result.Arguments);
            Assert.AreEqual(commandString, result.CommandString);
            Assert.AreEqual(expectedResult, result.ExpectedResult);
            Assert.AreEqual(includeFileNameTransformInArguments, result.IncludeFileNameTransformInArguments);
        }
    }
}
