// <copyright file="EncryptStreamStepTests.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2022 Chris Trout
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

#pragma warning disable CA2012 // Use ValueTasks correctly
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
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class EncryptStreamStepTests
    {
        [TestMethod]
        public void Constructs_EncryptStreamStep_Successfully()
        {
            // arrange
            var logger = new Mock<ILogger<EncryptStreamStep>>().Object;
            var options = new EncryptStreamOptions();

            var next = new Mock<IPipelineRequest>().Object;

            // act
            using (var result = new EncryptStreamStep(logger, options, next))
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
            var logger = new Mock<ILogger<EncryptStreamStep>>().Object;
            var options = new EncryptStreamOptions();

            var context = new PipelineContext();

            var nextMock = new Mock<IPipelineRequest>();
            nextMock.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = nextMock.Object;

            using (var source = new EncryptStreamStep(logger, options, next))
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
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Next_Step_Verifies_Values_Passed_Downstream()
        {
            // arrange
            string contents = "Hey na, hey na, my boyfriend's back.";
            byte[] workingContents = Encoding.UTF8.GetBytes(contents);
            using (var inputStream = new MemoryStream(workingContents))
            {
                using (var cryptoProvider = Aes.Create())
                {
                    cryptoProvider.IV = Encoding.UTF8.GetBytes(TestConstants.InitializationVector);
                    cryptoProvider.Key = Encoding.UTF8.GetBytes(TestConstants.Key);

                    var options = new EncryptStreamOptions()
                    {
                        EncryptionAlgorithm = cryptoProvider,
                    };

                    var logger = new Mock<ILogger<EncryptStreamStep>>().Object;

                    var context = new PipelineContext();
                    context.Items.Add(options.InputStreamContextName, inputStream);

                    var mockNext = new Mock<IPipelineRequest>();
                    mockNext.Setup(x => x.InvokeAsync(context))
                                            .Callback(() =>
                                            {
                                                Assert.IsTrue(context.Items.ContainsKey(options.OutputStreamContextName));

                                                var outputStream = (context.Items[options.OutputStreamContextName] as Stream)!;

                                                outputStream.Position = 0;
                                                outputStream.Position = 0;
                                                using var decryptedStream = new MemoryStream();
                                                using (var decryptTransform = cryptoProvider.CreateDecryptor())
                                                {
                                                    using (var cryptoStream = new CryptoStream(outputStream, decryptTransform, CryptoStreamMode.Read, leaveOpen: true))
                                                    {
                                                        byte[] bytes = new byte[16];
                                                        int read;
                                                        do
                                                        {
                                                            // stream.Read() and stream.Write() have slightly different behavior than
                                                            // stream.ReadAsync() and stream.WriteAsync() which causes major problems
                                                            // when testing this section.
                                                            // While .GetAwaiter().GetResult() can cause deadlocks, the synchronous versions
                                                            // of these methods cause data failures.
                                                            read = cryptoStream.ReadAsync(bytes).GetAwaiter().GetResult();

                                                            decryptedStream.WriteAsync(bytes.AsMemory(0, read)).GetAwaiter().GetResult();
                                                        }
                                                        while (read > 0);
                                                    }
                                                }

                                                decryptedStream.Position = 0;
                                                using (var reader = new StreamReader(decryptedStream))
                                                {
                                                    string output = reader.ReadToEndAsync().GetAwaiter().GetResult();

                                                    Assert.AreEqual(contents, output);
                                                }
                                            })
                                            .Returns(Task.CompletedTask);
                    var next = mockNext.Object;

                    using (var step = new EncryptStreamStep(logger, options, next))
                    {
                        await step.InvokeAsync(context).ConfigureAwait(false);
                    }
                }
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Options_Uses_Different_Context_Names()
        {
            // arrange
            string contents = "Hey na, hey na, my boyfriend's back.";
            byte[] workingContents = Encoding.UTF8.GetBytes(contents);
            using (var inputStream = new MemoryStream(workingContents))
            {
                using (var cryptoProvider = Aes.Create())
                {
                    cryptoProvider.IV = Encoding.UTF8.GetBytes(TestConstants.InitializationVector);
                    cryptoProvider.Key = Encoding.UTF8.GetBytes(TestConstants.Key);

                    var options = new EncryptStreamOptions()
                    {
                        EncryptionAlgorithm = cryptoProvider,
                        InputStreamContextName = "TESTING_DIFFERENT_INPUT_STREAM",
                        OutputStreamContextName = "TESTING_DIFFERENT_OUTPUT_STREAM"
                    };

                    var logger = new Mock<ILogger<EncryptStreamStep>>().Object;

                    var context = new PipelineContext();
                    context.Items.Add(options.InputStreamContextName, inputStream);

                    var mockNext = new Mock<IPipelineRequest>();
                    mockNext.Setup(x => x.InvokeAsync(context))
                                            .Callback(() =>
                                            {
                                                Assert.IsTrue(context.Items.ContainsKey(options.OutputStreamContextName));

                                                var outputStream = (context.Items[options.OutputStreamContextName] as Stream)!;

                                                outputStream.Position = 0;
                                                outputStream.Position = 0;
                                                using var decryptedStream = new MemoryStream();
                                                using (var decryptTransform = cryptoProvider.CreateDecryptor())
                                                {
                                                    using (var cryptoStream = new CryptoStream(outputStream, decryptTransform, CryptoStreamMode.Read, leaveOpen: true))
                                                    {
                                                        byte[] bytes = new byte[16];
                                                        int read;
                                                        do
                                                        {
                                                            // stream.Read() and stream.Write() have slightly different behavior than
                                                            // stream.ReadAsync() and stream.WriteAsync() which causes major problems
                                                            // when testing this section.
                                                            // While .GetAwaiter().GetResult() can cause deadlocks, the synchronous versions
                                                            // of these methods cause data failures.
                                                            read = cryptoStream.ReadAsync(bytes).GetAwaiter().GetResult();
                                                            decryptedStream.WriteAsync(bytes.AsMemory(0, read)).GetAwaiter().GetResult();
                                                        }
                                                        while (read > 0);
                                                    }
                                                }

                                                decryptedStream.Position = 0;
                                                using (var reader = new StreamReader(decryptedStream))
                                                {
                                                    string output = reader.ReadToEndAsync().GetAwaiter().GetResult();

                                                    Assert.AreEqual(contents, output);
                                                }
                                            })
                                            .Returns(Task.CompletedTask);
                    var next = mockNext.Object;

                    using (var step = new EncryptStreamStep(logger, options, next))
                    {
                        await step.InvokeAsync(context).ConfigureAwait(false);
                    }
                }
            }
        }

        public async Task Returns_PipelineContext_From_InvokeAsync_When_Step_Restores_Original_Values()
        {
            // arrange
            string contents = "Hey na, hey na, my boyfriend's back.";
            byte[] workingContents = Encoding.UTF8.GetBytes(contents);

            using (var outputStream = new MemoryStream())
            {
                using (var inputStream = new MemoryStream(workingContents))
                {
                    using (var cryptoProvider = Aes.Create())
                    {
                        cryptoProvider.IV = Encoding.UTF8.GetBytes(TestConstants.InitializationVector);
                        cryptoProvider.Key = Encoding.UTF8.GetBytes(TestConstants.Key);

                        var options = new EncryptStreamOptions()
                        {
                            EncryptionAlgorithm = cryptoProvider,
                            InputStreamContextName = "TESTING_DIFFERENT_INPUT_STREAM",
                            OutputStreamContextName = "TESTING_DIFFERENT_OUTPUT_STREAM"
                        };

                        var logger = new Mock<ILogger<EncryptStreamStep>>().Object;

                        var context = new PipelineContext();
                        context.Items.Add(options.InputStreamContextName, inputStream);

                        var mockNext = new Mock<IPipelineRequest>();
                        mockNext.Setup(x => x.InvokeAsync(context))
                                                .Callback(() =>
                                                {
                                                    // assert - phase 1.
                                                    Assert.IsTrue(context.Items.ContainsKey(options.OutputStreamContextName));
                                                    Assert.AreNotEqual(outputStream, context.Items[options.OutputStreamContextName]);
                                                })
                                                .Returns(Task.CompletedTask);
                        var next = mockNext.Object;

                        using (var step = new EncryptStreamStep(logger, options, next))
                        {
                            await step.InvokeAsync(context).ConfigureAwait(false);
                        }

                        // assert - phase 2
                        Assert.AreEqual(2, context.Items.Count);
                        Assert.IsTrue(context.Items.ContainsKey(options.OutputStreamContextName));
                        Assert.AreEqual(outputStream, context.Items[options.OutputStreamContextName]);
                    }
                }
            }
        }
    }
}
#pragma warning restore CA2012 // Use ValueTasks correctly