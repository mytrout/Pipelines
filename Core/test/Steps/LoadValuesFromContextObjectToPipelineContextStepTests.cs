// <copyright file="LoadValuesFromContextObjectToPipelineContextStepTests.cs" company="Chris Trout">
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
    public class LoadValuesFromContextObjectToPipelineContextStepTests
    {
        [TestMethod]
        public void Constructs_LoadValuesFromContextObjectToPipelineContextStep_Successfully()
        {
            // arrange
            ILogger<LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>> logger = new Mock<ILogger<LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>>>().Object;

            LoadValuesFromContextObjectToPipelineContextOptions options = new();

            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            // act
            using (var result = new LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>(logger, options, next))
            {
                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(logger, result.Logger);
                Assert.AreEqual(next, result.Next);
                Assert.AreEqual(options, result.Options);
            }
        }

        [TestMethod]
        public async Task Provides_Context_Object_Values_In_InvokeAsync_To_Downstream_Callers()
        {
            // arrange
            var errorCount = 0;

            ILogger<LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>> logger = new Mock<ILogger<LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>>>().Object;

            LoadValuesFromContextObjectToPipelineContextOptions options = new();

            SamplePerson inputObject = new SamplePerson()
            {
                LastName = "Dooray",
                FirstName = "Sunny"
            };
            var context = new PipelineContext();
            context.Items.Add(PipelineContextConstants.INPUT_OBJECT, inputObject);

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                        .Callback(() =>
                        {
                            // assertion here ensures that the expectedValue passed to downstream callers.
                            Assert.AreEqual(inputObject.LastName, context.Items["SamplePerson.LastName"]);
                            Assert.AreEqual(inputObject.FirstName, context.Items["SamplePerson.FirstName"]);
                        })
                        .Returns(Task.CompletedTask);

            IPipelineRequest next = mockNext.Object;
            using (var source = new LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>(logger, options, next))
            {
                var expectedItemsCount = 1;

                // act
                await source.InvokeAsync(context);

                // assert
                Assert.AreEqual(errorCount, context.Errors.Count);
                Assert.AreEqual(expectedItemsCount, context.Items.Count);
                Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.INPUT_OBJECT), "context does contain a key named '{0}'.", PipelineContextConstants.INPUT_OBJECT);
                Assert.AreEqual(inputObject, context.Items[PipelineContextConstants.INPUT_OBJECT], "context does contain a key named '{0}' with a value of '{1}'.", PipelineContextConstants.INPUT_OBJECT, inputObject);
            }
        }

        [TestMethod]
        public async Task Returns_Context_With_No_Alterations_From_InvokeAsync_After_Execution()
        {
            // arrange
            var errorCount = 0;
            ILogger<LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>> logger = new Mock<ILogger<LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>>>().Object;

            LoadValuesFromContextObjectToPipelineContextOptions options = new();

            SamplePerson inputObject = new SamplePerson()
            {
                LastName = "Dooray",
                FirstName = "Sunny"
            };
            var context = new PipelineContext();
            var contextName = Guid.NewGuid().ToString();
            var contextValue = Guid.NewGuid();

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockNext.Object;

            using (var source = new LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>(logger, options, next))
            {
                context.Items.Add(contextName, contextValue);
                context.Items.Add(PipelineContextConstants.INPUT_OBJECT, inputObject);

                var expectedItemsCount = 2;

                // act
                await source.InvokeAsync(context);

                // assert
                Assert.AreEqual(errorCount, context.Errors.Count);
                Assert.AreEqual(expectedItemsCount, context.Items.Count);
                Assert.IsTrue(context.Items.ContainsKey(contextName), "context does contain a key named '{0}'.", contextName);
                Assert.AreEqual(contextValue, context.Items[contextName], "context does contain a key named '{0}' with a value of '{1}'.", contextName, contextValue);
                Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.INPUT_OBJECT), "context does contain a key named '{0}'.", PipelineContextConstants.INPUT_OBJECT);
                Assert.AreEqual(inputObject, context.Items[PipelineContextConstants.INPUT_OBJECT], "context does contain a key named '{0}' with a value of '{1}'.", PipelineContextConstants.INPUT_OBJECT, inputObject);
            }
        }

