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
    /// <remarks>
    /// <para>Steps that require a step context <b>will not fire</b> the <see cref="StepAdded"/> event, by default.</para>
    /// <para>Steps that lack a step context <b>will fire</b> the <see cref="StepAdded"/> event, by default.</para>
    /// </remarks>
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
        /// Event triggered during when a new Step is added to the <see cref="PipelineBuilder"/>.
        /// </summary>
        public event EventHandler<StepAddedEventArgs>? StepAdded;

        /// <summary>
        /// Gets the types registered as Step for the Pipeline.
        /// </summary>
        private Stack<StepWithContext> StepTypes { get; } = new Stack<StepWithContext>();

        /// <summary>
        /// Adds step to this pipeline.
        /// </summary>
        /// <typeparam name="T">The generic type to add to the pipeline.</typeparam>
        /// <param name="fireStepAddedEvent">A value indicating whether the <see cref="StepAdded"/> event should be fired.</param>
        /// <returns>Returns a <see cref="PipelineBuilder" /> with the Step type added.</returns>
        /// <remarks>This method does not contain a stepContext and therefore <b>will</b> fire the <see cref="StepAdded"/> event, by default.</remarks>
        public PipelineBuilder AddStep<T>(bool fireStepAddedEvent = true)
            where T : class, IPipelineRequest
        {
            // NOTE TO FUTURE DEVELOPERS: This method handles pushing onto the Stack because the highest overload
            //                              requires a non-null stepContext value which this method cannot provide.
            var currentStep = new StepWithContext(typeof(T), null);

            this.StepTypes.Push(currentStep);

            if (fireStepAddedEvent)
            {
                this.StepAdded?.Invoke(this, new StepAddedEventArgs(currentStep));
            }

            return this;
        }

        /// <summary>
        /// Adds step to this pipeline.
        /// </summary>
        /// <typeparam name="T">The generic type to add to the pipeline.</typeparam>
        /// <param name="stepContext">The context used if multiple steps of the same type are specified; stepContext is ignored if the value is <see langword="null"/>.</param>
        /// <param name="fireStepAddedEvent">A value indicating whether the <see cref="StepAdded"/> event should be fired.</param>
        /// <returns>Returns a <see cref="PipelineBuilder" /> with the Step type added.</returns>
        /// <remarks>This method contains a <paramref name="stepContext"/> and therefore <b>will not</b> fire the <see cref="StepAdded"/> event, by default.</remarks>
        public PipelineBuilder AddStep<T>(string stepContext, bool fireStepAddedEvent = false)
            where T : class, IPipelineRequest
        {
            return this.AddStep(typeof(T), stepContext, fireStepAddedEvent);
        }

        /// <summary>
        /// Adds Step to this pipeline.
        /// </summary>
        /// <param name="stepType">The <see cref="Type" /> to add to the pipeline.</param>
        /// <param name="fireStepAddedEvent">A value indicating whether the <see cref="StepAdded"/> event should be fired.</param>
        /// <returns>Returns a <see cref="PipelineBuilder" /> with the step type added.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="stepType"/> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="stepType"/> is a value type or does not have the appropriate.</exception>
        /// <remarks>This method does not contain a stepContext and therefore <b>will</b> fire the <see cref="StepAdded"/> event, by default.</remarks>
        public PipelineBuilder AddStep(Type stepType, bool fireStepAddedEvent = true)
        {
            // NOTE TO FUTURE DEVELOPERS: This method handles pushing onto the Stack because the highest overload
            //                              requires a non-null stepContext value which this method cannot provide.
            stepType.AssertParameterIsNotNull(nameof(stepType));

            if (stepType.IsValueType)
            {
                throw new InvalidOperationException(Resources.TYPE_MUST_BE_REFERENCE(CultureInfo.CurrentCulture, nameof(stepType)));
            }

            if (!typeof(IPipelineRequest).IsAssignableFrom(stepType))
            {
                throw new InvalidOperationException(Resources.TYPE_MUST_IMPLEMENT_IPIPELINEREQUEST(CultureInfo.CurrentCulture, nameof(stepType)));
            }

            var currentStep = new StepWithContext(stepType, null);

            this.StepTypes.Push(currentStep);

            if (fireStepAddedEvent)
            {
                this.StepAdded?.Invoke(this, new StepAddedEventArgs(currentStep));
            }

            return this;
        }

        /// <summary>
        /// Adds Step to this pipeline.
        /// </summary>
        /// <param name="stepType">The <see cref="Type" /> to add to the pipeline.</param>
        /// <param name="stepContext">The context used if multiple steps of the same type are specified; stepCntext is ignored if the value is <see langword="null"/>.</param>
        /// <param name="fireStepAddedEvent">A value indicating whether the <see cref="StepAdded"/> event should be fired.</param>
        /// <returns>Returns a <see cref="PipelineBuilder" /> with the step type added.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="stepType"/> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="stepType"/> is a value type or does not have the appropriate.</exception>
        /// <remarks>This method contains a <paramref name="stepContext"/> and therefore <b>will not</b> fire the <see cref="StepAdded"/> event, by default.</remarks>
        public PipelineBuilder AddStep(Type stepType, string stepContext, bool fireStepAddedEvent = false)
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

            var currentStep = new StepWithContext(stepType, stepContext);

            this.StepTypes.Push(currentStep);

            if (fireStepAddedEvent)
            {
                this.StepAdded?.Invoke(this, new StepAddedEventArgs(currentStep));
            }

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
