// <copyright file="PipelineHostedService.cs" company="Chris Trout">
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
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a service to run pipelines using the <see cref="IHost" />.
    /// </summary>
    public class PipelineHostedService : IHostedService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineHostedService" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for the pipeline application service.</param>
        /// <param name="applicationLifetime">The lifetime of the <see cref="IHost" /> application.</param>
        /// <param name="stepActivator">The activator that constructs steps from <see cref="Type" />.</param>
        /// <param name="pipelineBuilders">The list of <see cref="PipelineBuilder" /> hosted by this <see cref="IHost" />.</param>
        public PipelineHostedService(
                                ILogger<PipelineHostedService> logger,
                                IHostApplicationLifetime applicationLifetime,
                                IStepActivator stepActivator,
                                params PipelineBuilder[] pipelineBuilders)
        {
            this.ApplicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.StepActivator = stepActivator ?? throw new ArgumentNullException(nameof(stepActivator));
            this.PipelineBuilders = pipelineBuilders ?? throw new ArgumentNullException(nameof(pipelineBuilders));
        }

        /// <summary>
        /// Gets the lifetime of the hosted application.
        /// </summary>
        public IHostApplicationLifetime ApplicationLifetime { get; }

        /// <summary>
        /// Gets the logger for this pipeline application service.
        /// </summary>
        public ILogger<PipelineHostedService> Logger { get; }

        /// <summary>
        /// Gets the list of <see cref="PipelineBuilder" /> hosted by this <see cref="IHost" />.
        /// </summary>
        public IEnumerable<PipelineBuilder> PipelineBuilders { get; }

        /// <summary>
        /// Gets the activator which creates instances of the step <see cref="Type" />.
        /// </summary>
        public IStepActivator StepActivator { get; }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.Logger.LogDebug(Resources.CALLING_METHOD_IN_TYPE(CultureInfo.CurrentCulture, nameof(this.StartAsync), nameof(PipelineHostedService)));

            this.ApplicationLifetime.ApplicationStarted.Register(async () => await this.OnApplicationStarted(this.ApplicationLifetime.ApplicationStopping).ConfigureAwait(false));
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.Logger.LogDebug(Resources.CALLING_METHOD_IN_TYPE(CultureInfo.CurrentCulture, nameof(this.StopAsync), nameof(PipelineHostedService)));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Executes the configured pipelines when the application is started.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the application execution has been aborted.</param>
        /// <returns>A <see cref="Task" />.</returns>
        private async Task OnApplicationStarted(CancellationToken cancellationToken)
        {
            this.Logger.LogInformation(Resources.PIPELINE_TASKS_STARTING(CultureInfo.CurrentCulture));

            await Task.Delay(5).ConfigureAwait(false);

            IEnumerable<PipelineRequest> pipelines = this.PipelineBuilders.Select(
                                            x => x.Build(this.StepActivator));

            IEnumerable<Task> results = pipelines.Select(x => x.Invoke(
                                                new PipelineContext()
                                                {
                                                    CancellationToken = cancellationToken
                                                }));

            this.Logger.LogDebug(Resources.BUILDERS_CONVERTED_TO_TASKS(CultureInfo.CurrentCulture));

            await Task.WhenAll(results).ConfigureAwait(false);

            this.Logger.LogInformation(Resources.TASKS_ARE_COMPLETED(CultureInfo.CurrentCulture));

            this.ApplicationLifetime.StopApplication();
        }
    }
}