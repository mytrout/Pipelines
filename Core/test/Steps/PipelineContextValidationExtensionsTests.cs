// <copyright file="PipelineContextValidationExtensionsTests.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2019-2022 Chris Trout
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
    using MyTrout.Pipelines.Core;
    using System;
    using System.Globalization;
    using System.IO;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class PipelineContextValidationExtensionsTests
    {
        [TestMethod]
        public void Returns_From_AssertStringIsNotWhiteSpace_When_ExpectedValue_Is_Valid()
        {
            // arrange
            PipelineContext context = new PipelineContext();
            string expectedValue = "Non-null value";
            string key = "key";

            context.Items.Add(key, expectedValue);

            // act
            context.AssertStringIsNotWhiteSpace(key);

            // assert - No assertions are necessary.  If no exception is thrown, the parameter is not null.
        }

        [TestMethod]
        public void Returns_From_AssertValueExists_When_ExpectedValue_Exists()
        {
            // arrange
            PipelineContext context = new PipelineContext();
            using (Stream expectedValue = new MemoryStream())
            {
                string key = "key";

                context.Items.Add(key, expectedValue);

                // act
                context.AssertValueExists(key);

                // assert - No assertions are necessary.  If no exception is thrown, the parameter is not null.
            }
        }

        [TestMethod]
        public void Returns_From_AssertValueIsValid_When_ExpectedValue_Is_Valid()
        {
            // arrange
            PipelineContext context = new PipelineContext();
            using (Stream expectedValue = new MemoryStream())
            {
                string key = "key";

                context.Items.Add(key, expectedValue);

                // act
                context.AssertValueIsValid<Stream>(key);

                // assert - No assertions are necessary.  If no exception is thrown, the parameter is not null.
            }
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_AssertStringIsNotWhiteSpace_When_Context_Is_Null()
        {
            // arrange
            PipelineContext source = null;
            string key = "key";
            string expectedParamName = nameof(source);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => source.AssertStringIsNotWhiteSpace(key));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AssertStringIsNotWhiteSpace_When_Context_Value_Is_Empty()
        {
            // arrange
            PipelineContext source = new PipelineContext();
            string key = "key";
            string value = string.Empty;

            source.Items.Add(key, value);

            string expectedMessage = Resources.CONTEXT_VALUE_IS_WHITESPACE(CultureInfo.CurrentCulture, key);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => source.AssertStringIsNotWhiteSpace(key));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AssertStringIsNotWhiteSpace_When_Context_Value_Is_Not_A_String()
        {
            // arrange
            PipelineContext source = new PipelineContext();
            string key = "key";
            int expectedValue = 1;

            source.Items.Add(key, expectedValue);

            string expectedMessage = Resources.CONTEXT_VALUE_NOT_EXPECTED_TYPE(CultureInfo.CurrentCulture, key, typeof(string));

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => source.AssertStringIsNotWhiteSpace(key));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AssertStringIsNotWhiteSpace_When_Context_Value_Is_Null()
        {
            // arrange
            PipelineContext source = new PipelineContext();
            string key = "key";
            string value = null;

            source.Items.Add(key, value);

            string expectedMessage = Resources.CONTEXT_VALUE_IS_NULL(CultureInfo.CurrentCulture, key);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => source.AssertStringIsNotWhiteSpace(key));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AssertStringIsNotWhiteSpace_When_Context_Value_Is_WhiteSpace()
        {
            // arrange
            PipelineContext source = new PipelineContext();
            string key = "key";
            string value = "\r\n\t";

            source.Items.Add(key, value);

            string expectedMessage = Resources.CONTEXT_VALUE_IS_WHITESPACE(CultureInfo.CurrentCulture, key);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => source.AssertStringIsNotWhiteSpace(key));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AssertStringIsNotWhiteSpace_When_Key_Does_Not_Exist_In_Context()
        {
            // arrange
            PipelineContext source = new PipelineContext();
            string key = "key";
            string expectedMessage = Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, key);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => source.AssertStringIsNotWhiteSpace(key));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_AssertValueIsValid_When_Context_Is_Null()
        {
            // arrange
            PipelineContext source = null;
            string key = "key";
            string expectedParamName = nameof(source);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => source.AssertValueIsValid<string>(key));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AssertValueExists_When_Key_Does_Not_Exist_In_Context()
        {
            // arrange
            PipelineContext source = new PipelineContext();
            string key = "key";
            string expectedMessage = Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, key);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => source.AssertValueExists(key));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AssertValueIsValid_When_Context_Value_Is_Not_Correct_Type()
        {
            // arrange
            PipelineContext source = new PipelineContext();
            string key = "key";
            using (Stream expectedValue = new MemoryStream())
            {
                source.Items.Add(key, expectedValue);

                string expectedMessage = Resources.CONTEXT_VALUE_NOT_EXPECTED_TYPE(CultureInfo.CurrentCulture, key, typeof(Exception));

                // act
                var result = Assert.ThrowsException<InvalidOperationException>(() => source.AssertValueIsValid<Exception>(key));

                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedMessage, result.Message);
            }
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AssertValueIsValid_When_Context_Value_Is_Null()
        {
            // arrange
            PipelineContext source = new PipelineContext();
            string key = "key";
            Stream expectedValue = null;

            source.Items.Add(key, expectedValue);

            string expectedMessage = Resources.CONTEXT_VALUE_IS_NULL(CultureInfo.CurrentCulture, key);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => source.AssertValueIsValid<Stream>(key));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AssertValueIsValid_When_Key_Does_Not_Exist_In_Context()
        {
            // arrange
            PipelineContext source = new PipelineContext();
            string key = "key";
            string expectedMessage = Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, key);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => source.AssertValueIsValid<string>(key));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }
    }
}
