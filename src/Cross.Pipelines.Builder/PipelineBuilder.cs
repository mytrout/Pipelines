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

namespace Cross.Pipelines.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Builds the Pipeline by adding different middleware implenentations.
    /// </summary>
    public class PipelineBuilder
    {
        public PipelineBuilder(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public virtual string MethodName => "InvokeAsync";

        public IServiceProvider ServiceProvider { get; }

        private Stack<Type> MiddlewareTypes { get; } = new Stack<Type>();

        public PipelineBuilder AddMiddleware<T>()
            where T : class
        {
            return this.AddMiddleware(typeof(T));
        }

        public PipelineBuilder AddMiddleware(Type middlewareType)
        {
            if (middlewareType == null)
            {
                throw new ArgumentNullException(nameof(middlewareType));
            }

            if (middlewareType.IsValueType)
            {
                throw new InvalidOperationException($"{nameof(middlewareType)} must be a reference type.");
            }

            var method = middlewareType.GetMethod(this.MethodName);
            var parameters = method?.GetParameters();
            if (method == null
                    || parameters == null
                    || parameters.Length != 1
                    || parameters[0].ParameterType != typeof(PipelineContext))
            {
                throw new InvalidOperationException("Middleware must contain an InvokeAsync method with one parameter of type 'PipelineContext'.");
            }

            this.MiddlewareTypes.Push(middlewareType);
            return this;
        }

        public PipelineRequest Build()
        {
            // Configures the "final" delegate in the Pipeline.
            var nextRequest = new PipelineRequest(context => Task.CompletedTask);

            while (this.MiddlewareTypes.Any())
            {
                Type middlewareType = this.MiddlewareTypes.Pop();

                if (middlewareType == null)
                {
                    throw new InvalidOperationException("A 'null' middleware was added to the Pipeline which prevents the pipeline from being built as requested.");
                }

                object nextInstance = this.ConstructNextInstance(middlewareType, nextRequest);

                if (nextInstance == null)
                {
                    throw new InvalidOperationException($"ServiceProvider does not contain one of the parameters required for '{middlewareType.Name}'.");
                }

                var nextInvoke = middlewareType.GetMethod(this.MethodName);

                if (nextInvoke == null)
                {
                    throw new InvalidOperationException($"'{middlewareType.Name}' middleware does not contain the '{this.MethodName}' method.");
                }

                nextRequest = (PipelineRequest)nextInvoke.CreateDelegate(typeof(PipelineRequest), nextInstance);
            }

            return nextRequest;
        }

        protected virtual object ConstructNextInstance(Type middlewareType, PipelineRequest nextRequest)
        {
            object result = null;
            bool nextRequestParameterFound = false;

            foreach (var constructor in middlewareType.GetConstructors()
                                                .OrderByDescending(x => x.GetParameters().Length))
            {
                var parametersForConstructor = new List<object>();
                foreach (var parameter in constructor.GetParameters())
                {
                    if (parameter.ParameterType == typeof(PipelineRequest))
                    {
                        parametersForConstructor.Add(nextRequest);
                        nextRequestParameterFound = true;
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
                catch (Exception)
                {
                    // Swallow the exception and try to construct the next constructor
                }
            }

            if (!nextRequestParameterFound)
            {
                throw new InvalidOperationException($"'{middlewareType.Name}' middleware does not contain a constructor that has a PipelineRequest parameter.");
            }

            return result;
        }
    }
}
