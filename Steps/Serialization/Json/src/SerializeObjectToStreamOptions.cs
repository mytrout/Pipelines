// <copyright file="SerializeObjectToStreamOptions.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.Serialization.Json
{
    using MyTrout.Pipelines.Core;
    using System;
    using System.Text.Json;

    /// <summary>
    /// Provides user-configuration options to alter the behavior of the <see cref="SerializeObjectToStreamStep{TObject}"/>.
    /// </summary>
    [Obsolete("Use the MyTrout.Pipelines.Steps.Serialization.Core library containing SerializeObjectToStreamStep<TObject> and the SerializeJsonObjectToStreamOptions from this library.")]
    public class SerializeObjectToStreamOptions
    {
        /// <summary>
        /// Gets or sets the name that intput object will be read from the <see cref="PipelineContext.Items"/>.
        /// </summary>
        public string InputObjectContextName { get; set; } = PipelineContextConstants.INPUT_OBJECT;

        /// <summary>
        /// Gets or sets the options used by <see cref="JsonSerializer"/>.
        /// </summary>
        [FromServices]
        public JsonSerializerOptions JsonSerializerOptions { get; set; } = new JsonSerializerOptions();

        /// <summary>
        /// Gets or sets the name that output stream will be added tog the <see cref="PipelineContext.Items"/>.
        /// </summary>
        public string OutputStreamContextName { get; set; } = PipelineContextConstants.OUTPUT_STREAM;
    }
}
