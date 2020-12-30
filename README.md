# Pipelines

[![Build Status](https://dev.azure.com/mytrout/Pipelines/_apis/build/status/mytrout.Pipelines.Core?branchName=master)](https://dev.azure.com/mytrout/Pipelines/_build/latest?definitionId=13&branchName=master)
[![nuget](https://img.shields.io/nuget/v/MyTrout.Pipelines.svg)](https://www.nuget.org/packages/MyTrout.Pipelines/)
[![GitHub stars](https://img.shields.io/github/stars/mytrout/Pipelines.svg)](https://github.com/stefanprodan/AspNetCoreRateLimit/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/mytrout/Pipelines.svg)](https://github.com/stefanprodan/AspNetCoreRateLimit/network)
[![License: MIT](https://img.shields.io/github/license/mytrout/Pipelines.svg)](https://licenses.nuget.org/MIT)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Core&metric=alert_status)](https://sonarcloud.io/dashboard?id=Pipelines.Core)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Core&metric=coverage)](https://sonarcloud.io/dashboard?id=Pipelines.Core)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Core&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=Pipelines.Core)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Core&metric=security_rating)](https://sonarcloud.io/dashboard?id=Pipelines.Core)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Core&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=Pipelines.Core)

## Introduction
Provides a non-HTTP pipeline similar to the ASP.NET Core request pipeline.

MyTrout.Pipelines targets [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)

If three steps named M1, M2, and M3 were added to the Pipeline, here is the execution path for the code.

![](./Core/pipeline-drawing.jpg)

The Pipeline automatically adds the NoOpStep as the last step in the Pipeline.

For more details on implementing Pipelines.Steps, see [Pipelines.Steps.Core](./Steps/Core/README.md)

For more details on implementing Pipelines.Hosting, see [Pipelines.Hosting](./Hosting/README.md)

## Installing via NuGet

    Install-Package MyTrout.Pipelines

## Software dependencies
    1. Microsoft.CSharp 4.7.0 (required because of dynamic keyword usage for multi-context steps)
    2. Microsoft.Extensions.Configuration.Abstractions 5.0.0
    3. Microsoft.Extensions.Logging.Abstractions 5.0.0

All software dependencies listed above use the [MIT License](https://licenses.nuget.org/MIT).

## How do I use Pipelines?

### PLEASE NOTE: 
* This example does not use ASP.NET Hosting or Generic Hosting to implement a pipeline.
* For Console Applications, please use [Pipelines.Hosting](./Hosting/README.md)
* For Pipeline Processes running within ASP.NET Core websites, there is no current recommendation.

```csharp

namespace MyTrout.Pipelines.Samples.Simple
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Core;
    using System;
    using System.Threading.Tasks;
    using System.Linq;

    public class SimplePipeline
    {
        public static async Task ConfigureAndRunPipeline()
        {
            // Steps are added in the order they are executed on the request side
            // and will be executed in reverse on the response side.
            var builder = new PipelineBuilder()
                            .AddStep<M1>()
                            .AddStep<M2>()
                            .AddStep<M3>();

            // While using dependency injection is overkill for the simple example, 
            // real-world scenarios will likely benefit from DI for non-trivial implementations.
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            serviceCollection.AddTransient<IStepActivator, StepActivator>();
    
            // TODO: add any constructor parameter dependencies required by steps here.

            var serviceProvider = serviceCollection.BuildServiceProvider();
    
            // Build the pipeline and instantiate all steps.
            var stepActivator = serviceProvider.GetService<IStepActivator>();

            var pipeline = builder.Build(stepActivator);

            // Any name-value pairs that are required during execution would be loaded here.
            var context = new PipelineContext();
    
            // Execute the pipeline and wait for results.
            await pipeline.InvokeAsync(context).ConfigureAwait(false);

            if(context.Errors.Any())
            {
                // Throw the first exception generated by the pipeline execution.
                throw context.Errors[0];
            }
        }
    }

    public class M1 : IPipelineRequest
    {
        private readonly IPipelineRequest next;

        public M1(IPipelineRequest next) => this.next = next;

        protected virtual string Message => "I";

        public ValueTask DisposeAsync()
        {
            return new ValueTask(Task.CompletedTask);
        }

        public Task InvokeAsync(IPipelineContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Items.ContainsKey("Message"))
            {
                context.Items["Message"] += $" {this.Message}";
            }
            else
            {
                context.Items.Add("Message", this.Message);
            }

            return this.next.InvokeAsync(context);
        }
    }

    public class M2 : M1
    {
        public M2(IPipelineRequest next) : base(next)
        {
            // no op
        }

        protected override string Message => "AM";
    }

    public class M3 : M1
    {
        public M3(IPipelineRequest next) : base(next)
        {
            // no op
        }

        protected override string Message => "HERE!";
    }
}
```

The previous code would execute per the text below:

* M1 Step - All code in M1.InvokeAsync() before the call to this.next.InvokeAsync().
* M2 Step - All code in M2.InvokeAsync() before the call to this.next.InvokeAsync().
* M3 Step - All code in M3.InvokeAsync() before the call to this.next.InvokeAsync().

* NoOpStep - Returns a CompletedTask to start the Response side of the Pipeline.

* M3 Step - All code in M3.InvokeAsync() after the call to this.next.InvokeAsync().
* M2 Step - All code in M2.InvokeAsync() after the call to this.next.InvokeAsync().
* M1 Step - All code in M1.InvokeAsync() after the call to this.next.InvokeAsync().

The result of the execution as defined above is a string named "Message" in PipelineContext.Items with the value "I AM HERE!";

## How do I write Steps?
Please refer to the [Pipeline.Steps.Core](./Steps/Core/README.md) for more details on how to write steps.

## Build the software locally.
    1. Clone the software from the Pipelines repository.
    2. Build the software in Visual Studio 2019 v16.8 or higher to pull down all of the dependencies from nuget.org.
    3. In Visual Studio, run all tests.  All of the should pass.
    4. If you have Visual Studio Enterprise 2019, analyze the code coverage; it should be 100%.

## Build the software in Azure DevOps.
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
    12. Create a New Pipeline and reference the azure-pipelines.yml file in the /Core directory.
    13. Run the newly created pipeline.
