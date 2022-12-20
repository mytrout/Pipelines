// <copyright file="SaveContextToDatabaseStep.cs" company="Chris Trout">
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
    using Dapper;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    ///  Saves data from the <see cref="IPipelineContext"/> to the database after downstream processing is completed.
    /// </summary>
    public class SaveContextToDatabaseStep : AbstractCachingPipelineStep<SaveContextToDatabaseStep, SaveContextToDatabaseOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveContextToDatabaseStep"/> class with the specified options.
        /// </summary>
        /// <param name="logger">The logger for this step.</param>
        /// <param name="next">The next step in the pipeline.</param>
        /// <param name="options">Step-specific options for altering behavior.</param>
        public SaveContextToDatabaseStep(ILogger<SaveContextToDatabaseStep> logger, SaveContextToDatabaseOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        /// <inheritdoc />
        public override IEnumerable<string> CachedItemNames => new List<string>() { this.Options.DatabaseRowsAffectedContextName };

        /// <summary>
        /// When downstream processing is completed, writes any context values that are configured to be written to the database.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        protected override async Task InvokeCachedCoreAsync(IPipelineContext context)
        {
            context.AssertStringIsNotWhiteSpace(this.Options.DatabaseStatementNameContextName);

            var sqlName = context.Items[this.Options.DatabaseStatementNameContextName] as string;
            var sql = this.Options.SqlStatements.FirstOrDefault(x => x.Name == sqlName);

            if (sql is null)
            {
                throw new InvalidOperationException(Resources.SQL_STATEMENT_NOT_FOUND(CultureInfo.CurrentCulture, sqlName));
            }

            sql.AssertValueIsNotNull(() => Resources.SQL_STATEMENT_NOT_FOUND(CultureInfo.CurrentCulture, sqlName));

            var parameters = new DynamicParameters();

            // Use the null-forgiving operator because AssertValueIsNotNull guarantees that the value of sql variable is not null.
            foreach (var parameterName in sql!.ParameterNames)
            {
                parameters.Add(parameterName, context.Items[parameterName]);
            }

            try
            {
                using (var connection = this.Options.DbProviderFactory.CreateConnection())
                {
                    if (connection is null)
                    {
                        throw new InvalidOperationException(Resources.CONNECTION_IS_NULL(this.Options.DbProviderFactory.GetType().Name));
                    }

                    connection.ConnectionString = await this.Options.RetrieveConnectionStringAsync.Invoke().ConfigureAwait(false);

                    await connection.OpenAsync().ConfigureAwait(false);

                    int result = await connection.ExecuteAsync(sql.Statement, param: parameters, commandType: sql.CommandType).ConfigureAwait(false);

                    context.Items.Add(this.Options.DatabaseRowsAffectedContextName, result);
                }

                await this.Next.InvokeAsync(context).ConfigureAwait(false);
            }
            finally
            {
                context.Items.Remove(this.Options.DatabaseRowsAffectedContextName);
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
