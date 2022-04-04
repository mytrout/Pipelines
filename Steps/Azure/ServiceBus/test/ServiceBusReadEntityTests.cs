// <copyright file="ReadMessageFromAzureStepTests.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2019-2022 Chris Trout
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

namespace MyTrout.Pipelines.Steps.Azure.ServiceBus.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class ServiceBusReadEntityTests
    {
        [TestMethod]
        public void Constructs_ServiceBusReadEntity_With_Subscription_Successfully()
        {
            // arrange
            string entityPath = "topic/subscription";
            string topicName = "topic";
            string subscriptionName = "subscription";
            var kind = ServiceBusReadEntityKind.Subscription;
            
            // act
            var result = new ServiceBusReadEntity(entityPath);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(entityPath, result.EntityPath);
            Assert.AreEqual(kind, result.Kind);
            Assert.AreEqual(topicName, result.TopicName);
            Assert.AreEqual(subscriptionName, result.SubscriptionName);
        }

        [TestMethod]
        public void Constructs_ServiceBusReadEntity_With_Queue_Successfully()
        {
            // arrange
            string entityPath = "queue";
            string queueName = "queue";
            var kind = ServiceBusReadEntityKind.Queue;
            
            // act
            var result = new ServiceBusReadEntity(entityPath);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(entityPath, result.EntityPath);
            Assert.AreEqual(kind, result.Kind);
            Assert.AreEqual(queueName, result.QueueName);
        }

        [TestMethod]
        public void Constructs_ServiceBusReadEntity_With_Unknown_EntityPath_Successfully()
        {
            // arrange
            string entityPath = "unknown/topic/subscription";
            var kind = ServiceBusReadEntityKind.Unknown;
            
            // act
            var result = new ServiceBusReadEntity(entityPath);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(entityPath, result.EntityPath);
            Assert.AreEqual(kind, result.Kind);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_EntityPath_Is_Null()
        {
            // arrange
            string entityPath = null;
            var paramName = nameof(entityPath);
            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new ServiceBusReadEntity(entityPath));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(paramName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_EntityPath_Is_Empty()
        {
            // arrange
            string entityPath = string.Empty;
            var paramName = nameof(entityPath);
            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new ServiceBusReadEntity(entityPath));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(paramName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_EntityPath_Is_Whitespace()
        {
            // arrange
            string entityPath = "\r\n\t";
            var paramName = nameof(entityPath);
            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new ServiceBusReadEntity(entityPath));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(paramName, result.ParamName);
        }
    }
}
