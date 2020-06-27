// <copyright file="ParameterValidationExtensions.cs" company="Chris Trout">
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
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// Provides standardized parameter validation for any file-based InvokeAsync methods.
    /// </summary>
    public static class ParameterValidationExtensions
    {
        /// <summary>
        /// Asserts that the provided parameter is not <see langword="null" />.
        /// </summary>
        /// <param name="source">The parameter value to be tested.</param>
        /// <param name="parameterName">The parameter name used in the <see cref="ArgumentNullException"/>, if the <paramref name="source"/> is <see langword="null" />.</param>
        /// <param name="baseDirectory">The base directory that limits where files are able to be read.</param>
        public static void AssertFileNameParameterIsValid(this IPipelineContext source, string parameterName, string baseDirectory)
        {
            source.AssertParameterIsNotNull(nameof(source));

            if (!source.Items.ContainsKey(parameterName))
            {
                throw new InvalidOperationException(Resources.KEY_DOES_NOT_EXIST_IN_CONTEXT(CultureInfo.CurrentCulture, parameterName));
            }
            else
            {
                string workingFile = source.Items[parameterName] as string;

                if (string.IsNullOrWhiteSpace(workingFile))
                {
                    throw new InvalidOperationException(Resources.VALUE_IS_WHITESPACE_IN_CONTEXT(CultureInfo.CurrentCulture, parameterName));
                }
                else
                {
                    if (!Path.IsPathFullyQualified(workingFile))
                    {
                        workingFile = Path.Combine(baseDirectory, workingFile);
                    }

                    workingFile = Path.GetFullPath(workingFile);

                    if (!workingFile.StartsWith(baseDirectory, StringComparison.Ordinal))
                    {
                        throw new InvalidOperationException(Resources.PATH_TRAVERSAL_ISSUE(CultureInfo.CurrentCulture, baseDirectory, workingFile));
                    }
                }
            }
        }
    }
}
