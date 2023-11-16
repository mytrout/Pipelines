// <copyright file="StepWithInstance{TStep,TOptions}.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines
{
    using System;

    /// <summary>
    /// Provides the ability to configure a Pipeline Step multiple times with different instances and/or functions in the same pipeline.
    /// </summary>
    /// <typeparam name="TStep">The class which implements the step wrapped by this instance.</typeparam>
    /// <typeparam name="TOptions">The class which provides configuration for the <typeparamref name="TStep"/>.</typeparam>
    public sealed record StepWithInstance<TStep, TOptions> : StepWithContext, IStepWithInstance
        where TStep : class, IPipelineRequest
        where TOptions : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StepWithInstance{TStep,TOptions}"/> class with a fully-constructed instance of <typeparamref name="TOptions"/> configured to <paramref name="stepContext"/>.
        /// </summary>
        /// <param name="stepContext">The context used if multiple steps of the same type are specified.</param>
        /// <param name="instance">A fully-constructed instance of <typeparamref name="TOptions"/>.</param>
        public StepWithInstance(string stepContext, TOptions instance)
            : base(typeof(TStep), typeof(TOptions), stepContext)
        {
            this.Instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        /// <summary>
        /// Gets a fully-constructed instance of <typeparamref name="TOptions"/>.
        /// </summary>
        public TOptions Instance { get; init; }

        /// <summary>
        /// Gets a fully-constructed instance of <typeparamref name="TOptions"/>.
        /// </summary>
        object IStepWithInstance.Instance => this.Instance;
    }
}
