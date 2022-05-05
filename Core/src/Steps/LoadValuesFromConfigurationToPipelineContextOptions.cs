// <copyright file="LoadValuesFromConfigurationToPipelineContextOptions.cs" company="Chris Trout">
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
    // This warning cannot be suppressed in the GlobalSuppressions file.
    // The ~Options class is specifically designed to be configured by IConfiguration and Dependency Injection
    // All values SHOULD BE checked at run-time to determine if the value is valid and rejected if no value is provided.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    using Microsoft.Extensions.Configuration;
    using MyTrout.Pipelines.Core;
    using System.Collections.Generic;

    /*
     *  IMPORTANT NOTE: As long as this class only contains compiler-generated functionality, it requires no unit tests.
     */

    /// <summary>
    /// Provides caller-configurable options to change the behavior of <see cref="LoadValuesFromConfigurationToPipelineContextStep"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class LoadValuesFromConfigurationToPipelineContextOptions
    {
        /// <summary>
        /// Gets or sets the the names to be loaded from <see cref="IConfiguration"/>.
        /// </summary>
        public List<string> ConfigurationNames { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the <see cref="IConfiguration"/> from which values will be loaded.
        /// </summary>
        [FromServices]
        public IConfiguration Configuration { get; set; }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
