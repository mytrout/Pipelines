// <copyright file="UsePipelinesExtensionsTests.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2019-2020 Chris Trout
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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MyTrout.Pipelines.Core;
    using System;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class UsePipelinesExtensionsTests
    {
        [TestMethod]
        public async Task Returns_Correct_Count_Of_Executions_From_Pipeline_When_Called()
        {
            // arrange
            string[] args = Array.Empty<string>();
            PipelineBuilder pipelineBuilder =
                new PipelineBuilder()
                        .AddStep<TestingStep1>()
                        .AddStep<TestingStep1>()
                        .AddStep<TestingStep1>();

            var host = Host.CreateDefaultBuilder(args)
                            .UsePipeline(pipelineBuilder)
                            .Build();

            var expectedExecutionCount = 3;

            // act
            await host.StartAsync().ConfigureAwait(false);

            int actualExecutionCount = (int)host.Services.GetService<PipelineContext>().Items[TestingStep1.EXECUTION_COUNT];

            // assert
            Assert.AreEqual(expectedExecutionCount, actualExecutionCount);

        }

        [TestMethod]
        public async Task Returns_Correct_Count_Of_Executions_From_Pipeline_When_Action_Is_Called()
        {
            // arrange
            string[] args = Array.Empty<string>();

            var host = Host.CreateDefaultBuilder(args)
                            .UsePipeline(builder => 
                            {
                                builder.AddStep<TestingStep2>()
                                        .AddStep<TestingStep2>()
                                        .AddStep<TestingStep2>();
                            })
                            .Build();

            var expectedExecutionCount = 3;

            // act
            await host.StartAsync().ConfigureAwait(false);

            int actualExecutionCount = (int)host.Services.GetService<PipelineContext>().Items[TestingStep1.EXECUTION_COUNT];

            // assert
            Assert.AreEqual(expectedExecutionCount, actualExecutionCount);
        }

        [TestMethod]
        public async Task Returns_Correct_Count_Of_Executions_From_Pipeline_When_UsePipelines_With_Activator_Is_Called()
        {
            // arrange
            string[] args = Array.Empty<string>();
            PipelineBuilder pipelineBuilder = new PipelineBuilder()
                                                    .AddStep<TestingStep3>()
                                                    .AddStep<TestingStep3>()
                                                    .AddStep<TestingStep3>();
            
            var host = Host.CreateDefaultBuilder(args)
                            .UsePipeline<StepActivator>(pipelineBuilder)
                            .Build();

            var expectedExecutionCount = 3;

            // act
            await host.StartAsync().ConfigureAwait(false);

            int actualExecutionCount = (int)host.Services.GetService<PipelineContext>().Items[TestingStep1.EXECUTION_COUNT];

            // assert
            Assert.AreEqual(expectedExecutionCount, actualExecutionCount);
        }

        [TestMethod]
        public async Task Returns_Correct_Count_Of_Executions_From_Pipeline_When_UsePipelines_With_Activator_And_Action_Is_Called()
        {
            // arrange
            string[] args = Array.Empty<string>();
            
            var host = Host.CreateDefaultBuilder(args)
                                .UsePipeline<StepActivator>(builder =>
                                {
                                    builder.AddStep<TestingStep4>()
                                            .AddStep<TestingStep4>()
                                            .AddStep<TestingStep4>();
                                })
                                .Build();

            var expectedExecutionCount = 3;

            // act
            await host.StartAsync().ConfigureAwait(false);

            int actualExecutionCount = (int)host.Services.GetService<PipelineContext>().Items[TestingStep1.EXECUTION_COUNT];

            // assert
            Assert.AreEqual(expectedExecutionCount, actualExecutionCount);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Pipeline_When_HostBuilder_Is_Null()
        {
            // arrange
            PipelineBuilder builder = new PipelineBuilder()
                                            .AddStep<TestingStep2>()
                                            .AddStep<TestingStep2>()
                                            .AddStep<TestingStep2>();

            IHostBuilder source = null;
            string expectedParamName = nameof(source);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => UsePipelineExtensions.UsePipeline(source, builder));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }
    }
}