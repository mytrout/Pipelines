// <copyright file="ReadMessageFromAzureStepTests.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.Azure.ServiceBus.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class ReadMessageFromStepOptionsTests
    {
        [TestMethod]
        public void Constructs_ReadMessageFromStepOptions_Successfully()
        {
            // arrange
            List<string> applicationProperties = new List<string>() { "UserId", "MessageId" };
            string azureServiceBusConnectionString = "ConnectionString";
            int batchSize = 100;
            int deliveryAttemptsBeforeDeadLetter = 2;
            string entityPath = "topic/subscription";
            string messagePrefix = "CHANGED_";
            TimeSpan timeToWaitBetweenMessageChecks = new TimeSpan(0, 0, 1);
            TimeSpan timeToWaitForNewMessage = new TimeSpan(0, 1, 0);


            // act
            var result = new ReadMessageFromAzureOptions
            {
                AzureServiceBusConnectionString = azureServiceBusConnectionString,
                BatchSize = batchSize,
                DeliveryAttemptsBeforeDeadLetter = deliveryAttemptsBeforeDeadLetter,
                EntityPath = entityPath,
                MessageContextItemsPrefix = messagePrefix,
                TimeToWaitBetweenMessageChecks = timeToWaitBetweenMessageChecks,
                TimeToWaitForNewMessage = timeToWaitForNewMessage
            };

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(azureServiceBusConnectionString, result.AzureServiceBusConnectionString);
            Assert.AreEqual(batchSize, result.BatchSize);
            Assert.AreEqual(deliveryAttemptsBeforeDeadLetter, result.DeliveryAttemptsBeforeDeadLetter);
            Assert.AreEqual(entityPath, result.EntityPath);
            Assert.AreEqual(messagePrefix, result.MessageContextItemsPrefix);
            Assert.AreEqual(timeToWaitBetweenMessageChecks, result.TimeToWaitBetweenMessageChecks);
            Assert.AreEqual(timeToWaitForNewMessage, result.TimeToWaitForNewMessage);

            Assert.AreEqual(azureServiceBusConnectionString, result.RetrieveConnectionString());
        }
    }
}
