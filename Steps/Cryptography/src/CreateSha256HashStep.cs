﻿// <copyright file="CreateSha256HashStep.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2020 Chris Trout
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
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Creates a SHA256 Hash of the pipeline's <see cref="PipelineContextConstants.OUTPUT_STREAM"/>.
    /// </summary>
    public class CreateSha256HashStep : AbstractPipelineStep<CreateSha256HashStep>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSha256HashStep" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public CreateSha256HashStep(ILogger<CreateSha256HashStep> logger, IPipelineRequest next)
            : base(logger, next)
        {
            // no op
        }

        /// <summary>
        /// Generates a SHA256 Hash of the <see cref="PipelineContextConstants.OUTPUT_STREAM"/>.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        /// <remarks><paramref name="context"/> is guaranteed to not be -<see langword="null" /> by the base class.</remarks>
        protected override async Task InvokeCoreAsync(IPipelineContext context)
        {
            context.AssertStreamParameterIsValid(PipelineContextConstants.OUTPUT_STREAM);

            if (context.Items.TryGetValue(CryptographyConstants.HASH_STREAM, out object previousHash))
            {
                context.Items.Remove(CryptographyConstants.HASH_STREAM);
            }

            if (context.Items.TryGetValue(CryptographyConstants.HASH_STRING, out object previousHashString))
            {
                context.Items.Remove(CryptographyConstants.HASH_STRING);
            }

            Stream inputStream = context.Items[PipelineContextConstants.OUTPUT_STREAM] as Stream;

            long previousStreamPosition = inputStream.Position;
            inputStream.Position = 0;

            try
            {
                using (StreamReader reader = new StreamReader(inputStream, Encoding.UTF8, false, 1024, true))
                {
                    byte[] workingResult = Encoding.UTF8.GetBytes(await reader.ReadToEndAsync().ConfigureAwait(false));

                    using (SHA256CryptoServiceProvider hashProvider = new SHA256CryptoServiceProvider())
                    {
                        byte[] workingHash = hashProvider.ComputeHash(workingResult);
                        string hexHash = BitConverter.ToString(workingHash).Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);

                        context.Items.Add(CryptographyConstants.HASH_STRING, hexHash);

                        using (var hashStream = new MemoryStream(workingHash))
                        {
                            context.Items.Add(CryptographyConstants.HASH_STREAM, hashStream);

                            await this.Next.InvokeAsync(context).ConfigureAwait(false);
                        }
                    }
                }
            }
            finally
            {
                context.Items.Remove(CryptographyConstants.HASH_STREAM);
                context.Items.Remove(CryptographyConstants.HASH_STRING);

                inputStream.Position = previousStreamPosition;

                if (previousHash != null)
                {
                    context.Items.Add(CryptographyConstants.HASH_STREAM, previousHash);
                }

                if (previousHashString != null)
                {
                    context.Items.Add(CryptographyConstants.HASH_STRING, previousHashString);
                }
            }
        }
    }
}
