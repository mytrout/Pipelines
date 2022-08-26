// <copyright file="DeserializeObjectFromStreamStepTests.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.Serialization.Core.Tests
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
    public class DeserializeObjectFromStreamStepTests
    {
        [TestMethod]
        public void Constructs_DeserializeObjectFromStreamStep_Successfully()
        {
            // arrange
            var logger = new Mock<ILogger<DeserializeObjectFromStreamStep<object>>>().Object;
            var options = new DeserializeObjectFromStreamOptions();
            var next = new Mock<IPipelineRequest>().Object;

            // act
            var result = new DeserializeObjectFromStreamStep<object>(logger, options, next);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(logger, result.Logger);
            Assert.AreEqual(options, result.Options);
            Assert.AreEqual(next, result.Next);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_Error_From_InvokeAsync_When_InputStream_Is_Not_In_Context()
        {
            // arrange
            var logger = new Mock<ILogger<DeserializeObjectFromStreamStep<object>>>().Object;
            var options = new DeserializeObjectFromStreamOptions();
            var next = new Mock<IPipelineRequest>().Object;

            var context = new PipelineContext();

            var expectedErrorCount = 1;

            var expectedMessage = Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, PipelineContextConstants.INPUT_STREAM);

            using (var sut = new DeserializeObjectFromStreamStep<object>(logger, options, next))
            {
                // act
                await sut.InvokeAsync(context);
            }

            // assert
            Assert.AreEqual(expectedErrorCount, context.Errors.Count);
            Assert.AreEqual(expectedMessage, context.Errors[0].Message);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Next_Step_Verifies_Values_Passed_Downstream()
        {
            // arrange
            var context = new PipelineContext();
            var expectedResult = new SampleItem() { ConnectionString = "value" };

            var logger = new Mock<ILogger<DeserializeObjectFromStreamStep<SampleItem>>>().Object;
            var options = new DeserializeObjectFromStreamOptions();
            var nextMock = new Mock<IPipelineRequest>();
            nextMock.Setup(x => x.InvokeAsync(context))
                                       .Callback(() =>
                                       {
                                           var inputStream = (context.Items[options.InputStreamContextName] as Stream)!;

                                           // assert - phase 1
                                           Assert.IsTrue(context.Items.ContainsKey(options.OutputObjectContextName), "context does not contain OUTPUT_OBJECT.");
                                           Assert.IsTrue(inputStream.CanRead, "Stream should not be closed.");

                                           var result = context.Items[options.OutputObjectContextName] as SampleItem;
                                           Assert.AreEqual(expectedResult, result);
                                       })
                                       .Returns(Task.CompletedTask);

            var next = nextMock.Object;
            string contents = "{ \"ConnectionString\": \"value\" }";

            var expectedInputStream = new MemoryStream(Encoding.UTF8.GetBytes(contents));

            context.Items.Add(options.InputStreamContextName, expectedInputStream);

            var expectedErrorCount = 0;

            using (var sut = new DeserializeObjectFromStreamStep<SampleItem>(logger, options, next))
            {
                // act
                await sut.InvokeAsync(context);
            }

            // assert - phase 2
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            Assert.AreEqual(expectedErrorCount, context.Errors.Count);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Options_Uses_Different_Context_Names()
        {
            // arrange
            var context = new PipelineContext();
            var expectedResult = new SampleItem() { ConnectionString = "value" };

            var logger = new Mock<ILogger<DeserializeObjectFromStreamStep<SampleItem>>>().Object;
            var options = new DeserializeObjectFromStreamOptions()
            {
                InputStreamContextName = "TESTING_OTHER_INPUT_NAME",
                OutputObjectContextName = "TESTING_OTHER_OUTPUT_STREAM"
            };
            var nextMock = new Mock<IPipelineRequest>();
            nextMock.Setup(x => x.InvokeAsync(context))
                                       .Callback(() =>
                                       {
                                           var inputStream = (context.Items[options.InputStreamContextName] as Stream)!;

                                           // assert - phase 1
                                           Assert.IsTrue(context.Items.ContainsKey(options.OutputObjectContextName), "context does not contain OUTPUT_OBJECT.");
                                           Assert.IsTrue(inputStream.CanRead, "Stream should not be closed.");

                                           var result = context.Items[options.OutputObjectContextName] as SampleItem;
                                           Assert.AreEqual(expectedResult, result);
                                       })
                                       .Returns(Task.CompletedTask);

            var next = nextMock.Object;
            string contents = "{ \"ConnectionString\": \"value\" }";

            var expectedInputStream = new MemoryStream(Encoding.UTF8.GetBytes(contents));

            context.Items.Add(options.InputStreamContextName, expectedInputStream);

            var expectedErrorCount = 0;

            using (var sut = new DeserializeObjectFromStreamStep<SampleItem>(logger, options, next))
            {
                // act
                await sut.InvokeAsync(context);
            }

            // assert - phase 2
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            Assert.AreEqual(expectedErrorCount, context.Errors.Count);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Stream_Is_Deserialized_Successfully()
        {
            // arrange
            var logger = new Mock<ILogger<DeserializeObjectFromStreamStep<SampleItem>>>().Object;
            var options = new DeserializeObjectFromStreamOptions();
            var next = new Mock<IPipelineRequest>().Object;
            string contents = "{ \"ConnectionString\": \"value\" }";

            var expectedInputStream = new MemoryStream(Encoding.UTF8.GetBytes(contents));

            var context = new PipelineContext();
            context.Items.Add(options.InputStreamContextName, expectedInputStream);

            var expectedErrorCount = 0;

            using (var sut = new DeserializeObjectFromStreamStep<SampleItem>(logger, options, next))
            {
                // act
                await sut.InvokeAsync(context);
            }

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            Assert.AreEqual(expectedErrorCount, context.Errors.Count);
        }

        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_With_Previously_Configured_Values_Restored()
        {
            // arrange
            var logger = new Mock<ILogger<DeserializeObjectFromStreamStep<SampleItem>>>().Object;
            var options = new DeserializeObjectFromStreamOptions();
            var next = new Mock<IPipelineRequest>().Object;
            string contents = "{ \"ConnectionString\": \"value\" }";

            var expectedInputStream = new MemoryStream(Encoding.UTF8.GetBytes(contents));
            var expectedOutputObject = new object();

            var context = new PipelineContext();
            context.Items.Add(options.InputStreamContextName, expectedInputStream);
            context.Items.Add(options.OutputObjectContextName, expectedOutputObject);

            var expectedItemCount = context.Items.Count;

            using (var sut = new DeserializeObjectFromStreamStep<SampleItem>(logger, options, next))
            {
                // act
                await sut.InvokeAsync(context);
            }

            // assert
            Assert.AreEqual(expectedItemCount, context.Items.Count);
            Assert.IsTrue(context.Items.ContainsKey(options.InputStreamContextName));
            Assert.AreEqual(expectedInputStream, context.Items[options.InputStreamContextName]);
            Assert.IsTrue(context.Items.ContainsKey(options.OutputObjectContextName));
            Assert.AreEqual(expectedOutputObject, context.Items[options.OutputObjectContextName]);
        }
    }
}