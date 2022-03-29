// <copyright file="GlobalSuppressions.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2019-2022 Chris Trout
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

[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "Constant names should contain underscores.", Scope = "member", Target = "~F:MyTrout.Pipelines.Steps.PipelineContextConstants.INPUT_STREAM")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "Constant names should contain underscores.", Scope = "member", Target = "~F:MyTrout.Pipelines.Steps.PipelineContextConstants.OUTPUT_STREAM")]
[assembly: SuppressMessage("Minor Code Smell", "S3267:Loops should be simplified with \"LINQ\" expressions", Justification = "This code doesn't have a simple LINQ expression conversion.", Scope = "member", Target = "~M:MyTrout.Pipelines.Steps.RenameContextItemStep.InvokeCachedCoreAsync(MyTrout.Pipelines.IPipelineContext)~System.Threading.Tasks.Task")]
