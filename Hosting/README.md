# MyTrout.Pipelines.Hosting

MyTrout.Pipelines.Hosting provides helper classes to run a pipeline using Microsoft's Generic Host.

[![Build Status](https://dev.azure.com/mytrout/Pipelines/_apis/build/status/mytrout.Pipelines.Hosting?branchName=master)](https://dev.azure.com/mytrout/Pipelines/_build/latest?definitionId=15&branchName=master)
[![nuget](https://img.shields.io/nuget/v/MyTrout.Pipelines.HOsting.svg)](https://www.nuget.org/packages/MyTrout.Pipelines.Hosting/)
[![GitHub stars](https://img.shields.io/github/stars/mytrout/Pipelines.svg)](https://github.com/mytrout/Pipelines/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/mytrout/Pipelines.svg)](https://github.com/mytrout/Pipelines/network)
[![License: MIT](https://img.shields.io/github/license/mytrout/Pipelines.svg)](https://licenses.nuget.org/MIT)

MyTrout.Pipelines targets [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)

For more details on Pipelines, see [Pipelines.Core](../Core/README.md)

For more details on Pipelines.Steps, see [Pipelines.Steps.Core](../Steps/Core/README.md)

# Installing via NuGet

    Install-Package MyTrout.Pipelines.Hosting

# Software dependencies
    1. Microsoft.Hosting 5.0.0
    2. Microsoft.Hosting.Abstractions 5.0.0
    3. MyTrout.Pipelines 2.0.7 minimum, 2.*.* is acceptable.

All software dependencies listed above use the [MIT License](https://licenses.nuget.org/MIT).

# How do I use Pipelines.Hosting?

```csharp

    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Hosting;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    namespace MyTrout.Pipeline.Hosting.Samples
    {
        public class Program
        {
            public static async Task Main(string[] args)
            {

                var host = Host.CreateDefaultBuilder(args)
                                    .UsePipeline(builder => 
                                    {
                                        builder.AddStep<Step1>()
                                                .AddStep<Step2>()
                                                .AddStep<Step3>();
                                    })
                                    .Build();

                //
                // IMPORTANT NOTE FOR DEVELOPERS:
                // 
                // Use StartAsync() to allow the caller to review the PipelineContext after execution.
                //
                await host.StartAsync().ConfigureAwait(false);

                var context = host.Services.GetService<PipelineContext>();

                if(context.Errors.Any())
                {
                    // TODO: Errors have already been logged, do any special error processing here.
                }

                return 0;
            }
        }
    }
}

```

# How do I use Pipelines.Hosting with different configurations for different instances of the same step.

If Step1 prints the Step1Options value with a trailing space to the Console when each step is called, then the following code will generate "Moe, Larry & Curly ".

```csharp

    using MyTrout.Pipelines.Core;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    namespace MyTrout.Pipeline.Hosting.Samples
    {
        public class Program
        {
            public static async Task Main(string[] args)
            {
                // IMPORTANT NOTE FOR DEVELOPERS !
                // 
                // Step Dependencies with context must be defined BEFORE UsePipelines() to load the dependencies correctly.
                //

                var host = Host.CreateDefaultBuilder(args)
                                    .AddStepDependency("context-1", new Step1Options("Moe,"))
                                    .AddStepDependency("context-2", new Step1Options("Larry"))
                                    .AddStepDependency("context-3", new Step1Options("&"))
                                    .AddStepDependency("context-4", new Step1Options("Curly"))
                                    .UsePipeline(builder => 
                                    {
                                        builder.AddStep<Step1>("context-1")
                                            .AddStep<Step1>("context-2")
                                            .AddStep<Step1>("context-3")
                                            .AddStep<Step1>("context-4")
                                    })
                                    .Build();
                
                // IMPORTANT NOTE FOR DEVELOPERS !
                // 
                // Use StartAsync() to allow the caller to review the PipelineContext after execution.
                //

                await host.StartAsync().ConfigureAwait(false);

                var context = host.Services.GetService<PipelineContext>();

                if(context.Errors.Any())
                {
                    // TODO: Errors have already been logged, do any special error processing here.
                }

                return 0;
            }
        }
    }
}
```