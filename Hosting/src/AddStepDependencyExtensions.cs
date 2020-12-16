// <copyright file="AddStepDependencyExtensions.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Hosting
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Extensions to add step dependencies t0 <see cref="IHostBuilder" /> for use with a Pipeline.
    /// </summary>
    public static class AddStepDependencyExtensions
    {
        /// <summary>
        /// Gets a key to use with <see cref="IHostBuilder.Properties"/>.
        /// </summary>
        public const string STEP_CONFIG_CONTEXT = "StepConfigContext-";

        /// <summary>
        /// Add step dependency without context.
        /// </summary>
        /// <typeparam name="TOptions">A <see cref="Type"/> that will be injected into a Step.</typeparam>
        /// <param name="source">The <see cref="IHostBuilder"/> on which this dependency should be configured.</param>
        /// <returns>An instance of <see cref="IHostBuilder"/> with the <typeparamref name="TOptions"/> configured.</returns>
        public static IHostBuilder AddStepDependency<TOptions>(this IHostBuilder source)
            where TOptions : class, new()
        {
            source.AssertParameterIsNotNull(nameof(source));

            return AddStepDependency<TOptions>(source, configKeys: Array.Empty<string>());
        }

        /// <summary>
        /// Add step dependency without context using <see cref="IConfiguration"/> with the specified <paramref name="configKeys"/>.
        /// </summary>
        /// <typeparam name="TOptions">A <see cref="Type"/> that will be injected into a Step.</typeparam>
        /// <param name="source">The <see cref="IHostBuilder"/> on which this dependency should be configured.</param>
        /// <param name="configKeys"><see cref="IConfiguration"/> context to be used to load this <typeparamref name="TOptions"/>.</param>
        /// <returns>An instance of <see cref="IHostBuilder"/> with the <typeparamref name="TOptions"/> configured.</returns>
        public static IHostBuilder AddStepDependency<TOptions>(this IHostBuilder source, string[] configKeys)
            where TOptions : class, new()
        {
            source.AssertParameterIsNotNull(nameof(source));
            configKeys.AssertParameterIsNotNull(nameof(configKeys));

            source.ConfigureServices(services =>
            {
                services.AddTransient((IServiceProvider serviceProvider) =>
                            AddStepDependencyExtensions.BindStepContextFromConfiguration<TOptions>(serviceProvider, configKeys));
            });

            return source;
        }

        /// <summary>
        /// Add a step dependency instance without context.
        /// </summary>
        /// <typeparam name="TOptions">A <see cref="Type"/> that will be injected into a Step.</typeparam>
        /// <param name="source">The <see cref="IHostBuilder"/> on which this dependency should be configured.</param>
        /// <param name="instance">An instance of <typeparamref name="TOptions"/>.</param>
        /// <returns>An instance of <see cref="IHostBuilder"/> with the <typeparamref name="TOptions"/> configured.</returns>
        public static IHostBuilder AddStepDependency<TOptions>(this IHostBuilder source, TOptions instance)
            where TOptions : class
        {
            source.AssertParameterIsNotNull(nameof(source));
            instance.AssertParameterIsNotNull(nameof(instance));

            source.ConfigureServices(services =>
            {
                services.AddSingleton(instance);
            });

            return source;
        }

        /// <summary>
        /// Add a step dependency without context that will be constructed at run-time.
        /// </summary>
        /// <typeparam name="TOptions">A <see cref="Type"/> that will be injected into a Step.</typeparam>
        /// <param name="source">The <see cref="IHostBuilder"/> on which this dependency should be configured.</param>
        /// <param name="instantiate">A function that returns an instance of <typeparamref name="TOptions"/>.</param>
        /// <returns>An instance of <see cref="IHostBuilder"/> with the <typeparamref name="TOptions"/> configured.</returns>
        public static IHostBuilder AddStepDependency<TOptions>(this IHostBuilder source, Func<IServiceProvider, TOptions> instantiate)
            where TOptions : class
        {
            source.AssertParameterIsNotNull(nameof(source));
            instantiate.AssertParameterIsNotNull(nameof(instantiate));

            source.ConfigureServices(services =>
            {
                services.AddTransient(instantiate);
            });

            return source;
        }

        /// <summary>
        /// Add a step dependency under a specific <paramref name="context"/> using <see cref="IConfiguration"/> to load values.
        /// </summary>
        /// <typeparam name="TOptions">A <see cref="Type"/> that will be injected into a Step.</typeparam>
        /// <param name="source">The <see cref="IHostBuilder"/> on which this dependency should be configured.</param>
        /// <param name="context">The context under which this dependency will be configured.</param>
        /// <returns>An instance of <see cref="IHostBuilder"/> with the <typeparamref name="TOptions"/> configured.</returns>
        public static IHostBuilder AddStepDependency<TOptions>(this IHostBuilder source, string context)
            where TOptions : class, new()
        {
            return AddStepDependencyExtensions.AddStepDependency<TOptions>(source, context, configKeys: Array.Empty<string>());
        }

        /// <summary>
        /// Add a step dependency under a specific <paramref name="context"/> using <see cref="IConfiguration"/> to load values from <paramref name="context"/> and <paramref name="configKeys"/>.
        /// </summary>
        /// <typeparam name="TOptions">A <see cref="Type"/> that will be injected into a Step.</typeparam>
        /// <param name="source">The <see cref="IHostBuilder"/> on which this dependency should be configured.</param>
        /// <param name="context">The context under which this dependency will be configured.</param>
        /// <param name="configKeys"><see cref="IConfiguration"/> context to be used to load this <typeparamref name="TOptions"/>.</param>
        /// <returns>An instance of <see cref="IHostBuilder"/> with the <typeparamref name="TOptions"/> configured.</returns>
        public static IHostBuilder AddStepDependency<TOptions>(this IHostBuilder source, string context, string[] configKeys)
            where TOptions : class, new()
        {
            source.AssertParameterIsNotNull(nameof(source));
            context.AssertParameterIsNotWhiteSpace(nameof(context));
            configKeys.AssertParameterIsNotNull(nameof(configKeys));

            var stepConfig = AddStepDependencyExtensions.RetrieveCurrentStepContexts<TOptions>(source);

            List<string> workingConfigKeys = new List<string>()
            {
                context
            };

            if (configKeys.Any())
            {
                workingConfigKeys.AddRange(configKeys);
            }

            // Build the Func that is going to be called to handle this context.
            stepConfig[typeof(TOptions)].Add(context, (IServiceProvider services) =>
            {
                return AddStepDependencyExtensions.BindStepContextFromConfiguration<TOptions>(services, workingConfigKeys.ToArray());
            });

            return source;
        }

        /// <summary>
        /// Add a step dependency instance under a specific <paramref name="context"/>.
        /// </summary>
        /// <typeparam name="TOptions">A <see cref="Type"/> that will be injected into a Step.</typeparam>
        /// <param name="source">The <see cref="IHostBuilder"/> on which this dependency should be configured.</param>
        /// <param name="context">The context under which this dependency will be configured.</param>
        /// <param name="instance">An instance of <typeparamref name="TOptions"/>.</param>
        /// <returns>An instance of <see cref="IHostBuilder"/> with the <typeparamref name="TOptions"/> configured.</returns>
        public static IHostBuilder AddStepDependency<TOptions>(this IHostBuilder source, string context, TOptions instance)
            where TOptions : class
        {
            source.AssertParameterIsNotNull(nameof(source));
            context.AssertParameterIsNotWhiteSpace(nameof(context));
            instance.AssertParameterIsNotNull(nameof(instance));

            var stepConfig = AddStepDependencyExtensions.RetrieveCurrentStepContexts<TOptions>(source);

            // Build the Func that is going to be called to handle this context.
            stepConfig[typeof(TOptions)].Add(context, (IServiceProvider services) =>
                                                                {
                                                                    return instance;
                                                                });

            return source;
        }

        /// <summary>
        /// Add a step dependency under a specific <paramref name="context"/> that will be constructed at run-time.
        /// </summary>
        /// <typeparam name="TOptions">A <see cref="Type"/> that will be injected into a Step.</typeparam>
        /// <param name="source">The <see cref="IHostBuilder"/> on which this dependency should be configured.</param>
        /// <param name="context">The context under which this dependency will be configured.</param>
        /// <param name="instantiate">A function that returns an instance of <typeparamref name="TOptions"/>.</param>
        /// <returns>An instance of <see cref="IHostBuilder"/> with the <typeparamref name="TOptions"/> configured.</returns>
        public static IHostBuilder AddStepDependency<TOptions>(this IHostBuilder source, string context, Func<IServiceProvider, TOptions> instantiate)
            where TOptions : class
        {
            source.AssertParameterIsNotNull(nameof(source));
            context.AssertParameterIsNotWhiteSpace(nameof(context));
            instantiate.AssertParameterIsNotNull(nameof(instantiate));

            var stepConfig = AddStepDependencyExtensions.RetrieveCurrentStepContexts<TOptions>(source);

            // Add the pre-built function to the step dependency configuration.
            stepConfig[typeof(TOptions)].Add(context, instantiate);

            return source;
        }

        /// <summary>
        /// Build the step context entries into the <see cref="IServiceProvider"/> for consumption at run-time by the Pipeline.
        /// </summary>
        /// <param name="source">The <see cref="IHostBuilder"/> on which this dependency should be built.</param>
        /// <returns>An instance of <see cref="IHostBuilder"/> with the step context entries added to <see cref="IServiceProvider"/>.</returns>
        internal static IHostBuilder BuildStepContext(IHostBuilder source)
        {
            source.AssertParameterIsNotNull(nameof(source));

            foreach (string key in source.Properties.Keys)
            {
                if (key.StartsWith(AddStepDependencyExtensions.STEP_CONFIG_CONTEXT, StringComparison.OrdinalIgnoreCase))
                {
                    dynamic sourceDictionary = source.Properties[key];

                    Type dependencyType = source.RetrieveValidStepContextType(key);

                    var contextBaseTypes = new Type[]
                        {
                            typeof(string),
                            dependencyType
                        };

                    // An interface is used to prevent close coupling in Pipelines.Core.
                    var contextBase = typeof(IDictionary<,>);
                    var contextType = contextBase.MakeGenericType(contextBaseTypes);

                    // A concrete implementation is required to add items to the collection.
                    var implementationBase = typeof(Dictionary<,>);
                    var implementationType = implementationBase.MakeGenericType(contextBaseTypes);

                    // Configure the function that will load the contextType.
                    source.ConfigureServices(services =>
                    {
                        services.AddTransient(contextType, (IServiceProvider provider) =>
                        {
                            // At pipelilne build-time, the pipeline needs an IDictionary<string, <type>>
                            // with all of the instances populated.
                            dynamic? serviceDictionary = Activator.CreateInstance(implementationType);

                            /*
                             * serviceDictionary cannot be null because base type is Dictionary<,>
                             * implementationType used to create serviceDictionary cannot be null because dependencyType is not null.
                             * dependencyType cannot be null because it is checked for null before usage by RetrieveValidStepContextType().
                             * Disable CS8602 and CS8603 because serviceDictionary cannot be null.
                             */

#pragma warning disable CS8602,CS8603
                            foreach (string key in sourceDictionary[dependencyType].Keys)
                            {
                                serviceDictionary.Add(key, sourceDictionary[dependencyType][key].Invoke(provider));
                            }

                            return serviceDictionary;
#pragma warning restore CS8602, CS8603
                        });
                    });
                }
            }

            return source;
        }

        /// <summary>
        /// Retrieve an instance of <typeparamref name="TOptions"/> from <see cref="IConfiguration"/> boound from <paramref name="configKeys"/>.
        /// </summary>
        /// <typeparam name="TOptions">A <see cref="Type"/> that will be injected into a Step.</typeparam>
        /// <param name="services">An <see cref="IServiceProvider"/> to provide the <see cref="IConfiguration"/> used to bind values.</param>
        /// <param name="configKeys"><see cref="IConfiguration"/> context to be used to load this <typeparamref name="TOptions"/>.</param>
        /// <returns>An instance of <see cref="IHostBuilder"/> with the <typeparamref name="TOptions"/> configured.</returns>
        internal static TOptions BindStepContextFromConfiguration<TOptions>(IServiceProvider services, string[] configKeys)
            where TOptions : class, new()
        {
            services.AssertParameterIsNotNull(nameof(services));
            configKeys.AssertParameterIsNotNull(nameof(configKeys));

            IConfiguration config = services.GetRequiredService<IConfiguration>();

            var results = new TOptions();

            // Bind to root configuration with no context.
            config.Bind(results);

            // Bind to type name configuration with no context.
            config.GetSection(typeof(TOptions).Name).Bind(results);

            foreach (string key in configKeys)
            {
                if (!string.IsNullOrWhiteSpace(key))
                {
                    config.GetSection(key).Bind(results);
                }
            }

            return results;
        }

        /// <summary>
        /// Retrieve step dependencies for <typeparamref name="TOptions"/> for all contexts.
        /// </summary>
        /// <typeparam name="TOptions">A <see cref="Type"/> that will be injected into a Step.</typeparam>
        /// <param name="source">The <see cref="IHostBuilder"/> on which this dependency should be configured.</param>
        /// <returns>A dictionary of all contexts for <typeparamref name="TOptions"/>.</returns>
        internal static Dictionary<Type, Dictionary<string, Func<IServiceProvider, TOptions>>> RetrieveCurrentStepContexts<TOptions>(IHostBuilder source)
            where TOptions : class
        {
            source.AssertParameterIsNotNull(nameof(source));

            string keyName = $"{AddStepDependencyExtensions.STEP_CONFIG_CONTEXT}{typeof(TOptions).FullName}";
            var results = new Dictionary<Type, Dictionary<string, Func<IServiceProvider, TOptions>>>
            {
                { typeof(TOptions), new Dictionary<string, Func<IServiceProvider, TOptions>>() }
            };

            if (source.Properties.ContainsKey(keyName))
            {
                // Since the key exists, pull the value from IHostBuilder.Properties, convert to the correct type, assign it to working storage.
                results = source.Properties[keyName] as Dictionary<Type, Dictionary<string, Func<IServiceProvider, TOptions>>>;

                if (results == null)
                {
                    throw new InvalidOperationException(Resources.CONTEXT_WRONG_TYPE(CultureInfo.CurrentCulture, typeof(TOptions).FullName));
                }
            }
            else
            {
                // Assign the newly created dictionary back to the IHostBuilder.Properties collection
                source.Properties.Add(keyName, results);
            }

            return results;
        }
    }
}
