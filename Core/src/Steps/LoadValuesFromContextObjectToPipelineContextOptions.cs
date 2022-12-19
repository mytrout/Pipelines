// <copyright file="LoadValuesFromContextObjectToPipelineContextOptions.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2022 Chris Trout
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
namespace MyTrout.Pipelines.Steps
{
    using MyTrout.Pipelines.Core;
    using System;

    /* NOTE TO DEVELOPERS:
     *
     * All unit tests for this class are handled by the LoadValuesFromContextObjectToPipelineContextStep unit tests.
     *
     */

    /// <summary>
    /// Provides caller-configurable options to change the behavior of <see cref="LoadValuesFromContextObjectToPipelineContextStep{TObject}"/>.
    /// </summary>
    public class LoadValuesFromContextObjectToPipelineContextOptions : IContextNameBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadValuesFromContextObjectToPipelineContextOptions"/> class.
        /// </summary>
        public LoadValuesFromContextObjectToPipelineContextOptions()
        {
            this.ContextNameBuilder = this;
        }

        /// <summary>
        /// Gets or sets a function to build the <see cref="IPipelineContext.Items"/> context name from the type name and property name.
        /// </summary>
        [Obsolete("Use the ContextNameBuilder property which allows dependency injection and uses an interface in lieu of BuildContextName")]
        public Func<string, string, string> BuildContextNameFunction { get; set; } = LoadValuesFromContextObjectToPipelineContextOptions.BuildDefaultName;

        /// <summary>
        /// Gets or sets a function to build the <see cref="IPipelineContext.Items"/> context name from the type name and property name.
        /// </summary>
        [FromServices]
        public IContextNameBuilder ContextNameBuilder { get; set; }

        /// <summary>
        /// Gets or sets the input object context name to convert into property values in <see cref="IPipelineContext.Items"/>.
        /// </summary>
        public string InputObjectContextName { get; set; } = PipelineContextConstants.INPUT_OBJECT;

        /// <summary>
        /// Builds a string representing that returns the <paramref name="typeName"/>.<paramref name="propertyName"/>.
        /// </summary>
        /// <param name="typeName">the name of the Options type.</param>
        /// <param name="propertyName">the name of the property being added to the <see cref="IPipelineContext.Items"/>.</param>
        /// <returns>A string representing the property's Context Name.</returns>
        public static string BuildDefaultName(string typeName, string propertyName)
        {
            return $"{typeName}.{propertyName}";
        }

#pragma warning disable CS0618
        /// <inheritdoc />
        public string BuildContextName(string typeName, string propertyName)
        {
            // This check allows the Obsolete BuildContextNameFunction to continue to take precedence over the
            // built-in function BuildDefaultName.  This check prevents this change from breaking existing clients.
            if (this.BuildContextNameFunction != LoadValuesFromContextObjectToPipelineContextOptions.BuildDefaultName)
            {
                return this.BuildContextNameFunction.Invoke(typeName, propertyName);
            }

            if (this.ContextNameBuilder != this)
            {
                return this.ContextNameBuilder.BuildContextName(typeName, propertyName);
            }

            return LoadValuesFromContextObjectToPipelineContextOptions.BuildDefaultName(typeName, propertyName);
        }
#pragma warning restore CS0618
    }
}
