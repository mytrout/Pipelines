// <copyright file="PipelineContextValidationExtensions.cs" company="Chris Trout">
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
    using System.IO;

    /// <summary>
    /// Provides standardized validation of values in the <see cref="PipelineContext"/>.
    /// </summary>
    public static class PipelineContextValidationExtensions
    {
        /// <summary>
        /// Asserts the <see cref="PipelineContext"/> contains a string for <paramref name="key"/> and that the value is not <see langword="null"/>, empty or whitespace.
        /// </summary>
        /// <param name="source">The <see cref="PipelineContext"/> being tested.</param>
        /// <param name="key">The name of the <see cref="Stream"/> value to test.</param>
        public static void AssertStringIsNotWhiteSpace(this IPipelineContext source, string key)
        {
            source.AssertValueIsValid<string>(key);

            if (string.IsNullOrWhiteSpace(source.Items[key] as string))
            {
                throw new InvalidOperationException(Resources.CONTEXT_VALUE_IS_WHITESPACE(CultureInfo.CurrentCulture, key));
            }
        }

        /// <summary>
        /// Asserts that the <see cref="PipelineContext"/> contains an item for <paramref name="key"/> and that the value can be cast to <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">The <see cref="Type"/> of the value to be tested.</typeparam>
        /// <param name="source">The <see cref="PipelineContext"/> being tested.</param>
        /// <param name="key">The name of the <see cref="Stream"/> value to test.</param>
        public static void AssertValueIsValid<TValue>(this IPipelineContext source, string key)
        {
            source.AssertParameterIsNotNull(nameof(source));
            key.AssertParameterIsNotWhiteSpace(nameof(key));

            if (!source.Items.TryGetValue(key, out object workingItem))
            {
                throw new InvalidOperationException(Resources.NO_KEY_IN_CONTEXT(CultureInfo.CurrentCulture, key));
            }
            else
            {
                try
                {
                    TValue typedValue = (TValue)workingItem;

                    if (typedValue == null)
                    {
                        throw new InvalidOperationException(Resources.CONTEXT_VALUE_IS_NULL(CultureInfo.CurrentCulture, key));
                    }
                }
                catch (InvalidCastException)
                {
                    // The InvalidCastException means the object is of the wrong type.
                    throw new InvalidOperationException(Resources.CONTEXT_VALUE_NOT_EXPECTED_TYPE(CultureInfo.CurrentCulture, key, typeof(TValue)));
                }
            }
        }
    }
}
