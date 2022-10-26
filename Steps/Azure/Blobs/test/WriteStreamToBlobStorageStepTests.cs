// <copyright file="WriteStreamToBlobStorageStepTests.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2020-2022 Chris Trout
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
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage]
    [TestClass]
    public class WriteStreamToBlobStorageStepTests
    {
        [TestMethod]
        public async Task Constructs_WriteStreamToBlobStorageStep_Successfully()
        {
            // arrange
            var logger = new Mock<ILogger<WriteStreamToBlobStorageStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;
            var options = new WriteStreamToBlobStorageOptions();

            // act
            var result = new WriteStreamToBlobStorageStep(logger, options, next);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(logger, result.Logger);
            Assert.AreEqual(next, result.Next);
            Assert.AreEqual(options, result.Options);

            // cleanup
            await result.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Errors_From_InvokeCoreAsync_When_TargetBlob_Does_Not_Exist()
        {
            // arrange
            IPipelineContext context = new PipelineContext();
            context.Items.Add(BlobConstants.TARGET_CONTAINER_NAME, Guid.NewGuid().ToString("N"));

            var logger = new Mock<ILogger<WriteStreamToBlobStorageStep>>().Object;
            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;
            var options = new WriteStreamToBlobStorageOptions();

            var source = new WriteStreamToBlobStorageStep(logger, options, next);

            var expectedErrorsCount = 1;
            var expectedMessage = Pipelines.Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, BlobConstants.TARGET_BLOB);

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            Assert.AreEqual(expectedErrorsCount, context.Errors.Count);
            Assert.AreEqual(expectedMessage, context.Errors[0].Message);

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Errors_From_InvokeCoreAsync_When_TargetBlob_Exists_In_Azure()
        {
            // arrange
            string blobName = Guid.NewGuid().ToString("N");
            string blobContainerName = Guid.NewGuid().ToString("N");

            IPipelineContext context = new PipelineContext();
            context.Items.Add(BlobConstants.TARGET_BLOB, blobName);
            context.Items.Add(BlobConstants.TARGET_CONTAINER_NAME, blobContainerName);

            string contents = "Enough is enough!";

            var logger = new Mock<ILogger<WriteStreamToBlobStorageStep>>().Object;
            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;

            var environmentVariableTarget = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? EnvironmentVariableTarget.Machine : EnvironmentVariableTarget.Process;
            var options = new WriteStreamToBlobStorageOptions()
            {
                RetrieveConnectionStringAsync = () => { return Task.FromResult(Environment.GetEnvironmentVariable("PIPELINE_TEST_AZURE_BLOB_CONNECTION_STRING", environmentVariableTarget)); }
            };

            BlobContainerClient containerClient = new BlobContainerClient(await options.RetrieveConnectionStringAsync().ConfigureAwait(false), blobContainerName);
            await containerClient.CreateIfNotExistsAsync().ConfigureAwait(false);
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
            {
                await containerClient.UploadBlobAsync(blobName, stream).ConfigureAwait(false);
            }

            int expectedErrorCount = 1;
            string expectedMessage = Resources.BLOB_ALREADY_EXISTS(CultureInfo.CurrentCulture, blobName, blobContainerName);

            var source = new WriteStreamToBlobStorageStep(logger, options, next);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
            {
                context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, stream);

                // act
                await source.InvokeAsync(context).ConfigureAwait(false);
            }

            // assert
            Assert.AreEqual(expectedErrorCount, context.Errors.Count);
            Assert.AreEqual(expectedMessage, context.Errors[0].Message);

            // cleanup
            await containerClient.DeleteAsync().ConfigureAwait(false);
            await source.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("\r\n\t")]
        public async Task Returns_PipelineContext_Errors_From_InvokeCoreAsync_When_TargetBlob_Is_Specified_Value(string contents)
        {
            // arrange
            IPipelineContext context = new PipelineContext();
            context.Items.Add(BlobConstants.TARGET_BLOB, contents);
            context.Items.Add(BlobConstants.TARGET_CONTAINER_NAME, Guid.NewGuid().ToString("N"));

            var logger = new Mock<ILogger<WriteStreamToBlobStorageStep>>().Object;
            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;
            var options = new WriteStreamToBlobStorageOptions();

            var source = new WriteStreamToBlobStorageStep(logger, options, next);

            var expectedErrorsCount = 1;
            var expectedMessage = Pipelines.Resources.CONTEXT_VALUE_IS_WHITESPACE(CultureInfo.CurrentCulture, BlobConstants.TARGET_BLOB);

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            Assert.AreEqual(expectedErrorsCount, context.Errors.Count);
            Assert.AreEqual(expectedMessage, context.Errors[0].Message);

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Errors_From_InvokeCoreAsync_When_TargetContainerName_Does_Not_Exist()
        {
            // arrange
            IPipelineContext context = new PipelineContext();
            context.Items.Add(BlobConstants.TARGET_BLOB, Guid.NewGuid().ToString("N"));

            var logger = new Mock<ILogger<WriteStreamToBlobStorageStep>>().Object;
            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;

            var options = new WriteStreamToBlobStorageOptions();

            var source = new WriteStreamToBlobStorageStep(logger, options, next);

            var expectedErrorsCount = 1;
            var expectedMessage = Pipelines.Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, BlobConstants.TARGET_CONTAINER_NAME);

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
        public async Task Returns_PipelineContext_Errors_From_InvokeCoreAsync_When_TargetContainerName_Is_Specified_Value(string contents)
        {
            // arrange
            IPipelineContext context = new PipelineContext();
            context.Items.Add(BlobConstants.TARGET_BLOB, Guid.NewGuid().ToString("N"));
            context.Items.Add(BlobConstants.TARGET_CONTAINER_NAME, contents);

            var logger = new Mock<ILogger<WriteStreamToBlobStorageStep>>().Object;
            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;

            var options = new WriteStreamToBlobStorageOptions();

            var source = new WriteStreamToBlobStorageStep(logger, options, next);

            var expectedErrorsCount = 1;
            var expectedMessage = Pipelines.Resources.CONTEXT_VALUE_IS_WHITESPACE(CultureInfo.CurrentCulture, BlobConstants.TARGET_CONTAINER_NAME);

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            Assert.AreEqual(expectedErrorsCount, context.Errors.Count);
            Assert.AreEqual(expectedMessage, context.Errors[0].Message);

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public void Throw_ArgumentNullException_From_Constructor_When_Logger_Is_Null()
        {
            // arrange
            var logger = null as ILogger<WriteStreamToBlobStorageStep>;
            var next = new Mock<IPipelineRequest>().Object;
            var options = new WriteStreamToBlobStorageOptions();

            var expectedParamName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new WriteStreamToBlobStorageStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throw_ArgumentNullException_From_Constructor_When_Next_Is_Null()
        {
            // arrange
            var logger = new Mock<ILogger<WriteStreamToBlobStorageStep>>().Object;
            var next = null as IPipelineRequest;
            var options = new WriteStreamToBlobStorageOptions();

            var expectedParamName = nameof(next);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new WriteStreamToBlobStorageStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throw_ArgumentNullException_From_Constructor_When_Options_Is_Null()
        {
            // arrange
            var logger = new Mock<ILogger<WriteStreamToBlobStorageStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;
            var options = null as WriteStreamToBlobStorageOptions;

            var expectedParamName = nameof(options);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new WriteStreamToBlobStorageStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public async Task Throw_ArgumentNullException_From_InvokeCoreAsync_When_IPipelineContext_Is_Null()
        {
            // arrange
            var logger = new Mock<ILogger<WriteStreamToBlobStorageStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;
            var options = new WriteStreamToBlobStorageOptions();

            var source = new WriteStreamToBlobStorageStep(logger, options, next);

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

        [TestMethod]
        public async Task Writes_OutputStream_From_InvokeAsync_To_Azure_Blob_Containers()
        {
            // arrange
            string blobName = Guid.NewGuid().ToString("N");
            string blobContainerName = Guid.NewGuid().ToString("N");

            IPipelineContext context = new PipelineContext();
            context.Items.Add(BlobConstants.TARGET_BLOB, blobName);
            context.Items.Add(BlobConstants.TARGET_CONTAINER_NAME, blobContainerName);

            string contents = "Enough is enough!";

            var logger = new Mock<ILogger<WriteStreamToBlobStorageStep>>().Object;
            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;

            var environmentVariableTarget = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? EnvironmentVariableTarget.Machine : EnvironmentVariableTarget.Process;

            var options = new WriteStreamToBlobStorageOptions()
            {
                RetrieveConnectionStringAsync = () => { return Task.FromResult(Environment.GetEnvironmentVariable("PIPELINE_TEST_AZURE_BLOB_CONNECTION_STRING", environmentVariableTarget)); }
            };

            var source = new WriteStreamToBlobStorageStep(logger, options, next);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
            {
                context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, stream);

                // act
                await source.InvokeAsync(context).ConfigureAwait(false);
            }

            // assert
            BlobContainerClient containerClient = new BlobContainerClient(await options.RetrieveConnectionStringAsync().ConfigureAwait(false), blobContainerName);
            Assert.IsTrue(await containerClient.ExistsAsync().ConfigureAwait(false), "Blob container does not exist.");

            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            Assert.IsTrue(await blobClient.ExistsAsync().ConfigureAwait(false), "Blob does not exist on the container.");

            using (var outputStream = new MemoryStream())
            {
                await blobClient.DownloadToAsync(outputStream).ConfigureAwait(false);
                Assert.AreEqual(contents, Encoding.UTF8.GetString(await outputStream.ConvertStreamToByteArrayAsync().ConfigureAwait(false)));
            }

            // cleanup
            await containerClient.DeleteAsync().ConfigureAwait(false);
            await source.DisposeAsync().ConfigureAwait(false);
        }
    }
}
