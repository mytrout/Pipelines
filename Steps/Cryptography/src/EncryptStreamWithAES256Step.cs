// <copyright file="EncryptStreamWithAES256Step.cs" company="Chris Trout">
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
    /// Encrypts the <see cref="PipelineContextConstants.OUTPUT_STREAM" /> using AES256.
    /// </summary>
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
        /// Converts a <see cref="Stream" /> into an array of <see cref="byte"/>.
        /// </summary>
        /// <param name="inputStream"><see cref="Stream"/> to be converted.</param>
        /// <returns>A byte array.</returns>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static byte[] ConvertStreamToByteArray(Stream inputStream)
        {
            if (inputStream == null)
            {
                throw new ArgumentNullException(nameof(inputStream));
            }

            inputStream.Position = 0;

            // The implementation of this method has been altered to guarantee that memoryStrem is never null.
            // This boolean check guarantees that the incoming inputStream is copied into a MemoryStream, only if needed.
            bool isMemoryStream = inputStream is MemoryStream;

#pragma warning disable CA2000 // Caller is responsible for disposing of inputStream.  This method dispose of new MemoryStream().
            MemoryStream memoryStream = (inputStream as MemoryStream) ?? new MemoryStream();
#pragma warning restore CA2000 // Dispose objects before losing scope

            try
            {
                if (!isMemoryStream)
                {
                    inputStream.CopyToAsync(memoryStream);
                }

                return memoryStream.ToArray();
            }
            finally
            {
                if (!isMemoryStream)
                {
                    memoryStream.Close();
                    memoryStream.Dispose();
                }
            }
        }

        /// <summary>
        /// Encrypts the context <see cref="Stream"/> named <see cref="PipelineContextConstants.OUTPUT_STREAM"/>.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        /// <remarks><paramref name="context"/> is guaranteed to not be -<see langword="null" /> by the base class.</remarks>
        protected async override Task InvokeCoreAsync(IPipelineContext context)
        {
            context.AssertStreamParameterIsValid(PipelineContextConstants.OUTPUT_STREAM);

            Stream unencryptedStream = context.Items[PipelineContextConstants.OUTPUT_STREAM] as Stream;

            var cryptoProvider = new AesCryptoServiceProvider();

            try
            {
                await this.Next.InvokeAsync(context).ConfigureAwait(false);

                byte[] key = this.Options.EncryptionEncoding.GetBytes(this.Options.EncryptionKey);
                byte[] initializationVector = this.Options.EncryptionEncoding.GetBytes(this.Options.EncryptionInitializationVector);

                ICryptoTransform encryptor = cryptoProvider.CreateEncryptor(key, initializationVector);

                var encryptedStream = new MemoryStream();

                using (CryptoStream cryptoStream = new CryptoStream(encryptedStream, encryptor, CryptoStreamMode.Write, leaveOpen: true))
                {
                    using (StreamReader unencryptedReader = new StreamReader(unencryptedStream, this.Options.EncryptionEncoding, false, 1024, true))
                    {
                        await cryptoStream.WriteAsync(ConvertStreamToByteArray(unencryptedStream)).ConfigureAwait(false);
                    }
                }

                // Reset to the starting position to allow writers to write the entire stream.
                encryptedStream.Position = 0;

                context.Items[PipelineContextConstants.OUTPUT_STREAM] = encryptedStream;
            }
            finally
            {
                cryptoProvider.Dispose();

                unencryptedStream.Close();
                unencryptedStream.Dispose();
            }
        }
    }
}
