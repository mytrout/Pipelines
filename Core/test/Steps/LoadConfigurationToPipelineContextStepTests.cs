// <copyright file="LoadConfigurationToPipelineContextStepTests.cs" company="Chris Trout">
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
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines.Core;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class LoadConfigurationToPipelineContextStepTests
    {
        [TestMethod]
        public void Constructs_LoadConfigurationToPipelineContextStep_Successfully()
        {
            // arrange
            ILogger<LoadValuesFromConfigurationToPipelineContextStep> logger = new Mock<ILogger<LoadValuesFromConfigurationToPipelineContextStep>>().Object;
            LoadValuesFromConfigurationToPipelineContextOptions options = new();
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            // act
            using (var result = new LoadValuesFromConfigurationToPipelineContextStep(logger, options, next))
            {
                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(logger, result.Logger);
                Assert.AreEqual(next, result.Next);
            }
        }

        [TestMethod]
        public async Task Provides_Configuration_Value_In_InvokeAsync_To_Downstream_Callers()
        {
            // arrange
            var errorCount = 0;
            IConfiguration configuration = new ConfigurationBuilder()
                                                    .AddJsonFile("input-stream.json")
                                                    .Build();

            ILogger<LoadValuesFromConfigurationToPipelineContextStep> logger = new Mock<ILogger<LoadValuesFromConfigurationToPipelineContextStep>>().Object;
            LoadValuesFromConfigurationToPipelineContextOptions options = new()
            {
                ConfigurationNames = new List<string>() { "InputStreamContextName" },
                Configuration = configuration
            };
            var context = new PipelineContext();
            var contextName = "InputStreamContextName";
            var expectedValue = PipelineContextConstants.INPUT_STREAM;

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context))
                        .Callback(() =>
                        {
                            // assertion here ensures that the expectedValue passed to downstream callers.
                            Assert.AreEqual(expectedValue, context.Items[contextName]);
                        })
                        .Returns(Task.CompletedTask);
            IPipelineRequest next = mockNext.Object;
            using (var source = new LoadValuesFromConfigurationToPipelineContextStep(logger, options, next))
            {
                var expectedItemsCount = 0;

                // act
                await source.InvokeAsync(context);

                // assert
                Assert.AreEqual(errorCount, context.Errors.Count);
                Assert.AreEqual(expectedItemsCount, context.Items.Count);
            }
        }

        [TestMethod]
        public async Task Returns_Context_With_No_Alterations_From_InvokeAsync_After_Execution()
        {
            // arrange
            var errorCount = 0;
            IConfiguration configuration = new ConfigurationBuilder()
                                                    .AddJsonFile("input-stream.json")
                                                    .Build();

            ILogger<LoadValuesFromConfigurationToPipelineContextStep> logger = new Mock<ILogger<LoadValuesFromConfigurationToPipelineContextStep>>().Object;
            LoadValuesFromConfigurationToPipelineContextOptions options = new()
            {
                ConfigurationNames = new List<string>() { "InputStreamContextName" },
                Configuration = configuration
            };
            var context = new PipelineContext();
            var contextName = "InputStreamContextName";
            var contextValue = Guid.NewGuid();

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            IPipelineRequest next = mockNext.Object;

            using (var source = new LoadValuesFromConfigurationToPipelineContextStep(logger, options, next))
            {
                context.Items.Add(contextName, contextValue);
                var expectedItemsCount = 1;

                // act
                await source.InvokeAsync(context);

                // assert
                Assert.AreEqual(errorCount, context.Errors.Count);
                Assert.AreEqual(expectedItemsCount, context.Items.Count);
                Assert.IsTrue(context.Items.ContainsKey(contextName), "context does contain a key named '{0}'.", contextName);
                Assert.AreEqual(contextValue, context.Items[contextName], "context does contain a key named '{0}' with a value of '{1}'.", contextName, contextValue);
            }
        }

        [TestMethod]
        public async Task Returns_Task_From_DisposeAsync()
        {
            // arrange
            ILogger<LoadValuesFromConfigurationToPipelineContextStep> logger = new Mock<ILogger<LoadValuesFromConfigurationToPipelineContextStep>>().Object;
            LoadValuesFromConfigurationToPipelineContextOptions options = new();
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            using (var source = new LoadValuesFromConfigurationToPipelineContextStep(logger, options, next))
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
            ILogger<LoadValuesFromConfigurationToPipelineContextStep> logger = null;
            LoadValuesFromConfigurationToPipelineContextOptions options = new();
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            string expectedParamName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new LoadValuesFromConfigurationToPipelineContextStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Next_Is_Null()
        {
            // arrange
            ILogger<LoadValuesFromConfigurationToPipelineContextStep> logger = new Mock<ILogger<LoadValuesFromConfigurationToPipelineContextStep>>().Object;
            LoadValuesFromConfigurationToPipelineContextOptions options = new();
            IPipelineRequest next = null;

            string expectedParamName = nameof(next);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new LoadValuesFromConfigurationToPipelineContextStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Options_Is_Null()
        {
            // arrange
            ILogger<LoadValuesFromConfigurationToPipelineContextStep> logger = new Mock<ILogger<LoadValuesFromConfigurationToPipelineContextStep>>().Object;
            LoadValuesFromConfigurationToPipelineContextOptions options = null;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            string expectedParamName = nameof(options);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new LoadValuesFromConfigurationToPipelineContextStep(logger, options, next));

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

            ILogger<LoadValuesFromConfigurationToPipelineContextStep> logger = new Mock<ILogger<LoadValuesFromConfigurationToPipelineContextStep>>().Object;
            LoadValuesFromConfigurationToPipelineContextOptions options = new();
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            using (var source = new LoadValuesFromConfigurationToPipelineContextStep(logger, options, next))
            {
                // act
                var result = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => source.InvokeAsync(context));

                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedParamName, result.ParamName);
            }
        }
    }
}