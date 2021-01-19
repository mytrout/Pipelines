# MyTrout.Pipelines.Steps.IO.Command

[![Build Status](https://dev.azure.com/mytrout/Pipelines/_apis/build/status/mytrout.Pipelines.Steps.IO.Command?branchName=master)](https://dev.azure.com/mytrout/Pipelines/_build/latest?definitionId=24&branchName=master)
[![nuget](https://buildstats.info/nuget/MyTrout.Pipelines.Steps.IO.Command?includePreReleases=true)](https://www.nuget.org/packages/MyTrout.Pipelines.Steps.IO.Command/)
[![GitHub stars](https://img.shields.io/github/stars/mytrout/Pipelines.svg)](https://github.com/mytrout/Pipelines/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/mytrout/Pipelines.svg)](https://github.com/mytrout/Pipelines/network)
[![License: MIT](https://img.shields.io/github/license/mytrout/Pipelines.svg)](https://licenses.nuget.org/MIT)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.IO.Command&metric=alert_status)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.IO.Command)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.IO.Command&metric=coverage)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.IO.Command)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.IO.Command&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.IO.Command)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.IO.Command&metric=security_rating)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.IO.Command)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.IO.Command&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.IO.Command)

## Introduction

MyTrout.Pipelines.Steps.IO.Command provides a Pipeline Step to execute a command line utility.

MyTrout.Pipelines.Steps.IO.Command targets [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)

For more details on Pipelines, see [Pipelines.Core](../../Core/README.md)

For more details on Pipelines.Hosting, see [Pipelines.Hosting](../../Hosting/README.md)

For a list of available steps, see [Available Steps](../README.md)

## Installing via NuGet

    Install-Package MyTrout.Pipelines.Steps.IO.Command

## Software dependencies

    1. MyTrout.Pipelines.Steps.IO.Files 2.0.1 minimum, 2.*.* is acceptable.

All software dependencies listed above use the [MIT License](https://licenses.nuget.org/MIT).

## How do I run the Windows Defender Antivirus command and add entries using this library ?

```csharp

    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Hosting;
    using MyTrout.Pipelines.Steps.IO.Command
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    namespace MyTrout.Pipeline.IO.Command.Samples
    {
        public class Program
        {
            public static async Task Main(string[] args)
            {
                var host = Host.CreateDefaultBuilder(args)
                                    .AddStepDependency<WindowsDefenderAmtivirusOptions>()
                                    .UsePipeline(builder => 
                                    {
                                        builder
                                            .AddStep<GetAFileNameToScanStep>()
                                            .AddStep<ExecuteCommandStep>()
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

## Build the software locally.
    1. Clone the software from the Pipelines repository.
    2. Build the software in Visual Studio 2019 to pull down all of the dependencies from nuget.org.
    3. In Visual Studio, run all tests.  All of the should pass.
    4. If you have Visual Studio Enterprise 2019, analyze the code coverage; it should be 100%.