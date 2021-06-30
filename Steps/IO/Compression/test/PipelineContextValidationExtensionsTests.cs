// <copyright file="PipelineContextValidationExtensionsTests.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.IO.Compression.Tests
{
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines.Core;
    using MyTrout.Pipelines.Steps.IO.Compression;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Reflection;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage]
    [TestClass]
    public class PipelineContextValidationExtensionsTests
    {
        [TestMethod]
        public void Returns_From_AssertZipArchiveIsReadable_When_Value_Is_Valid()
        {
            // arrange
            string key = CompressionConstants.ZIP_ARCHIVE;
            string zipFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}Disney.zip";

            using (var zipStream = new MemoryStream())
            {
                using (var inputStream = File.OpenRead(zipFilePath))
                {
                    inputStream.CopyTo(zipStream);
                }

                zipStream.Position = 0;

                using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read, leaveOpen: true))
                {
                    var source = new PipelineContext();
                    source.Items.Add(key, zipArchive);

                    // act
                    source.AssertZipArchiveIsReadable(key);

                    // assert - No assertions are necessary.  If no exception is thrown, the parameter is not null.
                }
            }
        }

        [TestMethod]
        public void Returns_From_AssertZipArchiveIsUpdatable_When_Value_Is_Valid()
        {
            // arrange
            string key = CompressionConstants.ZIP_ARCHIVE;
            string zipFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}Disney.zip";

            using (var zipStream = new MemoryStream())
            {
                using (var inputStream = File.OpenRead(zipFilePath))
                {
                    inputStream.CopyTo(zipStream);
                }

                zipStream.Position = 0;

                using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Update, leaveOpen: true))
                {
                    var source = new PipelineContext();
                    source.Items.Add(key, zipArchive);

                    // act
                    source.AssertZipArchiveIsUpdatable(key);

                    // assert - No assertions are necessary.  If no exception is thrown, the parameter is not null.
                }
            }
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_AssertZipArchiveIsReadable_When_Context_Is_Null()
        {
            // arrange
            PipelineContext source = null;
            string key = "key";
            string expectedParamName = nameof(source);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => source.AssertZipArchiveIsReadable(key));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_AssertZipArchiveIsReadable_When_Key_Is_Null()
        {
            // arrange
            var source = new PipelineContext();
            string key = null;
            string expectedParamName = nameof(key);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => source.AssertZipArchiveIsReadable(key));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_AssertZipArchiveIsUpdatable_When_Context_Is_Null()
        {
            // arrange
            PipelineContext source = null;
            string key = "key";
            string expectedParamName = nameof(source);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => source.AssertZipArchiveIsUpdatable(key));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_AssertZipArchiveIsUpdatable_When_Key_Is_Null()
        {
            // arrange
            var source = new PipelineContext();
            string key = null;
            string expectedParamName = nameof(key);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => source.AssertZipArchiveIsUpdatable(key));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AssertZipArchiveIsReadable_When_ZipArchive_Is_Null()
        {
            // arrange
            string key = CompressionConstants.ZIP_ARCHIVE;
            ZipArchive zipArchive = null;

            var source = new PipelineContext();
            source.Items.Add(key, zipArchive);

            string expectedErrorMessage = Pipelines.Resources.CONTEXT_VALUE_IS_NULL(CultureInfo.CurrentCulture, key);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => source.AssertZipArchiveIsReadable(key));

            // assert
            Assert.IsInstanceOfType(result, typeof(InvalidOperationException));
            Assert.AreEqual(expectedErrorMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AssertZipArchiveIsUpdatable_When_ZipArchive_Is_Null()
        {
            // arrange
            string key = CompressionConstants.ZIP_ARCHIVE;
            ZipArchive zipArchive = null;

            var source = new PipelineContext();
            source.Items.Add(key, zipArchive);

            string expectedErrorMessage = Pipelines.Resources.CONTEXT_VALUE_IS_NULL(CultureInfo.CurrentCulture, key);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => source.AssertZipArchiveIsUpdatable(key));

            // assert
            Assert.IsInstanceOfType(result, typeof(InvalidOperationException));
            Assert.AreEqual(expectedErrorMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AssertZipArchiveIsReadable_When_ZipArchive_Is_Not_Readable()
        {
            // arrange
            string key = CompressionConstants.ZIP_ARCHIVE;

            using (var zipStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
                {
                    var source = new PipelineContext();
                    source.Items.Add(key, zipArchive);

                    string expectedErrorMessage = Resources.ZIP_ARCHIVE_IS_NOT_READABLE(CultureInfo.CurrentCulture);

                    // act
                    var result = Assert.ThrowsException<InvalidOperationException>(() => source.AssertZipArchiveIsReadable(key));

                    // assert
                    Assert.IsInstanceOfType(result, typeof(InvalidOperationException));
                    Assert.AreEqual(expectedErrorMessage, result.Message);
                }
            }
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_AssertZipArchiveIsUpdatable_When_ZipArchive_Is_Read_Only()
        {
            // arrange
            string key = CompressionConstants.ZIP_ARCHIVE;
            string zipFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}Disney.zip";

            using (var zipStream = File.OpenRead(zipFilePath))
            {
                using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read, leaveOpen: true))
                {
                    var source = new PipelineContext();
                    source.Items.Add(key, zipArchive);

                    string expectedErrorMessage = Resources.ZIP_ARCHIVE_IS_READ_ONLY(CultureInfo.CurrentCulture);

                    // act
                    var result = Assert.ThrowsException<InvalidOperationException>(() => source.AssertZipArchiveIsUpdatable(key));

                    // assert
                    Assert.IsInstanceOfType(result, typeof(InvalidOperationException));
                    Assert.AreEqual(expectedErrorMessage, result.Message);
                }
            }
        }
    }
}
