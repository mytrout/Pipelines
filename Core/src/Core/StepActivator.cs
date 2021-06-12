// <copyright file="StepActivator.cs" company="Chris Trout">
// MIT License
//
// Copyright © 2019-2020 Chris Trout
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
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    /// <inheritdoc />
    public class StepActivator : IStepActivator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StepActivator" /> class with the specified <see cref="IServiceProvider" />.
        /// </summary>
        /// <param name="serviceProvider">.</param>
        /// <param name="logger"><see cref="ILogger{StepActivator}" /> to log information during run-time.</param>
        public StepActivator(ILogger<StepActivator> logger, IServiceProvider serviceProvider)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Gets the Logger reference for this <see cref="IStepActivator" />.
        /// </summary>
        public ILogger<StepActivator> Logger { get; }

        /// <summary>
        /// Gets the Dependency Injection-provided <see cref="IServiceProvider" />.
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <inheritdoc />
        public object? CreateInstance(StepWithContext pipelineStep, IPipelineRequest nextRequest)
        {
            pipelineStep.AssertParameterIsNotNull(nameof(pipelineStep));
            nextRequest.AssertParameterIsNotNull(nameof(nextRequest));

            object? result = null;

            // Indicates whether at least one constructor has a PipelineRequest parameter.
            bool oneConstructorContainsNextRequestParameter = false;

            foreach (var constructor in pipelineStep.StepType.GetConstructors()
                                                .OrderByDescending(x => x.GetParameters().Length))
            {
                bool nextRequestParameterFound = false;

                var parametersForConstructor = new List<object?>();
                foreach (var parameter in constructor.GetParameters())
                {
                    if (parameter.ParameterType == typeof(IPipelineRequest))
                    {
                        parametersForConstructor.Add(nextRequest);
                        nextRequestParameterFound = true;
                        oneConstructorContainsNextRequestParameter = true;
                    }
                    else
                    {
                        parametersForConstructor.Add(this.RetrieveParameter(this.ServiceProvider, pipelineStep, parameter));
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
                catch (Exception exc)
                {
                    // returns string.Empty if there are no parameters.
                    string parameters = string.Join(", ", constructor.GetParameters().Select(x => x.ParameterType.Name));
                    this.Logger.LogInformation(exc, Resources.TYPE_FAILED_TO_INITIALIZE(
                                                            CultureInfo.CurrentCulture,
                                                            pipelineStep.StepType.Name,
                                                            parameters));

                    // try other constructors rather than rethrowing the exception.
                }
            }

            if (!oneConstructorContainsNextRequestParameter)
            {
                throw new InvalidOperationException(Resources.CONSTRUCTOR_LACKS_PIPELINEREQUEST_PARAMETER(CultureInfo.CurrentCulture, pipelineStep.StepType.Name));
            }

            return result;
        }

        /// <summary>
        /// Retrieve a parameter using different methodologies provided by <see cref="StepWithContext"/>, <see cref="StepWithInstance{TStep, TOptions}"/> and <see cref="StepWithFactory{TStep, TOptions}"/>.
        /// </summary>
        /// <param name="services">An injected dependency service that provides some parameters.</param>
        /// <param name="pipelineStep">The step being configured.</param>
        /// <param name="parameter">The constructor parameter being built.</param>
        /// <returns>A fully-constructed parameter for the constructor.</returns>
        protected virtual object? RetrieveParameter(IServiceProvider services, StepWithContext pipelineStep, ParameterInfo parameter)
        {
            if (pipelineStep.StepDependencyType != null)
            {
                if (pipelineStep is IStepWithFactory factoryStep)
                {
                    return factoryStep.Invoke(services);
                }

                if (pipelineStep is IStepWithInstance instanceStep)
                {
                    return instanceStep.Instance;
                }
            }

            var results = services.GetService(parameter.ParameterType);

            // If the results aren't defined in IServiceProvider, load them from IConfiguration.
            if (results == null && !parameter.ParameterType.IsInterface)
            {
                results = Activator.CreateInstance(parameter.ParameterType, nonPublic: true);

                IConfiguration config = services.GetRequiredService<IConfiguration>();

                // Bind to root configuration from IConfiguration.
                config.Bind(results);

                // Bind to type name keys from IConfiguration.
                var paramTypeConfig = config.GetSection(parameter.ParameterType.Name);
                if (paramTypeConfig.Exists())
                {
                    paramTypeConfig.Bind(results);
                }

                if (pipelineStep.StepContext != null)
                {
                    var contextConfig = config.GetSection(pipelineStep.StepContext);

                    if (!contextConfig.Exists())
                    {
                        throw new InvalidOperationException(Resources.STEP_CONTEXT_NOT_FOUND(CultureInfo.CurrentCulture, parameter.ParameterType.Name, pipelineStep.StepType.Name));
                    }

                    // Bind to StepContext if it is not null.
                    config.GetSection(pipelineStep.StepContext).Bind(results);
                }

                // Bind additional configuration keys, if available.
                if (pipelineStep.ConfigKeys != null)
                {
                    // ConfigKeys are consumed in the order in which they are entered in the list.
                    // The last key is the final key to be configured on the result.
                    foreach (string key in pipelineStep.ConfigKeys)
                    {
                        if (!string.IsNullOrWhiteSpace(key))
                        {
                            var keyConfig = config.GetSection(key);

                            if (!keyConfig.Exists())
                            {
                                this.Logger.LogWarning(Resources.MISSING_CONFIGKEY(CultureInfo.CurrentCulture, key, parameter.ParameterType.Name, pipelineStep.StepType.Name, pipelineStep.StepContext));
                            }
                            else
                            {
                                keyConfig.Bind(results);
                            }
                        }
                    }
                }
            }

            return results;
        }
    }
}