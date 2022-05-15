// <copyright file="EncryptStreamWithAes256Options.cs" company="Chris Trout">
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
    using System;
    using System.Text;

    /// <summary>
    /// Provides caller-configurable options to change the behavior of <see cref="EncryptStreamWithAes256Step"/>.
    /// </summary>
    [Obsolete("Use EncryptStreamOptions and EncryptStreamStep.")]
    public class EncryptStreamWithAes256Options
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptStreamWithAes256Options"/> class.
        /// </summary>
        public EncryptStreamWithAes256Options()
        {
            this.RetrieveEncryptionInitializationVector = this.RetrieveEncryptionInitializationVectorCore;
            this.RetrieveEncryptionKey = this.RetrieveEncryptionKeyCore;
        }

        /// <summary>
        /// Gets or sets the <see cref="Encoding"/> used to encrypt this value.
        /// </summary>
        public Encoding EncryptionEncoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Gets or sets the initialization vector used to encrypt the value.
        /// </summary>
        public string EncryptionInitializationVector { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the key used to encrypt the value.
        /// </summary>
        public string EncryptionKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a user-defined function to retrieve the encryption initialization vector.
        /// </summary>
        /// <returns>A string representing a valid encryption initialization vector.</returns>
        public Func<string> RetrieveEncryptionInitializationVector { get; set; }

        /// <summary>
        /// Gets or sets a user-defined function to retrieve the encryption key.
        /// </summary>
        /// <returns>A string representing a valid encryption key.</returns>
        public Func<string> RetrieveEncryptionKey { get; set; }

        /// <summary>
        /// Provides any special handling of the <see cref="EncryptionInitializationVector"/> before the <see cref="EncryptStreamWithAes256Step"/> uses the value.
        /// </summary>
        /// <returns>a string representing a valid encryption initialization vector.</returns>
        protected virtual string RetrieveEncryptionInitializationVectorCore()
        {
            return this.EncryptionInitializationVector;
        }

        /// <summary>
        /// Provides any special handling of the <see cref="EncryptionKey"/> before the <see cref="EncryptStreamWithAes256Step"/> uses the value.
        /// </summary>
        /// <returns>a string representing a valid encryption key.</returns>
        protected virtual string RetrieveEncryptionKeyCore()
        {
            return this.EncryptionKey;
        }
    }
}
