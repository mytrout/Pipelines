// <copyright file="IContextNameBuilder.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines
{
    /// <summary>
    /// Provides the capability of building an <see cref="IPipelineContext.Items"/> context name for a given type name and property name.
    /// </summary>
    public interface IContextNameBuilder
    {
        /// <summary>
        /// Builds an <see cref="IPipelineContext.Items"/> context name for a given type name and property name.
        /// </summary>
        /// <param name="typeName">The <see cref="System.Type" /> from which this context name is built.</param>
        /// <param name="propertyName">The property from which this context name is built.</param>
        /// <returns>A context name for use in <see cref="IPipelineContext.Items"/>.</returns>
        string BuildContextName(string typeName, string propertyName);
    }
}
