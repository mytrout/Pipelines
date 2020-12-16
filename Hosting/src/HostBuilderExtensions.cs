// <copyright file="HostBuilderExtensions.cs" company="Chris Trout">
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
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Provides standardized parameter validation for any method.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Guarantees that the Property in IHostBuilder has the correct construction.
        /// </summary>
        /// <param name="source">The <see cref="IHostBuilder"/> from which to retrieve the StepContext type.</param>
        /// <param name="key">The property name for <see cref="IHostBuilder.Properties"/>.</param>
        /// <returns>The <see cref="Type"/> of the Step Context provided by the <paramref name="key"/>.</returns>
        public static Type RetrieveValidStepContextType(this IHostBuilder source, string key)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            // Verify that key is valid and the value is a Dictionary<Type, Dictionary<string, Func<IServiceProvider, TOptions>>> type.
            if (key.StartsWith(AddStepDependencyExtensions.STEP_CONFIG_CONTEXT, StringComparison.OrdinalIgnoreCase)
                    && source.Properties.ContainsKey(key)
                    && source.Properties[key].GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>)
                    && source.Properties[key].GetType().GetGenericArguments()[0] == typeof(Type)
                    && source.Properties[key].GetType().GetGenericArguments()[1].GetGenericTypeDefinition() == typeof(Dictionary<,>)
                    && source.Properties[key].GetType().GetGenericArguments()[1].GetGenericArguments()[0] == typeof(string)
                    && source.Properties[key].GetType().GetGenericArguments()[1].GetGenericArguments()[1].GetGenericTypeDefinition() == typeof(Func<,>)
                    && source.Properties[key].GetType().GetGenericArguments()[1].GetGenericArguments()[1].GetGenericArguments()[0] == typeof(IServiceProvider)
                    && source.Properties[key].GetType().GetGenericArguments()[1].GetGenericArguments()[1].GetGenericArguments()[1].IsClass
                    && key == $"{AddStepDependencyExtensions.STEP_CONFIG_CONTEXT}{source.Properties[key].GetType().GetGenericArguments()[1].GetGenericArguments()[1].GetGenericArguments()[1].FullName}")
            {
                return source.Properties[key].GetType().GetGenericArguments()[1].GetGenericArguments()[1].GetGenericArguments()[1];
            }
            else
            {
                string typeName = key.Remove(0, AddStepDependencyExtensions.STEP_CONFIG_CONTEXT.Length);
                throw new InvalidOperationException(Resources.CONTEXT_WRONG_TYPE(CultureInfo.CurrentCulture, typeName));
            }
        }
    }
}
