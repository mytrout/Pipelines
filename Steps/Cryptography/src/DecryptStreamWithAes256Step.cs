// <copyright file="DecryptStreamWithAes256Step.cs" company="Chris Trout">
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
    using System.IO;
    using System.Security.Cryptography;
    using System.Threading.Tasks;

    /// <summary>
    /// Decrypts the <see cref="PipelineContextConstants.INPUT_STREAM" /> using AES256.
    /// </summary>
    public class DecryptStreamWithAes256Step : AbstractPipelineStep<DecryptStreamWithAes256Step, DecryptStreamWithAes256Options>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DecryptStreamWithAes256Step" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        public DecryptStreamWithAes256Step(ILogger<DecryptStreamWithAes256Step> logger, DecryptStreamWithAes256Options options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        /// <summary>
        /// Decrypts the context <see cref="Stream"/> named <see cref="PipelineContextConstants.INPUT_STREAM"/>.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        /// <remarks><paramref name="context"/> is guaranteed to not be -<see langword="null" /> by the base class.</remarks>
        protected async override Task InvokeCoreAsync(IPipelineContext context)
        {
            context.AssertValueIsValid<Stream>(PipelineContextConstants.INPUT_STREAM);

#pragma warning disable CS8600, CS8604 // context.AssertValueIsValid() guarantees a non-null value.

            Stream encryptedStream = context.Items[PipelineContextConstants.INPUT_STREAM] as Stream;

            context.Items.Remove(PipelineContextConstants.INPUT_STREAM);

            // Creates a FIPS-compliant hashProvider, if FIPS-compliance is on.  Otherwise, creates the ~Cng version.
            var cryptoProvider = Aes.Create();

            try
            {
                byte[] key = this.Options.DecryptionEncoding.GetBytes(this.Options.RetrieveDecryptionKey());
                byte[] initializationVector = this.Options.DecryptionEncoding.GetBytes(this.Options.RetrieveDecryptionInitializationVector());

                ICryptoTransform decryptor = cryptoProvider.CreateDecryptor(key, initializationVector);

                using (var cryptoStream = new CryptoStream(encryptedStream, decryptor, CryptoStreamMode.Read, leaveOpen: true))
                {
                    using (var reader = new StreamReader(cryptoStream, this.Options.DecryptionEncoding, false, 1024, true))
                    {
                        byte[] output = this.Options.DecryptionEncoding.GetBytes(await reader.ReadToEndAsync().ConfigureAwait(false));

                        using (var outputStream = new MemoryStream(output))
                        {
                            context.Items.Add(PipelineContextConstants.INPUT_STREAM, outputStream);

                            await this.Next.InvokeAsync(context).ConfigureAwait(false);
                        }
                    }
                }
            }
            finally
            {
                cryptoProvider.Dispose();

                if (context.Items.ContainsKey(PipelineContextConstants.INPUT_STREAM))
                {
                    context.Items.Remove(PipelineContextConstants.INPUT_STREAM);
                }

                context.Items.Add(PipelineContextConstants.INPUT_STREAM, encryptedStream);

#pragma warning restore CS8600, CS8604 // context.AssertValueIsValid() guarantees a non-null value.
            }
        }
    }
}
