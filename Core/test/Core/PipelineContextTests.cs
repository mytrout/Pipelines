// <copyright file="PipelineContextTests.cs" company="Chris Trout">
// MIT License
//
// Copyright © 2019-2021 Chris Trout
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

namespace MyTrout.Pipelines.Core.Tests
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Threading;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class PipelineContextTests
    {
        [TestMethod]
        public void Constructs_PipelineContext_Successfully()
        {
            // arrange
            DateTimeOffset startingOffset = DateTimeOffset.UtcNow;
            const int expectedItemCount = 0;
            const int expectedExceptionCount = 0;
            Guid invalidCorrelationId = Guid.Empty;
            using (var expectedCancellationSource = new CancellationTokenSource())
            {
                // act
                var result = new PipelineContext()
                {
                    CancellationToken = expectedCancellationSource.Token
                };

                DateTimeOffset endingOffset = DateTimeOffset.UtcNow;

                // assert
                Assert.IsNotNull(result, "PipelineContext should not be null.");
                Assert.AreEqual(expectedCancellationSource.Token, result.CancellationToken, "CancellationToken should be CancellationToken.None.");
#pragma warning disable CS0618 // Type or member is obsolete - The values will be removed in the next major release.
                Assert.IsNull(result.Configuration, "Configuration should be null.");
                Assert.IsFalse(result.IsConfigurationAvailable, "IsConfigurationAvailable should be false.");
#pragma warning restore CS0618 // Type or member is obsolete
                Assert.AreNotEqual(invalidCorrelationId, result.CorrelationId, "CorrelationID cannot be Guid.Empty.");
                Assert.IsNotNull(result.Errors, "PipelineContext.Errors should not be null.");
                Assert.AreEqual(expectedExceptionCount, result.Errors.Count);
                Assert.IsNotNull(result.Items, "PipelineContext.Items should not be null.");
                Assert.AreEqual(expectedItemCount, result.Items.Count);
                Assert.IsTrue(startingOffset < result.Timestamp, "StartingOffset is greater than or equal to the PipelineContext.Timestamp.");
                Assert.IsTrue(endingOffset > result.Timestamp, "EndingOffset is less than or equal to the PipelineContext.Timestamp.");
            }
        }

        [TestMethod]
        public void Returns_True_From_IsConfigurationAvailable_When_Configuration_Property_Is_Not_Null()
        {
            // arrange
            var context = new PipelineContext();
            IConfiguration expectedConfiguration = new Mock<IConfiguration>().Object;

#pragma warning disable CS0618 // Type or member is obsolete - The values will be removed in the next major release.

            // act
            context.Configuration = expectedConfiguration;

            // assert
            Assert.AreEqual(expectedConfiguration, context.Configuration);
#pragma warning restore CS0618
        }
    }
}
