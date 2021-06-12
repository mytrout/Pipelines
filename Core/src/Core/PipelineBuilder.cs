// <copyright file="PipelineBuilder.cs" company="Chris Trout">
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
        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineBuilder" /> class.
        /// </summary>
        public PipelineBuilder()
        {
            // no op
        }

        /// <summary>
        /// Event triggered during when a new Step is added to the <see cref="PipelineBuilder"/>.
        /// </summary>
        private event EventHandler<StepAddedEventArgs>? StepAdded;

        /// <summary>
        /// Gets the types registered as Step for the Pipeline.
        /// </summary>
        private Stack<StepWithContext> StepTypes { get; } = new Stack<StepWithContext>();

        /// <summary>
        /// Adds a <paramref name="stepAddedEventHandler"/> to <see cref="StepAdded"/> and maintains builder semantics.
        /// </summary>
        /// <param name="stepAddedEventHandler">An <see cref="EventHandler{StepAddedEventArgs}"/> to run when the <see cref="PipelineBuilder"/> is being built.</param>
        /// <returns>Returns a <see cref="PipelineBuilder" /> with the handler added.</returns>
        public PipelineBuilder AddStepAddedEventHandler(EventHandler<StepAddedEventArgs> stepAddedEventHandler)
        {
            this.StepAdded += stepAddedEventHandler;

            return this;
        }

        /// <summary>
        /// Adds Step to this pipeline.
        /// </summary>
        /// <param name="stepWithContext">The step configuration to add to the pipeline.</param>
        /// <returns>Returns a <see cref="PipelineBuilder" /> with the step type added.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="stepWithContext"/> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="stepWithContext"/> is a value type or does not have the appropriate.</exception>
        public PipelineBuilder AddStep(StepWithContext stepWithContext)
        {
            stepWithContext.AssertParameterIsNotNull(nameof(stepWithContext));

            if (stepWithContext.StepType.IsValueType)
            {
                throw new InvalidOperationException(Resources.TYPE_MUST_BE_REFERENCE(CultureInfo.CurrentCulture, nameof(stepWithContext.StepType)));
            }

            if (!typeof(IPipelineRequest).IsAssignableFrom(stepWithContext.StepType))
            {
                throw new InvalidOperationException(Resources.TYPE_MUST_IMPLEMENT_IPIPELINEREQUEST(CultureInfo.CurrentCulture, nameof(stepWithContext.StepType)));
            }

            this.StepTypes.Push(stepWithContext);

            this.StepAdded?.Invoke(this, new StepAddedEventArgs(stepWithContext));

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
                StepWithContext step = this.StepTypes.Pop();

                if (step == null || step.StepType == null)
                {
                    throw new InvalidOperationException(Resources.NULL_MIDDLEWARE(CultureInfo.CurrentCulture));
                }

                if (stepActivator.CreateInstance(step, nextRequest) is not IPipelineRequest nextInstance)
                {
                    throw new InvalidOperationException(Resources.TYPE_MUST_IMPLEMENT_IPIPELINEREQUEST(CultureInfo.CurrentCulture, step.StepType.Name));
                }

                // Setting up for the next loop...
                nextRequest = nextInstance;
            }

            return nextRequest;
        }
    }
}
