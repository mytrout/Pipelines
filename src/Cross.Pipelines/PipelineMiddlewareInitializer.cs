// <copyright file="PipelineMiddlewareInitializer.cs" company="Chris Trout">
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
    using System.Linq;

    public class PipelineMiddlewareInitializer : IPipelineMiddlewareInitializer
    {
        public PipelineMiddlewareInitializer(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IServiceProvider ServiceProvider { get; }

        public object InitializeMiddleware(Type middlewareType, PipelineRequest nextRequest)
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
