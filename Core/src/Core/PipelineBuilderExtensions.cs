// <copyright file="PipelineBuilderExtensions.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Core
{
    using System;

    /// <summary>
    /// Extension methods to the <see cref="PipelineBuilder"/> to simplify usage for developers.
    /// </summary>
    public static class PipelineBuilderExtensions
    {
        /// <summary>
        /// Adds step to this pipeline.
        /// </summary>
        /// <typeparam name="TStep">The generic type to add to the pipeline.</typeparam>
        /// <param name="source">The <see cref="PipelineBuilder"/> to which to add the step.</param>
        /// <returns>Returns a <see cref="PipelineBuilder" /> with the Step type added.</returns>
        public static PipelineBuilder AddStep<TStep>(this PipelineBuilder source)
            where TStep : class, IPipelineRequest
        {
            return source.AddStep(typeof(TStep), null);
        }

        /// <summary>
        /// Adds step to this pipeline.
        /// </summary>
        /// <typeparam name="TStep">The generic type to add to the pipeline.</typeparam>
        /// <param name="source">The <see cref="PipelineBuilder"/> to which to add the step.</param>
        /// <param name="stepContext">The context used if multiple steps of the same type are specified.</param>
        /// <returns>Returns a <see cref="PipelineBuilder" /> with the Step type added.</returns>
        public static PipelineBuilder AddStep<TStep>(this PipelineBuilder source, string stepContext)
            where TStep : class, IPipelineRequest
        {
            source.AssertParameterIsNotNull(nameof(source));
            return source.AddStep(typeof(TStep), stepContext);
        }

        /// <summary>
        /// Adds step to this pipeline.
        /// </summary>
        /// <typeparam name="TStep">The generic type to add to the pipeline.</typeparam>
        /// <param name="source">The <see cref="PipelineBuilder"/> to which to add the step.</param>
        /// <param name="dependencyType">The dependency type that should be added to the step. </param>
        /// <param name="stepContext">The context used if multiple steps of the same type are specified; stepContext is ignored if the value is <see langword="null"/>.</param>
        /// <returns>Returns a <see cref="PipelineBuilder" /> with the Step type added.</returns>
        public static PipelineBuilder AddStep<TStep>(this PipelineBuilder source, Type dependencyType, string stepContext)
            where TStep : class, IPipelineRequest
        {
            return source.AddStep(typeof(TStep), dependencyType, stepContext);
        }

        /// <summary>
        /// Adds step to this pipeline.
        /// </summary>
        /// <typeparam name="TStep">The generic type to add to the pipeline.</typeparam>
        /// <typeparam name="TOptions">The generic type to add options for the step in the pipeline.</typeparam>
        /// <param name="source">The <see cref="PipelineBuilder"/> to which to add the step.</param>
        /// <param name="stepContext">The context used if multiple steps of the same type are specified; stepContext is ignored if the value is <see langword="null"/>.</param>
        /// <returns>Returns a <see cref="PipelineBuilder" /> with the Step type added.</returns>
        public static PipelineBuilder AddStep<TStep, TOptions>(this PipelineBuilder source, string stepContext)
            where TStep : class, IPipelineRequest
            where TOptions : class
        {
            return source.AddStep(typeof(TStep), typeof(TOptions), stepContext);
        }

        /// <summary>
        /// Adds Step to this pipeline.
        /// </summary>
        /// <param name="source">The <see cref="PipelineBuilder"/> to which to add the step.</param>
        /// <param name="stepType">The <see cref="Type" /> to add to the pipeline.</param>
        /// <param name="stepContext">The context used if multiple steps of the same type are specified; stepCntext is ignored if the value is <see langword="null"/>.</param>
        /// <returns>Returns a <see cref="PipelineBuilder" /> with the step type added.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="stepType"/> is <see langword="null" />.</exception>
        public static PipelineBuilder AddStep(this PipelineBuilder source, Type stepType, string? stepContext = null)
        {
            source.AssertParameterIsNotNull(nameof(source));
            stepType.AssertParameterIsNotNull(nameof(stepType));

            var currentStep = new StepWithContext(stepType, null, stepContext);

            return source.AddStep(currentStep);
        }

        /// <summary>
        /// Adds step to this pipeline.
        /// </summary>
        /// <param name="source">The <see cref="PipelineBuilder"/> to which to add the step.</param>
        /// <param name="stepType">The step type that should be added to the pipeline.</param>
        /// <param name="dependencyType">The dependency type that should be added to the pipeline. </param>
        /// <param name="stepContext">The context used if multiple steps of the same type are specified; stepContext is ignored if the value is <see langword="null"/>.</param>
        /// <returns>Returns a <see cref="PipelineBuilder" /> with the Step type added.</returns>
        public static PipelineBuilder AddStep(this PipelineBuilder source, Type stepType, Type dependencyType, string stepContext)
        {
            stepType.AssertParameterIsNotNull(nameof(stepType));
            dependencyType.AssertParameterIsNotNull(nameof(dependencyType));
            stepContext.AssertParameterIsNotNull(nameof(stepContext));

            var currentStep = new StepWithContext(stepType, dependencyType, stepContext);

            return source.AddStep(currentStep);
        }
    }
}
