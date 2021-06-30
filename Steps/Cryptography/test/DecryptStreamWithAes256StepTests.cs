// <copyright file="DecryptStreamWithAes256StepTests.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.Cryptography.Tests
{
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines.Core;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class DecryptStreamWithAes256StepTests
    {
        [TestMethod]
        public void Constructs_DecryptStreamWithAes256Step_Successfully()
        {
            // arrange
            var logger = new Mock<ILogger<DecryptStreamWithAes256Step>>().Object;
            var options = new DecryptStreamWithAes256Options();

            var next = new Mock<IPipelineRequest>().Object;

            // act
            using (var result = new DecryptStreamWithAes256Step(logger, options, next))
            {
                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(logger, result.Logger);
                Assert.AreEqual(options, result.Options);
                Assert.AreEqual(next, result.Next);
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_InputStream_Is_Not_In_Context()
        {
            // arrange
            int errorCount = 1;
            var logger = new Mock<ILogger<DecryptStreamWithAes256Step>>().Object;
            var options = new DecryptStreamWithAes256Options();

            var context = new PipelineContext();

            var nextMock = new Mock<IPipelineRequest>();
            nextMock.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = nextMock.Object;

            using (var source = new DecryptStreamWithAes256Step(logger, options, next))
            {
                var expectedMessage = Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, PipelineContextConstants.INPUT_STREAM);

                // act
                await source.InvokeAsync(context);

                // assert
                Assert.AreEqual(errorCount, context.Errors.Count);
                Assert.AreEqual(expectedMessage, context.Errors[0].Message);
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Stream_Is_Decrypted_Successfully()
        {
            // arrange
            string contents = "Hey na, hey na, my boyfriend's back.";

            using (var inputStream = await DecryptStreamWithAes256StepTests.EncryptValueAsync(contents))
            {
                var context = new PipelineContext();
                context.Items.Add(PipelineContextConstants.INPUT_STREAM, inputStream);
                context.Items.Add("TEST_CHECK_DECRYPTED_CONTENTS", contents);

                var logger = new Mock<ILogger<DecryptStreamWithAes256Step>>().Object;
                var options = new DecryptStreamWithAes256Options()
                {
                    // Must be 16 bytes
                    DecryptionInitializationVector = TestConstants.InitializationVector,

                    // Must be 32 bytes
                    DecryptionKey = TestConstants.Key
                };

                var nextMock = new Mock<IPipelineRequest>();
                nextMock.Setup(x => x.InvokeAsync(context))
                                        .Callback(() => DecryptStreamWithAes256StepTests.AssertDecryptedValue(context))
                                        .Returns(Task.CompletedTask);
                var next = nextMock.Object;

                using (var source = new DecryptStreamWithAes256Step(logger, options, next))
                {
                    // act
                    await source.InvokeAsync(context);

                    // assert
                    if (context.Errors.Any())
                    {
                        throw context.Errors[0];
                    }

                    Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.INPUT_STREAM));
                    Assert.AreEqual(inputStream, context.Items[PipelineContextConstants.INPUT_STREAM]);
                }
            }
        }

        internal static async Task<Stream> EncryptValueAsync(string contents)
        {
            byte[] workingContents = Encoding.UTF8.GetBytes(contents);

            var encryptedStream = new MemoryStream();

            using (var cryptoProvider = new AesCryptoServiceProvider())
            {
                byte[] key = Encoding.UTF8.GetBytes(TestConstants.Key);
                byte[] initializationVector = Encoding.UTF8.GetBytes(TestConstants.InitializationVector);

                ICryptoTransform encryptor = cryptoProvider.CreateEncryptor(key, initializationVector);

                using (var cryptoStream = new CryptoStream(encryptedStream, encryptor, CryptoStreamMode.Write, leaveOpen: true))
                {
                    await cryptoStream.WriteAsync(workingContents);
                }

                // Reset to the starting position to allow readers to read the entire stream.
                encryptedStream.Position = 0;

                return encryptedStream;
            }
        }

        private static void AssertDecryptedValue(PipelineContext context)
        {
            var stream = context.Items[PipelineContextConstants.INPUT_STREAM] as Stream;

            using (var reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                Assert.AreEqual(context.Items["TEST_CHECK_DECRYPTED_CONTENTS"], result);
            }
        }
    }
}
