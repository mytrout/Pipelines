// <copyright file="PipelineContextValidationExtensions.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2020 Chris Trout
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
    using System;
    using System.Globalization;
    using System.IO.Compression;

    /// <summary>
    /// Provides standardized validation of values in the <see cref="PipelineContext"/>.
    /// </summary>
    public static class PipelineContextValidationExtensions
    {
        /// <summary>
        /// Asserts that the <see cref="PipelineContext"/> contains a <see cref="ZipArchive"/> for <paramref name="key"/> and is readable.
        /// </summary>
        /// <param name="source">The <see cref="PipelineContext"/> being tested.</param>
        /// <param name="key">The name of the item to test.</param>
        public static void AssertZipArchiveIsReadable(this IPipelineContext source, string key)
        {
            source.AssertParameterIsNotNull(nameof(source));
            key.AssertParameterIsNotWhiteSpace(nameof(key));

            source.AssertValueIsValid<ZipArchive>(key);

            ZipArchive zipArchive = source.Items[key] as ZipArchive;

            if (zipArchive.Mode == ZipArchiveMode.Create)
            {
                throw new InvalidOperationException(Resources.ZIP_ARCHIVE_IS_NOT_READABLE(CultureInfo.CurrentCulture));
            }
        }

        /// <summary>
        /// Asserts that the <see cref="PipelineContext"/> contains a <see cref="ZipArchive"/> for <paramref name="key"/> and is updatable.
        /// </summary>
        /// <param name="source">The <see cref="PipelineContext"/> being tested.</param>
        /// <param name="key">The name of the item to test.</param>
        public static void AssertZipArchiveIsUpdatable(this IPipelineContext source, string key)
        {
            source.AssertParameterIsNotNull(nameof(source));
            key.AssertParameterIsNotWhiteSpace(nameof(key));

            source.AssertValueIsValid<ZipArchive>(key);

            ZipArchive zipArchive = source.Items[key] as ZipArchive;

            if (zipArchive.Mode == ZipArchiveMode.Read)
            {
                throw new InvalidOperationException(Resources.ZIP_ARCHIVE_IS_READ_ONLY(CultureInfo.CurrentCulture));
            }
        }
    }
}
