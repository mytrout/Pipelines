// <copyright file="DeserializeObjectFromStreamOptions.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.Serialization.Core.Tests
{
    using MyTrout.Pipelines.Core;
    using MyTrout.Pipelines.Steps.Serialization.Core;
    using System.IO;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides user-configuration options to alter the behavior of <see cref="DeserializeObjectFromStreamStep{TObject}"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class DeserializeObjectFromStreamOptions : AbstractDeserializeObjectFromStreamOptions
    {
        /// <summary>
        /// Gets or sets the options used by <see cref="System.Text.Json.JsonSerializer"/>.
        /// </summary>
        [FromServices]
        public JsonSerializerOptions JsonSerializerOptions { get; set; } = new JsonSerializerOptions();

        /// <inheritdoc />
        public async override Task<TObject> DeserializeAsync<TObject>(Stream inputStream, CancellationToken cancellationToken)
        {
            var result = await JsonSerializer.DeserializeAsync<TObject>(inputStream, this.JsonSerializerOptions, cancellationToken);

            return result!;
        }
    }
}
