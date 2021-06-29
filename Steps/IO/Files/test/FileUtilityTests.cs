// <copyright file="FileUtilityTests.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2019-2021 Chris Trout
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
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [ExcludeFromCodeCoverage]
    [TestClass]
    public class FileUtilityTests
    {
        // Removes ~/Pipelines/Steps/IO/Files/test/bin/Release/net5.0 from the path.
        public static readonly string RootPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).Parent.Parent.Parent.Parent.Parent.Parent.Parent.FullName;

        [TestMethod]
        public void Returns_Fully_Qualified_Path_From_GetFullyQualifiedPath_When_Relative_FilePath_Is_Provided()
        {
            // arrange
            string filePath = $"location{Path.DirectorySeparatorChar}filename.txt";
            string basePath = "C:\\FirstDirectory";
            string expectedFilePath = "C:\\FirstDirectory\\location\\filename.txt";

            // All other supported OSPlatforms are a flavor of Linux: FreeBSD, Linux, and OSX.
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                basePath = FileUtilityTests.RootPath;
                expectedFilePath = $"{FileUtilityTests.RootPath}location/filename.txt";
            }

            // act
            var result = FileUtility.GetFullyQualifiedPath(filePath, basePath);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedFilePath, result);
        }

        [TestMethod]
        public void Returns_Fully_Qualified_Path_From_GetFullyQualifiedPath_When_BasePath_Includes_Ending_Directory_Separator_Character()
        {
            string filePath = $"location{Path.DirectorySeparatorChar}filename.txt";
            string basePath = "C:\\FirstDirectory";
            string expectedFilePath = "C:\\FirstDirectory\\location\\filename.txt";

            // All other supported OSPlatforms are a flavor of Linux: FreeBSD, Linux, and OSX.
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                basePath = FileUtilityTests.RootPath + Path.DirectorySeparatorChar;
                expectedFilePath = $"{FileUtilityTests.RootPath}location/filename.txt";
            }

            // act
            var result = FileUtility.GetFullyQualifiedPath(filePath, basePath);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedFilePath, result);
        }

        [TestMethod]
        public void Returns_Fully_Qualified_Path_From_GetFullyQualifiedPath_When_Fully_Qualified_FilePath_Is_Provided()
        {
            // arrange
            string filePath = "C:\\FirstDirectory\\location\\filename.txt";
            string basePath = "C:\\FirstDirectory";
            string expectedFilePath = "C:\\FirstDirectory\\location\\filename.txt";

            // All other supported OSPlatforms are a flavor of Linux: FreeBSD, Linux, and OSX.
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                basePath = FileUtilityTests.RootPath + Path.DirectorySeparatorChar;
                filePath = $"{FileUtilityTests.RootPath}location/filename.txt";
                expectedFilePath = $"{FileUtilityTests.RootPath}location/filename.txt";
            }

            // act
            var result = FileUtility.GetFullyQualifiedPath(filePath, basePath);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedFilePath, result);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_GetFullyQualifiedPath_When_BasePath_Is_Null()
        {
            // arrange
            string filePath = "filename.txt";
            string basePath = null;

            string expectedParamName = nameof(basePath);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => FileUtility.GetFullyQualifiedPath(filePath, basePath));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_GetFullyQualifiedPath_When_BasePath_Is_Empty()
        {
            // arrange
            string filePath = "filename.txt";
            string basePath = string.Empty;

            string expectedParamName = nameof(basePath);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => FileUtility.GetFullyQualifiedPath(filePath, basePath));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_GetFullyQualifiedPath_When_BasePath_Is_WhiteSpace()
        {
            // arrange
            string filePath = "filename.txt";
            string basePath = "   \r\n";

            string expectedParamName = nameof(basePath);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => FileUtility.GetFullyQualifiedPath(filePath, basePath));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_GetFullyQualifiedPath_When_FilePath_Is_Null()
        {
            // arrange
            string filePath = null;
            string basePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";

            string expectedParamName = nameof(filePath);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => FileUtility.GetFullyQualifiedPath(filePath, basePath));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_GetFullyQualifiedPath_When_FilePath_Is_Empty()
        {
            // arrange
            string filePath = string.Empty;
            string basePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";

            string expectedParamName = nameof(filePath);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => FileUtility.GetFullyQualifiedPath(filePath, basePath));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_GetFullyQualifiedPath_When_FilePath_Is_WhiteSpace()
        {
            // arrange
            string filePath = "   \r\n";
            string basePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";

            string expectedParamName = nameof(filePath);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => FileUtility.GetFullyQualifiedPath(filePath, basePath));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }
    }
}
