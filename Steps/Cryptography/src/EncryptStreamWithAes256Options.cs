﻿// <copyright file="EncryptStreamWithAes256Options.cs" company="Chris Trout">
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
    using System.Text;

    /*
     *  IMPORTANT NOTE: As long as this class only contains compiler-generated functionality, it requires no unit tests.
     */
#pragma warning disable CS8618 // These properties must be initialized by configuration.

    /// <summary>
    /// Provides caller-configurable options to change the behavior of <see cref="EncryptStreamWithAes256Step"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class EncryptStreamWithAes256Options
    {
        /// <summary>
        /// Gets or sets the <see cref="Encoding"/> used to encrypt this value.
        /// </summary>
        public Encoding EncryptionEncoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Gets or sets the initialization vector used to encrypt the value.
        /// </summary>
        public string EncryptionInitializationVector { get; set; }

        /// <summary>
        /// Gets or sets the key used to encrypt the value.
        /// </summary>
        public string EncryptionKey { get; set; }
    }
#pragma warning restore CS8618 // These properties must be initialized by configuration.
}
