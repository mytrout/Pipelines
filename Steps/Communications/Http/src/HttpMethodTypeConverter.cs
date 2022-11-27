// <copyright file="HttpMethodTypeConverter.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2022 Chris Trout
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

namespace MyTrout.Pipelines.Steps.Communications.Http
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Net.Http;

    /// <summary>
    /// Provides a <see cref="TypeConverter"/> to handle <see cref="IConfiguration"/> for ~Options classes. 
    /// </summary>
    public class HttpMethodTypeConverter : TypeConverter
    {
        /// <summary>
        /// Provides a flag indicating whether the <paramref name="sourceType"/> can be converted from <see cref="string"/> to <see cref="HttpMethod"/>.
        /// </summary>
        /// <param name="context">The type descriptor context.</param>
        /// <param name="sourceType">The source type to be converted.</param>
        /// <returns><see langword="true"/> if the type can be converted, otherwise <see langword="false"/>.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// Provides a flag indicating whether the <paramref name="destinationType"/> can be converted from <see cref="HttpMethod"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="context">The type descriptor context.</param>
        /// <param name="destinationType">The destination type from which to be converted.</param>
        /// <returns><see langword="true"/> if the type can be converted, otherwise <see langword="false"/>.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        {
            return destinationType == typeof(HttpMethod);
        }

        /// <summary>
        /// Performs the conversion from <see cref="string"/> to <see cref="HttpMethod"/>.
        /// </summary>
        /// <param name="context">The type descriptor context.</param>
        /// <param name="culture">The culture in which the conversion will take place.</param>
        /// <param name="value">The value to be converted.</param>
        /// <returns>A valid <see cref="HttpMethod"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="value"/> is <see langword="null" /> or <paramref name="value"/> is not a <see cref="string"/>.</exception>
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value == null || value is not string)
            {
                throw new NotSupportedException();
            }

            return new HttpMethod((string)value);
        }

        /// <summary>
        /// Performs the conversion from <see cref="HttpMethod"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="context">The type descriptor context.</param>
        /// <param name="culture">The culture in which the conversion will take place.</param>
        /// <param name="value">The value to be converted.</param>
        /// <returns>A valid <see cref="string"/> representation of the <see cref="HttpMethod"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="value"/> is <see langword="null" /> or <paramref name="value"/> is not a <see cref="HttpMethod"/>.</exception>
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (value == null || value is not HttpMethod)
            {
                throw new NotSupportedException();
            }

            return ((HttpMethod)value).Method;
        }
    }
}
