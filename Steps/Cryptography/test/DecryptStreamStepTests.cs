// <copyright file="DecryptStreamStepTests.cs" company="Chris Trout">
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
    public class DecryptStreamStepTests
    {
        [TestMethod]
        public void Constructs_DecryptStreamStep_Successfully()
        {
            // arrange
            var logger = new Mock<ILogger<DecryptStreamStep>>().Object;
            var options = new DecryptStreamOptions();

            var next = new Mock<IPipelineRequest>().Object;

            // act
            using (var result = new DecryptStreamStep(logger, options, next))
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
            var logger = new Mock<ILogger<DecryptStreamStep>>().Object;
            var options = new DecryptStreamOptions();

            var context = new PipelineContext();

            var nextMock = new Mock<IPipelineRequest>();
            nextMock.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = nextMock.Object;

            using (var source = new DecryptStreamStep(logger, options, next))
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
            int expectedErrorCount = 0;
            string contents = "Hey na, hey na, my boyfriend's back.";

            byte[] workingContents = Encoding.UTF8.GetBytes(contents);

            using (var inputStream = new MemoryStream())
            {
                using (var cryptoProvider = Aes.Create())
                {
                    cryptoProvider.IV = Encoding.UTF8.GetBytes(TestConstants.InitializationVector);
                    cryptoProvider.Key = Encoding.UTF8.GetBytes(TestConstants.Key);

                    ICryptoTransform encryptor = cryptoProvider.CreateEncryptor();

                    // Encrypt the contents.
                    using (var cryptoStream = new CryptoStream(inputStream, encryptor, CryptoStreamMode.Write, leaveOpen: true))
                    {
                        await cryptoStream.WriteAsync(workingContents);
                    }

                    // Add Step Here...
                    var options = new DecryptStreamOptions
                    {
                        DecryptionAlgorithm = cryptoProvider
                    };

                    var logger = new Mock<ILogger<DecryptStreamStep>>().Object;
                    var context = new PipelineContext();
                    var mockNext = new Mock<IPipelineRequest>();
                    mockNext.Setup(x => x.InvokeAsync(context))
                                            .Callback(() =>
                                            {
                                                Assert.IsTrue(context.Items.ContainsKey(options.OutputStreamContextName));

                                                var outputStream = (context.Items[options.OutputStreamContextName] as Stream)!;
                                                outputStream.Position = 0;

                                                using (var outputReader = new StreamReader(outputStream, leaveOpen: true))
                                                {
                                                    string result = outputReader.ReadToEnd();

                                                    Assert.AreEqual(contents, result);
                                                }
                                            })
                                            .Returns(Task.CompletedTask);
                    var next = mockNext.Object;

                    context.Items.Add(options.InputStreamContextName, inputStream);

                    // act
                    using (var sut = new DecryptStreamStep(logger, options, next))
                    {
                        await sut.InvokeAsync(context).ConfigureAwait(false);
                    }

                    // assert - phase 2
                    Assert.AreEqual(expectedErrorCount, context.Errors.Count, "Step should decrypt the code without errors:\r\n{0} ", context.Errors.FirstOrDefault());
                }
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Options_Uses_Different_Context_Names()
        {
            // arrange
            int expectedErrorCount = 0;
            string contents = "Hey na, hey na, my boyfriend's back.";
            string contentInBigEndianUnicode = "\0H\0e\0y\0 \0n\0a\0,\0 \0h\0e\0y\0 \0n\0a\0,\0 \0m\0y\0 \0b\0o\0y\0f\0r\0i\0e\0n\0d\0'\0s\0 \0b\0a\0c\0k\0.";

            byte[] workingContents = Encoding.BigEndianUnicode.GetBytes(contents);

            using (var inputStream = new MemoryStream())
            {
                using (var cryptoProvider = Aes.Create())
                {
                    cryptoProvider.IV = Encoding.UTF8.GetBytes(TestConstants.InitializationVector);
                    cryptoProvider.Key = Encoding.UTF8.GetBytes(TestConstants.Key);

                    ICryptoTransform encryptor = cryptoProvider.CreateEncryptor();

                    // Encrypt the contents.
                    using (var cryptoStream = new CryptoStream(inputStream, encryptor, CryptoStreamMode.Write, leaveOpen: true))
                    {
                        await cryptoStream.WriteAsync(workingContents);
                    }

                    // Add Step Here...
                    var options = new DecryptStreamOptions()
                    {
                        DecryptionAlgorithm = cryptoProvider,
                        InputStreamContextName = "TESTING_INPUT_GOOFY_STREAM",
                        OutputStreamContextName = "TESTING_OUTPUT_GOOFY_STREAM",
                    };

                    var logger = new Mock<ILogger<DecryptStreamStep>>().Object;
                    var context = new PipelineContext();
                    var mockNext = new Mock<IPipelineRequest>();
                    mockNext.Setup(x => x.InvokeAsync(context))
                                            .Callback(() =>
                                            {
                                                Assert.IsTrue(context.Items.ContainsKey(options.OutputStreamContextName));

                                                var outputStream = (context.Items[options.OutputStreamContextName] as Stream)!;
                                                outputStream.Position = 0;

                                                using (var outputReader = new StreamReader(outputStream, leaveOpen: true))
                                                {
                                                    string result = outputReader.ReadToEnd();

                                                    Assert.AreEqual(contentInBigEndianUnicode, result);
                                                }
                                            })
                                            .Returns(Task.CompletedTask);
                    var next = mockNext.Object;

                    context.Items.Add(options.InputStreamContextName, inputStream);

                    // act
                    using (var sut = new DecryptStreamStep(logger, options, next))
                    {
                        await sut.InvokeAsync(context).ConfigureAwait(false);
                    }

                    // assert - phase 2
                    Assert.AreEqual(expectedErrorCount, context.Errors.Count, "Step should decrypt the code without errors:\r\n{0} ", context.Errors.FirstOrDefault());
                }
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Step_Restores_Original_Values()
        {
            // arranges()
            int expectedErrorCount = 0;
            int expectedItemCount = 1;
            string contents = "Hey na, hey na, my boyfriend's back.";

            byte[] workingContents = Encoding.UTF8.GetBytes(contents);

            using (var inputStream = new MemoryStream())
            {
                using (var cryptoProvider = Aes.Create())
                {
                    cryptoProvider.IV = Encoding.UTF8.GetBytes(TestConstants.InitializationVector);
                    cryptoProvider.Key = Encoding.UTF8.GetBytes(TestConstants.Key);

                    ICryptoTransform encryptor = cryptoProvider.CreateEncryptor();

                    // Encrypt the contents.
                    using (var cryptoStream = new CryptoStream(inputStream, encryptor, CryptoStreamMode.Write, leaveOpen: true))
                    {
                        await cryptoStream.WriteAsync(workingContents);
                    }

                    // Add Step Here...
                    var options = new DecryptStreamOptions
                    {
                        DecryptionAlgorithm = cryptoProvider
                    };

                    var logger = new Mock<ILogger<DecryptStreamStep>>().Object;
                    var context = new PipelineContext();
                    var mockNext = new Mock<IPipelineRequest>();
                    mockNext.Setup(x => x.InvokeAsync(context))
                                            .Returns(Task.CompletedTask);
                    var next = mockNext.Object;

                    context.Items.Add(options.InputStreamContextName, inputStream);

                    // act
                    using (var sut = new DecryptStreamStep(logger, options, next))
                    {
                        await sut.InvokeAsync(context).ConfigureAwait(false);
                    }

                    // assert - phase 2
                    Assert.AreEqual(expectedItemCount, context.Items.Count, "context.Items should only contain the Options.InputStreamContextName value, but contains more than one value.");
                    Assert.IsTrue(context.Items.ContainsKey(options.InputStreamContextName));
                    Assert.AreEqual(context.Items[options.InputStreamContextName], inputStream);
                    Assert.AreEqual(expectedErrorCount, context.Errors.Count, "Step should decrypt the code without errors:\r\n{0} ", context.Errors.FirstOrDefault());
                }
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Stream_Is_Decrypted_Successfully()
        {
            // arrange
            int expectedErrorCount = 0;
            string contents = "Hey na, hey na, my boyfriend's back.";

            byte[] workingContents = Encoding.UTF8.GetBytes(contents);

            using (var inputStream = new MemoryStream())
            {
                using (var cryptoProvider = Aes.Create())
                {
                    cryptoProvider.IV = Encoding.UTF8.GetBytes(TestConstants.InitializationVector);
                    cryptoProvider.Key = Encoding.UTF8.GetBytes(TestConstants.Key);

                    ICryptoTransform encryptor = cryptoProvider.CreateEncryptor();

                    // Encrypt the contents.
                    using (var cryptoStream = new CryptoStream(inputStream, encryptor, CryptoStreamMode.Write, leaveOpen: true))
                    {
                        await cryptoStream.WriteAsync(workingContents);
                    }

                    // Add Step Here...
                    var options = new DecryptStreamOptions
                    {
                        DecryptionAlgorithm = cryptoProvider
                    };

                    var logger = new Mock<ILogger<DecryptStreamStep>>().Object;
                    var context = new PipelineContext();
                    var mockNext = new Mock<IPipelineRequest>();
                    mockNext.Setup(x => x.InvokeAsync(context))
                                            .Returns(Task.CompletedTask);
                    var next = mockNext.Object;

                    context.Items.Add(options.InputStreamContextName, inputStream);

                    // act
                    using (var sut = new DecryptStreamStep(logger, options, next))
                    {
                        await sut.InvokeAsync(context).ConfigureAwait(false);
                    }

                    // assert - phase 2
                    Assert.AreEqual(expectedErrorCount, context.Errors.Count, "Step should decrypt the code without errors:\r\n{0} ", context.Errors.FirstOrDefault());
                }
            }
        }

        [TestMethod]
        public async Task Encrypt_Decrypt_Encrypt()
        {
            string contents = "Hey na, hey na, my boyfriend's back.";
            byte[] workingContents = Encoding.UTF8.GetBytes(contents);

            using (var inputStream = new MemoryStream(workingContents))
            {
                using (var cryptoProvider = Aes.Create())
                {
                    cryptoProvider.IV = Encoding.UTF8.GetBytes(TestConstants.InitializationVector);
                    cryptoProvider.Key = Encoding.UTF8.GetBytes(TestConstants.Key);

                    // Encrypt
                    using var outputStream = new MemoryStream();
                    using (var encryptTransform = cryptoProvider.CreateEncryptor())
                    {
                        using (var cryptoStream = new CryptoStream(outputStream, encryptTransform, CryptoStreamMode.Write, leaveOpen: true))
                        {
                            byte[] bytes = new byte[16];
                            int read;
                            do
                            {
                                read = await inputStream.ReadAsync(bytes);
                                await cryptoStream.WriteAsync(bytes.AsMemory(0, read));
                            }
                            while (read > 0);
                        }
                    }

                    // Decrypt
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
                                read = await cryptoStream.ReadAsync(bytes);
                                await decryptedStream.WriteAsync(bytes.AsMemory(0, read));
                            }
                            while (read > 0);
                        }
                    }

                    decryptedStream.Position = 0;
                    using (var streamReader = new StreamReader(decryptedStream))
                    {
                        Assert.AreEqual(contents, await streamReader.ReadToEndAsync());
                    }
                }
            }
        }
    }
}