#pragma warning disable CS0618
        [TestMethod]
        public async Task Provides_Context_Object_Values_In_InvokeAsync_When_BuildContextNameFunction_Is_Configured_Differently()
        {
            // arrange
            var errorCount = 0;

            ILogger<LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>> logger = new Mock<ILogger<LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>>>().Object;

            LoadValuesFromContextObjectToPipelineContextOptions options = new()
            {
                BuildContextNameFunction = (string typeName, string propertyName) => { return $"{propertyName}.{typeName}"; }
            };

            SamplePerson inputObject = new SamplePerson()
            {
                LastName = "Dooray",
                FirstName = "Sunny"
            };
            var context = new PipelineContext();
            context.Items.Add(PipelineContextConstants.INPUT_OBJECT, inputObject);

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                        .Callback(() =>
                        {
                            // assertion here ensures that the expectedValue passed to downstream callers.
                            Assert.AreEqual(inputObject.LastName, context.Items["LastName.SamplePerson"]);
                            Assert.AreEqual(inputObject.FirstName, context.Items["FirstName.SamplePerson"]);
                        })
                        .Returns(Task.CompletedTask);

            IPipelineRequest next = mockNext.Object;
            using (var source = new LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>(logger, options, next))
            {
                var expectedItemsCount = 1;

                // act
                await source.InvokeAsync(context);

                // assert
                Assert.AreEqual(errorCount, context.Errors.Count);
                Assert.AreEqual(expectedItemsCount, context.Items.Count);
                Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.INPUT_OBJECT), "context does contain a key named '{0}'.", PipelineContextConstants.INPUT_OBJECT);
                Assert.AreEqual(inputObject, context.Items[PipelineContextConstants.INPUT_OBJECT], "context does contain a key named '{0}' with a value of '{1}'.", PipelineContextConstants.INPUT_OBJECT, inputObject);
            }
        }
