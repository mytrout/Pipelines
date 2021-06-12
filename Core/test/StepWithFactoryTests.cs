﻿// <copyright file="StepWithFactoryTests.cs" company="Chris Trout">
// MIT License
//
// Copyright © 2021 Chris Trout
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
    using MyTrout.Pipelines.Samples.Tests;
    using System;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class StepWithFactoryTests
    {
        [TestMethod]
        public void Constructs_StepWithFactory_Successfully()
        {
            // arrange
            var expectedStepType = typeof(SampleStep1);
            var expectedDependencyType = typeof(SampleOptions);
            string expectedStepContext = "context";

            Func<IServiceProvider, SampleOptions> expectedImplementationFactory =
                (IServiceProvider services) =>
                {
                    return new SampleOptions("string");
                };

            // act
            var result = new StepWithFactory<SampleStep1, SampleOptions>(expectedStepContext, expectedImplementationFactory);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedStepType, result.StepType);
            Assert.AreEqual(expectedStepContext, result.StepContext);
            Assert.AreEqual(expectedDependencyType, result.StepDependencyType);
            Assert.IsNull(result.ConfigKeys);
            Assert.AreEqual(expectedImplementationFactory, result.ImplementationFactory);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_ImplementationFactory_Is_Null()
        {
            // arrange
            string expectedStepContext = "context";

            Func<IServiceProvider, SampleOptions> expectedImplementationFactory = null;

            // act
            string expectedParamName = "implementationFactory";

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new StepWithFactory<SampleStep1, SampleOptions>(expectedStepContext, expectedImplementationFactory));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }
    }
}
