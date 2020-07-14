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

namespace MyTrout.Pipelines.Steps.IO.Files.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;

    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ParameterValidationExtensionsTests
    {
        [TestMethod]
        public void Returns_From_AssertFileNameParameterIsValid_When_Absolute_File_Path_Is_Valid()
        {
            // arrange
            int errorCount = 0;

            PipelineContext source = new PipelineContext();
            string validPath = "C:\\Metallica\\Nothing\\Else\\Matters\\";
            string validFileName = $"{validPath}{Guid.NewGuid()}.txt";

            source.Items.Add(FileConstants.SOURCE_FILE, validFileName);

            // act
            ParameterValidationExtensions.AssertFileNameParameterIsValid(source, FileConstants.SOURCE_FILE, validPath);

            // assert
            // On succesful test, throw the error if one exists.
            if (source.Errors.Any())
            {
                throw source.Errors[0];
            }

            Assert.AreEqual(errorCount, source.Errors.Count);
        }

        [TestMethod]
        public void Returns_From_AssertFileNameParameterIsValid_When_Relative_File_Path_Is_Valid()
        {
            // arrange
            int errorCount = 0;

            PipelineContext source = new PipelineContext();
            string validPath = "C:\\Metallica\\Nothing\\Else\\Matters\\";
            string validFileName = $"{Guid.NewGuid()}.txt";

            source.Items.Add(FileConstants.SOURCE_FILE, validFileName);

            // act
            ParameterValidationExtensions.AssertFileNameParameterIsValid(source, FileConstants.SOURCE_FILE, validPath);

            // assert
            // On succesful test, throw the error if one exists.
            if (source.Errors.Any())
            {
                throw source.Errors[0];
            }

            Assert.AreEqual(errorCount, source.Errors.Count);
        }

        [TestMethod]
        public void Returns_Pipeline_Errors_From_AssertFileNameParameterIsValid_When_FileName_Is_Not_In_Context()
        {
            // arrange
            PipelineContext source = new PipelineContext();
            string validPath = "C:\\Metallica\\Nothing\\Else\\Matters\\";
            string validFileName = $"{Guid.NewGuid()}.txt";

            string expectedMessage = Pipelines.Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, FileConstants.TARGET_FILE);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => ParameterValidationExtensions.AssertFileNameParameterIsValid(source, FileConstants.TARGET_FILE, validPath));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Returns_Pipeline_Errors_From_AssertFileNameParameterIsValid_When_FileName_Value_Is_WhiteSpace()
        {
            // arrange
            PipelineContext source = new PipelineContext();
            string validPath = "C:\\Metallica\\Nothing\\Else\\Matters\\";
            string validFileName = $"{Guid.NewGuid()}.txt";
            string invalidFileName = "\r\n\t";

            source.Items.Add(FileConstants.TARGET_FILE, invalidFileName);
            string expectedMessage = Pipelines.Resources.CONTEXT_VALUE_IS_WHITESPACE(CultureInfo.CurrentCulture, FileConstants.TARGET_FILE);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => ParameterValidationExtensions.AssertFileNameParameterIsValid(source, FileConstants.TARGET_FILE, validPath));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Returns_Pipeline_Errors_From_AssertFileNameParameterIsValid_When_Path_Traveral_Is_Discovered()
        {
            // arrange
            PipelineContext source = new PipelineContext();
            string validPath = "C:\\Metallica\\Nothing\\Else\\Matters\\";
            string validFileName = $"{Guid.NewGuid()}.txt";
            string invalidFileName = $"D:\\{validFileName}";

            source.Items.Add(FileConstants.TARGET_FILE, invalidFileName);
            string expectedMessage = Resources.PATH_TRAVERSAL_ISSUE(CultureInfo.CurrentCulture, validPath, invalidFileName);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => ParameterValidationExtensions.AssertFileNameParameterIsValid(source, FileConstants.TARGET_FILE, validPath));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }
    }
}
