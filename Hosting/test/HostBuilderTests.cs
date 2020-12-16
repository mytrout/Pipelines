// <copyright file="HostBuilderTests.cs" company="Chris Trout">
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
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class HostBuilderTests
    {
        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Key_Is_Null()
        {
            // arrange
            IHostBuilder source = Host.CreateDefaultBuilder();
            string key = null;
            var stepContext = new List<ICloneable>();

            string paramName = nameof(key);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => source.RetrieveValidStepContextType(key));

            // assert
            Assert.AreEqual(paramName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Source_Is_Null()
        {
            // arrange
            IHostBuilder source = null;
            string key = "hello";
            var stepContext = new List<ICloneable>();
            
            string paramName = nameof(source);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => source.RetrieveValidStepContextType(key));

            // assert
            Assert.AreEqual(paramName, result.ParamName);
        }

        // Tests that ensure that the Outer Dictionary's isn't a different type.
        [TestMethod]
        public void Throws_InvalidOperationException_From_RetrieveValidStepContextType_When_Outer_Dictionary_Is_Different_Type()
        {
            // arrange
            IHostBuilder builder = Host.CreateDefaultBuilder();
            var stepContext = new List<ICloneable>();

            var key = $"{AddStepDependencyExtensions.STEP_CONFIG_CONTEXT}{typeof(TestingOptions).FullName}";

            var message = Resources.CONTEXT_WRONG_TYPE(CultureInfo.CurrentCulture, typeof(TestingOptions).FullName);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => builder.RetrieveValidStepContextType(key));

            // assert
            Assert.AreEqual(message, result.Message);
        }

        // Tests that ensure that the Outer Dictionary's expected key isn't a different type.
        [TestMethod]
        public void Throws_InvalidOperationException_From_RetrieveValidStepContextType_When_Outer_Dictionary_Key_Is_Different_Type()
        {
            // arrange
            IHostBuilder builder = Host.CreateDefaultBuilder();
            var stepContext = new Dictionary<ICloneable, Dictionary<string, Func<IServiceProvider, TestingOptions>>>();

            var key = $"{AddStepDependencyExtensions.STEP_CONFIG_CONTEXT}{typeof(TestingOptions).FullName}";

            var message = Resources.CONTEXT_WRONG_TYPE(CultureInfo.CurrentCulture, typeof(TestingOptions).FullName);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => builder.RetrieveValidStepContextType(key));

            // assert
            Assert.AreEqual(message, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_RetrieveValidStepContextType_When_Expected_Func_Is_Wrong_Type()
        {
            // arrange
            IHostBuilder builder = Host.CreateDefaultBuilder();
            var stepContext = new Dictionary<Type, Dictionary<string, Dictionary<IServiceProvider, TestingOptions>>>();

            var key = $"{AddStepDependencyExtensions.STEP_CONFIG_CONTEXT}{typeof(TestingOptions).FullName}";

            var message = Resources.CONTEXT_WRONG_TYPE(CultureInfo.CurrentCulture, typeof(TestingOptions).FullName);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => builder.RetrieveValidStepContextType(key));

            // assert
            Assert.AreEqual(message, result.Message);
        }

        // Tests that ensure Inner Dictionary isn't a different type.
        [TestMethod]
        public void Throws_InvalidOperationException_From_RetrieveValidStepContextType_When_Expected_Inside_Dictionary_Is_Wrong_Type()
        {
            // arrange
            IHostBuilder builder = Host.CreateDefaultBuilder();
            var stepContext = new Dictionary<Type, List<TestingOptions>>();

            var key = $"{AddStepDependencyExtensions.STEP_CONFIG_CONTEXT}{typeof(TestingOptions).FullName}";

            var message = Resources.CONTEXT_WRONG_TYPE(CultureInfo.CurrentCulture, typeof(TestingOptions).FullName);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => builder.RetrieveValidStepContextType(key));

            // assert
            Assert.AreEqual(message, result.Message);
        }

        // Tests that ensure that the Inner Dictionary's expected string key isn't a different type.
        [TestMethod]
        public void Throws_InvalidOperationException_From_RetrieveValidStepContextType_When_Inner_Dictionary_Key_Is_Different_Type()
        {
            // arrange
            IHostBuilder builder = Host.CreateDefaultBuilder();
            var stepContext = new Dictionary<Type, Dictionary<int, Func<IServiceProvider, TestingOptions>>>();

            var key = $"{AddStepDependencyExtensions.STEP_CONFIG_CONTEXT}{typeof(TestingOptions).FullName}";

            var message = Resources.CONTEXT_WRONG_TYPE(CultureInfo.CurrentCulture, typeof(TestingOptions).FullName);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => builder.RetrieveValidStepContextType(key));

            // assert
            Assert.AreEqual(message, result.Message);
        }

        // Tests that ensure the Func's first generic parameter is IServiceProvider.
        [TestMethod]
        public void Throws_InvalidOperationException_From_RetrieveValidStepContextType_When_Func_Does_Not_Use_IServiceProvider_Parameter()
        {
            // arrange
            IHostBuilder builder = Host.CreateDefaultBuilder();
            var stepContext = new Dictionary<Type, Dictionary<string, Func<IAsyncDisposable, TestingOptions>>>();

            var key = $"{AddStepDependencyExtensions.STEP_CONFIG_CONTEXT}{typeof(TestingOptions).FullName}";

            var message = Resources.CONTEXT_WRONG_TYPE(CultureInfo.CurrentCulture, typeof(TestingOptions).FullName);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => builder.RetrieveValidStepContextType(key));

            // assert
            Assert.AreEqual(message, result.Message);
        }

        // Tests the ensures Func's return value is a reference type.
        [TestMethod]
        public void Throws_InvalidOperationException_From_RetrieveValidStepContextType_When_Func_Uses_Value_Type_Parameter()
        {
            // arrange
            IHostBuilder builder = Host.CreateDefaultBuilder();
            var stepContext = new Dictionary<Type, Dictionary<string, Func<IServiceProvider, int>>>();

            var key = $"{AddStepDependencyExtensions.STEP_CONFIG_CONTEXT}{typeof(TestingOptions).FullName}";

            var message = Resources.CONTEXT_WRONG_TYPE(CultureInfo.CurrentCulture, typeof(TestingOptions).FullName);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => builder.RetrieveValidStepContextType(key));

            // assert
            Assert.AreEqual(message, result.Message);
        }

        // Tests the key == 'string' comparison.
        [TestMethod]
        public void Throws_InvalidOperationException_From_RetrieveValidStepContextType_When_Key_Does_Not_Match()
        {
            // arrange
            IHostBuilder builder = Host.CreateDefaultBuilder();
            var stepContext = new Dictionary<Type, Dictionary<string, Func<IServiceProvider, TestingOptions>>>();

            var key = $"{AddStepDependencyExtensions.STEP_CONFIG_CONTEXT}{typeof(string).FullName}";

            var message = Resources.CONTEXT_WRONG_TYPE(CultureInfo.CurrentCulture, typeof(string).FullName);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => builder.RetrieveValidStepContextType(key));

            // assert
            Assert.AreEqual(message, result.Message);
        }
    }
}
