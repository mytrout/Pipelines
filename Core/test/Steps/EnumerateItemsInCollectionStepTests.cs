// <copyright file="EnumerateItemsInCollectionStepTests.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.Tests
{
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines.Core;
    using MyTrout.Pipelines.Tests;
    using System;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class EnumerateItemsInCollectionStepTests
    {
        [TestMethod]
        public void Constructs_EnumerateItemsInCollectionStep_Successfully()
        {
            // arrange
            ILogger<EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>> logger = new Mock<ILogger<EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>>>().Object;
            EnumerateItemsInCollectionOptions options = new();
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            // act
            using (var result = new EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>(logger, options, next))
            {
                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(logger, result.Logger);
                Assert.AreEqual(next, result.Next);
            }
        }

        [TestMethod]
        public async Task Returns_Items_From_Enumerator_In_InvokeAsync_To_Downstream_Callers()
        {
            // arrange
            var parent = new SamplePersonEnumerator();
            parent.Persons.Add(new SamplePerson() { LastName = "GoesThere", FirstName = "Who" });
            parent.Persons.Add(new SamplePerson() { LastName = "Name", FirstName = "Whatsher" });

            PipelineContext context = new();
            context.Items.Add(PipelineContextConstants.INPUT_OBJECT, parent);

            ILogger<EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>> logger = new Mock<ILogger<EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>>>().Object;

            EnumerateItemsInCollectionOptions options = new();

            Mock<IPipelineRequest> mockNext = new();

            var itemToCheck = 0;
            mockNext.Setup(x => x.InvokeAsync(It.IsAny<IPipelineContext>())).Callback(() =>
            {
                // assert - phase 1
                Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.OUTPUT_OBJECT));
                Assert.AreEqual(parent.Persons[itemToCheck], context.Items[PipelineContextConstants.OUTPUT_OBJECT]);
                itemToCheck++;
            });

            var next = mockNext.Object;

            using (var sut = new EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>(logger, options, next))
            {
                // act
                await sut.InvokeAsync(context);

                // assert
                if (context.Errors.Count > 0)
                {
                    // Because the exception is thrown if there is one.
                    Assert.IsTrue(context.Errors.Count == 0, context.Errors[0].ToString());
                }
            }
        }

        [TestMethod]
        public async Task Returns_Context_With_No_Alterations_From_InvokeAsync_After_Execution()
        {
            // arrange
            var parent = new SamplePersonEnumerator();
            parent.Persons.Add(new SamplePerson() { LastName = "GoesThere", FirstName = "Who" });
            parent.Persons.Add(new SamplePerson() { LastName = "Name", FirstName = "Whatsher" });

            int errorCount = 0;
            ILogger<EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>> logger = new Mock<ILogger<EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>>>().Object;
            EnumerateItemsInCollectionOptions options = new();
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            using (var sut = new EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>(logger, options, next))
            {
                var context = new PipelineContext();
                var contextName = Guid.NewGuid().ToString();
                var contextValue = Guid.NewGuid();
                context.Items.Add(contextName, contextValue);
                context.Items.Add(PipelineContextConstants.INPUT_OBJECT, parent);

                var expectedItemsCount = 2;

                // act
                await sut.InvokeAsync(context);

                // assert
                Assert.AreEqual(errorCount, context.Errors.Count);
                Assert.AreEqual(expectedItemsCount, context.Items.Count);
                Assert.IsTrue(context.Items.ContainsKey(contextName), "context does contain a key named '{0}'.", contextName);
                Assert.AreEqual(contextValue, context.Items[contextName], "context does contain a key named '{0}' with a value of '{1}'.", contextName, contextValue);
                Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.INPUT_OBJECT), "context does contain a key named '{0}'.", PipelineContextConstants.INPUT_OBJECT);
                Assert.AreEqual(parent, context.Items[PipelineContextConstants.INPUT_OBJECT], "context does contain a key named '{0}' with a value of '{1}'.", PipelineContextConstants.INPUT_OBJECT, parent);
            }
        }

        [TestMethod]
        public async Task Returns_Task_From_DisposeAsync()
        {
            // arrange
            ILogger<EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>> logger = new Mock<ILogger<EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>>>().Object;
            EnumerateItemsInCollectionOptions options = new();
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            using (var sut = new EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>(logger, options, next))
            {
                // act
                await sut.DisposeAsync();

                // assert
                Assert.IsTrue(true);

                // No exceptions mean this worked appropriately.
            }
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Logger_Is_Null()
        {
            // arrange
            ILogger<EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>> logger = null;
            EnumerateItemsInCollectionOptions options = new();
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            string expectedParamName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Next_Is_Null()
        {
            // arrange
            ILogger<EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>> logger = new Mock<ILogger<EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>>>().Object;
            EnumerateItemsInCollectionOptions options = new();
            IPipelineRequest next = null;

            string expectedParamName = nameof(next);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Options_Is_Null()
        {
            // arrange
            ILogger<EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>> logger = new Mock<ILogger<EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>>>().Object;
            EnumerateItemsInCollectionOptions options = null;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            string expectedParamName = nameof(options);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public async Task Throws_ArgumentNullException_From_InvokeAsync_When_Context_Is_Null()
        {
            // arrange
            PipelineContext context = null;
            string expectedParamName = nameof(context);

            ILogger<EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>> logger = new Mock<ILogger<EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>>>().Object;
            EnumerateItemsInCollectionOptions options = new();
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            using (var source = new EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>(logger, options, next))
            {
                // act
                var result = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => source.InvokeAsync(context));

                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedParamName, result.ParamName);
            }
        }

        [TestMethod]
        public async Task Throws_InvalidOperationException_When_Context_Does_Not_Contain_InputObjectContextName()
        {
            // arrange
            var expectedMessage = "'PIPELINE_INPUT_OBJECT' does not exist in the Pipeline context.";
            var expectedErrorCount = 1;

            ILogger<EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>> logger = new Mock<ILogger<EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>>>().Object;
            EnumerateItemsInCollectionOptions options = new();
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            using (var sut = new EnumerateItemsInCollectionStep<SamplePersonEnumerator, SamplePerson>(logger, options, next))
            {
                var context = new PipelineContext();
                var contextName = Guid.NewGuid().ToString();
                var contextValue = Guid.NewGuid();
                context.Items.Add(contextName, contextValue);

                // act
                await sut.InvokeAsync(context);

                // assert
                Assert.AreEqual(expectedErrorCount, context.Errors.Count);
                Assert.AreEqual(expectedMessage, context.Errors[0].Message);
            }
        }
    }
}