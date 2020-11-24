// <copyright file="ResourcesTests.cs" company="Chris Trout">
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
namespace MyTrout.Pipelines.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Globalization;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class ResourcesTests
    {
        [TestMethod]
        public void Returns_US_English_On_SMOKE_TEST()
        {
            // arrange
            var cultureInfo = new CultureInfo("en-US");
            const string expectedSmokeTest = "Smoke Test (en-US)";

            // act
            string result = Resources._SMOKE_TEST(cultureInfo);

            // assert
            Assert.AreEqual(expectedSmokeTest, result);
        }

        [TestMethod]
        public void Returns_GB_English_On_SMOKE_TEST()
        {
            // arrange
            var cultureInfo = new CultureInfo("en-GB");
            const string expectedSmokeTest = "Smoke Test (en-GB)";

            // act
            string result = Resources._SMOKE_TEST(cultureInfo);

            // assert
            Assert.AreEqual(expectedSmokeTest, result);
        }
    }
}
