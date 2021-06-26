// <copyright file="ParameterCreationDelegate.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2021 Chris Trout
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
    using Microsoft.Extensions.Logging;
    using System;
    using System.Reflection;

    /// <summary>
    /// Retrieve a parameter to be used by <see cref="StepActivator"/> in the construction of Pipeline steps and their dependencies.
    /// </summary>
    /// <param name="logger">A logger for debug logging.</param>
    /// <param name="services">An injected dependency service that provides some parameters.</param>
    /// <param name="pipelineStep">The step being configured.</param>
    /// <param name="parameter">The constructor parameter being built.</param>
    /// <returns>A <see cref="ParameterCreationResult"/> indicating whether this delegate suggests further processing is required and the instance on which processing should be or has been done.</returns>
    public delegate ParameterCreationResult ParameterCreationDelegate(ILogger<StepActivator> logger, IServiceProvider services, StepWithContext pipelineStep, ParameterInfo parameter);
}
