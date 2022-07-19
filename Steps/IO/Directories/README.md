# MyTrout.Pipelines.Steps.IO.Directories

[![Build Status](https://github.com/mytrout/Pipelines/actions/workflows/build-pipelines-steps-io-files.yaml/badge.svg)](https://github.com/mytrout/Pipelines/actions/workflows/build-pipelines-steps-io-files.yaml)
[![nuget](https://buildstats.info/nuget/MyTrout.Pipelines.Steps.IO.Directories?includePreReleases=true)](https://www.nuget.org/packages/MyTrout.Pipelines.Steps.IO.Directories/)
[![GitHub stars](https://img.shields.io/github/stars/mytrout/Pipelines.svg)](https://github.com/mytrout/Pipelines/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/mytrout/Pipelines.svg)](https://github.com/mytrout/Pipelines/network)
[![License: MIT](https://img.shields.io/github/license/mytrout/Pipelines.svg)](https://licenses.nuget.org/MIT)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.IO.Directories&metric=alert_status)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.IO.Directories)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.IO.Directories&metric=coverage)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.IO.Directories)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.IO.Directories&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.IO.Directories)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.IO.Directories&metric=security_rating)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.IO.Directories)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.IO.Directories&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.IO.Directories)

## Introduction

MyTrout.Pipelines.Steps.IO.Directories provides Pipeline steps to read, write, delete, and move directories on the file system.

MyTrout.Pipelines.Steps.IO.Directories targets [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0) and [.NET 7.0](https://dotnet.microsoft.com/download/dotnet/7.0)

[Pipelines.Hosting](../../../Hosting/README.md) runs the Pipeline in a Console Application using Generic Host.

[List of Available Steps](../../../Steps/README.md) 

## Installing via NuGet

    Install-Package MyTrout.Pipelines.Steps.IO.Directories

## Software dependencies

    1. MyTrout.Pipelines 4.0.0 minimum.

All software dependencies listed above use the [MIT License](https://licenses.nuget.org/MIT).

## How do I use the steps in this library?

### sample code

```csharp

    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Hosting;
    using MyTrout.Pipelines.Steps.Cryptography
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
                                    .AddStepDependency<DeleteFileOptions>()
                                    .UsePipeline(builder => 
                                    {
                                        builder
                                        // The first step doesn't exist and  must be user-provided.
                                            .AddStep<LoadTargetFileNameStep>()
                                            .AddStep<DeleteFileStep>()
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
### sample appsettings.json file

This configuration provides a base directory to prevent path traversal issues. 

```json
{
    "DeleteFileBaseDirectory": "C:\"
}
```

## How do I use Pipelines.Hosting with different configurations for different instances of the same step.

If Step1 prints the Step1Options value with a trailing space to the Console when each step is called, then the following code will generate "Moe, Larry & Curly ".

### sample code
```csharp

    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Hosting;
    using MyTrout.Pipelines.Steps.Cryptography
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
                                    .UsePipeline(builder => 
                                    {
                                        builder
                                            .AddStep<LoadTheFirstMofeFileLocationsStep>()
                                            .AddStep<MoveFileStep, MoveFileOptions>("context-A")
                                            .AddStep<LoadTheSecondMoveFileLocationStep>()
                                            .AddStep<MoveFileStepStep, MoveFileOptions>("context-B")
                                            .AddStep<DoOtherActionsStep>()
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

### sample appsettings.json file

```json
{
    "context-A": {
        "MoveSourceFileBaseDirectory": "C:\\source\\",
        "MoveTargetFileBaseDirectory: "D:\\source\\"
    },
    "context-B": {
        "MoveSourceFileBaseDirectory": "D:\\source\\",
        "MoveTargetFileBaseDirectory: "E:\\source\\"
    }
}
```

## Build the software locally.
    1. Clone the software from the Pipelines repository.
    2. Build the software in Visual Studio 2019 to pull down all of the dependencies from nuget.org.
    3. In Visual Studio, run all tests.  All of the should pass.
    4. If you have Visual Studio Enterprise 2019, analyze the code coverage; it should be 100%.
