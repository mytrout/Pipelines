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

namespace MyTrout.Pipelines
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Provides standardized parameter validation for any method.
    /// </summary>
    public static class ParameterValidationExtensions
    {
        /// <summary>
        /// Asserts that the provided parameter is not <see langword="null" />.
        /// </summary>
        /// <typeparam name="TParameterType">The type of parameter being tested.</typeparam>
        /// <param name="source">The parameter value to be tested.</param>
        /// <param name="parameterName">The parameter name used in the <see cref="ArgumentNullException"/>, if the <paramref name="source"/> is <see langword="null" />.</param>
        public static void AssertParameterIsNotNull<TParameterType>(this TParameterType source, string parameterName)
            where TParameterType : class
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new InvalidOperationException(Resources.PARAMETER_MUST_BE_SUPPLIED(CultureInfo.CurrentCulture));
            }

            if (source == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        /// <summary>
        /// Asserts that the provided parameter is not <see langword="null" />.
        /// </summary>
        /// <param name="source">The parameter value to be tested.</param>
        /// <param name="parameterName">The parameter name used in the <see cref="ArgumentNullException"/>, if the <paramref name="source"/> is <see langword="null" />.</param>
        public static void AssertParameterIsNotWhiteSpace(this string source, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new InvalidOperationException(Resources.PARAMETER_MUST_BE_SUPPLIED(CultureInfo.CurrentCulture));
            }

            if (string.IsNullOrWhiteSpace(source))
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }
}
