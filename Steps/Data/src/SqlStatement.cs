// <copyright file="SqlStatement.cs" company="Chris Trout">
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
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// Gets the SQL statement and all of the parameters to execute a specific operation.
    /// </summary>
    public class SqlStatement
    {
        /// <summary>
        /// Gets or sets the <see cref="CommandType"/> to be used for the Step.
        /// </summary>
        /// <remarks>
        /// When attempting to load this value from <see cref="Microsoft.Extensions.Configuration.IConfiguration"/>,
        /// please use the numeric value representing the <see cref="CommandType"/> as follows:<br />
        /// <see cref="CommandType.Text"/> is 1.<br />
        /// <see cref="CommandType.StoredProcedure"/> is 4.<br />
        /// <see cref="CommandType.TableDirect"/> is 512.<br />
        /// </remarks>
        public CommandType CommandType { get; set; }

        /// <summary>
        /// Gets or sets the name of the statement.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets that parameter names required by <see cref="SqlStatement"/>.
        /// </summary>
        public List<string> ParameterNames { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the SQL Statement that should be executed by this step.
        /// </summary>
        public string Statement { get; set; } = string.Empty;
    }
}
