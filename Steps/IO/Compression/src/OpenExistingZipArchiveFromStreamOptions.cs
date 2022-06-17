﻿// <copyright file="OpenExistingZipArchiveFromStreamOptions.cs" company="Chris Trout">
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
    using System.IO.Compression;

    /// <summary>
    /// Provides the user-configurable options for the <see cref="OpenExistingZipArchiveFromStreamStep"/> step.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class OpenExistingZipArchiveFromStreamOptions
    {
        /// <summary>
        /// Gets or sets the name used for the reading from the input stream from <see cref="IPipelineContext.Items"/>.
        /// </summary>
        public string InputStreamContextName { get; set; } = PipelineContextConstants.INPUT_STREAM;

        /// <summary>
        /// Gets or sets the name used for loading the zip archive from <see cref="IPipelineContext.Items"/>.
        /// </summary>
        public string ZipArchiveContextName { get; set; } = CompressionConstants.ZIP_ARCHIVE;

        /// <summary>
        /// Gets or sets the mode under which the <see cref="ZipArchive"/> will operate.
        /// </summary>
        public ZipArchiveMode ZipArchiveMode { get; set; } = ZipArchiveMode.Read;
    }
}
