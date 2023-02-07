// <copyright file="PipelineHostedService.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2019-2022 Chris Trout
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
    public class PipelineHostedService : BackgroundService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineHostedService" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for the pipeline application service.</param>
        /// <param name="stepActivator">The activator that constructs steps from <see cref="Type" />.</param>
        /// <param name="pipelineBuilder">The <see cref="PipelineBuilder" /> hosted by this <see cref="IHost" />.</param>
        /// <param name="context">The <see cref="PipelineContext" /> used by this <see cref="PipelineContext" />.</param>
        public PipelineHostedService(
                                ILogger<PipelineHostedService> logger,
                                IStepActivator stepActivator,
                                PipelineBuilder pipelineBuilder,
                                PipelineContext context)
            : base()
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.PipelineBuilder = pipelineBuilder ?? throw new ArgumentNullException(nameof(pipelineBuilder));
            this.StepActivator = stepActivator ?? throw new ArgumentNullException(nameof(stepActivator));
        }

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

        /// <inheritdoc />
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            this.Logger.LogDebug(Resources.CALLING_METHOD_IN_TYPE(CultureInfo.CurrentCulture, nameof(this.StartAsync), nameof(PipelineHostedService)));

            return base.StartAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            this.Logger.LogDebug(Resources.CALLING_METHOD_IN_TYPE(CultureInfo.CurrentCulture, nameof(this.StopAsync), nameof(PipelineHostedService)));
            return base.StopAsync(cancellationToken);
        }
#pragma warning disable S125
        /*
         * NOTE TO FUTURE DEVELOPERS: According to CS1058, all non-CLS exceptions will be wrapped in a RuntimeWrappedException
         *                            unless a specific assembly level attribute is configured.
         *                            RuntimeCompatibilityAttribute(WrapNonExceptionThrows = false)]
         *                            DO NOT CONFIGURE THIS ATTRIBUTE unless you add a catch() statement here or exceptions could be unhandled.
        */
#pragma warning restore S125

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                this.Context.CancellationToken = stoppingToken;

                this.Logger.LogInformation(Resources.PIPELINE_TASKS_STARTING(CultureInfo.CurrentCulture, this.Context.PipelineName));

                DateTime startingPipelineBuild = DateTime.UtcNow;

                IPipelineRequest pipeline = this.PipelineBuilder.Build(this.StepActivator);

                this.Logger.LogDebug(Resources.PIPELINE_BUILD_ELAPSED_TIME(CultureInfo.CurrentCulture), this.Context.PipelineName, (DateTime.UtcNow - startingPipelineBuild).TotalSeconds);

                DateTime startingPipelineExecution = DateTime.UtcNow;

                await pipeline.InvokeAsync(this.Context).ConfigureAwait(false);

                this.Logger.LogDebug(Resources.PIPELINE_EXECUTION_ELAPSED_TIME(CultureInfo.CurrentCulture), this.Context.PipelineName, (DateTime.UtcNow - startingPipelineExecution).TotalSeconds);

                this.Logger.LogInformation(Resources.TASKS_ARE_COMPLETED(CultureInfo.CurrentCulture, this.Context.PipelineName));
            }
            catch (Exception exc)
            {
                this.Context.Errors.Add(exc);
            }
            finally
            {
                foreach (var error in this.Context.Errors)
                {
                    this.Logger.LogError(error, Resources.HANDLED_EXCEPTION(CultureInfo.CurrentCulture, this.Context.PipelineName));
                }

                this.Logger.LogDebug(Resources.PIPELINE_TOTAL_EXECUTION_ELAPSED_TIME(CultureInfo.CurrentCulture, this.Context.PipelineName), (DateTime.UtcNow - this.Context.Timestamp).TotalSeconds);
            }
        }
    }
}