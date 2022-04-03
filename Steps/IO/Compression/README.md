# MyTrout.Pipelines.Steps.IO.Compression

[![Build Status](https://github.com/mytrout/Pipelines/actions/workflows/build-pipelines-steps-io-compression.yaml/badge.svg)](https://github.com/mytrout/Pipelines/actions/workflows/build-pipelines-steps-io-compression.yaml)
[![nuget](https://buildstats.info/nuget/MyTrout.Pipelines.Steps.IO.Compression?includePreReleases=true)](https://www.nuget.org/packages/MyTrout.Pipelines.Steps.IO.Compression/)
[![GitHub stars](https://img.shields.io/github/stars/mytrout/Pipelines.svg)](https://github.com/mytrout/Pipelines/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/mytrout/Pipelines.svg)](https://github.com/mytrout/Pipelines/network)
[![License: MIT](https://img.shields.io/github/license/mytrout/Pipelines.svg)](https://licenses.nuget.org/MIT)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.IO.Compression&metric=alert_status)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.IO.Compression)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.IO.Compression&metric=coverage)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.IO.Compression)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.IO.Compression&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.IO.Compression)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.IO.Compression&metric=security_rating)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.IO.Compression)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.IO.Compression&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.IO.Compression)

## Introduction

MyTrout.Pipelines.Steps.IO.Compression provides Pipeline steps to manage zip archives and zip archive entries.

MyTrout.Pipelines.Steps.IO.Compression targets [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0)

For more details on Pipelines, see [Pipelines.Core](../../Core/README.md)

For more details on Pipelines.Hosting, see [Pipelines.Hosting](../../Hosting/README.md)

For a list of available steps, see [Available Steps](../README.md)

## Installing via NuGet

    Install-Package MyTrout.Pipelines.Steps.IO.Compression

## Software dependencies

    1. MyTrout.Pipelines.Steps 3.2.0 minimum.

All software dependencies listed above use the [MIT License](https://licenses.nuget.org/MIT).

## How do I create a new zip archive and add entries using this library ?

```csharp

    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Hosting;
    using MyTrout.Pipelines.Steps;
    using MyTrout.Pipelines.Steps.IO.Compression;
    // using MyTrout.Pipelines.Steps.IO.Directories; -> Coming soon.
    using MyTrout.Pipelines.Steps.IO.Files;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    namespace MyTrout.Pipeline.IO.Compression.Samples
    {
        public class Program
        {
            public static async Task Main(string[] args)
            {

                var host = Host.CreateDefaultBuilder(args)
                                    .UsePipeline(builder => 
                                    {
                                        builder
                                            .AddStep<CreateNewZipArchiveStep>()
                                            
                                            // Configured for ExecutionTimings.After only to force the step to write (on the Response side).
                                            .AddStep<WriteStreamToFileSystemStep>()
                                            
                                            // Closes the Archive Step so the OUTPUT_STREAM is populated (on the Response side)
                                            .AddStep<CloseZipArchiveStep>()
                                            
                                            // This step functionality add each file one at a time to an INPUT_STREAM. (coming soon as Steps.IO.Directories)
                                            // .AddStep<LoopThroughFilesInDirectoryStep>()
                                            
                                            // Read a file from the file system into the INPUT_STREAM.
                                            .AddStep<ReadStreamFromFileSystem>()
                                            
                                            // ReadStreamFromFileSystem names the Stream INPUT_STREAM.
                                            // AddZipArchiveEntry needs the Stream named OUTPUT_STREAM.
                                            // Located in Pipelines.Steps
                                            .AddStep<MoveInputStreamToOutputStream>() 
                                            
                                            .AddStep<AddZipArchiveEntryStep>();
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
    2. Build the software in Visual Studio 2022 to pull down all of the dependencies from nuget.org.
    3. In Visual Studio, run all tests.  All of the should pass.
    4. If you have Visual Studio Enterprise 2022, analyze the code coverage; it should be 100%.
