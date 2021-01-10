﻿// <copyright file="ReadStreamFromBlobStorageOptionsTests.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.Azure.Blobs.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class ReadStreamFromBlobStorageOptionsTests
    {
        // NOTE TO FUTURE DEVELOPERS: The RetrieveConnectionStringAsync property setter is covered by the constructor.
        [TestMethod]
        public void Constructs_ReadStreamFromBlobStorageOptions_Successfully()
        {
            // arrange
            var readBlobStorageConnectionString = "connectionString";

            // act
            var result = new ReadStreamFromBlobStorageOptions
            {
                ReadBlobStorageConnectionString = readBlobStorageConnectionString
            };

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(readBlobStorageConnectionString, result.ReadBlobStorageConnectionString);
            Assert.AreEqual(readBlobStorageConnectionString, result.RetrieveConnectionStringAsync().Result);
        }
    }
}