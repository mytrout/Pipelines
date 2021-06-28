// <copyright file="StepActivator.cs" company="Chris Trout">
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
        /// Gets the functions that create a parameter used by the Step.
        /// </summary>
        public IList<ParameterCreationDelegate> ParameterCreators { get; }
            = new List<ParameterCreationDelegate>()
                {
                    StepActivator.CreateParameterFromStepFactory,
                    StepActivator.CreateParameterFromStepInstance,
                    StepActivator.CreateParameterFromDependencyInjectionInstance,
                    StepActivator.CreateParameterFromConfiguration
                };

        /// <summary>
        /// Gets the Dependency Injection-provided <see cref="IServiceProvider" />.
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Retrieve a parameter from the <see cref="IConfiguration"/>.
        /// </summary>
        /// <param name="logger">A logger for debug logging.</param>
        /// <param name="services">An injected dependency service that provides some parameters.</param>
        /// <param name="pipelineStep">The step being configured.</param>
        /// <param name="parameter">The constructor parameter being built.</param>
        /// <returns>A fully-constructed parameter for the constructor.</returns>
        /// <remarks>This method always returns <see langword="true" /> for the <see cref="ParameterCreationResult.SkipRemainingCreators"/>.</remarks>
        public static ParameterCreationResult CreateParameterFromConfiguration(ILogger<StepActivator> logger, IServiceProvider services, StepWithContext pipelineStep, ParameterInfo parameter)
        {
            services.AssertParameterIsNotNull(nameof(services));
            parameter.AssertParameterIsNotNull(nameof(parameter));
            pipelineStep.AssertParameterIsNotNull(nameof(pipelineStep));

            if (parameter.ParameterType.IsInterface)
            {
                return new ParameterCreationResult(null, false);
            }

            var results = Activator.CreateInstance(parameter.ParameterType, nonPublic: true);

            IConfiguration config = services.GetRequiredService<IConfiguration>();

            // Bind to root configuration from IConfiguration.
            config.Bind(results);

            // Bind to type name keys from IConfiguration.
            StepActivator.BindConfiguration(config, parameter.ParameterType.Name, results);

            if (pipelineStep.StepContext != null)
            {
                var contextConfig = config.GetSection(pipelineStep.StepContext);

                if (!contextConfig.Exists())
                {
                    throw new InvalidOperationException(Resources.STEP_CONTEXT_NOT_FOUND(CultureInfo.CurrentCulture, parameter.ParameterType.Name, pipelineStep.StepType.Name));
                }

                // Bind to StepContext if it is not null.
                contextConfig.Bind(results);
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
                            logger.LogWarning(Resources.MISSING_CONFIGKEY(CultureInfo.CurrentCulture, key, parameter.ParameterType.Name, pipelineStep.StepType.Name, pipelineStep.StepContext));
                        }
                        else
                        {
                            keyConfig.Bind(results);
                        }
                    }
                }
            }

            return new ParameterCreationResult(results, true);
        }

        /// <summary>
        /// Retrieve a parameter from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="logger">A logger for debug logging.</param>
        /// <param name="services">An injected dependency service that provides some parameters.</param>
        /// <param name="pipelineStep">The step being configured.</param>
        /// <param name="parameter">The constructor parameter being built.</param>
        /// <returns>A fully-constructed parameter for the constructor.</returns>
        public static ParameterCreationResult CreateParameterFromDependencyInjectionInstance(ILogger<StepActivator> logger, IServiceProvider services, StepWithContext pipelineStep, ParameterInfo parameter)
        {
            services.AssertParameterIsNotNull(nameof(services));
            parameter.AssertParameterIsNotNull(nameof(parameter));
            pipelineStep.AssertParameterIsNotNull(nameof(pipelineStep));

            var instanceResult = services.GetService(parameter.ParameterType);

            if (instanceResult != null)
            {
                return new ParameterCreationResult(instanceResult, true);
            }

            return new ParameterCreationResult(null, false);
        }

        /// <summary>
        /// Retrieve a parameter from the <see cref="IConfiguration"/>.
        /// </summary>
        /// <param name="next">The next pipeline step instance.</param>
        /// <param name="logger">A logger for debug logging.</param>
        /// <param name="services">An injected dependency service that provides some parameters.</param>
        /// <param name="pipelineStep">The step being configured.</param>
        /// <param name="parameter">The constructor parameter being built.</param>
        /// <returns>A fully-constructed parameter for the constructor.</returns>
        /// <remarks>This method always returns <see langword="true" /> for the <see cref="ParameterCreationResult.SkipRemainingCreators"/>.</remarks>
        public static ParameterCreationResult CreateParameterForNextPipelineRequest(IPipelineRequest next, ILogger<StepActivator> logger, IServiceProvider services, StepWithContext pipelineStep, ParameterInfo parameter)
        {
            next.AssertParameterIsNotNull(nameof(next));
            logger.AssertParameterIsNotNull(nameof(logger));
            services.AssertParameterIsNotNull(nameof(services));
            parameter.AssertParameterIsNotNull(nameof(parameter));
            pipelineStep.AssertParameterIsNotNull(nameof(pipelineStep));

            if (parameter.ParameterType == typeof(IPipelineRequest))
            {
                return new ParameterCreationResult(next, true);
            }

            return new ParameterCreationResult(null, false);
        }

        /// <summary>
        /// Retrieve a parameter from <see cref="StepWithFactory{TStep, TOptions}"/>.
        /// </summary>
        /// <param name="logger">A logger for debug logging.</param>
        /// <param name="services">An injected dependency service that provides some parameters.</param>
        /// <param name="pipelineStep">The step being configured.</param>
        /// <param name="parameter">The constructor parameter being built.</param>
        /// <returns>A fully-constructed parameter for the constructor.</returns>
        public static ParameterCreationResult CreateParameterFromStepFactory(ILogger<StepActivator> logger, IServiceProvider services, StepWithContext pipelineStep, ParameterInfo parameter)
        {
            services.AssertParameterIsNotNull(nameof(services));
            parameter.AssertParameterIsNotNull(nameof(parameter));
            pipelineStep.AssertParameterIsNotNull(nameof(pipelineStep));

            if (pipelineStep.StepDependencyType != null && pipelineStep is IStepWithFactory factoryStep)
            {
                return new ParameterCreationResult(factoryStep.Invoke(services), true);
            }

            return new ParameterCreationResult(null, false);
        }

        /// <summary>
        /// Retrieve a parameter from <see cref="StepWithInstance{TStep, TOptions}" />.
        /// </summary>
        /// <param name="logger">A logger for debug logging.</param>
        /// <param name="services">An injected dependency service that provides some parameters.</param>
        /// <param name="pipelineStep">The step being configured.</param>
        /// <param name="parameter">The constructor parameter being built.</param>
        /// <returns>A fully-constructed parameter for the constructor.</returns>
        public static ParameterCreationResult CreateParameterFromStepInstance(ILogger<StepActivator> logger, IServiceProvider services, StepWithContext pipelineStep, ParameterInfo parameter)
        {
            services.AssertParameterIsNotNull(nameof(services));
            parameter.AssertParameterIsNotNull(nameof(parameter));
            pipelineStep.AssertParameterIsNotNull(nameof(pipelineStep));

            if (pipelineStep.StepDependencyType != null && pipelineStep is IStepWithInstance factoryStep)
            {
                return new ParameterCreationResult(factoryStep.Instance, true);
            }

            return new ParameterCreationResult(null, false);
        }

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

                if (!constructor.GetParameters().Any(x => x.ParameterType == typeof(IPipelineRequest)))
                {
                    // Skip this constructor because no next PipelineRequest parameter was found.
                    continue;

                }

                oneConstructorContainsNextRequestParameter = true;

                var indexedParameterCreators = new List<ParameterCreationDelegate>(this.ParameterCreators);

                // Inject the NextPipelineRequest into the ParameterCreators methods to allow every parameter to be treated exactly the same.
                indexedParameterCreators.Insert(
                            0,
                            (ILogger<StepActivator> logger, IServiceProvider services, StepWithContext pipelineStep, ParameterInfo parameter)
                                => StepActivator.CreateParameterForNextPipelineRequest(nextRequest, logger, services, pipelineStep, parameter));

                var parametersForConstructor = new List<object?>();

                // Cycle through each parameter on this constructor and get the expected parameter.
                foreach (var parameter in constructor.GetParameters())
                {
                    foreach (var parameterCreator in indexedParameterCreators)
                    {
                        var creationResult = parameterCreator.Invoke(this.Logger, this.ServiceProvider, pipelineStep, parameter);
                        if (creationResult.SkipRemainingCreators)
                        {
                            parametersForConstructor.Add(creationResult.Instance);
                            break;
                        }
                    }
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

        private static object? BindConfiguration(IConfiguration config, string key, object? instance)
        {
            // Bind to type name keys from IConfiguration.
            var keyConfig = config.GetSection(key);
            if (keyConfig.Exists())
            {
                config.Bind(instance);
            }

            return instance;
        }
    }
}