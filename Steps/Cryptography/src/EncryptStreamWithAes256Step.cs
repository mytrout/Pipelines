// <copyright file="EncryptStreamWithAes256Step.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2020-2022 Chris Trout
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
    using System.Threading.Tasks;

    /// <summary>
    /// Encrypts the <see cref="PipelineContextConstants.OUTPUT_STREAM" /> using AES256.
    /// </summary>
    [Obsolete("Use EncryptStreamStep with default options.")]
    public class EncryptStreamWithAes256Step : AbstractPipelineStep<EncryptStreamWithAes256Step, EncryptStreamWithAes256Options>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptStreamWithAes256Step" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        public EncryptStreamWithAes256Step(ILogger<EncryptStreamWithAes256Step> logger, EncryptStreamWithAes256Options options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        /// <summary>
        /// Encrypts the context <see cref="Stream"/> named <see cref="PipelineContextConstants.OUTPUT_STREAM"/>.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        /// <remarks><paramref name="context"/> is guaranteed to not be -<see langword="null" /> by the base class.</remarks>
        protected async override Task InvokeCoreAsync(IPipelineContext context)
        {
            await this.Next.InvokeAsync(context).ConfigureAwait(false);

            context.AssertValueIsValid<Stream>(PipelineContextConstants.OUTPUT_STREAM);

            // Creates a FIPS-compliant hashProvider, if FIPS-compliance is on.  Otherwise, creates the ~Cng version.
            using (var cryptoProvider = Aes.Create())
            {
                // Guaranteed to be non-null and a stream by AssertValueIsValid<Stream>
                using (var unencryptedStream = (context.Items[PipelineContextConstants.OUTPUT_STREAM] as Stream)!)
                {
                    byte[] key = this.Options.EncryptionEncoding.GetBytes(this.Options.RetrieveEncryptionKey());
                    byte[] initializationVector = this.Options.EncryptionEncoding.GetBytes(this.Options.RetrieveEncryptionInitializationVector());

                    ICryptoTransform encryptor = cryptoProvider.CreateEncryptor(key, initializationVector);

                    var encryptedStream = new MemoryStream();

                    using (var cryptoStream = new CryptoStream(encryptedStream, encryptor, CryptoStreamMode.Write, leaveOpen: true))
                    {
                        using (var unencryptedReader = new StreamReader(unencryptedStream, this.Options.EncryptionEncoding, false, 1024, false))
                        {
                            byte[] workingArray = await StreamExtensions.ConvertStreamToByteArrayAsync(unencryptedStream).ConfigureAwait(false);
                            await cryptoStream.WriteAsync(workingArray).ConfigureAwait(false);
                        }
                    }

                    // Reset to the starting position to allow writers to write the entire stream.
                    encryptedStream.Position = 0;

                    context.Items[PipelineContextConstants.OUTPUT_STREAM] = encryptedStream;
                }
            }
        }
    }
}
