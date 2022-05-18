namespace MyTrout.Pipelines.Steps.Data
{
    using MyTrout.Pipelines.Core;
    using System;
    using System.Data.Common;
    using System.Threading.Tasks;

    public class SupplementContextWithObjectFromDatabaseOptions
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SupplementContextWithDatabaseRecordOptions"/> class.
        /// </summary>
        public SupplementContextWithObjectFromDatabaseOptions()
        {
            this.RetrieveConnectionStringAsync = this.RetrieveConnectionStringCoreAsync;
        }

        /// <summary>
        /// Gets or sets the connection string used to connect to the database.
        /// </summary>
        public string DatabaseConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the <see cref="DbProviderFactory"/> used by this step.
        /// </summary>
        [FromServices]
        public DbProviderFactory DbProviderFactory { get; set; }

        /// <summary>
        /// Gets or sets the SQL Statement that should be executed by this step.
        /// </summary>
        public SqlStatement SqlStatement { get; set; } = new SqlStatement();

        /// <summary>
        /// Gets or sets a user-defined function to retrieve the Connection String.
        /// </summary>
        public Func<Task<string>> RetrieveConnectionStringAsync { get; set; }

        /// <summary>
        /// Returns a connection string loaded into the <see cref="DatabaseConnectionString"/> property.
        /// </summary>
        /// <returns>A connection string.</returns>
        protected Task<string> RetrieveConnectionStringCoreAsync()
        {
            return Task.FromResult(this.DatabaseConnectionString);
        }

        public string InputObjectContextName { get; set; } = PipelineContextConstants.INPUT_OBJECT;
    }
}
