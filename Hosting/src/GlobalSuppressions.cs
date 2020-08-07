// <copyright file="GlobalSuppressions.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2019-2020 Chris Trout
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

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "AssertParameterIsNotNull handles the null check for this parameter.", Scope = "member", Target = "~M:MyTrout.Pipelines.Hosting.AddStepDependencyExtensions.AddStepDependency``1(Microsoft.Extensions.Hosting.IHostBuilder,System.String,System.String[])~Microsoft.Extensions.Hosting.IHostBuilder")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "AssertParameterIsNotNull handles the null check for this parameter.", Scope = "member", Target = "~M:MyTrout.Pipelines.Hosting.AddStepDependencyExtensions.AddStepDependency``1(Microsoft.Extensions.Hosting.IHostBuilder,System.String,``0)~Microsoft.Extensions.Hosting.IHostBuilder")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "AssertParameterIsNotNull handles the null check for this parameter.", Scope = "member", Target = "~M:MyTrout.Pipelines.Hosting.AddStepDependencyExtensions.AddStepDependency``1(Microsoft.Extensions.Hosting.IHostBuilder,System.String,System.Func{System.IServiceProvider,``0})~Microsoft.Extensions.Hosting.IHostBuilder")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "AssertParameterIsNotNull handles the null check for this parameter.", Scope = "member", Target = "~M:MyTrout.Pipelines.Hosting.UsePipelineExtensions.UsePipeline``1(Microsoft.Extensions.Hosting.IHostBuilder,System.Action{MyTrout.Pipelines.Core.PipelineBuilder})~Microsoft.Extensions.Hosting.IHostBuilder")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "AssertParameterIsNotNull handles the null check for this parameter.", Scope = "member", Target = "~M:MyTrout.Pipelines.Hosting.UsePipelineExtensions.UsePipeline``1(Microsoft.Extensions.Hosting.IHostBuilder,MyTrout.Pipelines.Core.PipelineBuilder)~Microsoft.Extensions.Hosting.IHostBuilder")]

[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Constants should contain underscores.", Scope = "member", Target = "~F:MyTrout.Pipelines.Hosting.AddStepDependencyExtensions.STEP_CONFIG_CONTEXT")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "Constants should contain underscores.", Scope = "member", Target = "~F:MyTrout.Pipelines.Hosting.AddStepDependencyExtensions.STEP_CONFIG_CONTEXT")]
