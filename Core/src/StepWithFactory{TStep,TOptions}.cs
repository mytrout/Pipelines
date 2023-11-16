// <copyright file="StepWithFactory{TStep,TOptions}.cs" company="Chris Trout">
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
    public record StepWithFactory<TStep, TOptions> : StepWithContext, IStepWithFactory
        where TStep : class, IPipelineRequest
        where TOptions : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StepWithFactory{TStep,TOptions}"/> class with a fully-constructed instance of <typeparamref name="TOptions"/> configured to <paramref name="stepContext"/>.
        /// </summary>
        /// <param name="stepContext">The context used if multiple steps of the same type are specified.</param>
        /// <param name="implementationFactory">A function returning a full-constructed instance of <typeparamref name="TOptions"/>.</param>
        public StepWithFactory(string stepContext, Func<IServiceProvider, TOptions> implementationFactory)
            : base(typeof(TStep), typeof(TOptions), stepContext)
        {
            this.ImplementationFactory = implementationFactory ?? throw new ArgumentNullException(nameof(implementationFactory));
        }

        /// <summary>
        /// Gets a function returning a fully-constructed instance of <typeparamref name="TOptions"/>.
        /// </summary>
        public Func<IServiceProvider, TOptions> ImplementationFactory { get; init; }

        /// <inheritdoc />
        public object Invoke(IServiceProvider serviceProvider)
        {
            return this.ImplementationFactory.Invoke(serviceProvider);
        }
    }
}
