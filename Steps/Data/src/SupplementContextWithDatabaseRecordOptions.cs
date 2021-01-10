﻿// <copyright file="SupplementContextWithDatabaseRecordOptions.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2020-2021 Chris Trout
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
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides user-configurable options for the <see cref="SaveContextToDatabaseStep" /> step.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class SupplementContextWithDatabaseRecordOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SupplementContextWithDatabaseRecordOptions"/> class.
        /// </summary>
        public SupplementContextWithDatabaseRecordOptions()
        {
            this.RetrieveConnectionStringAsync = this.RetrieveConnectionStringCoreAsync;
        }

        /// <summary>
        /// Gets or sets the Command Type of the <see cref="SqlStatement"/>.
        /// </summary>
        public CommandType CommandType { get; set; } = CommandType.StoredProcedure;

        /// <summary>
        /// Gets or sets the connection string used to connect to the database.
        /// </summary>
        public string DatabaseConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets that parameter names required by <see cref="SqlStatement"/>.
        /// </summary>
        public IEnumerable<string> ParameterNames { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the SQL Statement that should be executed by this step.
        /// </summary>
        public string SqlStatement { get; set; }

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
}
