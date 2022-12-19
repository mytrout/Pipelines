// <copyright file="SaveContextToDatabaseOptions.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.Data
{
    using MyTrout.Pipelines.Core;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Threading.Tasks;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// Provides user-configurable options for the <see cref="SaveContextToDatabaseStep" /> step.
    /// </summary>
    public class SaveContextToDatabaseOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveContextToDatabaseOptions"/> class.
        /// </summary>
        public SaveContextToDatabaseOptions()
        {
            this.RetrieveConnectionStringAsync = this.RetrieveConnectionStringCoreAsync;
        }

        /// <summary>
        /// Gets or sets the connection string used to connect to the database.
        /// </summary>
        public string DatabaseConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Database statement context name.
        /// </summary>
        public string DatabaseStatementNameContextName { get; set; } = DatabaseConstants.DATABASE_STATEMENT_NAME;

        /// <summary>
        /// Gets or sets the context name used to record the number of rows affected.
        /// </summary>
        public string DatabaseRowsAffectedContextName { get; set; } = DatabaseConstants.DATABASE_ROWS_AFFECTED;

        /// <summary>
        /// Gets or sets the <see cref="DbProviderFactory"/> used by this step.
        /// </summary>
        [FromServices]
        public DbProviderFactory DbProviderFactory { get; set; }

        /// <summary>
        /// Gets or sets the Sql Statements that can be executed by this step.
        /// </summary>
        public List<SqlStatement> SqlStatements { get; set; } = new List<SqlStatement>();

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
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