#pragma warning restore CS0618

        [TestMethod]
        public async Task Provides_Context_Object_Values_In_InvokeAsync_When_ContextNameBuilder_Is_Configured_Differently()
        {
            // arrange
            var errorCount = 0;

            ILogger<LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>> logger = new Mock<ILogger<LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>>>().Object;

            Mock<IContextNameBuilder> mockContextNameBuilder = new Mock<IContextNameBuilder>();
            mockContextNameBuilder.Setup(x => x.BuildContextName(nameof(SamplePerson), It.IsAny<string>()))
                                                .Returns((string typeName, string propertyName) => { return $"{propertyName}.{typeName}"; });

            LoadValuesFromContextObjectToPipelineContextOptions options = new()
            {
                ContextNameBuilder = mockContextNameBuilder.Object
            };

            SamplePerson inputObject = new SamplePerson()
            {
                LastName = "Dooray",
                FirstName = "Sunny"
            };
            var context = new PipelineContext();
            context.Items.Add(PipelineContextConstants.INPUT_OBJECT, inputObject);

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                        .Callback(() =>
                        {
                            // assertion here ensures that the expectedValue passed to downstream callers.
                            Assert.AreEqual(inputObject.LastName, context.Items["LastName.SamplePerson"]);
                            Assert.AreEqual(inputObject.FirstName, context.Items["FirstName.SamplePerson"]);
                        })
                        .Returns(Task.CompletedTask);

            IPipelineRequest next = mockNext.Object;
            using (var source = new LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>(logger, options, next))
            {
                var expectedItemsCount = 1;

                // act
                await source.InvokeAsync(context);

                // assert
                Assert.AreEqual(errorCount, context.Errors.Count);
                Assert.AreEqual(expectedItemsCount, context.Items.Count);
                Assert.IsTrue(context.Items.ContainsKey(PipelineContextConstants.INPUT_OBJECT), "context does contain a key named '{0}'.", PipelineContextConstants.INPUT_OBJECT);
                Assert.AreEqual(inputObject, context.Items[PipelineContextConstants.INPUT_OBJECT], "context does contain a key named '{0}' with a value of '{1}'.", PipelineContextConstants.INPUT_OBJECT, inputObject);
            }
        }

        [TestMethod]
        public async Task Returns_Context_From_Invoke_Async_When_InputObjectContextName_Is_Configured_Differently()
        {
            // arrange
            var errorCount = 0;
            ILogger<LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>> logger = new Mock<ILogger<LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>>>().Object;

            var differentInputObjectContextName = "TESTING_DIFFERENT_CONTEXT_NAME";
            LoadValuesFromContextObjectToPipelineContextOptions options = new()
            {
                InputObjectContextName = differentInputObjectContextName
            };

            SamplePerson inputObject = new SamplePerson()
            {
                LastName = "Dooray",
                FirstName = "Sunny"
            };
            var context = new PipelineContext();
            var contextName = Guid.NewGuid().ToString();
            var contextValue = Guid.NewGuid();

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockNext.Object;

            using (var source = new LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>(logger, options, next))
            {
                context.Items.Add(contextName, contextValue);
                context.Items.Add(differentInputObjectContextName, inputObject);

                var expectedItemsCount = 2;

                // act
                await source.InvokeAsync(context);

                // assert
                Assert.AreEqual(errorCount, context.Errors.Count);
                Assert.AreEqual(expectedItemsCount, context.Items.Count);
                Assert.IsTrue(context.Items.ContainsKey(contextName), "context does contain a key named '{0}'.", contextName);
                Assert.AreEqual(contextValue, context.Items[contextName], "context does contain a key named '{0}' with a value of '{1}'.", contextName, contextValue);
                Assert.IsTrue(context.Items.ContainsKey(differentInputObjectContextName), "context does contain a key named '{0}'.", differentInputObjectContextName);
                Assert.AreEqual(inputObject, context.Items[differentInputObjectContextName], "context does contain a key named '{0}' with a value of '{1}'.", differentInputObjectContextName, inputObject);
            }
        }

        [TestMethod]
        public async Task Returns_Task_From_DisposeAsync()
        {
            // arrange
            ILogger<LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>> logger = new Mock<ILogger<LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>>>().Object;

            LoadValuesFromContextObjectToPipelineContextOptions options = new();

            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            using (var source = new LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>(logger, options, next))
            {
                // act
                await source.DisposeAsync();

                // assert
                Assert.IsTrue(true);

                // No exceptions mean this worked appropriately.
            }
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Logger_Is_Null()
        {
            // arrange
            ILogger<LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>> logger = null;

            LoadValuesFromContextObjectToPipelineContextOptions options = new();

            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            string expectedParamName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Next_Is_Null()
        {
            // arrange
            ILogger<LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>> logger = new Mock<ILogger<LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>>>().Object;

            LoadValuesFromContextObjectToPipelineContextOptions options = new();

            IPipelineRequest next = null;

            string expectedParamName = nameof(next);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Options_Is_Null()
        {
            // arrange
            ILogger<LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>> logger = new Mock<ILogger<LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>>>().Object;

            LoadValuesFromContextObjectToPipelineContextOptions options = null;

            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            string expectedParamName = nameof(options);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>(logger, options, next));

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

            ILogger<LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>> logger = new Mock<ILogger<LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>>>().Object;

            LoadValuesFromContextObjectToPipelineContextOptions options = new();

            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            using (var source = new LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>(logger, options, next))
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

            ILogger<LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>> logger = new Mock<ILogger<LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>>>().Object;

            LoadValuesFromContextObjectToPipelineContextOptions options = new();

            var context = new PipelineContext();

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);

            IPipelineRequest next = mockNext.Object;
            using (var source = new LoadValuesFromContextObjectToPipelineContextStep<SamplePerson>(logger, options, next))
            {
                // act
                await source.InvokeAsync(context);

                // assert
                Assert.AreEqual(expectedErrorCount, context.Errors.Count);
                Assert.AreEqual(expectedMessage, context.Errors[0].Message);
            }
        }
    }
}