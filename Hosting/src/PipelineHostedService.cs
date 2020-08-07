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
    using MyTrout.Pipelines.Core;
    using System;
    using System.Globalization;
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
        /// <param name="pipelineBuilder">The <see cref="PipelineBuilder" /> hosted by this <see cref="IHost" />.</param>
        /// <param name="context">The <see cref="PipelineContext" /> used by this <see cref="PipelineContext" />.</param>
        public PipelineHostedService(
                                ILogger<PipelineHostedService> logger,
                                IHostApplicationLifetime applicationLifetime,
                                IStepActivator stepActivator,
                                PipelineBuilder pipelineBuilder,
                                PipelineContext context)
        {
            this.ApplicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.PipelineBuilder = pipelineBuilder ?? throw new ArgumentNullException(nameof(pipelineBuilder));
            this.StepActivator = stepActivator ?? throw new ArgumentNullException(nameof(stepActivator));
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
        public PipelineBuilder PipelineBuilder { get; }

        /// <summary>
        /// Gets the <see cref="PipelineContext"/> used by the pipeline.
        /// </summary>
        public PipelineContext Context { get; }

        /// <summary>
        /// Gets the activator which creates instances of the step <see cref="Type" />.
        /// </summary>
        public IStepActivator StepActivator { get; }

        /* NOTES TO DEVELOPERS: The original implementation below used an async lambda until the Task.RunSynchronously() blocking
         *                      thread method was found.  This stupidity is required because GenericHost implementors
         *                      did not account for an async ApplicationStarted delegate and how exceptions could be thrown
         *                      from a void delegate returning from the ApplicationStarted capability which would need to be
         *                      handled via an AppDomain.Current.UnhandledException event handler.  The implementation below does
         *                      not suffer from the same issue for **MOST** exceptions at the cost of blocking threads on a delegate call.
         */

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.Logger.LogDebug(Resources.CALLING_METHOD_IN_TYPE(CultureInfo.CurrentCulture, nameof(this.StartAsync), nameof(PipelineHostedService)));

            this.ApplicationLifetime.ApplicationStarted.Register(() => this.OnApplicationStarted(this.ApplicationLifetime.ApplicationStopping));
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.Logger.LogDebug(Resources.CALLING_METHOD_IN_TYPE(CultureInfo.CurrentCulture, nameof(this.StopAsync), nameof(PipelineHostedService)));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Forces the OnApplicationStartedAsync to be run synchronously and block the calling thread.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the application execution has been aborted.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD002:Avoid problematic synchronous waits", Justification = "Using GetResult() in Console Applications where multiple threads are not being run is acceptable.")]

        private void OnApplicationStarted(CancellationToken cancellationToken)
        {
            this.OnApplicationStartedAsync(cancellationToken).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Executes the configured pipelines when the application is started.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the application execution has been aborted.</param>
        /// <returns>A <see cref="Task" />.</returns>
        private async Task OnApplicationStartedAsync(CancellationToken cancellationToken)
        {
            this.Context.CancellationToken = cancellationToken;

            try
            {
                this.Logger.LogInformation(Resources.PIPELINE_TASKS_STARTING(CultureInfo.CurrentCulture));

                IPipelineRequest pipeline = this.PipelineBuilder.Build(this.StepActivator);

                await pipeline.InvokeAsync(this.Context).ConfigureAwait(false);

                this.Logger.LogInformation(Resources.TASKS_ARE_COMPLETED(CultureInfo.CurrentCulture));
            }
#pragma warning disable CA1031 // Prevent any Exception-derived Exception from causing the application to crash.
            catch (Exception exc)
            {
                this.Context.Errors.Add(exc);
            }
#pragma warning restore CA1031 // Do not catch general exception types
            finally
            {
                foreach (var error in this.Context.Errors)
                {
                    this.Logger.LogError(error, Resources.HANDLED_EXCEPTION(CultureInfo.CurrentCulture));
                }

                this.ApplicationLifetime.StopApplication();
            }
        }
    }
}