// <copyright file="SupplementContextWithObjectFromDatabaseStep.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.Data
{
    using Dapper;
    using Microsoft.Extensions.Logging;
    using MyTrout.Pipelines;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SupplementContextWithObjectFromDatabaseStep<TObject> : AbstractCachingPipelineStep<SupplementContextWithObjectFromDatabaseStep<TObject>, SupplementContextWithObjectFromDatabaseOptions>
        where TObject : class
    {
        public SupplementContextWithObjectFromDatabaseStep(ILogger<SupplementContextWithObjectFromDatabaseStep<TObject>> logger, SupplementContextWithObjectFromDatabaseOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        /// <inheritdoc />
        public override IEnumerable<string> CachedItemNames => new List<string>() { this.Options.InputObjectContextName };

        protected override async Task InvokeCachedCoreAsync(IPipelineContext context)
        {
            var parameters = new DynamicParameters();

            foreach (var parameterName in this.Options.SqlStatement.ParameterNames)
            {
                parameters.Add(parameterName, context.Items[parameterName]);
            }

            var addedFields = new List<string>();
            var previousValues = new Dictionary<string, object>();

            try
            {
                using (var connection = this.Options.DbProviderFactory.CreateConnection())
                {
                    connection.AssertValueIsNotNull(() => Resources.CONNECTION_IS_NULL(this.Options.DbProviderFactory.GetType().Name));

#pragma warning disable CS8602 // AssertValueIsNotNull guarantees a non-null value here.

                    connection.ConnectionString = await this.Options.RetrieveConnectionStringAsync.Invoke().ConfigureAwait(false);

                    await connection.OpenAsync().ConfigureAwait(false);

                    connection.QueryAsync()
                    using (var reader = await connection.ExecuteReaderAsync(this.Options.SqlStatement.Statement, param: parameters, commandType: this.Options.SqlStatement.CommandType).ConfigureAwait(false))
                    {
                        if (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            for (int index = 0; index < reader.FieldCount; index++)
                            {
                                string fieldName = reader.GetName(index);
                                if (context.Items.ContainsKey(fieldName))
                                {
                                    // Cache the previous values to restore them after the "next" processing is completed.
                                    previousValues.Add(fieldName, context.Items[fieldName]);

                                    // Reduce Cognitive Complexity by always removing the fieldName.
                                    context.Items.Remove(fieldName);
                                }

                                context.Items.Add(fieldName, reader.GetValue(index));
                                addedFields.Add(fieldName);
                            }
                        }
                        else
                        {
                            context.Errors.Add(new InvalidOperationException(Resources.NO_DATA_FOUND(CultureInfo.CurrentCulture, nameof(SupplementContextWithDatabaseRecordStep))));
                        }
                    }

#pragma warning restore CS8602 // AssertValueIsNotNull guarantees a non-null value here.

                } // Closed the connection to prevent any resource leakage into later steps.

                await this.Next.InvokeAsync(context).ConfigureAwait(false);
            }
            finally
            {
                // Remove all of the field values that were added or altered.
                foreach (string field in addedFields)
                {
                    context.Items.Remove(field);
                }

                // Add the previous values back.
                foreach (KeyValuePair<string, object> field in previousValues)
                {
                    context.Items.Add(field.Key, field.Value);
                }
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
    }
}
