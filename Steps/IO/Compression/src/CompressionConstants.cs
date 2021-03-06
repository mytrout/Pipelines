﻿// <copyright file="CompressionConstants.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.IO.Compression
{
    /// <summary>
    /// Constants defined for <see cref="MyTrout.Pipelines.Core.PipelineContext" /> items for Compression-related steps.
    /// </summary>
    public static class CompressionConstants
    {
        /// <summary>
        /// Indicates that this <see cref="MyTrout.Pipelines.Core.PipelineContext" /> item is a <see cref="System.IO.Compression.ZipArchive" />.
        /// </summary>
        public const string ZIP_ARCHIVE = "COMPRESSION_ZIP_ARCHIVE";

        /// <summary>
        /// Inidicaltes that this <see cref="MyTrout.Pipelines.Core.PipelineContext"/> item is the name of a <see cref="System.IO.Compression.ZipArchiveEntry"/>.
        /// </summary>
        public const string ZIP_ARCHIVE_ENTRY_NAME = "COMPRESSION_ZIP_ARCHIVE_ENTRY_NAME";
    }
}
