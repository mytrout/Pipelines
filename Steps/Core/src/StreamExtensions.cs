// <copyright file="StreamExtensions.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2019-2020 Chris Trout
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

namespace MyTrout.Pipelines.Steps
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides extension methods for <see cref="Stream" />.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Converts a <see cref="Stream" /> into an array of <see cref="byte"/>.
        /// </summary>
        /// <param name="source"><see cref="Stream"/> to be converted.</param>
        /// <returns>A byte array.</returns>
        public static Task<byte[]> ConvertStreamToByteArrayAsync(this Stream source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.ConvertStreamToByteArrayCoreAsync();
        }

        /// <summary>
        /// Converts a <see cref="Stream" /> into an array of <see cref="byte"/>.
        /// </summary>
        /// <param name="source"><see cref="Stream"/> to be converted.</param>
        /// <returns>A byte array.</returns>
        private static async Task<byte[]> ConvertStreamToByteArrayCoreAsync(this Stream source)
        {
            source.Position = 0;

            // The implementation of this method has been altered to guarantee that memoryStrem is never null.
            // This check guarantees that the incoming source Stream is copied into a new MemoryStream, only if needed.
            bool isMemoryStream = source is MemoryStream;

            // Caller is responsible for disposing of source Stream.
            // This method will dispose of the new MemoryStream(), if a new one is created.
#pragma warning disable CA2000
            MemoryStream memoryStream = (source as MemoryStream) ?? new MemoryStream();
#pragma warning restore CA2000 // Dispose objects before losing scope

            try
            {
                if (!isMemoryStream)
                {
                    await source.CopyToAsync(memoryStream).ConfigureAwait(false);
                }

                return memoryStream.ToArray();
            }
            finally
            {
                if (!isMemoryStream)
                {
                    memoryStream.Close();
                    await memoryStream.DisposeAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
