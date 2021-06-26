// <copyright file="PipelineHostedServiceTests.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2019-2021 Chris Trout
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

namespace MyTrout.Pipelines.Hosting.Tests
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines.Core;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class PipelineHostedServiceTests
    {
        /*
         * TO FUTURE DEVELOPERS: ALL NON-CONSTRUCTOR METHODS ARE TESTED IN THE
         *                       PIPELINE_HOST_BUILDER_EXTENSIONS_TESTS.
         */

        [TestMethod]
        public void Successfully_Constructs_A_PipelineHostedService_Instance()
        {
            // arrange
            ILogger<PipelineHostedService> logger = new Mock<ILogger<PipelineHostedService>>().Object;
            IHostApplicationLifetime applicationLifetime = new Mock<IHostApplicationLifetime>().Object;
            IStepActivator stepActivator = new Mock<IStepActivator>().Object;
            PipelineBuilder pipelineBuilder = new PipelineBuilder();
            PipelineContext context = new PipelineContext();

            // act
            var result = new PipelineHostedService(logger, applicationLifetime, stepActivator, pipelineBuilder, context);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(applicationLifetime, result.ApplicationLifetime);
            Assert.AreEqual(logger, result.Logger);
            Assert.AreEqual(pipelineBuilder, result.PipelineBuilder);
            Assert.AreEqual(stepActivator, result.StepActivator);
        }

        [TestMethod]
        public async Task Successfully_Catches_An_Exception_During_Pipeline_Run_Without_Crashing_PipelineServiceHost()
        {
            // arrange
            string[] args = Array.Empty<string>();
            PipelineBuilder pipelineBuilder = new PipelineBuilder()
                                                    .AddStep<TestingStepException>();

            var host = Host.CreateDefaultBuilder(args)
                            .UsePipeline(pipelineBuilder)
                            .Build();

            Type expectedFirstException = typeof(InvalidTimeZoneException);
            Type expectedSecondException = typeof(InvalidOperationException);
            string expectedMessage = "Try to destroy the service.";

            try
            {
                // act
                await host.StartAsync().ConfigureAwait(false);

                // assert
                var context = host.Services.GetService<PipelineContext>();

                Assert.AreEqual(2, context.Errors.Count, "Exactly 2 exceptions should be provided by the PipelineContext for this test.");
                Assert.IsInstanceOfType(context.Errors[0], expectedFirstException);
                Assert.IsInstanceOfType(context.Errors[1], expectedSecondException);
                Assert.AreEqual(expectedMessage, context.Errors[1].Message);
            }
            finally
            {
                // cleanup
                await host.StopAsync().ConfigureAwait(false);
            }
        }

  
        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_ApplicationLifetime_Parameter_Is_Null()
        {
            // arrange
            ILogger<PipelineHostedService> logger = new Mock<ILogger<PipelineHostedService>>().Object;
            IHostApplicationLifetime applicationLifetime = null;
            IStepActivator stepActivator = new Mock<IStepActivator>().Object;
            PipelineBuilder pipelineBuilder = new PipelineBuilder();
            PipelineContext context = new PipelineContext();

            string expectedParamName = nameof(applicationLifetime);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new PipelineHostedService(logger, applicationLifetime, stepActivator, pipelineBuilder, context));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Logger_Parameter_Is_Null()
        {
            // arrange
            ILogger<PipelineHostedService> logger = null;
            IHostApplicationLifetime applicationLifetime = new Mock<IHostApplicationLifetime>().Object;
            IStepActivator stepActivator = new Mock<IStepActivator>().Object;
            PipelineBuilder pipelineBuilder = new PipelineBuilder();
            PipelineContext context = new PipelineContext();

            string expectedParamName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new PipelineHostedService(logger, applicationLifetime, stepActivator, pipelineBuilder, context));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_PipelineBuilders_Parameter_Is_Null()
        {
            // arrange
            ILogger<PipelineHostedService> logger = new Mock<ILogger<PipelineHostedService>>().Object;
            IHostApplicationLifetime applicationLifetime = new Mock<IHostApplicationLifetime>().Object;
            IStepActivator stepActivator = new Mock<IStepActivator>().Object;
            PipelineBuilder pipelineBuilder = null;
            PipelineContext context = new PipelineContext();

            string expectedParamName = nameof(pipelineBuilder);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new PipelineHostedService(logger, applicationLifetime, stepActivator, pipelineBuilder, context));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_PipelineContext_Parameter_Is_Null()
        {
            // arrange
            ILogger<PipelineHostedService> logger = new Mock<ILogger<PipelineHostedService>>().Object;
            IHostApplicationLifetime applicationLifetime = new Mock<IHostApplicationLifetime>().Object;
            IStepActivator stepActivator = new Mock<IStepActivator>().Object;
            PipelineBuilder pipelineBuilder = new PipelineBuilder();
            PipelineContext context = null;

            string expectedParamName = nameof(context);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new PipelineHostedService(logger, applicationLifetime, stepActivator, pipelineBuilder, context));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_StepActivator_Parameter_Is_Null()
        {
            // arrange
            ILogger<PipelineHostedService> logger = new Mock<ILogger<PipelineHostedService>>().Object;
            IHostApplicationLifetime applicationLifetime = new Mock<IHostApplicationLifetime>().Object;
            IStepActivator stepActivator = null;
            PipelineBuilder pipelineBuilder = new PipelineBuilder();
            PipelineContext context = new PipelineContext();

            string expectedParamName = nameof(stepActivator);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new PipelineHostedService(logger, applicationLifetime, stepActivator, pipelineBuilder, context));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }
    }
}
