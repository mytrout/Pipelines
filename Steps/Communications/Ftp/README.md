# MyTrout.Pipelines.Steps.Communications.Ftp

[![Build Status](https://github.com/mytrout/Pipelines/actions/workflows/build-pipelines-steps-communications-ftp.yaml/badge.svg)](https://github.com/mytrout/Pipelines/actions/workflows/build-pipelines-steps-communications-ftp.yaml)
[![nuget](https://buildstats.info/nuget/MyTrout.Pipelines.Steps.Communications.Ftp?includePreReleases=true)](https://www.nuget.org/packages/MyTrout.Pipelines.Steps.Communications.Ftp/)
[![GitHub stars](https://img.shields.io/github/stars/mytrout/Pipelines.svg)](https://github.com/mytrout/Pipelines/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/mytrout/Pipelines.svg)](https://github.com/mytrout/Pipelines/network)
[![License: MIT](https://img.shields.io/github/license/mytrout/Pipelines.svg)](https://licenses.nuget.org/MIT)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.Communications.Ftp&metric=alert_status)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.Communications.Ftp)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.Communications.Ftp&metric=coverage)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.Communications.Ftp)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.Communications.Ftp&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.Communications.Ftp)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.Communications.Ftp&metric=security_rating)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.Communications.Ftp)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.Communications.Ftp&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.Communications.Ftp)

## Introduction

MyTrout.Pipelines.Steps.Communications.Ftp provides a Pipeline steps to send information via an HTTP Request.

MyTrout.Pipelines.Steps.Communications.Ftp targets [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)

For more details on Pipelines, see [Pipelines.Core](../../Core/README.md)

For more details on Pipelines.Hosting, see [Pipelines.Hosting](../../Hosting/README.md)

For a list of available steps, see [Available Steps](../../README.md)

## Installing via NuGet

    Install-Package MyTrout.Pipelines.Steps.Communications.Ftp

## Software dependencies

    1. MyTrout.Pipelines 3.1.0
    2. WinSCP 5.19.2

All software dependencies listed above use the [MIT License](https://licenses.nuget.org/MIT).

## How do I use [SendFtpRequestStep](./src/SendFtpRequestStep.cs) in this library?

### sample C# code

```csharp

    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Hosting;
    using MyTrout.Pipelines.Steps.Communications.Ftp;
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
                                        builder
                                            .AddStep<StepThatLoadsStream>()
                                            .AddStep<SendFtpRequest>()
                                            .AddStep<StepThatProcessesTheStream>();
                                    })
                                    .ConfigureServices(services => 
                                    {
                                        services.AddSingleton<FtpClient>(new FtpClient());
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
### sample appsettings.json file

```json
{
    "Method": "FtpMethod.GET",
    "HeaderNames": [ "Accept", "Authorization", "User-Agent"],
    "FtpEndPoint": "https://api.someapi.com",
    "UploadInputStreamAsContent": true
}
```

### Notes about usage
1. Any Header names configured from ~Options should be added to the PipelineContext by another step.
For instance, each of the values in the previous example ("Accept", "Authorization", and "User-Agent") would need to be 
added into the context.Items collection with the respective name to be picked up and used for Headers by the SendFtpRequestStep.

2. An FtpClient must be loaded into the Dependency Injection provider as a Singleton to ensure that this process does not exhaust sockets.