// <copyright file="SupplementContextWithDatabaseRecordStep.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2020 Chris Trout
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
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Net.Mime;
    using System.Runtime.InteropServices.ComTypes;
    using System.Threading.Tasks;

    public class SupplementContextWithDatabaseRecordStep : AbstractPipelineStep<SupplementContextWithDatabaseRecordStep, SupplementContextWithDatabaseRecordOptions>
    {
        public SupplementContextWithDatabaseRecordStep(ILogger<SupplementContextWithDatabaseRecordStep> logger, SupplementContextWithDatabaseRecordOptions options, IPipelineRequest next)
            : base(logger, options, next)
        {
            // no op
        }

        protected override async Task InvokeCoreAsync(IPipelineContext context)
        {
            var factory = DbProviderFactories.GetFactory(this.Options.ProviderInvariantName);


            object parameters = null;

            List<string> addedFields = new List<string>();
            Dictionary<string, object> previousValues = new Dictionary<string, object>();
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = await this.Options.RetrieveConnectionStringAsync.Invoke().ConfigureAwait(false);

                await connection.OpenAsync().ConfigureAwait(false);

                using (var reader = await connection.ExecuteReaderAsync(this.Options.SqlStatement, parameters).ConfigureAwait(false))
                {
                    reader.Read();

                    for (int index = 0; index < reader.FieldCount; index++)
                    {
                        string fieldName = reader.GetName(index);
                        if (context.Items.ContainsKey(fieldName))
                        {
                            // Cache the previous values to restore them after the "next" processing is completed.
                            previousValues.Add(fieldName, context.Items[fieldName]);
                            context.Items[fieldName] = reader.GetValue(index);
                        }
                        else
                        {
                            // Add the new value into a different cache to be removed after "next" processing.
                            context.Items.Add(fieldName, reader.GetValue(index));
                            addedFields.Add(fieldName);
                        }
                    }

                    await this.Next.InvokeAsync(context).ConfigureAwait(false);

                    // Remove the fields that were added.
                    foreach (string field in addedFields)
                    {
                        if (context.Items.ContainsKey(field))
                        {
                            context.Items.Remove(field);
                        }
                    }

                    // Change the fields back to their previous values.
                    foreach (KeyValuePair<string, object> field in previousValues)
                    {
                        if (context.Items.ContainsKey(field.Key))
                        {
                            context.Items[field.Key] = field.Value;
                        }
                    }
                }
            }
        }
    }
}
