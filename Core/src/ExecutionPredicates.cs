// <copyright file="ExecutionPredicates.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2022-2023 Chris Trout
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
namespace MyTrout.Pipelines
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides a collection of Predicates with default configuration to 
    /// </summary>
    public sealed class ExecutionPredicates
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionPredicates"/> class.
        /// </summary>
        public ExecutionPredicates()
        {
            // no op
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionPredicates"/> class with the specifed predicates.
        /// </summary>
        /// <remarks>This constructor should ONLY be used by this class, for building a new instance.</remarks>
        private ExecutionPredicates(Dictionary<ExecutionPredicateKind, Predicate<IPipelineContext>> predicates)
        {
            this.Predicates = predicates;
        }

        private Dictionary<ExecutionPredicateKind, Predicate<IPipelineContext>> Predicates { get; } = new Dictionary<ExecutionPredicateKind, Predicate<IPipelineContext>>();

        /// <summary>
        /// Provides an indexer to items in the <see cref="ExecutionPredicates"/> class.
        /// </summary>
        /// <param name="kind">The location where the predicate should be executed.</param>
        /// <returns>If the <paramref name="kind"/> exists, returns the predicate; otherwise returns <see cref="ExecutionPredicates.AlwaysExecutePredicate(IPipelineContext)"/>.</returns>
        public Predicate<IPipelineContext> this[ExecutionPredicateKind kind]
        {
            get
            {
                if (this.Predicates.ContainsKey(kind))
                {
                    return this.Predicates[kind];
                }

                return ExecutionPredicates.AlwaysExecutePredicate;
            }
        }

        /// <summary>
        /// Provides a predicate which always forces the method to execute to preserve backwards and forwards compatibility.
        /// </summary>
        /// <param name="context">The <see cref="IPipelineContext">context</see> passed during pipeline execution.</param>
        /// <returns>Always returns <see langword="true"/>.</returns>
        public static bool AlwaysExecutePredicate(IPipelineContext context)
        {
            return true;
        }

        /// <summary>
        /// Copies this instance and adds or replaces a Predicate to the collection and returns a new <see cref="ExecutionPredicates"/>.
        /// </summary>
        /// <param name="kind">Kind of predicate to add.</param>
        /// <param name="context">The predicate containing an <see cref="IPipelineContext"/> as a parameter to the predicate.</param>
        /// <returns>A new <see cref="ExecutionPredicates"/> containing the added value.</returns>
        public ExecutionPredicates Add(ExecutionPredicateKind kind, Predicate<IPipelineContext> context)
        {
            var otherPredicates = this.Predicates.Where(x => x.Key != kind) ?? new List<KeyValuePair<ExecutionPredicateKind, Predicate<IPipelineContext>>>();

            var replacedPredicates = new Dictionary<ExecutionPredicateKind, Predicate<IPipelineContext>>(otherPredicates)
            {
                { kind, context }
            };

            return new ExecutionPredicates(replacedPredicates);
        }

        /// <summary>
        /// Determines if a predicate exists in the current collection.
        /// </summary>
        /// <param name="kind">Kind of predicate to search.</param>
        /// <returns><see langword="true" /> if the <see cref="ExecutionPredicateKind"/> exists in the collection; otherwise <see langword="false"/>.</returns>
        public bool Exists(ExecutionPredicateKind kind)
        {
            return this.Predicates.ContainsKey(kind);
        }

        /// <summary>
        /// Copies this instance and removes a Predicate from the collection and returns a new <see cref="ExecutionPredicates"/>.
        /// </summary>
        /// <param name="kind">Kind of predicate to add.</param>
        /// <returns>A new <see cref="ExecutionPredicates"/> containing the added value.</returns>
        public ExecutionPredicates Remove(ExecutionPredicateKind kind)
        {
            var otherPredicates = this.Predicates.Where(x => x.Key != kind) ?? new List<KeyValuePair<ExecutionPredicateKind, Predicate<IPipelineContext>>>();
            var replacedPredicates = new Dictionary<ExecutionPredicateKind, Predicate<IPipelineContext>>(otherPredicates);

            return new ExecutionPredicates(replacedPredicates);
        }
    }
}
