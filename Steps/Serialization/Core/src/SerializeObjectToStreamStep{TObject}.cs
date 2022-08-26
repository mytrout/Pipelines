// <copyright file="SerializeObjectToStreamStep{TObject}.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.Serialization
{
    using Microsoft.Extensions.Logging;
    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Core;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Serializes a <typeparamref name="TObject"/> to a <see cref="Stream"/> added to <see cref="PipelineContext.Items"/>.
    /// </summary>
    /// <typeparam name="TObject">The target object for deserialization.
    /// </typeparam>
    public class SerializeObjectToStreamStep<TObject> : AbstractCachingPipelineStep<SerializeObjectToStreamStep<TObject>, ISerializeObjectToStreamOptions>
        where TObject : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializeObjectToStreamStep{TObject}" /> class with the specified parameters.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        /// <param name="next">The next step in the pipeline.</param>
        public SerializeObjectToStreamStep(ILogger<SerializeObjectToStreamStep<TObject>> logger, ISerializeObjectToStreamOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        /// <inheritdoc />
        public override IEnumerable<string> CachedItemNames => new List<string>() { this.Options.OutputStreamContextName };

        /// <summary>
        /// Guarantees that <see cref="ISerializeObjectToStreamOptions.InputObjectContextName"/> exists and is non-null from <paramref name="context"/>.
        /// </summary>
        /// <param name="context">PipelineContext.</param>
        /// <returns>A completed <see cref="Task"/>.</returns>
        protected override Task InvokeBeforeCacheAsync(IPipelineContext context)
        {
            context.AssertValueIsValid<TObject>(this.Options.InputObjectContextName);
            return base.InvokeBeforeCacheAsync(context);
        }

        /// <summary>
        /// Serializes a <typeparamref name="TObject"/> into a <see cref="Stream"/> and add it to <see cref="PipelineContext.Items"/>.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        /// <remarks><paramref name="context"/> is guaranteed to not be -<see langword="null" /> by the base class.</remarks>
        protected override async Task InvokeCachedCoreAsync(IPipelineContext context)
        {
            var inputObject = (context.Items[this.Options.InputObjectContextName] as TObject)!;

            try
            {
                using (var outputStream = new MemoryStream())
                {
                    await this.Options.SerializeAsync(outputStream, inputObject, context.CancellationToken);
                    outputStream.Position = 0;

                    context.Items.Add(this.Options.OutputStreamContextName, outputStream);

                    await this.Next.InvokeAsync(context);
                }
            }
            finally
            {
                context.Items.Remove(this.Options.OutputStreamContextName);
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
