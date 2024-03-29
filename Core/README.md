# Pipelines

[![Build Status](https://github.com/mytrout/Pipelines/actions/workflows/build-pipelines-core.yaml/badge.svg)](https://github.com/mytrout/Pipelines/actions/workflows/build-pipelines-core.yaml)
[![nuget](https://img.shields.io/nuget/v/MyTrout.Pipelines.svg)](https://www.nuget.org/packages/MyTrout.Pipelines/)
[![GitHub stars](https://img.shields.io/github/stars/mytrout/Pipelines.svg)](https://github.com/mytrout/Pipelines/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/mytrout/Pipelines.svg)](https://github.com/mytrout/Pipelines/network)
[![License: MIT](https://img.shields.io/github/license/mytrout/Pipelines.svg)](https://github.com/mytrout/Pipelines/blob/master/LICENSE)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Core&metric=alert_status)](https://sonarcloud.io/dashboard?id=Pipelines.Core)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Core&metric=coverage)](https://sonarcloud.io/dashboard?id=Pipelines.Core)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Core&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=Pipelines.Core)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Core&metric=security_rating)](https://sonarcloud.io/dashboard?id=Pipelines.Core)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Core&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=Pipelines.Core)

## Introduction

MyTrout.Pipelines provides a non-HTTP pipeline similar to the ASP.NET Core request pipeline.

MyTrout.Pipelines targets [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0), [.NET 7.0](https://dotnet.microsoft.com/download/dotnet/7.0), and [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)

If three steps named M1, M2, and M3 were added to the Pipeline, here is the execution path for the code.

![](pipeline-drawing.jpg)

The Pipeline automatically adds the NoOpStep as the last step in the Pipeline.

For more details on implementing Pipelines.Steps, see [Pipelines.Steps.Core](../Steps/Core/README.md)

For more details on implementing Pipelines.Hosting, see [Pipelines.Hosting](../Hosting/README.md)

## Installing via NuGet

    Install-Package MyTrout.Pipelines

## Software dependencies
    1. Microsoft.Extensions.Configuration.Abstractions 8.0.0
    2. Microsoft.Extensions.Configuration.Binder 8.0.0
    3. Microsoft.Extensions.DependencyInjection.Abstractions 8.0.0
    4. Microsoft.Extensions.Logging.Abstractions 8.0.0

All software dependencies listed above use the [MIT License](https://licenses.nuget.org/MIT).

## NON-BREAKING CHANGES INTRODUCED WITH 4.0.1

See [Non-breaking Changes for 4.0.1](./pipelines-core-nonbreaking-changes-4-0-1.md)

## BREAKING CHANGES INTRODUCED WITH 4.0.0

See [Breaking Changes for 4.0.0](./pipelines-core-breaking-changes-4-0-0.md)

## How do I use Pipelines?

### PLEASE NOTE: 
* This example does not use ASP.NET Hosting or Generic Hosting to implement a pipeline.
* For Console Applications, please use [Pipelines.Hosting](../Hosting/README.md)
* For Pipeline Processes running within ASP.NET Core websites, there is no current recommendation.

```csharp

namespace MyTrout.Pipelines.Samples.Simple
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Core;
    using System;
    using System.Threading.Tasks;
    using System.Linq;

    public class Program
    {
        static async Task Main()
        {
            // For this simple example, nothing needs to be configured, but the IConfiguration root is required.
            var config = new ConfigurationBuilder()
                            .Build();
            
            var serviceProvider = new ServiceCollection()
                                            .AddLogging();
                                            .AddTransient<IStepActivator, StepActivator>();
                                            .AddScoped<IConfiguration>(_ => config) 
                                            .Build();

            // Steps are added in the order they are executed on the request side
            // and will be executed in reverse on the response side.
            var builder = new PipelineBuilder()
                            .AddStep<M1>()
                            .AddStep<M2>()
                            .AddStep<M3>();
    
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
Please refer to the [Pipeline.Steps.Core](../Steps/Core/README.md) for more details on how to write steps.

## Build the software locally.
    1. Clone the software from the Pipelines repository.
    2. Build the software in Visual Studio 2019 v16.8 or higher to pull down all of the dependencies from nuget.org.
    3. In Visual Studio, run all tests.  All of the should pass.
    4. If you have Visual Studio Enterprise 2019, analyze the code coverage; it should be 100%.


## More Documentation!

* [Non-breaking Changes for 4.0.1](./pipelines-core-nonbreaking-changes-4-0-1.md)
* [Breaking Changes for 4.0.0](./pipelines-core-breaking-changes-4-0-0.md)
* [Repairing DACPAC in March 2022](./repairing-dacpac-build-after-march-2022-failures.md)
* [OpenCover is Dead!  Long Live AltCover](./opencover-is-dead-long-live-altcover.md)
* [Breaking Changes for 3.0.0](./pipelines-core-breaking-changes-3-0-0.md)
* [Github Actions and .NET 5.0](./github-actions-and-net-5.0.md)