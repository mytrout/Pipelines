// <copyright file="PipelineHostBuilderExtensionsTests.cs" company="Chris Trout">
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
    using Microsoft.Extensions.Hosting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Hosting;
    using System;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class PipelineHostBuilderExtensionsTests
    {
        [TestMethod]
        public async Task Returns_Correct_Count_Of_Executions_From_Pipeline_When_Called()
        {
            // arrange
            string[] args = Array.Empty<string>();
            PipelineBuilder[] pipelineBuilders = new PipelineBuilder[1]
            {
                new PipelineBuilder()
                        .AddStep<TestingStep1>()
                        .AddStep<TestingStep1>()
                        .AddStep<TestingStep1>()
            };

            var host = Host.CreateDefaultBuilder(args)
                            .UsePipelines(pipelineBuilders)
                            .Build();

            var expectedExecutionCount = 3;

            // act
            await host.RunAsync().ConfigureAwait(false);

            // assert
            Assert.AreEqual(expectedExecutionCount, TestingStep1.ExecutionCount);
        }

        [TestMethod]
        public async Task Returns_Correct_Count_Of_Executions_From_Pipeline_When_UsePipelines_With_Activator_Is_Called()
        {
            // arrange
            string[] args = Array.Empty<string>();
            PipelineBuilder[] pipelineBuilders = new PipelineBuilder[1]
            {
                new PipelineBuilder()
                     .AddStep<TestingStep2>()
                    .AddStep<TestingStep2>()
                    .AddStep<TestingStep2>()
            };

            var host = Host.CreateDefaultBuilder(args)
                            .UsePipelines<StepActivator>(pipelineBuilders)
                            .Build();

            var expectedExecutionCount = 3;

            // act
            await host.RunAsync().ConfigureAwait(false);

            // assert
            Assert.AreEqual(expectedExecutionCount, TestingStep2.ExecutionCount);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Pipeline_When_HostBuilder_Is_Null()
        {
            // arrange
            string[] args = Array.Empty<string>();
            PipelineBuilder[] builders = new PipelineBuilder[1]
            {
                new PipelineBuilder()
                     .AddStep<TestingStep2>()
                    .AddStep<TestingStep2>()
                    .AddStep<TestingStep2>()
            };

            IHostBuilder source = null;
            string expectedParamName = nameof(source);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => PipelineHostBuilderExtensions.UsePipelines(source, builders));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }
    }

    // Allow multiple testing classes here because there are so many options to test.
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable CA1801 // Remove unused parameter
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class TestingStep1
    {
        private readonly PipelineRequest next = null;

        public TestingStep1(PipelineRequest next) => this.next = next;

        public static int ExecutionCount { get; set; } = 0;

        public Task InvokeAsync(PipelineContext context)
        {
            Console.WriteLine($"TestingStep1 {TestingStep1.ExecutionCount}");
            TestingStep1.ExecutionCount++;
            return this.next(context);
        }
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class TestingStep2
    {
        private readonly PipelineRequest next = null;

        public TestingStep2(PipelineRequest next) => this.next = next;

        public static int ExecutionCount { get; set; } = 0;

        public Task InvokeAsync(PipelineContext context)
        {
            Console.WriteLine($"TestingStep2 {TestingStep2.ExecutionCount}");
            TestingStep2.ExecutionCount++;
            return this.next(context);
        }
    }
#pragma warning restore CA1801 // Remove unused parameter
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CA1822 // Mark members as static
#pragma warning restore SA1402 // File may only contain a single type
}