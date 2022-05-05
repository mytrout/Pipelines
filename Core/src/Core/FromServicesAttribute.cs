// <copyright file="FromServicesAttribute.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Core
{
    using System;

    /// <summary>
    /// Specifies that a Options property should be bound using the a service from Dependency Injection.
    /// </summary>
    /// <example>
    /// In this example an implementation of IConfiguration will be injected into this SpecialOptions class from Dependency Injection
    /// while the ConnectionString property will be set during object construction.
    /// <code>
    /// public class SpecialOptions
    /// {
    ///     public string ConnectionString { get; set; }
    ///
    ///     [FromServices]
    ///     public IConfiguration Configuration { get; set; }
    /// }
    /// </code>
    /// </example>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class FromServicesAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FromServicesAttribute"/> class.
        /// </summary>
        public FromServicesAttribute()
        {
            // no op
        }
    }
}
