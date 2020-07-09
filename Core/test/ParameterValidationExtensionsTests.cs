// <copyright file="ParameterValidationExtensionsTests.cs" company="Chris Trout">
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
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ParameterValidationExtensionsTests
    {
        [TestMethod]
        public void Returns_From_AssertParameterIsNotNull_When_Source_Is_Not_Null()
        {
            // arrange
            string expectedValue = "Non-null value";
            string parameterName = "paramName";

            // act
            expectedValue.AssertParameterIsNotNull<string>(parameterName);

            // assert - No assertions are necessary.  If no exception is thrown, the parameter is not null.
        }

        [TestMethod]
        public void Returns_From_AssertParameterIsNotWhiteSpace_When_Source_Is_Not_WhiteSpace()
        {
            // arrange
            string expectedValue = "Non-null value";
            string parameterName = "paramName";

            // act
            expectedValue.AssertParameterIsNotWhiteSpace(parameterName);

            // assert - No assertions are necessary.  If no exception is thrown, the parameter is not whitespace
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_AssertParameterIsNotNull_When_Source_Is_Null()
        {
            // arrange
            string expectedValue = null;
            string parameterName = "paramName";

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => expectedValue.AssertParameterIsNotNull<string>(parameterName));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(parameterName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_AssertParameterIsNotWhiteSpace_When_Source_Is_Null()
        {
            // arrange
            string expectedValue = null;
            string parameterName = "paramName";

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => expectedValue.AssertParameterIsNotWhiteSpace(parameterName));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(parameterName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_AssertParameterIsNotWhiteSpace_When_Source_Is_Empty()
        {
            // arrange
            string expectedValue = string.Empty;
            string parameterName = "paramName";

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => expectedValue.AssertParameterIsNotWhiteSpace(parameterName));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(parameterName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_AssertParameterIsNotWhiteSpace_When_Source_Is_WhiteSpace()
        {
            // arrange
            string expectedValue = "\r\n\t";
            string parameterName = "paramName";

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => expectedValue.AssertParameterIsNotWhiteSpace(parameterName));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(parameterName, result.ParamName);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AssertParameterIsNotNull_When_ParameterName_Is_Null()
        {
            // arrange
            string expectedValue = "Non-null value";
            string parameterName = null;

            string expectedMessage = "parameterName must be supplied in AssertParameterIsNotNull.";

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => expectedValue.AssertParameterIsNotNull<string>(parameterName));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AssertParameterIsNotWhiteSpace_When_ParameterName_Is_Null()
        {
            // arrange
            string expectedValue = "Non-null value";
            string parameterName = null;

            string expectedMessage = "parameterName must be supplied in AssertParameterIsNotNull.";

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => expectedValue.AssertParameterIsNotWhiteSpace(parameterName));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }
    }
}
