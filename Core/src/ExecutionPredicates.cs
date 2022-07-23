namespace MyTrout.Pipelines
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
        /// Provides a predicate which always forces the method to execute to preserve backwards and forwards compatibility.
        /// </summary>
        /// <param name="context">The <see cref="IPipelineContext">context</see> passed during pipeline execution.</param>
        /// <returns>Always returns <see langword="true"/>.</returns>
        public static bool AlwaysExecutePredicate(IPipelineContext context)
        {
            return true;
        }

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

        public ExecutionPredicates Add(ExecutionPredicateKind kind, Predicate<IPipelineContext> context)
        {
            var otherPredicates = this.Predicates.Where(x => x.Key != kind) ?? new List<KeyValuePair<ExecutionPredicateKind, Predicate<IPipelineContext>>>();

            var replacedPredicates = new Dictionary<ExecutionPredicateKind, Predicate<IPipelineContext>>(otherPredicates)
            {
                { kind, context }
            };

            return new ExecutionPredicates(replacedPredicates);
        }

        public bool Exists(ExecutionPredicateKind kind)
        {
            return this.Predicates.ContainsKey(kind);
        }

        public ExecutionPredicates Remove(ExecutionPredicateKind kind)
        {
            var otherPredicates = this.Predicates.Where(x => x.Key != kind) ?? new List<KeyValuePair<ExecutionPredicateKind, Predicate<IPipelineContext>>>();
            var replacedPredicates = new Dictionary<ExecutionPredicateKind, Predicate<IPipelineContext>>(otherPredicates);

            return new ExecutionPredicates(replacedPredicates);
        }
    }
}
