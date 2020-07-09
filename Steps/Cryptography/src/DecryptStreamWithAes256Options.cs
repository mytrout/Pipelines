﻿// <copyright file="DecryptStreamWithAes256Options.cs" company="Chris Trout">
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

    /// <summary>
    /// Provides caller-configurable options to change the behavior of <see cref="DecryptStreamWithAes256Step"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class DecryptStreamWithAes256Options
    {
        /// <summary>
        /// Gets or sets the <see cref="Encoding"/> used to decrypt this value.
        /// </summary>
        public Encoding DecryptionEncoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Gets or sets the initialization vector used to decrypt the value.
        /// </summary>
        public string DecryptionInitializationVector { get; set; }

        /// <summary>
        /// Gets or sets the key used to decrypt the value.
        /// </summary>
        public string DecryptionKey { get; set; }
    }
}
