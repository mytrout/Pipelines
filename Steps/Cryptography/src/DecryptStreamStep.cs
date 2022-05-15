// <copyright file="DecryptStreamStep.cs" company="Chris Trout">
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
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Threading.Tasks;

    /// <summary>
    /// Decrypts the <see cref="PipelineContextConstants.INPUT_STREAM" /> using the specified options.
    /// </summary>
    public class DecryptStreamStep : AbstractCachingPipelineStep<DecryptStreamStep, DecryptStreamOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DecryptStreamStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        public DecryptStreamStep(ILogger<DecryptStreamStep> logger, DecryptStreamOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        /// <inheritdoc />
        public override IEnumerable<string> CachedItemNames => new List<string>() { this.Options.OutputStreamContextName };

        /// <summary>
        /// Guarantees that <see cref="DecryptStreamOptions.InputStreamContextName"/> exists and is non-null from <paramref name="context"/>.
        /// </summary>
        /// <param name="context">PipelineContext.</param>
        /// <returns>A completed <see cref="Task"/>.</returns>
        protected override Task InvokeBeforeCacheAsync(IPipelineContext context)
        {
            context.AssertValueIsValid<Stream>(this.Options.InputStreamContextName);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Decrypts the context <see cref="Stream"/> named <see cref="DecryptStreamOptions.InputStreamContextName"/>.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        /// <remarks><paramref name="context"/> is guaranteed to not be -<see langword="null" />- by the base class.</remarks>
        protected override async Task InvokeCachedCoreAsync(IPipelineContext context)
        {
            // InvokeBeforeCache guarantees that tempStream will not be null.
            Stream inputStream = (context.Items[this.Options.InputStreamContextName] as Stream)!;
            inputStream.Position = 0;

            using (ICryptoTransform decryptor = this.Options.DecryptionAlgorithm.CreateDecryptor())
            {
                // Reset to the starting position to allow readers to read the entire stream.
                inputStream.Position = 0;

                // Decrypt the contents
                using (var outputCryptoStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read, leaveOpen: true))
                {
                    using (var reader = new StreamReader(outputCryptoStream, this.Options.DecryptionEncoding, false, 1024, leaveOpen: true))
                    {
                        string result = await reader.ReadToEndAsync().ConfigureAwait(false);
                        byte[] workingResult = this.Options.DecryptionEncoding.GetBytes(result);

                        using (var outputStream = new MemoryStream(workingResult))
                        {
                            try
                            {
                                context.Items.Add(this.Options.OutputStreamContextName, outputStream);

                                await this.Next.InvokeAsync(context).ConfigureAwait(false);
                            }
                            finally
                            {
                                context.Items.Remove(this.Options.OutputStreamContextName);
                            }
                        }
                    }
                }
            }
        }
    }
}
