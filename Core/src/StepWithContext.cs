﻿// <copyright file="StepWithContext.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2019-2021 Chris Trout
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
    /// Provides the ability to configure a Pipeline Step multiple times with different configurations in the same pipeline.
    /// </summary>
    public record StepWithContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StepWithContext" /> class with the requested parameters.
        /// </summary>
        /// <param name="stepType">The type to be constructed.</param>
        /// <param name="stepContext">The context used if multiple steps of the same type are specified. stepContext is ignored if the value is <see langword="null"/>.</param>
        public StepWithContext(Type stepType, string? stepContext = null)
            : this(stepType, null, stepContext)
        {
            // no op
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StepWithContext" /> class with the requested parameters.
        /// </summary>
        /// <param name="stepType">The type to be constructed.</param>
        /// <param name="stepContext">The context used if multiple steps of the same type are specified. stepContext is ignored if the value is <see langword="null"/>.</param>
        /// <param name="stepDependencyType">The type to be used as the step's dependency.</param>
        public StepWithContext(Type stepType, Type? stepDependencyType = null, string? stepContext = null)
        {
            this.StepType = stepType ?? throw new ArgumentNullException(nameof(stepType));
            this.StepDependencyType = stepDependencyType;
            this.StepContext = stepContext;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StepWithContext" /> class with the requested parameters.
        /// </summary>
        /// <param name="stepType">The type to be constructed.</param>
        /// <param name="stepContext">The context used if multiple steps of the same type are specified. stepContext is ignored if the value is <see langword="null"/>.</param>
        /// <param name="stepDependencyType">The type to be used as the step's dependency.</param>
        /// <param name="configKeys">Additional configuration keys that can be used to bind step dependency information from <see cref="Microsoft.Extensions.Configuration.IConfiguration"/>.</param>
        public StepWithContext(Type stepType, Type? stepDependencyType = null, string? stepContext = null, params string[] configKeys)
            : this(stepType, stepDependencyType, stepContext)
        {
            this.ConfigKeys = configKeys;
        }

        /// <summary>
        /// Gets the additional configuration keys that will be used to load the <see cref="StepDependencyType"/>.
        /// </summary>
        public string[]? ConfigKeys { get; init; } = null;

        /// <summary>
        /// Gets the step to be configured multiple times.
        /// </summary>
        public Type StepType { get; init; }

        /// <summary>
        /// Gets the context to be used to configure this instance of the step.
        /// </summary>
        public string? StepContext { get; init; }

        /// <summary>
        /// Gets the type used to configure options for this step.
        /// </summary>
        public Type? StepDependencyType { get; init; }
    }
}
