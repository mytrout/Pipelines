using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyTrout.Pipelines.Steps.Data
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class SupplementContextWithDatabaseRecordOptions
    {
        public string ProviderInvariantName { get; set; }

        public string SqlStatement { get; set; }

        public Func<Task<string>> RetrieveConnectionStringAsync { get; set; }
    }
}
