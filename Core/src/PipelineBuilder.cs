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

namespace MyTrout.Pipelines
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

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
        /// Gets the name of the method used by the pipeline.
        /// </summary>
        public virtual string MethodName => "InvokeAsync";

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
            where T : class
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
            if (stepType == null)
            {
                throw new ArgumentNullException(nameof(stepType));
            }

            if (stepType.IsValueType)
            {
                throw new InvalidOperationException(Resources.TYPE_MUST_BE_REFERENCE(CultureInfo.CurrentCulture, nameof(stepType)));
            }

            var method = stepType.GetMethod(this.MethodName);
            this.ValidateMethod(stepType, method);

            this.StepTypes.Push(stepType);
            return this;
        }

        /// <summary>
        /// Builds a pipeline from the configured step.
        /// </summary>
        /// <param name="stepActivator">The service used to initialize instances of step.</param>
        /// <returns>A pipeline to execute.</returns>
        public PipelineRequest Build(IStepActivator stepActivator)
        {
            if (stepActivator == null)
            {
                throw new ArgumentNullException(nameof(stepActivator));
            }

            // Configures the "final" delegate in the Pipeline.
            var nextRequest = new PipelineRequest(context => Task.CompletedTask);

            while (this.StepTypes.Any())
            {
                Type stepType = this.StepTypes.Pop();

                if (stepType == null)
                {
                    throw new InvalidOperationException(Resources.NULL_MIDDLEWARE(CultureInfo.CurrentCulture));
                }

                object nextInstance = stepActivator.createInstance(stepType, nextRequest);

                if (nextInstance == null)
                {
                    throw new InvalidOperationException(Resources.SERVICEPROVIDER_LACKS_PARAMETER(CultureInfo.CurrentCulture, stepType.Name));
                }

                // if the IStepActivator returns a different type for any reason,
                // using nextInstance.GetType() guarantees it contains the correct MethodName.
                var nextInvoke = nextInstance.GetType().GetMethod(this.MethodName);
                this.ValidateMethod(nextInstance.GetType(), nextInvoke);

                nextRequest = (PipelineRequest)nextInvoke.CreateDelegate(typeof(PipelineRequest), nextInstance);
            }

            return nextRequest;
        }

        private void ValidateMethod(Type originatingType, MethodInfo method)
        {
            var parameters = method?.GetParameters();
            if (method == null
                    || parameters == null
                    || parameters.Length != 1
                    || parameters[0].ParameterType != typeof(PipelineContext)
                    || method.ReturnType != typeof(Task))
            {
                throw new InvalidOperationException(Resources.METHOD_NOT_FOUND(CultureInfo.CurrentCulture, originatingType.Name, this.MethodName));
            }
        }
    }
}
