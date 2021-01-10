// <copyright file="DeleteBlobStepTests.cs" company="Chris Trout">
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
    using System.Text;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage]
    [TestClass]
    public class DeleteBlobStepTests
    {
        [TestMethod]
        public async Task Constructs_DeleteBlobStep_Successfully()
        {
            // arrange
            var logger = new Mock<ILogger<DeleteBlobStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;
            var options = new DeleteBlobOptions();

            // act
            var result = new DeleteBlobStep(logger, options, next);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(logger, result.Logger);
            Assert.AreEqual(next, result.Next);
            Assert.AreEqual(options, result.Options);

            // cleanup
            await result.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Blob_Is_Deleted_Successfully_After_Next()
        {
            // arrange
            DeleteBlobOptions options = new DeleteBlobOptions()
            {
                ExecutionTiming = DeleteBlobTimings.After,
                RetrieveConnectionStringAsync = () => { return Task.FromResult(Environment.GetEnvironmentVariable("PIPELINE_TEST_AZURE_BLOB_CONNECTION_STRING", EnvironmentVariableTarget.Machine)); }
            };

            string blobContainerName = Guid.NewGuid().ToString("D");
            string blobName = Guid.NewGuid().ToString("N");

            BlobContainerClient containerClient = new BlobContainerClient(await options.RetrieveConnectionStringAsync().ConfigureAwait(false), blobContainerName);
            await containerClient.CreateIfNotExistsAsync().ConfigureAwait(false);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("Enough is enough!")))
            {
                await containerClient.UploadBlobAsync(blobName, stream).ConfigureAwait(false);
            }

            PipelineContext context = new PipelineContext();
            context.Items.Add(BlobConstants.TARGET_BLOB, blobName);
            context.Items.Add(BlobConstants.TARGET_CONTAINER_NAME, blobContainerName);

            var logger = new Mock<ILogger<DeleteBlobStep>>().Object;

            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Callback(async () =>
                                    {
                                        // Assert that the BEFORE delete was NOT called.
                                        Assert.IsTrue(await containerClient.GetBlobClient(blobName).ExistsAsync().ConfigureAwait(false), "The blob does not exist in the BEFORE block and should.");
                                    })
                                    .Returns(Task.CompletedTask);

            var next = mockNext.Object;

            var source = new DeleteBlobStep(logger, options, next);

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            Assert.IsFalse(await containerClient.GetBlobClient(blobName).ExistsAsync().ConfigureAwait(false), "The blob still exists in the AFTER block and should not.");

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
            await containerClient.DeleteAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Blob_Is_Deleted_Successfully_Before_Next()
        {
            // arrange
            DeleteBlobOptions options = new DeleteBlobOptions()
            {
                ExecutionTiming = DeleteBlobTimings.Before,
                RetrieveConnectionStringAsync = () => { return Task.FromResult(Environment.GetEnvironmentVariable("PIPELINE_TEST_AZURE_BLOB_CONNECTION_STRING", EnvironmentVariableTarget.Machine)); }
            };

            string blobContainerName = Guid.NewGuid().ToString("D");
            string blobName = Guid.NewGuid().ToString("N");

            BlobContainerClient containerClient = new BlobContainerClient(await options.RetrieveConnectionStringAsync().ConfigureAwait(false), blobContainerName);
            await containerClient.CreateIfNotExistsAsync().ConfigureAwait(false);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("Enough is enough!")))
            {
                await containerClient.UploadBlobAsync(blobName, stream).ConfigureAwait(false);
            }

            PipelineContext context = new PipelineContext();
            context.Items.Add(BlobConstants.TARGET_BLOB, blobName);
            context.Items.Add(BlobConstants.TARGET_CONTAINER_NAME, blobContainerName);

            var logger = new Mock<ILogger<DeleteBlobStep>>().Object;

            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Callback(async () =>
                                    {
                                        // Assert that the BEFORE delete was called.
                                        Assert.IsFalse(await containerClient.GetBlobClient(blobName).ExistsAsync().ConfigureAwait(false), "The blob still exists in the BEFORE block and should not.");
                                    })
                                    .Returns(Task.CompletedTask);

            var next = mockNext.Object;

            var source = new DeleteBlobStep(logger, options, next);

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
            await containerClient.DeleteAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Errors_From_InvokeCoreAsync_When_TargetBlob_Does_Not_Exist()
        {
            // arrange
            IPipelineContext context = new PipelineContext();
            context.Items.Add(BlobConstants.TARGET_CONTAINER_NAME, Guid.NewGuid().ToString("N"));

            var logger = new Mock<ILogger<DeleteBlobStep>>().Object;
            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;
            var options = new DeleteBlobOptions();

            var source = new DeleteBlobStep(logger, options, next);

            var expectedErrorsCount = 1;
            var expectedMessage = Pipelines.Steps.Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, BlobConstants.TARGET_BLOB);

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
        public async Task Returns_PipelineContext_Errors_From_InvokeCoreAsync_When_TargetBlob_Is_Specified_Value(string contents)
        {
            // arrange
            IPipelineContext context = new PipelineContext();
            context.Items.Add(BlobConstants.TARGET_BLOB, contents);
            context.Items.Add(BlobConstants.TARGET_CONTAINER_NAME, Guid.NewGuid().ToString("N"));

            var logger = new Mock<ILogger<DeleteBlobStep>>().Object;
            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;
            var options = new DeleteBlobOptions();

            var source = new DeleteBlobStep(logger, options, next);

            var expectedErrorsCount = 1;
            var expectedMessage = Pipelines.Steps.Resources.CONTEXT_VALUE_IS_WHITESPACE(CultureInfo.CurrentCulture, BlobConstants.TARGET_BLOB);

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

            var logger = new Mock<ILogger<DeleteBlobStep>>().Object;
            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;

            var options = new DeleteBlobOptions();

            var source = new DeleteBlobStep(logger, options, next);

            var expectedErrorsCount = 1;
            var expectedMessage = Steps.Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, BlobConstants.TARGET_CONTAINER_NAME);

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

            var logger = new Mock<ILogger<DeleteBlobStep>>().Object;
            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;

            var options = new DeleteBlobOptions();

            var source = new DeleteBlobStep(logger, options, next);

            var expectedErrorsCount = 1;
            var expectedMessage = Steps.Resources.CONTEXT_VALUE_IS_WHITESPACE(CultureInfo.CurrentCulture, BlobConstants.TARGET_CONTAINER_NAME);

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            Assert.AreEqual(expectedErrorsCount, context.Errors.Count);
            Assert.AreEqual(expectedMessage, context.Errors[0].Message);

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Blob_Does_Not_Exist()
        {
            // arrange
            DeleteBlobOptions options = new DeleteBlobOptions()
            {
                RetrieveConnectionStringAsync = () => { return Task.FromResult(Environment.GetEnvironmentVariable("PIPELINE_TEST_AZURE_BLOB_CONNECTION_STRING", EnvironmentVariableTarget.Machine)); }
            };

            string blobContainerName = Guid.NewGuid().ToString("D");
            string blobName = Guid.NewGuid().ToString("N");

            BlobClient client = new BlobClient(await options.RetrieveConnectionStringAsync().ConfigureAwait(false), blobContainerName, blobName);

            PipelineContext context = new PipelineContext();
            context.Items.Add(BlobConstants.TARGET_BLOB, blobName);
            context.Items.Add(BlobConstants.TARGET_CONTAINER_NAME, blobContainerName);

            var logger = new Mock<ILogger<DeleteBlobStep>>().Object;

            Mock<IPipelineRequest> mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                                    .Returns(Task.CompletedTask);
            var next = mockNext.Object;

            var source = new DeleteBlobStep(logger, options, next);

            // act
            await source.InvokeAsync(context).ConfigureAwait(false);

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            Assert.IsFalse(await client.ExistsAsync().ConfigureAwait(false), "The blob exists and should have never existed.");

            // cleanup
            await source.DisposeAsync().ConfigureAwait(false);
            await client.DeleteIfExistsAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public void Throw_ArgumentNullException_From_Constructor_When_Logger_Is_Null()
        {
            // arrange
            var logger = null as ILogger<DeleteBlobStep>;
            var next = new Mock<IPipelineRequest>().Object;
            var options = new DeleteBlobOptions();

            var expectedParamName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new DeleteBlobStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throw_ArgumentNullException_From_Constructor_When_Next_Is_Null()
        {
            // arrange
            var logger = new Mock<ILogger<DeleteBlobStep>>().Object;
            var next = null as IPipelineRequest;
            var options = new DeleteBlobOptions();

            var expectedParamName = nameof(next);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new DeleteBlobStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throw_ArgumentNullException_From_Constructor_When_Options_Is_Null()
        {
            // arrange
            var logger = new Mock<ILogger<DeleteBlobStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;
            var options = null as DeleteBlobOptions;

            var expectedParamName = nameof(options);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new DeleteBlobStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public async Task Throw_ArgumentNullException_From_InvokeCoreAsync_When_IPipelineContext_Is_Null()
        {
            // arrange
            var logger = new Mock<ILogger<DeleteBlobStep>>().Object;
            var next = new Mock<IPipelineRequest>().Object;
            var options = new DeleteBlobOptions();

            var source = new DeleteBlobStep(logger, options, next);

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
