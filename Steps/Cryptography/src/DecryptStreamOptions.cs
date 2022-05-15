// <copyright file="DecryptStreamOptions.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.Cryptography
{
    using MyTrout.Pipelines.Core;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Provides caller-configurable options to change the behavior of <see cref="DecryptStreamWithAes256Step"/>.
    /// </summary>
    public class DecryptStreamOptions
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// Gets or sets the algorithm with Initialization Vector and Key already set.
        /// </summary>
        [FromServices]
        public SymmetricAlgorithm DecryptionAlgorithm { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        /// Gets or sets the context name of the stream to be decrypted.
        /// </summary>
        public string InputStreamContextName { get; set; } = PipelineContextConstants.INPUT_STREAM;

        /// <summary>
        /// Gets or sets the context name of the stream of the decrypted output stream.
        /// </summary>
        public string OutputStreamContextName { get; set; } = PipelineContextConstants.OUTPUT_STREAM;
    }
}
