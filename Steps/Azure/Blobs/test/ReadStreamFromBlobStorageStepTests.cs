// <copyright file="ReadStreamFromBlobStorageStepTests.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.Azure.Blobs.Tests
{
    using global::Azure.Storage.Blobs;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines.Core;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ReadStreamFromBlobStorageStepTests
    {
        [TestMethod]
        public async Task Constructs_ReadStreamFromBlobStorageStep_Successfully()
        {
            // arrange
            var logger = new Mock<ILogger<ReadStreamFromBlobStorageStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;
            var options = new ReadStreamFromBlobStorageOptions();

            // act
            var result = new ReadStreamFromBlobStorageStep(logger, options, next);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(logger, result.Logger);
            Assert.AreEqual(next, result.Next);
            Assert.AreEqual(options, result.Options);

            // cleanup
            await result.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Errors_From_InvokeCoreAsync_When_SourceBlob_Does_Not_Exist()
        {
            // arrange
            IPipelineContext context = new PipelineContext();
            context.Items.Add(BlobConstants.SOURCE_CONTAINER_NAME, Guid.NewGuid().ToString("N"));

            var logger = new Mock<ILogger<ReadStreamFromBlobStorageStep>>().Object;
            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;
            var options = new ReadStreamFromBlobStorageOptions();

            var source = new ReadStreamFromBlobStorageStep(logger, options, next);

            var expectedErrorsCount = 1;
            var expectedMessage = Pipelines.Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, BlobConstants.SOURCE_BLOB);

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            Assert.AreEqual(expectedErrorsCount, context.Errors.Count);
            Assert.AreEqual(expectedMessage, context.Errors[0].Message);

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("\r\n\t")]
        public async Task Returns_PipelineContext_Errors_From_InvokeCoreAsync_When_SourceBlob_Is_Specified_Value(string contents)
        {
            // arrange
            IPipelineContext context = new PipelineContext();
            context.Items.Add(BlobConstants.SOURCE_BLOB, contents);
            context.Items.Add(BlobConstants.SOURCE_CONTAINER_NAME, Guid.NewGuid().ToString("N"));

            var logger = new Mock<ILogger<ReadStreamFromBlobStorageStep>>().Object;
            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;
            var options = new ReadStreamFromBlobStorageOptions();

            var source = new ReadStreamFromBlobStorageStep(logger, options, next);

            var expectedErrorsCount = 1;
            var expectedMessage = Pipelines.Resources.CONTEXT_VALUE_IS_WHITESPACE(CultureInfo.CurrentCulture, BlobConstants.SOURCE_BLOB);

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            Assert.AreEqual(expectedErrorsCount, context.Errors.Count);
            Assert.AreEqual(expectedMessage, context.Errors[0].Message);

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Errors_From_InvokeCoreAsync_When_SourceContainerName_Does_Not_Exist()
        {
            // arrange
            IPipelineContext context = new PipelineContext();
            context.Items.Add(BlobConstants.SOURCE_BLOB, Guid.NewGuid().ToString("N"));

            var logger = new Mock<ILogger<ReadStreamFromBlobStorageStep>>().Object;
            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;

            var options = new ReadStreamFromBlobStorageOptions();

            var source = new ReadStreamFromBlobStorageStep(logger, options, next);

            var expectedErrorsCount = 1;
            var expectedMessage = Pipelines.Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, BlobConstants.SOURCE_CONTAINER_NAME);

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            Assert.AreEqual(expectedErrorsCount, context.Errors.Count);
            Assert.AreEqual(expectedMessage, context.Errors[0].Message);

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Errors_From_InvokeAsync_When_SourceBlob_Does_Not_Exist()
        {
            // arrange
            string blobName = Guid.NewGuid().ToString("N");
            string blobContainerName = Guid.NewGuid().ToString("N");

            IPipelineContext context = new PipelineContext();
            context.Items.Add(BlobConstants.SOURCE_BLOB, blobName);
            context.Items.Add(BlobConstants.SOURCE_CONTAINER_NAME, blobContainerName);

            var logger = new Mock<ILogger<ReadStreamFromBlobStorageStep>>().Object;
            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;

            var environmentVariableTarget = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? EnvironmentVariableTarget.Machine : EnvironmentVariableTarget.Process;
            var options = new ReadStreamFromBlobStorageOptions()
            {
                RetrieveConnectionStringAsync = () => { return Task.FromResult(Environment.GetEnvironmentVariable("PIPELINE_TEST_AZURE_BLOB_CONNECTION_STRING", environmentVariableTarget)); }
            };

            var source = new ReadStreamFromBlobStorageStep(logger, options, next);

            BlobContainerClient containerClient = new BlobContainerClient(await options.RetrieveConnectionStringAsync().ConfigureAwait(false), blobContainerName);
            await containerClient.CreateIfNotExistsAsync().ConfigureAwait(false);

            int expectedErrorsCount = 1;
            string expectedMessage = Resources.BLOB_DOES_NOT_EXIST(CultureInfo.CurrentCulture, blobContainerName, blobName);

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            Assert.AreEqual(expectedErrorsCount, context.Errors.Count);
            Assert.AreEqual(expectedMessage, context.Errors[0].Message);

            // cleanup
            await containerClient.DeleteAsync().ConfigureAwait(false);
            await source.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Errors_From_InvokeAsync_When_SourceBlobContainer_Does_Not_Exist()
        {
            // arrange
            string blobName = Guid.NewGuid().ToString("N");
            string blobContainerName = Guid.NewGuid().ToString("N");

            IPipelineContext context = new PipelineContext();
            context.Items.Add(BlobConstants.SOURCE_BLOB, blobName);
            context.Items.Add(BlobConstants.SOURCE_CONTAINER_NAME, blobContainerName);

            var logger = new Mock<ILogger<ReadStreamFromBlobStorageStep>>().Object;
            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;

            var environmentVariableTarget = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? EnvironmentVariableTarget.Machine : EnvironmentVariableTarget.Process;
            var options = new ReadStreamFromBlobStorageOptions()
            {
                RetrieveConnectionStringAsync = () => { return Task.FromResult(Environment.GetEnvironmentVariable("PIPELINE_TEST_AZURE_BLOB_CONNECTION_STRING", environmentVariableTarget)); }
            };

            var source = new ReadStreamFromBlobStorageStep(logger, options, next);

            int expectedErrorsCount = 1;
            string expectedMessage = Resources.CONTAINER_DOES_NOT_EXIST(CultureInfo.CurrentCulture, blobContainerName);

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            Assert.AreEqual(expectedErrorsCount, context.Errors.Count);
            Assert.AreEqual(expectedMessage, context.Errors[0].Message);

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("\r\n\t")]
        public async Task Returns_PipelineContext_Errors_From_InvokeCoreAsync_When_SourceContainerName_Is_Specified_Value(string contents)
        {
            // arrange
            IPipelineContext context = new PipelineContext();
            context.Items.Add(BlobConstants.SOURCE_BLOB, Guid.NewGuid().ToString("N"));
            context.Items.Add(BlobConstants.SOURCE_CONTAINER_NAME, contents);

            var logger = new Mock<ILogger<ReadStreamFromBlobStorageStep>>().Object;
            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;

            var options = new ReadStreamFromBlobStorageOptions();

            var source = new ReadStreamFromBlobStorageStep(logger, options, next);

            var expectedErrorsCount = 1;
            var expectedMessage = Pipelines.Resources.CONTEXT_VALUE_IS_WHITESPACE(CultureInfo.CurrentCulture, BlobConstants.SOURCE_CONTAINER_NAME);

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            Assert.AreEqual(expectedErrorsCount, context.Errors.Count);
            Assert.AreEqual(expectedMessage, context.Errors[0].Message);

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_With_InputStream_From_InvokeAsync()
        {
            // arrange
            string blobName = Guid.NewGuid().ToString("N");
            string blobContainerName = Guid.NewGuid().ToString("N");

            IPipelineContext context = new PipelineContext();
            context.Items.Add(BlobConstants.SOURCE_BLOB, blobName);
            context.Items.Add(BlobConstants.SOURCE_CONTAINER_NAME, blobContainerName);

            string contents = "Enough is enough!";

            var logger = new Mock<ILogger<ReadStreamFromBlobStorageStep>>().Object;
            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Callback(() =>
                                    {
                                        var outputArray = (context.Items[PipelineContextConstants.INPUT_STREAM] as Stream).ConvertStreamToByteArrayAsync().Result;
                                        string result = Encoding.UTF8.GetString(outputArray);

                                        // assert
                                        Assert.AreEqual(contents, result);
                                    })
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;

            var environmentVariableTarget = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? EnvironmentVariableTarget.Machine : EnvironmentVariableTarget.Process;
            var options = new ReadStreamFromBlobStorageOptions()
            {
                RetrieveConnectionStringAsync = () => { return Task.FromResult(Environment.GetEnvironmentVariable("PIPELINE_TEST_AZURE_BLOB_CONNECTION_STRING", environmentVariableTarget)); }
            };

            var source = new ReadStreamFromBlobStorageStep(logger, options, next);

            BlobContainerClient containerClient = new BlobContainerClient(await options.RetrieveConnectionStringAsync().ConfigureAwait(false), blobContainerName);
            await containerClient.CreateIfNotExistsAsync().ConfigureAwait(false);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
            {
                await containerClient.UploadBlobAsync(blobName, stream).ConfigureAwait(false);
            }

            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            // cleanup
            await containerClient.DeleteAsync().ConfigureAwait(false);
            await source.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_With_No_Alterations_From_InvokeAsync_After_Execution()
        {
            // arrange
            string blobName = Guid.NewGuid().ToString("N");
            string blobContainerName = Guid.NewGuid().ToString("N");

            IPipelineContext context = new PipelineContext();
            context.Items.Add(BlobConstants.SOURCE_BLOB, blobName);
            context.Items.Add(BlobConstants.SOURCE_CONTAINER_NAME, blobContainerName);
            var expectedItemCount = context.Items.Count;

            string contents = "Enough is enough!";

            var logger = new Mock<ILogger<ReadStreamFromBlobStorageStep>>().Object;
            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            var next = mockNext.Object;

            var environmentVariableTarget = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? EnvironmentVariableTarget.Machine : EnvironmentVariableTarget.Process;
            var options = new ReadStreamFromBlobStorageOptions()
            {
                RetrieveConnectionStringAsync = () => { return Task.FromResult(Environment.GetEnvironmentVariable("PIPELINE_TEST_AZURE_BLOB_CONNECTION_STRING", environmentVariableTarget)); }
            };

            var source = new ReadStreamFromBlobStorageStep(logger, options, next);

            BlobContainerClient containerClient = new BlobContainerClient(await options.RetrieveConnectionStringAsync().ConfigureAwait(false), blobContainerName);
            await containerClient.CreateIfNotExistsAsync().ConfigureAwait(false);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
            {
                await containerClient.UploadBlobAsync(blobName, stream).ConfigureAwait(false);
            }

            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }
            Assert.AreEqual(expectedItemCount, context.Items.Count);
            Assert.IsTrue(context.Items.ContainsKey(BlobConstants.SOURCE_BLOB), "SOURCE_BLOB does not exist in PipelineContext.Items after execution.");
            Assert.AreEqual(blobName, context.Items[BlobConstants.SOURCE_BLOB], "SOURCE_BLOB value does not match expected value after execution.");
            Assert.IsTrue(context.Items.ContainsKey(BlobConstants.SOURCE_CONTAINER_NAME), "SOURCE_CONTAINER_NAME does not exist in PipelineContext.Items after execution.");
            Assert.AreEqual(blobContainerName, context.Items[BlobConstants.SOURCE_CONTAINER_NAME], "SOURCE_CONTAINER_NAME value does not match expected value after execution.");

            // cleanup
            await containerClient.DeleteAsync().ConfigureAwait(false);
            await source.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public void Throw_ArgumentNullException_From_Constructor_When_Logger_Is_Null()
        {
            // arrange
            var logger = null as ILogger<ReadStreamFromBlobStorageStep>;
            var next = new Mock<IPipelineRequest>().Object;
            var options = new ReadStreamFromBlobStorageOptions();

            var expectedParamName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new ReadStreamFromBlobStorageStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throw_ArgumentNullException_From_Constructor_When_Next_Is_Null()
        {
            // arrange
            var logger = new Mock<ILogger<ReadStreamFromBlobStorageStep>>().Object;
            var next = null as IPipelineRequest;
            var options = new ReadStreamFromBlobStorageOptions();

            var expectedParamName = nameof(next);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new ReadStreamFromBlobStorageStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throw_ArgumentNullException_From_Constructor_When_Options_Is_Null()
        {
            // arrange
            var logger = new Mock<ILogger<ReadStreamFromBlobStorageStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;
            var options = null as ReadStreamFromBlobStorageOptions;

            var expectedParamName = nameof(options);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new ReadStreamFromBlobStorageStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public async Task Throw_ArgumentNullException_From_InvokeCoreAsync_When_IPipelineContext_Is_Null()
        {
            // arrange
            var logger = new Mock<ILogger<ReadStreamFromBlobStorageStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;
            var options = new ReadStreamFromBlobStorageOptions();

            var source = new ReadStreamFromBlobStorageStep(logger, options, next);

            IPipelineContext context = null;

            var expectedParamName = nameof(context);

            // act
            var result = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await source.InvokeAsync(context).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
        }
    }
}
