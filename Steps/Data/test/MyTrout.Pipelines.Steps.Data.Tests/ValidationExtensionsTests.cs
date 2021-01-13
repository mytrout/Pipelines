// <copyright file="ValidationExtensionsTests.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2020-2021 Chris Trout
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


namespace MyTrout.Pipelines.Steps.Data.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class ValidationExtensionsTests
    {
        [TestMethod]
        public void Returns_Void_From_AssertValueIsNotNull_When_Value_Is_Not_Null()
        {
            // arrange
            string valueToTest = "Something";
            string expectedMessage = "Not valid.";

            Func<string> exceptionMessage = () => { return expectedMessage; };

            // act
            valueToTest.AssertValueIsNotNull(exceptionMessage);

            // assert - no exception means this method executed successfully.
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_AssertValueIsNotNull_When_ExceptionMessage_Parameter_Is_Null()
        {
            // arrange
            string valueToTest = "Something";
            Func<string> exceptionMessage = null;

            string paramName = nameof(exceptionMessage);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => valueToTest.AssertValueIsNotNull(exceptionMessage));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ParamName, paramName);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AssertValueIsNotNull_When_Value_Is_Null()
        {
            // arrange
            string valueToTest = null;
            string expectedMessage = "Not valid.";

            Func<string> exceptionMessage = () => { return expectedMessage; };

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => valueToTest.AssertValueIsNotNull(exceptionMessage));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Message, expectedMessage);
        }
    }
}
