﻿// <copyright file="CreateHashStep.cs" company="Chris Trout">
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
    using System.Threading.Tasks;

    /// <summary>
    /// Creates a Hash of the pipeline's <see cref="CreateHashOptions.InputStreamContextName" />.
    /// </summary>
    public class CreateHashStep : AbstractCachingPipelineStep<CreateHashStep, CreateHashOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateHashStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public CreateHashStep(ILogger<CreateHashStep> logger, CreateHashOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        /// <inheritdoc />
        public override IEnumerable<string> CachedItemNames => new List<string>() { this.Options.HashStreamContextName, this.Options.HashStringContextName };

        /// <inheritdoc />
        protected override Task InvokeBeforeCacheAsync(IPipelineContext context)
        {
            context.AssertValueIsValid<Stream>(this.Options.InputStreamContextName);
            return base.InvokeBeforeCacheAsync(context);
        }

        /// <summary>
        /// Generates a SHA256 Hash of the <see cref="CreateHashOptions.InputStreamContextName" />.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        /// <remarks><paramref name="context"/> is guaranteed to not be -<see langword="null" /> by the base class.</remarks>
        protected override async Task InvokeCachedCoreAsync(IPipelineContext context)
        {
            // AssertValueIsValid guarantees that this.Options.HashStreamKey is non-null and a Stream.
            Stream inputStream = (context.Items[this.Options.InputStreamContextName] as Stream)!;

            inputStream.Position = 0;

            using (var reader = new StreamReader(inputStream, this.Options.HashEncoding, false, 1024, true))
            {
                byte[] workingResult = this.Options.HashEncoding.GetBytes(await reader.ReadToEndAsync().ConfigureAwait(false));

                var hashProvider = this.Options.HashAlgorithm;

                byte[] workingHash = hashProvider.ComputeHash(workingResult);
                string hexHash = BitConverter.ToString(workingHash).Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);

                context.Items.Add(this.Options.HashStringContextName, hexHash);

                using (var hashStream = new MemoryStream(this.Options.HashEncoding.GetBytes(hexHash)))
                {
                    context.Items.Add(this.Options.HashStreamContextName, hashStream);
                    await this.Next.InvokeAsync(context).ConfigureAwait(false);
                }
            }
        }
    }
}
