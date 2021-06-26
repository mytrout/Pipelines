// <copyright file="ParameterCreationResult.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2021 Chris Trout
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

namespace MyTrout.Pipelines.Core
{
    /// <summary>
    /// Provides the result when a Parameter Creation attempt is made by one of the <see cref="StepActivator"/>-configured delegates.
    /// </summary>
    public record ParameterCreationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterCreationResult" /> class with the requested parameters.
        /// </summary>
        /// <param name="instance">A working or completed instance of the parameter being constructed..</param>
        /// <param name="skipRemainingCreators">A valud indicating whether other delegates should be skipped.</param>
        public ParameterCreationResult(object? instance, bool skipRemainingCreators = false)
        {
            this.Instance = instance;
            this.SkipRemainingCreators = skipRemainingCreators;
        }

        /// <summary>
        /// Gets or sets an instance of the parameter that may either be completed or require further processing, depending upon the <see cref="SkipRemainingCreators"/> value.
        /// </summary>
        public object? Instance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether other delegates should be skipped.
        /// </summary>
        public bool SkipRemainingCreators { get; set; } = false;
    }
}
