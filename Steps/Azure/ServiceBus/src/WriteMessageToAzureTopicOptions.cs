// <copyright file="WriteMessageToAzureTopicOptions.cs" company="Chris Trout">
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
    using System.Collections.Generic;

    /// <summary>
    /// Provides user-configurable options for the <see cref="WriteMessageToAzureTopicStep" /> step.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class WriteMessageToAzureTopicOptions
    {
        /// <summary>
        /// Gets or sets the connection string for Azure Service Bus.
        /// </summary>
        public string AzureServiceBusConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Topic Name to which the message will be written.
        /// </summary>
        public string TopicName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets UserProperty names that will be added to the Message from <see cref="PipelineContext" />, if they are available.
        /// </summary>
        public IEnumerable<string> UserProperties { get; set; } = new List<string>();
    }
}
