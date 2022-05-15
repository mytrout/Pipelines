// <copyright file="CreateHashStepTests.cs" company="Chris Trout">
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
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CreateHashStepTests
    {
        [TestMethod]
        public void Constructs_CreateHashStep_Successfully()
        {
            // arrange
            var logger = new Mock<ILogger<CreateHashStep>>().Object;
            var options = new CreateHashOptions();

            var next = new Mock<IPipelineRequest>().Object;

            // act
            using (var result = new CreateHashStep(logger, options, next))
            {
                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(logger, result.Logger);
                Assert.AreEqual(next, result.Next);
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_InputStream_Is_Not_In_Context()
        {
            // arrange
            int errorCount = 1;
            var logger = new Mock<ILogger<CreateHashStep>>().Object;
            var options = new CreateHashOptions();

            var context = new PipelineContext();

            var nextMock = new Mock<IPipelineRequest>();
            nextMock.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = nextMock.Object;

            using (var source = new CreateHashStep(logger, options, next))
            {
                var expectedMessage = Pipelines.Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, PipelineContextConstants.INPUT_STREAM);

                // act
                await source.InvokeAsync(context);

                // assert
                Assert.AreEqual(errorCount, context.Errors.Count);
                Assert.AreEqual(expectedMessage, context.Errors[0].Message);
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Stream_Is_Hashed_Successfully()
        {
            // arrange
            string contents = "Hey na, hey na, my boyfriend's back.";
            string previousHashString = "1ed507463ae3051f28aa79751aa042d2ff872498dca98d0a593ad83f62781947";

            using (var previousHash = new MemoryStream(Encoding.UTF8.GetBytes(previousHashString)))
            {
                using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
                {
                    inputStream.Position = 0;
                    var context = new PipelineContext();
                    context.Items.Add(PipelineContextConstants.INPUT_STREAM, inputStream);
                    context.Items.Add(CryptographyConstants.HASH_STREAM, previousHash);
                    context.Items.Add(CryptographyConstants.HASH_STRING, previousHashString);

                    var logger = new Mock<ILogger<CreateHashStep>>().Object;
                    var options = new CreateHashOptions();

                    var nextMock = new Mock<IPipelineRequest>();
                    nextMock.Setup(x => x.InvokeAsync(context))
                                            .Returns(Task.CompletedTask);
                    var next = nextMock.Object;

                    using (var source = new CreateHashStep(logger, options, next))
                    {
                        // act
                        await source.InvokeAsync(context);

                        // assert
                        if (context.Errors.Any())
                        {
                            throw context.Errors[0];
                        }

                        Assert.AreEqual(0, context.Errors.Count, "Errors were thrown.");
                    }
                }
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Next_Step_Verifies_Values_Passed_Downstream()
        {
            // arrange
            string contents = "Hey na, hey na, my boyfriend's back.";
            string expectedHash = "8F54BD082762BA536EF10CAB44C1B6CBFCC95648F15ACD5AB77BF193BB1EBBCF";

            using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
            {
                inputStream.Position = 0;
                var context = new PipelineContext();
                context.Items.Add(PipelineContextConstants.INPUT_STREAM, inputStream);

                var logger = new Mock<ILogger<CreateHashStep>>().Object;
                var options = new CreateHashOptions();

                var nextMock = new Mock<IPipelineRequest>();
                nextMock.Setup(x => x.InvokeAsync(context))
                                        .Callback(() =>
                                        {
                                            // assert
                                            Assert.IsTrue(context.Items.ContainsKey(CryptographyConstants.HASH_STREAM), "context does not contain HASH_STREAM.");
                                            Assert.IsTrue(inputStream.CanRead, "Stream should not be closed.");

                                            using (var hashStream = context.Items[CryptographyConstants.HASH_STREAM] as Stream)
                                            {
                                                hashStream.Position = 0;
                                                using (var reader = new StreamReader(hashStream, Encoding.UTF8, leaveOpen: true))
                                                {
                                                    var actualHash = reader.ReadToEnd();
                                                    Assert.AreEqual(expectedHash, actualHash);
                                                }
                                            }

                                            Assert.AreEqual(expectedHash, context.Items[CryptographyConstants.HASH_STRING]);
                                        })
                                        .Returns(Task.CompletedTask);
                var next = nextMock.Object;

                using (var source = new CreateHashStep(logger, options, next))
                {
                    // act
                    await source.InvokeAsync(context);

                    // assert - phase 2
                    if (context.Errors.Any())
                    {
                        throw context.Errors[0];
                    }
                }
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_With_Previously_Configured_Values_Restored()
        {
            // arrange
            string contents = "Hey na, hey na, my boyfriend's back.";
            string previousHashString = "1ed507463ae3051f28aa79751aa042d2ff872498dca98d0a593ad83f62781947";
            var options = new CreateHashOptions();

            using (var previousHash = new MemoryStream(Encoding.UTF8.GetBytes(previousHashString)))
            {
                using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
                {
                    inputStream.Position = 0;
                    var context = new PipelineContext();
                    context.Items.Add(options.InputStreamContextName, inputStream);
                    context.Items.Add(options.HashStreamContextName, previousHash);
                    context.Items.Add(options.HashStringContextName, previousHashString);

                    var logger = new Mock<ILogger<CreateHashStep>>().Object;

                    var nextMock = new Mock<IPipelineRequest>();
                    nextMock.Setup(x => x.InvokeAsync(context))
                                            .Returns(Task.CompletedTask);
                    var next = nextMock.Object;

                    using (var source = new CreateHashStep(logger, options, next))
                    {
                        // act
                        await source.InvokeAsync(context);

                        // assert
                        if (context.Errors.Any())
                        {
                            throw context.Errors[0];
                        }

                        Assert.AreEqual(previousHash, context.Items[options.HashStreamContextName]);
                        Assert.AreEqual(previousHashString, context.Items[options.HashStringContextName]);
                        Assert.AreEqual(inputStream, context.Items[options.InputStreamContextName]);
                    }
                }
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Options_Uses_Different_Context_Names()
        {
            // arrange
            string contents = "Hey na, hey na, my boyfriend's back.";
            string previousHashString = "1ed507463ae3051f28aa79751aa042d2ff872498dca98d0a593ad83f62781947";
            string expectedHash = "8F54BD082762BA536EF10CAB44C1B6CBFCC95648F15ACD5AB77BF193BB1EBBCF";

            var options = new CreateHashOptions()
            {
                InputStreamContextName = "NOT_INPUT_STREAM",
                HashStreamContextName = "NOT_HASH_STREAM",
                HashStringContextName = "NOT_HASH_STRING",
            };

            using (var previousHash = new MemoryStream(Encoding.UTF8.GetBytes(previousHashString)))
            {
                using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
                {
                    inputStream.Position = 0;
                    var context = new PipelineContext();
                    context.Items.Add(options.InputStreamContextName, inputStream);
                    context.Items.Add(options.HashStreamContextName, previousHash);
                    context.Items.Add(options.HashStringContextName, previousHashString);

                    var logger = new Mock<ILogger<CreateHashStep>>().Object;

                    var nextMock = new Mock<IPipelineRequest>();
                    nextMock.Setup(x => x.InvokeAsync(context))
                                            .Callback(() =>
                                            {
                                                // assert
                                                Assert.IsTrue(context.Items.ContainsKey(options.HashStreamContextName), "context does not contain HASH_STREAM.");
                                                Assert.IsTrue(inputStream.CanRead, "Stream should not be closed.");

                                                using (var hashStream = context.Items[options.HashStreamContextName] as Stream)
                                                {
                                                    hashStream.Position = 0;
                                                    using (var reader = new StreamReader(hashStream, Encoding.UTF8, leaveOpen: true))
                                                    {
                                                        var actualHash = reader.ReadToEnd();
                                                        Assert.AreEqual(expectedHash, actualHash);
                                                    }
                                                }

                                                Assert.AreEqual(expectedHash, context.Items[options.HashStringContextName]);
                                            })
                                            .Returns(Task.CompletedTask);
                    var next = nextMock.Object;

                    using (var source = new CreateHashStep(logger, options, next))
                    {
                        // act
                        await source.InvokeAsync(context);

                        // assert - phase 2
                        if (context.Errors.Any())
                        {
                            throw context.Errors[0];
                        }

                        Assert.AreEqual(previousHash, context.Items[options.HashStreamContextName]);
                        Assert.AreEqual(previousHashString, context.Items[options.HashStringContextName]);
                        Assert.AreEqual(inputStream, context.Items[options.InputStreamContextName]);
                    }
                }
            }
        }
    }
}