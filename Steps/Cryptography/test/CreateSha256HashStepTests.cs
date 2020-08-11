// <copyright file="CreateSha256HashStepTests.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2020 Chris Trout
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

namespace MyTrout.Pipelines.Steps.Cryptography.Tests
{
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines.Core;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CreateSha256HashStepTests
    {
        [TestMethod]
        public void Constructs_CreateSha256HashStep_Successfully()
        {
            // arrange
            var logger = new Mock<ILogger<CreateSha256HashStep>>().Object;
            var options = new CreateSha256HashOptions();

            var next = new Mock<IPipelineRequest>().Object;

            // act
            var result = new CreateSha256HashStep(logger, options, next);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(logger, result.Logger);
            Assert.AreEqual(next, result.Next);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_OutputStream_Is_Not_In_Context()
        {
            // arrange
            int errorCount = 1;
            var logger = new Mock<ILogger<CreateSha256HashStep>>().Object;
            var options = new CreateSha256HashOptions();

            PipelineContext context = new PipelineContext();

            var nextMock = new Mock<IPipelineRequest>();
            nextMock.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = nextMock.Object;

            var source = new CreateSha256HashStep(logger, options, next);

            var expectedMessage = Pipelines.Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, PipelineContextConstants.OUTPUT_STREAM);

            // act
            await source.InvokeAsync(context);

            // assert
            Assert.AreEqual(errorCount, context.Errors.Count);
            Assert.AreEqual(expectedMessage, context.Errors[0].Message);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Stream_Is_Hashed_Successfully()
        {
            // arrange
            string contents = "Hey na, hey na, my boyfriend's back.";
            string expectedHash = "8F54BD082762BA536EF10CAB44C1B6CBFCC95648F15ACD5AB77BF193BB1EBBCF";
            string previousHashString = "1ed507463ae3051f28aa79751aa042d2ff872498dca98d0a593ad83f62781947";

            using (var previousHash = new MemoryStream())
            {
                using (var outputStream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
                {
                    outputStream.Position = 0;
                    PipelineContext context = new PipelineContext();
                    context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, outputStream);
                    context.Items.Add(CryptographyConstants.HASH_STREAM, previousHash);
                    context.Items.Add(CryptographyConstants.HASH_STRING, previousHashString);
                    context.Items.Add("TEST_CHECK_HASH", expectedHash);

                    var logger = new Mock<ILogger<CreateSha256HashStep>>().Object;
                    var options = new CreateSha256HashOptions();

                    var nextMock = new Mock<IPipelineRequest>();
                    nextMock.Setup(x => x.InvokeAsync(context))
                                            .Returns(Task.CompletedTask);
                    var next = nextMock.Object;

                    var source = new CreateSha256HashStep(logger, options, next);

                    // act
                    await source.InvokeAsync(context);

                    // assert
                    if (context.Errors.Any())
                    {
                        throw context.Errors[0];
                    }

                    Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_STREAM));
                    Assert.AreEqual(outputStream, context.Items[PipelineContextConstants.OUTPUT_STREAM]);
                    Assert.IsTrue(outputStream.CanRead, "Stream should not be closed.");

                    using (var hashStream = context.Items[CryptographyConstants.HASH_STREAM] as Stream)
                    {
                        using (StreamReader reader = new StreamReader(hashStream, leaveOpen: true))
                        {
                            var actualHash = await reader.ReadToEndAsync();
                            Assert.AreEqual(expectedHash, actualHash);
                        }
                    }

                    Assert.AreEqual(expectedHash, context.Items[CryptographyConstants.HASH_STRING]);
                }
            }
        }
    }
}
