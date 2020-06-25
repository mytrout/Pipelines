// <copyright file="PipelineBuilder.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Core
{
    using MyTrout.Pipelines.Steps;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Builds the Pipeline by adding different step implenentations.
    /// </summary>
    public class PipelineBuilder
    {
        // NOTE TO DEVELOPERS: If the PipelineBuilder constructor changes, then remove the
        //                     ExcludeFromCodeCoverage attribute and write unit tests.

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineBuilder" /> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public PipelineBuilder()
        {
            // no op
        }

        /// <summary>
        /// Gets the types registered as Step for the Pipeline.
        /// </summary>
        private Stack<Type> StepTypes { get; } = new Stack<Type>();

        /// <summary>
        /// Adds step to this pipeline.
        /// </summary>
        /// <typeparam name="T">The generic type to add to the pipeline.</typeparam>
        /// <returns>Returns a <see cref="PipelineBuilder" /> with the Step type added.</returns>
        public PipelineBuilder AddStep<T>()
            where T : class, IPipelineRequest
        {
            return this.AddStep(typeof(T));
        }

        /// <summary>
        /// Adds Step to this pipeline.
        /// </summary>
        /// <param name="stepType">The <see cref="Type" /> to add to the pipeline.</param>
        /// <returns>Returns a <see cref="PipelineBuilder" /> with the step type added.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="stepType"/> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="stepType"/> is a value type or does not have the appropriate.</exception>
        public PipelineBuilder AddStep(Type stepType)
        {
            stepType.AssertParameterIsNotNull(nameof(stepType));

            if (stepType.IsValueType)
            {
                throw new InvalidOperationException(Resources.TYPE_MUST_BE_REFERENCE(CultureInfo.CurrentCulture, nameof(stepType)));
            }

            if (!typeof(IPipelineRequest).IsAssignableFrom(stepType))
            {
                throw new InvalidOperationException(Resources.TYPE_MUST_IMPLEMENT_IPIPELINEREQUEST(CultureInfo.CurrentCulture, nameof(stepType)));
            }

            this.StepTypes.Push(stepType);

            return this;
        }

        /// <summary>
        /// Builds a pipeline from the configured step.
        /// </summary>
        /// <param name="stepActivator">The service used to initialize instances of step.</param>
        /// <returns>A pipeline to execute.</returns>
        public IPipelineRequest Build(IStepActivator stepActivator)
        {
            stepActivator.AssertParameterIsNotNull(nameof(stepActivator));

            // Configures the "final" delegate in the Pipeline.
            IPipelineRequest nextRequest = new NoOpStep();

            while (this.StepTypes.Any())
            {
                Type stepType = this.StepTypes.Pop();

                if (stepType == null)
                {
                    throw new InvalidOperationException(Resources.NULL_MIDDLEWARE(CultureInfo.CurrentCulture));
                }

                IPipelineRequest nextInstance = stepActivator.CreateInstance(stepType, nextRequest) as IPipelineRequest;

                if (nextInstance == null)
                {
                    throw new InvalidOperationException(Resources.TYPE_MUST_IMPLEMENT_IPIPELINEREQUEST(CultureInfo.CurrentCulture, stepType.Name));
                }

                // Setting up for the next loop...
                nextRequest = nextInstance;
            }

            return nextRequest;
        }
    }
}
