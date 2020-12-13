# MyTrout.Pipelines.Steps

MyTrout.Pipelines.Steps provides step base classes and pipeline Stream manipulation implementations MyTrout.Pipelines.

MyTrout.Pipelines.Steps targets [.NET Standard 2.1](https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support)

If three steps named M1, M2, and M3 were added to the Pipeline, here is the execution path for the code.

![](pipeline-drawing.jpg)

For more details on Pipelines, see [Pipelines.Core](../Core/README.md)

For more details on Pipelines.Hosting, see [Pipelines.Hosting](../../Hosting/README.md)

For a list of available steps, see [Available Steps](../Steps/README.md)

# Installing via NuGet

    Install-Package MyTrout.Pipelines.Steps

# Software dependencies
    1. Microsoft.CSharp 4.7.0 (required because of dynamic keyword usage for multi-context steps)
    2. Microsoft.Extensions.Configuration.Abstractions 3.1.5
    3. Microsoft.Extensions.Logging.Abstractions 3.1.5
    4. MyTrout.Pipelines 1.1.*

All software dependencies listed above use the [MIT License](https://licenses.nuget.org/MIT).

# How do I write a "simple" Step?
At a minimum, each step should implement the [IPipelineRequest](../Core/src/IPipelineRequest.cs) interface which implements [IAsyncDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.iasyncdisposable?view=dotnet-plat-ext-3.1). 

Review the [NoOpStep](../Core/src/Steps/NoOpStep.cs) for the minimum viable implementation.

# How do I write a "simple" step without user-configurable options?
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

# How do I write a "simple" step that has an Options configurable class in the recommended pattern?
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

# Other implementations in this library

## MoveInputStreamToOutputStreamStep
- moves a stream defined as PipelineContext.Items["PIPELINE_INPUT_STREAM"] to PipelineContext.Items["PIPELINE_OUTPUT_STREAM"] on the request side.  When the response side is executed, the step restored the value from PipelineContext.Items["PIPELINE_OUTPUT_STREAM"] to PipelineContext.Items["PIPELINE_INPUT_STREAM"].
- requires no options to execute.
- uses [PipelineContextConstants](src/PipelineContextConstants.cs) for key values defined above.
- does not close or otherwise manipulate the Stream.

## MoveOuputStreamToInputStreamStep
- moves a stream defined as PipelineContext.Items["PIPELINE_OUTPUT_STREAM"] to PipelineContext.Items["PIPELINE_INPUT_STREAM"] on the request side.  When the response side is executed, the step restored the value from PipelineContext.Items["PIPELINE_INPUT_STREAM"] to PipelineContext.Items["PIPELINE_OUTPUT_STREAM"].
- requires no options to execute.
- uses [PipelineContextConstants](src/PipelineContextConstants.cs) for key values defined above.
- does not close or otherwise manipulate the Stream.

## MoveInputStreamToOutputStreamStep

# Build the software locally.
    1. Clone the software from the Pipelines repository.
    2. Build the software in Visual Studio 2019 to pull down all of the dependencies from nuget.org.
    3. In Visual Studio, run all tests.  All of the should pass.
    4. If you have Visual Studio Enterprise 2019, analyze the code coverage; it should be 100%.

# Build the software in Azure DevOps.
    1. In Organization Settings, select Extensions option.
    2. Install the SonarCloud Extension.
    3. Login to the SonarQube instance and generate a SonarQube token with the user account to use for running analysis.
    4. In Project Settings, select Service Connections option.
    5. Add a Service Connection for SonarQube and enter the token.
    6. Make sure you check the 'Grant access permission to all pipelines' checkbox or configure appropriate security to this connection.
    7. In Artifacts, add a new Feed named mytrout.
    8. On the mytrout Artifacts feed, select the gear icon to configure the feed.
    9. Select the Permissions tab, and click the ...
    10. Click on Allow builds and Releases (which will add Project Collection Build Services as a Contributor).
    11. Click on Allow project-scoped builds (which will add Pipeline Build Service as a Contributor)
    12. Create a New Pipeline and reference the azure-pipelines.yml file in the /Steps/Core directory.
    13. Run the newly created pipeline.
