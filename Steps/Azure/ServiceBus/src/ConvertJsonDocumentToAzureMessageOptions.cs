// <copyright file="ConvertJsonDocumentToAzureMessageOptions.cs" company="Chris Trout">
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
namespace MyTrout.Pipelines.Steps.Azure.ServiceBus
{
    using Microsoft.Azure.ServiceBus;
    using System.Collections.Generic;
    using System.Text.Json;

    /// <summary>
    /// Options for converting a <see cref="JsonDocument" /> to an Azure <see cref="Message" />.
    /// </summary>
    public class ConvertJsonDocumentToAzureMessageOptions
    {
        /// <summary>
        /// Gets or sets the pipeline context key to retrieve the <see cref="JsonDocument" />.
        /// </summary>
        public string JsonDocumentKey { get; set; }

        /// <summary>
        /// Gets the UserProperties that should be loaded from the <see cref="JsonDocument" /> to the <see cref="Message" />.
        /// </summary>
        public IList<string> UserPropertyNames { get; } = new List<string>();
    }
}
