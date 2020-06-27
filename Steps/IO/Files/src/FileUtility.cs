// <copyright file="FileUtility.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.IO.Files
{
    using System;
    using System.IO;

    /// <summary>
    /// Provides utility methods that are used throughout the file-based steps.
    /// </summary>
    public static class FileUtility
    {
        /// <summary>
        /// Gets a fully-qualified path and ensures that the path exists within the <paramref name="basePath"/> to prevent path traversal issues.
        /// </summary>
        /// <param name="filePath">A fully-qualified or relative path.</param>
        /// <param name="basePath">A fully-qualified path.</param>
        /// <returns>A fully-qualified path that has the same root path as <paramref name="basePath"/>.</returns>
        public static string GetFullyQualifiedPath(this string filePath, string basePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (string.IsNullOrWhiteSpace(basePath))
            {
                throw new ArgumentNullException(nameof(basePath));
            }

            string result = filePath;

            if (!basePath.EndsWith(Path.DirectorySeparatorChar))
            {
                basePath = basePath + Path.DirectorySeparatorChar;
            }

            if (!Path.IsPathFullyQualified(result))
            {
                result = Path.Combine(basePath, result);
            }

            result = Path.GetFullPath(result);

            return result;
        }
    }
}
