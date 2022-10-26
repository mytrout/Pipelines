// <copyright file="SerializeProtobufObjectToStreamOptionsTests.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.Serialization.Protobuf.Tests
{
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines.Core;
    using MyTrout.Pipelines.Steps.Serialization;
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class SerializeProtobufObjectToStreamOptionsTests
    {
        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Next_Step_Verifies_Values_Passed_Downstream()
        {
            // arrange
            var context = new PipelineContext();
            var expectedInputObject = new SampleItem() { ConnectionString = "value", CreatedDate = DateTime.MinValue, CreatedById = 1000 };
            var expectedContents = "CgV2YWx1ZRIECAEQDxjoBw==";

            var logger = new Mock<ILogger<SerializeObjectToStreamStep<SampleItem>>>().Object;
            var options = new SerializeProtobufObjectToStreamOptions();

            var nextMock = new Mock<IPipelineRequest>();
            nextMock.Setup(x => x.InvokeAsync(context))
                                       .Callback(() =>
                                       {
                                           var inputObject = (context.Items[options.InputObjectContextName] as SampleItem)!;

                                           // assert - phase 1
                                           Assert.AreEqual(inputObject, expectedInputObject);

                                           Assert.IsTrue(context.Items.ContainsKey(options.OutputStreamContextName), "context does not contain OUTPUT_STREAM.");
                                           using (var outputStream = (context.Items[options.OutputStreamContextName] as MemoryStream)!)
                                           {
                                               var buffer = outputStream.GetBuffer();
                                               string contents = Convert.ToBase64String(buffer, 0, (int)outputStream.Length);
                                               Assert.AreEqual(expectedContents, contents);
                                           }
                                       })
                                       .Returns(Task.CompletedTask);

            var next = nextMock.Object;

            context.Items.Add(options.InputObjectContextName, expectedInputObject);

            var expectedErrorCount = 0;

            using (var sut = new SerializeObjectToStreamStep<SampleItem>(logger, options, next))
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
            var expectedInputObject = new SampleItem() { ConnectionString = "value", CreatedDate = DateTime.MinValue, CreatedById = 1000 };

            var expectedContents = "CgV2YWx1ZRIECAEQDxjoBw==";

            var logger = new Mock<ILogger<SerializeObjectToStreamStep<SampleItem>>>().Object;
            var options = new SerializeProtobufObjectToStreamOptions()
            {
                InputObjectContextName = "TESTING_OTHER_INPUT_OBJECT",
                OutputStreamContextName = "TESTING_OTHER_OUTPUT_STREAM"
            };

            var nextMock = new Mock<IPipelineRequest>();
            nextMock.Setup(x => x.InvokeAsync(context))
                                       .Callback(() =>
                                       {
                                           var inputObject = (context.Items[options.InputObjectContextName] as SampleItem)!;

                                           // assert - phase 1
                                           Assert.AreEqual(inputObject, expectedInputObject);

                                           Assert.IsTrue(context.Items.ContainsKey(options.OutputStreamContextName), "context does not contain OUTPUT_STREAM.");
                                           using (var outputStream = (context.Items[options.OutputStreamContextName] as MemoryStream)!)
                                           {
                                               var buffer = outputStream.GetBuffer();
                                               string contents = Convert.ToBase64String(buffer, 0, (int)outputStream.Length);
                                               Assert.AreEqual(expectedContents, contents);
                                           }
                                       })
                                       .Returns(Task.CompletedTask);

            var next = nextMock.Object;

            context.Items.Add(options.InputObjectContextName, expectedInputObject);

            var expectedErrorCount = 0;

            using (var sut = new SerializeObjectToStreamStep<SampleItem>(logger, options, next))
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
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Stream_Is_Serialized_Successfully()
        {
            // arrange
            var context = new PipelineContext();
            var expectedInputObject = new SampleItem() { ConnectionString = "value", CreatedDate = DateTime.MinValue, CreatedById = 1000 };

            var logger = new Mock<ILogger<SerializeObjectToStreamStep<SampleItem>>>().Object;
            var options = new SerializeProtobufObjectToStreamOptions();

            var nextMock = new Mock<IPipelineRequest>();
            nextMock.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);

            var next = nextMock.Object;

            context.Items.Add(options.InputObjectContextName, expectedInputObject);

            var expectedErrorCount = 0;

            using (var sut = new SerializeObjectToStreamStep<SampleItem>(logger, options, next))
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
    }
}
