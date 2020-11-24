// <copyright file="NoOpStep.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a step that does nothing.
    /// </summary>
    public sealed class NoOpStep : IPipelineRequest
    {
        // NOTE TO DEVELOPERS: THIS CONSTRUCTOR DOES NOT HAVE ANY CODE.
        //                     IF THAT CHANGES, UNIT TESTS WILL NEED TO BE WRITTEN.

        /// <summary>
        /// Initializes a new instance of the <see cref="NoOpStep" /> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public NoOpStep()
        {
            // no op
        }

        /// <summary>
        /// Disposes of nothing since this step does nothing.
        /// </summary>
        /// <returns>A completed <see cref="ValueTask" />.</returns>
        public ValueTask DisposeAsync()
        {
            return new ValueTask(Task.CompletedTask);
        }

        /// <summary>
        /// Provides a step that does nothing.
        /// </summary>
        /// <param name="context">The <see cref="IPipelineContext">context</see> passed during pipeline execution.</param>
        /// <returns>A <see cref="Task" />.</returns>
        public Task InvokeAsync(IPipelineContext context)
        {
            return Task.CompletedTask;
        }
    }
}