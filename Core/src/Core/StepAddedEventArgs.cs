#pragma warning disable SA1515, SA1633, SA1636 // Copyright header is 2021 only.
// <copyright file="StepAddedEventArgs.cs" company="Chris Trout">
// MIT License
//
// Copyright © 2021 Chris Trout
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
#pragma warning restore SA1515, SA1633, SA1636
namespace MyTrout.Pipelines.Core
{
    using System;

    /// <summary>
    /// Provides EventArgs after a <see cref="StepWithContext"/> has been added to the <see cref="PipelineBuilder"/>.
    /// </summary>
    public class StepAddedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StepAddedEventArgs" /> class with the requested parameters.
        /// </summary>
        /// <param name="currentStep">The <see cref="StepWithContext"/> added to the <see cref="PipelineBuilder"/>.</param>
        public StepAddedEventArgs(StepWithContext currentStep)
        {
            this.CurrentStep = currentStep ?? throw new ArgumentNullException(nameof(currentStep));
        }

        /// <summary>
        /// Gets the <see cref="StepWithContext"/> added to the <see cref="PipelineBuilder"/>.
        /// </summary>
        public StepWithContext CurrentStep { get; init; }
    }
}
