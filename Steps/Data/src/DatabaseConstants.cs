﻿// <copyright file="DatabaseConstants.cs" company="Chris Trout">
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
#pragma warning disable CA1710, SA1310
    /// <summary>
    /// Provides constants for the Database steps.
    /// </summary>
    public static class DatabaseConstants
    {
        /// <summary>
        /// Indicates that this <see cref="MyTrout.Pipelines.Core.PipelineContext" /> item is the number of database rows affected.
        /// </summary>
        public const string DATABASE_ROWS_AFFECTED = "DATABASE_ROWS_AFFECTED";

        /// <summary>
        /// Provides the name of the database statement that needs to be run.
        /// </summary>
        public const string DATABASE_STATEMENT_NAME = "DATABASE_STATEMENT_NAME";
    }
#pragma warning restore CA1710, SA1310
}
