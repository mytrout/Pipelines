// <copyright file="ServiceBusReadEntity.cs" company="Chris Trout">
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
    using System;

    /// <summary>
    /// Provides an object representation of an Azure Service Bus queue or subscription identifier and easy access to the parts of the identifer.
    /// </summary>
    public class ServiceBusReadEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBusReadEntity"/> class with the specified EntityPath.
        /// </summary>
        /// <param name="entityPath">A value representing either a queue or subscription with topic name.</param>
        public ServiceBusReadEntity(string entityPath)
        {
            this.EntityPath = entityPath ?? throw new ArgumentNullException(nameof(entityPath));

            if (!string.IsNullOrWhiteSpace(this.EntityPath))
            {
                if (!this.EntityPath.Contains('/'))
                {
                    this.Kind = ServiceBusReadEntityKind.Queue;
                    this.QueueName = entityPath;
                }
                else if (this.EntityPath.IndexOf('/') == this.EntityPath.LastIndexOf('/'))
                {
                    this.Kind = ServiceBusReadEntityKind.Subscription;
                    this.SubscriptionName = this.EntityPath.Substring(startIndex: this.EntityPath.IndexOf('/') + 1);
                    this.TopicName = this.EntityPath.Substring(startIndex: 0, length: this.EntityPath.IndexOf('/'));
                }
            }
        }

        /// <summary>
        /// Gets a value representing an Azure Service Bus queue or subscription with topic name.
        /// </summary>
        public string EntityPath { get; init; }

        /// <summary>
        /// Gets the <see cref="ServiceBusReadEntityKind"/> after <see cref="EntityPath"/> has been parsed.
        /// </summary>
        public ServiceBusReadEntityKind Kind { get; init; } = ServiceBusReadEntityKind.Unknown;

        /// <summary>
        /// Gets the Azure Service Bus queue name, if <see cref="Kind"/> is <see cref="ServiceBusReadEntityKind.Queue"/>.
        /// </summary>
        public string QueueName { get; init; } = string.Empty;

        /// <summary>
        /// Gets the Azure Service Bus subscription name, if <see cref="Kind"/> is <see cref="ServiceBusReadEntityKind.Subscription"/>.
        /// </summary>
        public string SubscriptionName { get; init; } = string.Empty;

        /// <summary>
        /// Gets the Azure Service Bus topic name, if <see cref="Kind"/> is <see cref="ServiceBusReadEntityKind.Subscription"/>.
        /// </summary>
        public string TopicName { get; init; } = string.Empty;
    }
}
