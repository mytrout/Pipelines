// <copyright file="RenameContextItemOptions.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2021-2022 Chris Trout
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

namespace MyTrout.Pipelines.Steps
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    /*
     *  IMPORTANT NOTE: As long as this class only contains compiler-generated functionality, it requires no unit tests.
     */

    /// <summary>
    /// Provides caller-configurable options to change the behavior of <see cref="RenameContextItemStep"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class RenameContextItemOptions
    {
        /// <summary>
        /// Gets or sets whether the OS System is Windows or *nix so the correct <see cref="EnvironmentVariableTarget"/> can be used.
        /// </summary>
        /// <remarks>
        /// While this is not strictly "compiler-generated" functionality,
        /// it is a framework-level check which is extremely unlikely to fail.
        /// Anyone who can force this to fail without doing stupid ILDASM tricks, please contact me.
        /// </remarks>
        public EnvironmentVariableTarget EnvironmentVariableTarget { get; set; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? EnvironmentVariableTarget.Machine : EnvironmentVariableTarget.Process;

        /// <summary>
        /// Gets or sets the Environment Variable Name for the PreventCollisionsWithRenamedValueNames.
        /// </summary>
        /// <remarks>
        /// <para>This property is used for testing the Prevent Collisions with Renamed Values by changing the name
        /// of the Environment Variable prevent unit test threading race conditions.</para>
        /// <para>It can be used to redefine the name of the environment variable, but it is not recommended.</para>
        /// </remarks>
        public string PreventCollisionsWithRenamedValueNamesEnvironmentVariableName { get; set; } = "PIPELINE_PreventCollisionsWithRenamedValueNames";

        /// <summary>
        /// Gets or sets a list of names to be renamed in <see cref="IPipelineContext.Items"/>.
        /// </summary>
        public Dictionary<string, string> RenameValues { get; set; } = new Dictionary<string, string>();
    }
}
