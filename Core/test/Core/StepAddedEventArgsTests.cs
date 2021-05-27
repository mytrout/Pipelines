#pragma warning disable SA1515,SA1633,SA1636
// <copyright file="StepAddedEventArgsTests.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2021 Chris Trout
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
#pragma warning restore SA1515,SA1633,SA1636
namespace MyTrout.Pipelines.Steps.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MyTrout.Pipelines.Core;
    using System;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class StepAddedEventArgsTests
    {
        [TestMethod]
        public void Constructs_StepAddedEventArgs_Successfully()
        {
            // arrange
            Type expectedStepType = typeof(NoOpStep);
            string expectedStepContext = "context";
            var expectedStepWithContext = new StepWithContext(expectedStepType, expectedStepContext);

            // act
            var result = new StepAddedEventArgs(expectedStepWithContext);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedStepWithContext, result.CurrentStep);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_CurrentStep_Is_Null()
        {
            // arrange
            StepWithContext currentStep = null;

            string expectedParamName = nameof(currentStep);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new StepAddedEventArgs(currentStep));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }
    }
}
