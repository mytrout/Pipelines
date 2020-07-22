// <copyright file="StepWithContextTests.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class StepWithContextTests
    {
        [TestMethod]
        public void Constructs_StepWithContext_Successfully()
        {
            // arrange
            Type expectedStepType = typeof(NoOpStep);
            string expectedStepContext = "context";

            // act
            var result = new StepWithContext(expectedStepType, expectedStepContext);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedStepType, result.StepType);
            Assert.AreEqual(expectedStepContext, result.StepContext);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_StepType_Is_Null()
        {
            // arrange
            Type expectedStepType = null;
            string expectedStepContext = "context";

            string expectedParamName = "stepType";

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new StepWithContext(expectedStepType, expectedStepContext));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }
    }
}
