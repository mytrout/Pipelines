// <copyright file="PipelineBuilder.cs" company="Chris Trout">
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
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Builds the Pipeline by adding different middleware implenentations.
    /// </summary>
    public class PipelineBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineBuilder" /> class with dependency injection.
        /// </summary>
        /// <param name="serviceProvider">Dependency Injection provider.</param>
        public PipelineBuilder(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            var middlewareInitializer = this.ServiceProvider.GetService(typeof(IPipelineMiddlewareInitializer)) as IPipelineMiddlewareInitializer;

            // If Dependency Injection has a IPipelineTypeInitializer, use it instead of the default.
            this.PipelineMiddlewareInitializer = middlewareInitializer ?? new PipelineMiddlewareInitializer(serviceProvider);
        }

        /// <summary>
        /// Gets the name of the method used by the pipeline.
        /// </summary>
        public virtual string MethodName => "InvokeAsync";

        /// <summary>
        /// Gets the Dependency Injection Provider used to provide parameters to the Middleware instances.
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the capability to intialize middleware using the dependency injection provider.
        /// </summary>
        public IPipelineMiddlewareInitializer PipelineMiddlewareInitializer { get; }

        /// <summary>
        /// Gets the types registered as Middleware for the Pipeline.
        /// </summary>
        private Stack<Type> MiddlewareTypes { get; } = new Stack<Type>();

        /// <summary>
        /// Adds middleware to this pipeline.
        /// </summary>
        /// <typeparam name="T">The generic type to add to the pipeline.</typeparam>
        /// <returns>Returns a <see cref="PipelineBuilder" /> with the middleware type added.</returns>
        public PipelineBuilder AddMiddleware<T>()
            where T : class
        {
            return this.AddMiddleware(typeof(T));
        }

        /// <summary>
        /// Adds middleware to this pipeline.
        /// </summary>
        /// <param name="middlewareType">The <see cref="Type" /> to add to the pipeline.</param>
        /// <returns>Returns a <see cref="PipelineBuilder" /> with the middleware type added.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="middlewareType"/> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="middlewareType"/> is a value type or does not have the appropriate.</exception>
        public PipelineBuilder AddMiddleware(Type middlewareType)
        {
            if (middlewareType == null)
            {
                throw new ArgumentNullException(nameof(middlewareType));
            }

            if (middlewareType.IsValueType)
            {
                throw new InvalidOperationException(Resources.TYPE_MUST_BE_REFERENCE(CultureInfo.CurrentCulture, nameof(middlewareType)));
            }

            var method = middlewareType.GetMethod(this.MethodName);
            var parameters = method?.GetParameters();
            if (method == null
                    || parameters == null
                    || parameters.Length != 1
                    || parameters[0].ParameterType != typeof(PipelineContext))
            {
                throw new InvalidOperationException(Resources.INVOKEASYNC_DOES_NOT_EXIST(CultureInfo.CurrentCulture));
            }

            this.MiddlewareTypes.Push(middlewareType);
            return this;
        }

        /// <summary>
        /// Builds a pipeline from the configured middleware.
        /// </summary>
        /// <returns>A pipeline to execute.</returns>
        public PipelineRequest Build()
        {
            // Configures the "final" delegate in the Pipeline.
            var nextRequest = new PipelineRequest(context => Task.CompletedTask);

            while (this.MiddlewareTypes.Any())
            {
                Type middlewareType = this.MiddlewareTypes.Pop();

                if (middlewareType == null)
                {
                    throw new InvalidOperationException(Resources.NULL_MIDDLEWARE(CultureInfo.CurrentCulture));
                }

                object nextInstance = this.PipelineMiddlewareInitializer.InitializeMiddleware(middlewareType, nextRequest);

                if (nextInstance == null)
                {
                    throw new InvalidOperationException(Resources.SERVICEPROVIDER_LACKS_PARAMETER(CultureInfo.CurrentCulture, middlewareType.Name));
                }

                // if the PipelineMiddlewareInitializer returns a different type for any reason,
                // using nextInstance.GetType() guarantees it contains the correct MethodName.
                var nextInvoke = nextInstance.GetType().GetMethod(this.MethodName);

                if (nextInvoke == null)
                {
                    throw new InvalidOperationException(Resources.INVOKEASYNC_MISSING(CultureInfo.CurrentCulture));
                }

                nextRequest = (PipelineRequest)nextInvoke.CreateDelegate(typeof(PipelineRequest), nextInstance);
            }

            return nextRequest;
        }
    }
}
