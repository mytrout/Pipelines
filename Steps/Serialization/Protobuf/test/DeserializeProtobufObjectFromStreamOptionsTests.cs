// <copyright file="DeserializeProtobufObjectFromStreamOptionsTests.cs" company="Chris Trout">
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
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class DeserializeProtobufObjectFromStreamOptionsTests
    {
        [TestMethod]
        public async Task Returns_PipelineContext_From_InvokeAsync_When_Next_Step_Verifies_Values_Passed_Downstream()
        {
            // arrange
            var context = new PipelineContext();
            var expectedResult = new SampleItem() { ConnectionString = "NotAValue", CreatedDate = new DateTime(2001, 01, 01, 01, 15, 48, DateTimeKind.Utc), CreatedById = 1001 };

            var logger = new Mock<ILogger<DeserializeObjectFromStreamStep<SampleItem>>>().Object;
            var options = new DeserializeProtobufObjectFromStreamOptions();
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

            // Mark Gravell is absolutely my hero.
            // https://stackoverflow.com/questions/23045750/protobuf-net-deserializing-arithmetic-operation-resulted-in-an-overflow
            // https://blog.marcgravell.com/2013/02/how-many-ways-can-you-mess-up-io.html
            // When you mess things up, he provides blog posts from 9 years ago to fix you...and make you realize you aren't as smart as you thought you were.
            string contents = "CglOb3RBVmFsdWUSCAiI6f6kBxADGOkH";

            var expectedInputStream = new MemoryStream(Convert.FromBase64String(contents))
            {
                Position = 0
            };

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
            var expectedResult = new SampleItem() { ConnectionString = "NotAValue", CreatedDate = new DateTime(2001, 01, 01, 01, 15, 48, DateTimeKind.Utc), CreatedById = 1001 };

            var logger = new Mock<ILogger<DeserializeObjectFromStreamStep<SampleItem>>>().Object;
            var options = new DeserializeProtobufObjectFromStreamOptions()
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

            // Mark Gravell is absolutely my hero.
            // https://stackoverflow.com/questions/23045750/protobuf-net-deserializing-arithmetic-operation-resulted-in-an-overflow
            // https://blog.marcgravell.com/2013/02/how-many-ways-can-you-mess-up-io.html
            // When you mess things up, he provides blog posts from 9 years ago to fix you...and make you realize you aren't as smart as you thought you were.
            string contents = "CglOb3RBVmFsdWUSCAiI6f6kBxADGOkH";

            var expectedInputStream = new MemoryStream(Convert.FromBase64String(contents))
            {
                Position = 0
            };

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
            var options = new DeserializeProtobufObjectFromStreamOptions();
            var next = new Mock<IPipelineRequest>().Object;

            // Mark Gravell is absolutely my hero.
            // https://stackoverflow.com/questions/23045750/protobuf-net-deserializing-arithmetic-operation-resulted-in-an-overflow
            // https://blog.marcgravell.com/2013/02/how-many-ways-can-you-mess-up-io.html
            // When you mess things up, he provides blog posts from 9 years ago to fix you...and make you realize you aren't as smart as you thought you were.
            string contents = "CglOb3RBVmFsdWUSCAiI6f6kBxADGOkH";

            var expectedInputStream = new MemoryStream(Convert.FromBase64String(contents))
            {
                Position = 0
            };

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
    }
}
