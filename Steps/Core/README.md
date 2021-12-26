# MyTrout.Pipelines.Steps 
*All classes that were contained in MyTrout.Pipelines.Steps v2.1.1 and lower have been combined into MyTrout.Pipelines 3.0.0 and higher*

[![Build Status](https://dev.azure.com/mytrout/Pipelines/_apis/build/status/mytrout.Pipelines.Steps.Core?branchName=master)](https://dev.azure.com/mytrout/Pipelines/_build/latest?definitionId=14&branchName=master)
[![nuget](https://img.shields.io/nuget/v/MyTrout.Pipelines.Steps.svg)](https://www.nuget.org/packages/MyTrout.Pipelines.Steps/)
[![GitHub stars](https://img.shields.io/github/stars/mytrout/Pipelines.svg)](https://github.com/stefanprodan/AspNetCoreRateLimit/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/mytrout/Pipelines.svg)](https://github.com/stefanprodan/AspNetCoreRateLimit/network)
[![License: MIT](https://img.shields.io/github/license/mytrout/Pipelines.svg)](https://licenses.nuget.org/MIT)


[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.Core&metric=alert_status)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.Core)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.Core&metric=coverage)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.Core)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.Core&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.Core)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.Core&metric=security_rating)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.Core)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.Core&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.Core)

## Introduction
MyTrout.Pipelines.Steps provides step base classes and pipeline Stream manipulation implementations MyTrout.Pipelines.

MyTrout.Pipelines.Steps targets [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)

If three steps named M1, M2, and M3 were added to the Pipeline, here is the execution path for the code.

![](pipeline-drawing.jpg)

For more details on Pipelines, see [Pipelines.Core](../../Core/README.md)

For more details on Pipelines.Hosting, see [Pipelines.Hosting](../../Hosting/README.md)

For a list of available steps, see [Available Steps](../README.md)

## Installing via NuGet

    Install-Package MyTrout.Pipelines.Steps

## Software dependencies
    1. Microsoft.CSharp 4.7.0 (required because of dynamic keyword usage for multi-context steps)
    2. Microsoft.Extensions.Configuration.Abstractions 5.0.0
    3. Microsoft.Extensions.Logging.Abstractions 5.0.0
    4. MyTrout.Pipelines 2.0.7 minimum, 2.*.* is acceptable.

All software dependencies listed above use the [MIT License](https://licenses.nuget.org/MIT).

## How do I write a "simple" Step?
At a minimum, each step should implement the [IPipelineRequest](../../Core/src/IPipelineRequest.cs) interface.

Review the [NoOpStep](../Core/src/Steps/NoOpStep.cs) for the minimum viable implementation.

## How do I write a "simple" step without user-configurable options?
To simplify the implemenation of a step, use the [AbstractPipelineStep&lt;TStep&gt;](Core/src/Steps/AbstractPipelineStep{TStep}.cs).

The base class provides the following capabilities to shortcut development time:
* an implementation of IAsyncDisposable so that steps that have no resources can ignore that implementation.
* an assertion that the context parameter is not null.
* unhandled exceptions in the step will automatically be added to the context.Errors collection.

```csharp

// Documentation removed for brevity.
namespace Steps.SampleCode
{
    using Microsoft.Extensions.Logging;
    using MyTrout.Pipelines.Core;
    
    public class MyFirstStep : AbstractPipelineStep<MyFirstStep>
    {
        // All steps require a logger
        public MyFirstStep(ILogger<MyFirstStep> logger, IPipelineRequest next)
        : base(logger, next)
        { }
        
        protected override async Task InvokeCoreAsync(IPipelineContext context)
        {
            // code on the request side of the pipeline goes here!
            
            await this.Next.InvokeAsync(context).ConfigureAwait(false);

            // code on the response side of the pipeline goes here!
        }
    }
}
```

## How do I write a "simple" step that has an Options configurable class in the recommended pattern?
To simplify the implemenation of a step, use the [AbstractPipelineStep&lt;TStep,tOptions&gt;](Core/src/Steps/AbstractPipelineStep{TStep,TOptions}.cs).

The base class provides the following capabilities to shortcut development time:
* an implementation of IAsyncDisposable so that steps that have no resources can ignore that implementation.
* an assertion that the context parameter is not null.
* an assertion that the options parameter is not null.
* unhandled exceptions in the step will automatically be added to the context.Errors collection.

```csharp

// Documentation removed for brevity.
namespace Steps.SampleCode
{
    using Microsoft.Extensions.Logging;
    using MyTrout.Pipelines.Core;
    
    public class MyFirstStep : AbstractPipelineStep<MyFirstStep, MyFirstOptions>
    {
        // All steps require a logger
        public MyFirstStep(ILogger<MyFirstStep> logger, MyFirstOptions options, IPipelineRequest next)
        : base(logger, options, next)
        { }
        
        protected override async Task InvokeCoreAsync(IPipelineContext context)
        {
            // access custom options via property named this.Options.
            if(string.IsNullOrWhiteSpace(this.Options.ConnectionString))
            {
                throw new InvalidOperationException("Connection String is null, empty or whitespace.");
            }

            // code on the request side of the pipeline goes here!
            
            await this.Next.InvokeAsync(context).ConfigureAwait(false);

            // code on the response side of the pipeline goes here!
        }
    }

    public class MyFirstOptions
    {
        public string ConnectionString { get; set; }
    }
}
```

## Other implementations in this library

### MoveInputStreamToOutputStreamStep
- moves a stream defined as PipelineContext.Items["PIPELINE_INPUT_STREAM"] to PipelineContext.Items["PIPELINE_OUTPUT_STREAM"] on the request side.  When the response side is executed, the step restored the value from PipelineContext.Items["PIPELINE_OUTPUT_STREAM"] to PipelineContext.Items["PIPELINE_INPUT_STREAM"].
- requires no options to execute.
- uses [PipelineContextConstants](src/PipelineContextConstants.cs) for key values defined above.
- does not close or otherwise manipulate the Stream.

### MoveOuputStreamToInputStreamStep
- moves a stream defined as PipelineContext.Items["PIPELINE_OUTPUT_STREAM"] to PipelineContext.Items["PIPELINE_INPUT_STREAM"] on the request side.  When the response side is executed, the step restored the value from PipelineContext.Items["PIPELINE_INPUT_STREAM"] to PipelineContext.Items["PIPELINE_OUTPUT_STREAM"].
- requires no options to execute.
- uses [PipelineContextConstants](src/PipelineContextConstants.cs) for key values defined above.
- does not close or otherwise manipulate the Stream.

## Build the software locally.
    1. Clone the software from the Pipelines repository.
    2. Build the software in Visual Studio 2019 to pull down all of the dependencies from nuget.org.
    3. In Visual Studio, run all tests.  All of the should pass.
    4. If you have Visual Studio Enterprise 2019, analyze the code coverage; it should be 100%.
