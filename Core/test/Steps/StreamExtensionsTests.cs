// <copyright file="StreamExtensionsTests.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class StreamExtensionsTests
    {
        [TestMethod]
        public async Task Returns_ByteArray_From_ConvertStreamToByteArray_When_Source_Is_A_MemoryStream()
        {
            // arrange
            string contents = "Who's afraid of the Big Bad Wolf?";
            byte[] byteContents = Encoding.UTF8.GetBytes(contents);
            using (MemoryStream stream = new MemoryStream(byteContents))
            {
                // act
                byte[] result = await stream.ConvertStreamToByteArrayAsync();

                // assert
                Assert.IsTrue(stream.CanRead, "Stream should remain open.");
                Assert.AreEqual(contents, Encoding.UTF8.GetString(result));
            }
        }

        [TestMethod]
        public async Task Returns_ByteArray_From_ConvertStreamToByteArray_When_Source_Is_Not_A_MemoryStream()
        {
            // arrange
            using (var stream = File.OpenRead(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "test.txt"))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    string contents = Encoding.UTF8.GetString(memoryStream.ToArray());

                    // act
                    byte[] result = await stream.ConvertStreamToByteArrayAsync();

                    // assert
                    Assert.IsTrue(stream.CanRead, "Stream should remain open.");
                    string actualContents = Encoding.UTF8.GetString(result);
                    Assert.AreEqual(contents, actualContents);
                }
            }
        }

        [TestMethod]
        public async Task Throws_ArgumentNullException_From_ConvertStreamToByteArray_When_Source_Is_Null()
        {
            // arrange
            Stream expectedValue = null;
            string parameterName = "source";

            // act
            var result = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await expectedValue.ConvertStreamToByteArrayAsync());

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(parameterName, result.ParamName);
        }
    }
}
