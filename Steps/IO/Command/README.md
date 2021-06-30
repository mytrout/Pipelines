# MyTrout.Pipelines.Steps.IO.Command

[![Build Status](https://github.com/mytrout/Pipelines/actions/workflows/build-pipelines-steps-io-command.yaml/badge.svg)](https://github.com/mytrout/Pipelines/actions/workflows/build-pipelines-steps-io-command.yaml)
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

    1. MyTrout.Pipelines.Steps.IO.Files 3.0.0 minimum.

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

                await host.StopAsync().ConfigureAwait(false);

                return 0;
            }
        }
    }
}

```

## How do I run dotnet Console Applications on Windows using ExecuteCommandOptions 

This configuration will run a simple Windows Console Application named AppName.exe.

```json
{
    "ExecuteCommandOptions":
    {
        Arguments: "--argument1 value1",
        CommandString: "C:\\Program Files\\AppName\\AppName.exe",
        ExpectedResult: "Hysterical Monkey"
    }
}
```
This configuration would run:
C:\\Program Files\\AppName\\AppName.exe --argument1 value1

If the execution of the command results in an output that contains 'Hysterical Monkey', a "CommandLineStatus" item of "Succeeded" will be placed in PipelineContext.
If the execution of the command results in an output does not contain 'Hysterical Monkey', a "CommandLineStatus" item of "Failed" will be placed in PipelineContext.

## How do I run dotnet Console Applications on Windows using ExecuteCommandOptions and use the IncludeFileNameTransformInArguments

The step will perform a string replacement using a value that is passed in the PipelineContext via FileConstants.TARGET_FILE.
Any number of arguments may be passed in, but must the replacement value "{0}" must be included to perform the transform.

```json
{
    "ExecuteCommandOptions":
    {
        Arguments: "--fileName: /"{0}/"",
        CommandString: "C:\\Program Files\\AppName\\AppName.exe",
        ExpectedResult: "Hysterical Monkey",
        IncludeFileNameTransformInArguments = true
    }
}
```
Once an entry named "TARGET_FILE" with a value of "D:\\Data\1234.txt" is added to PipelineContext.Items, this configuration would run:
C:\\Program Files\\AppName\\AppName.exe --fileName "D:\\Data\1234.txt"

If the execution of the command results in an output that contains 'Hysterical Monkey', a "CommandLineStatus" item of "Succeeded" will be placed in PipelineContext.Items.
If the execution of the command results in an output does not contain 'Hysterical Monkey', a "CommandLineStatus" item of "Failed" will be placed in PipelineContext.Items.

## How do I run dotnet Console Applications on Linux using ExecuteCommandOptions

The configuration to run a standard Linux Console application is slightly different compared to the Windows version.
First, the Linux Console application compiles to a .dll which must be run using the "dotnet" command.

```json
{
    "ExecuteCommandOptions":
    {
        Arguments: "/home/app/AppName/AppName.dll" --fileName {0}",
        CommandString: "dotnet",
        ExpectedResult: "Hysterical Monkey",
        IncludeFileNameTransformInArguments = true
    }
}
```
Once an entry named "TARGET_FILE" with a value of "/home/app/AppName/data.txt" is added to PipelineContext.Items, this configuration would run:
dotnet /home/app/AppName/AppName.dll --fileName /home/app/AppName/data.txt


## Build the software locally.
    1. Clone the software from the Pipelines repository.
    2. Build the software in Visual Studio 2019 to pull down all of the dependencies from nuget.org.
    3. In Visual Studio, run all tests.  All of the should pass.
    4. If you have Visual Studio Enterprise 2019, analyze the code coverage; it should be 100%.