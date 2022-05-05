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
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines.Core;
    using System;
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
            LoadValuesFromConfigurationToPipelineContextOptions options = new LoadValuesFromConfigurationToPipelineContextOptions();
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
        public async Task Returns_Task_From_DisposeAsync()
        {
            // arrange
            ILogger<LoadValuesFromConfigurationToPipelineContextStep> logger = new Mock<ILogger<LoadValuesFromConfigurationToPipelineContextStep>>().Object;
            LoadValuesFromConfigurationToPipelineContextOptions options = new LoadValuesFromConfigurationToPipelineContextOptions();
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
            LoadValuesFromConfigurationToPipelineContextOptions options = new LoadValuesFromConfigurationToPipelineContextOptions();
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
            LoadValuesFromConfigurationToPipelineContextOptions options = new LoadValuesFromConfigurationToPipelineContextOptions();
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
            LoadValuesFromConfigurationToPipelineContextOptions options = new LoadValuesFromConfigurationToPipelineContextOptions();
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