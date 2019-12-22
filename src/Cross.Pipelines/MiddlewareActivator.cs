﻿// <copyright file="MiddlewareActivator.cs" company="Chris Trout">
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

namespace Cross.Pipelines
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <inheritdoc />
    public class MiddlewareActivator : IMiddlewareActivator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MiddlewareActivator" /> class with the specified <see cref="IServiceProvider" />.
        /// </summary>
        /// <param name="serviceProvider">.</param>
        /// <param name="logger"><see cref="ILogger{MiddlewareActivator}" /> to log information during run-time.</param>
        public MiddlewareActivator(ILogger<MiddlewareActivator> logger, IServiceProvider serviceProvider)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Gets the Logger reference for this <see cref="IMiddlewareActivator" />.
        /// </summary>
        public ILogger<MiddlewareActivator> Logger { get; }

        /// <summary>
        /// Gets the Dependency Injection-provided <see cref="IServiceProvider" />.
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <inheritdoc />
        public object CreateInstance(Type middlewareType, PipelineRequest nextRequest)
        {
            if (middlewareType == null)
            {
                throw new ArgumentNullException(nameof(middlewareType));
            }

            if (nextRequest == null)
            {
                throw new ArgumentNullException(nameof(nextRequest));
            }

            object result = null;

            // Indicates whether at least one constructor has a PipelineRequest parameter.
            bool oneConstructorContainsNextRequestParameter = false;

            foreach (var constructor in middlewareType.GetConstructors()
                                                .OrderByDescending(x => x.GetParameters().Length))
            {
                bool nextRequestParameterFound = false;

                var parametersForConstructor = new List<object>();
                foreach (var parameter in constructor.GetParameters())
                {
                    if (parameter.ParameterType == typeof(PipelineRequest))
                    {
                        parametersForConstructor.Add(nextRequest);
                        nextRequestParameterFound = true;
                        oneConstructorContainsNextRequestParameter = true;
                    }
                    else
                    {
                        parametersForConstructor.Add(this.ServiceProvider.GetService(parameter.ParameterType));
                    }
                }

                if (!nextRequestParameterFound)
                {
                    // Skip this constructor because no next PipelineRequest parameter was found.
                    continue;
                }

                try
                {
                    result = constructor.Invoke(parametersForConstructor.ToArray());
                    break;
                }
#pragma warning disable CA1031 // Catch the general exception so all failures can be logged.
                catch (Exception exc)
#pragma warning restore CA1031
                {
                    // returns string.Empty if there are no parameters.
                    string parameters = string.Join(", ", constructor.GetParameters().Select(x => x.ParameterType.Name));
                    this.Logger.LogInformation(exc, Resources.TYPE_FAILED_TO_INITIALIZE(
                                                            CultureInfo.CurrentCulture,
                                                            middlewareType.Name,
                                                            parameters));

                    // try other constructors rather than rethrowing the exception.
                }
            }

            if (!oneConstructorContainsNextRequestParameter)
            {
                throw new InvalidOperationException(Resources.CONSTRUCTOR_LACKS_PIPELINEREQUEST_PARAMETER(CultureInfo.CurrentCulture, middlewareType.Name));
            }

            return result;
        }
    }
}
