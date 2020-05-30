﻿// <copyright file="NoOpStepTests.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class NoOpStepTests
    {
        [TestMethod]
        public async Task Returns_Task_From_DisposeAsync()
        {
            // arrange
            var source = new NoOpStep();

            // act
            await source.DisposeAsync();

            // assert
            Assert.IsTrue(true);

            // No exceptions mean this worked appropriately.
        }

#pragma warning disable VSTHRD200 // Suppressed because the member name is the suffix of the test method name.
        [TestMethod]
        public void Returns_Task_From_InvokeAsync()
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
        {
            // arrange
            PipelineContext context = new PipelineContext();

            var step = new NoOpStep();

            // act
            var result = step.InvokeAsync(context);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(Task.CompletedTask, result);
        }

#pragma warning disable VSTHRD200 // Test method name should reflect what it is testing, not Async.
        [TestMethod]
        public async Task Throws_ArgumentNullException_From_InvokeAsync_When_Context_Is_Null()
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
        {
            // arrange
            PipelineContext context = null;
            string expectedParamName = nameof(context);

            var step = new NoOpStep();

            // act
            var result = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => step.InvokeAsync(context));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }
    }
}