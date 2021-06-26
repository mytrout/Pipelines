// <copyright file="UsePipelineExtensions.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Hosting
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Hosting;
    using MyTrout.Pipelines.Core;
    using System;

    /// <summary>
    /// Extensions to configure pipeline services in <see cref="IHostBuilder" />.
    /// </summary>
    public static class UsePipelineExtensions
    {
        /// <summary>
        /// Configures a pipeline service with the default <see cref="IStepActivator" />.
        /// </summary>
        /// <param name="source">An <see cref="IHostBuilder" /> instance.</param>
        /// <param name="builder">The pipelines to be configured.</param>
        /// <returns>An <see cref="IHostBuilder" /> with the specified pipelines configured.</returns>
        public static IHostBuilder UsePipeline(this IHostBuilder source, PipelineBuilder builder)
        {
            return source.UsePipeline<StepActivator>(builder);
        }

        /// <summary>
        /// Configures a pipeline service with a default <see cref="IStepActivator" />.
        /// </summary>
        /// <param name="source">An <see cref="IHostBuilder" /> instance.</param>
        /// <param name="pipelineAction">An <see cref="Action{PipelineBuilder}"/> to build the <see cref="PipelineBuilder"/>.</param>
        /// <returns>An <see cref="IHostBuilder" /> with the specified pipelines configured.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is <see langword="null" />.</exception>
        public static IHostBuilder UsePipeline(this IHostBuilder source, Action<PipelineBuilder> pipelineAction)
        {
            return source.UsePipeline<StepActivator>(pipelineAction);
        }

        /// <summary>
        /// Configures a pipeline service with a custom <see cref="IStepActivator" />.
        /// </summary>
        /// <typeparam name="TStepActivator">A custom <see cref="IStepActivator" /> implementation.</typeparam>
        /// <param name="source">An <see cref="IHostBuilder" /> instance.</param>
        /// <param name="builder">The pipelines to be configured.</param>
        /// <returns>An <see cref="IHostBuilder" /> with the specified pipelines configured.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is <see langword="null" />.</exception>
        public static IHostBuilder UsePipeline<TStepActivator>(this IHostBuilder source, PipelineBuilder builder)
            where TStepActivator : class, IStepActivator
        {
            source.AssertParameterIsNotNull(nameof(source));
            source.AssertParameterIsNotNull(nameof(builder));

            // TO FUTURE DEVELOPERS:
            // Using TryAdd for IStepActivator means that if a calling developer has already registered an IStepActivator,
            // then this registration will not replace that registration.
            var result = source.ConfigureServices((hostContext, services) =>
                            {
                                services.AddLogging();
                                services.AddSingleton(builder);
                                services.TryAdd(new ServiceDescriptor(typeof(IStepActivator), typeof(TStepActivator), ServiceLifetime.Transient));
                                services.AddHostedService<PipelineHostedService>();
                                services.AddSingleton(new PipelineContext());
                            })
                            .UseConsoleLifetime(options => options.SuppressStatusMessages = true);

            return result;
        }

        /// <summary>
        /// Configures a pipeline service with the default <see cref="IStepActivator" />.
        /// </summary>
        /// <typeparam name="TStepActivator">A custom <see cref="IStepActivator" /> implementation.</typeparam>
        /// <param name="source">An <see cref="IHostBuilder" /> instance.</param>
        /// <param name="pipelineAction">An <see cref="Action{PipelineBuilder}"/> to build the <see cref="PipelineBuilder"/>.</param>
        /// <returns>An <see cref="IHostBuilder" /> with the specified pipelines configured.</returns>
        public static IHostBuilder UsePipeline<TStepActivator>(this IHostBuilder source, Action<PipelineBuilder> pipelineAction)
            where TStepActivator : class, IStepActivator
        {
            source.AssertParameterIsNotNull(nameof(source));
            pipelineAction.AssertParameterIsNotNull(nameof(pipelineAction));

            PipelineBuilder builder = new PipelineBuilder();

            pipelineAction.Invoke(builder);

            return source.UsePipeline<TStepActivator>(builder);
        }
    }
}