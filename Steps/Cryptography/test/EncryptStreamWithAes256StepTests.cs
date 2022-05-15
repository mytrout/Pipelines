// <copyright file="EncryptStreamWithAes256StepTests.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2020-2021 Chris Trout
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

#pragma warning disable CS0618 // Type or member is obsolete
namespace MyTrout.Pipelines.Steps.Cryptography.Tests
{
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines.Core;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class EncryptStreamWithAes256StepTests
    {
        [TestMethod]
        public void Constructs_EncryptStreamWithAes256Step_Successfully()
        {
            // arrange
            var logger = new Mock<ILogger<EncryptStreamWithAes256Step>>().Object;

            var options = new EncryptStreamWithAes256Options();

            var next = new Mock<IPipelineRequest>().Object;

            // act
            using (var result = new EncryptStreamWithAes256Step(logger, options, next))
            {
                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(logger, result.Logger);
                Assert.AreEqual(options, result.Options);
                Assert.AreEqual(next, result.Next);
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_OutputStream_Is_Not_In_Context()
        {
            // arrange
            int errorCount = 1;
            var logger = new Mock<ILogger<EncryptStreamWithAes256Step>>().Object;
            var options = new EncryptStreamWithAes256Options();

            var context = new PipelineContext();

            var nextMock = new Mock<IPipelineRequest>();
            nextMock.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = nextMock.Object;

            using (var source = new EncryptStreamWithAes256Step(logger, options, next))
            {
                var expectedMessage = Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, PipelineContextConstants.OUTPUT_STREAM);

                // act
                await source.InvokeAsync(context);

                // assert
                Assert.AreEqual(errorCount, context.Errors.Count);
                Assert.AreEqual(expectedMessage, context.Errors[0].Message);
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Stream_Is_Encrypted_Successfully()
        {
            // arrange
            string contents = "Hey na, hey na, my boyfriend's back.";
            var outputStream = new MemoryStream(Encoding.UTF8.GetBytes(contents));

            var logger = new Mock<ILogger<EncryptStreamWithAes256Step>>().Object;
            var options = new EncryptStreamWithAes256Options()
            {
                // Must be 16 bytes
                EncryptionInitializationVector = TestConstants.InitializationVector,

                // Must be 32 bytes
                EncryptionKey = TestConstants.Key
            };

            string encryptedContents = string.Empty;
            using (var workingStream = await DecryptStreamWithAes256StepTests.EncryptValueAsync(contents).ConfigureAwait(false))
            {
                using (var reader = new StreamReader(workingStream, leaveOpen: true))
                {
                    encryptedContents = await reader.ReadToEndAsync().ConfigureAwait(false);
                }
            }

            var context = new PipelineContext();
            context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, outputStream);

            var nextMock = new Mock<IPipelineRequest>();
            nextMock.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = nextMock.Object;

            using (var source = new EncryptStreamWithAes256Step(logger, options, next))
            {
                // act
                await source.InvokeAsync(context);

                // assert
                if (context.Errors.Any())
                {
                    throw context.Errors[0];
                }

                Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_STREAM));

                var resultStream = context.Items[PipelineContextConstants.OUTPUT_STREAM] as Stream;

                Assert.IsTrue(resultStream.CanRead, "This stream should still be open.");

                using (var reader = new StreamReader(resultStream))
                {
                    string result = await reader.ReadToEndAsync().ConfigureAwait(false);
                    Assert.AreEqual(result, encryptedContents);
                }
            }
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete