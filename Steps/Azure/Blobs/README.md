# MyTrout.Pipelines.Steps.Azure.Blobs

[![Build Status](https://github.com/mytrout/Pipelines/actions/workflows/build-pipelines-steps-azure-blobs.yaml/badge.svg)](https://github.com/mytrout/Pipelines/actions/workflows/build-pipelines-steps-azure-blobs.yaml)
[![nuget](https://buildstats.info/nuget/MyTrout.Pipelines.Steps.Azure.Blobs?includePreReleases=true)](https://www.nuget.org/packages/MyTrout.Pipelines.Steps.Azure.Blobs/)
[![GitHub stars](https://img.shields.io/github/stars/mytrout/Pipelines.svg)](https://github.com/mytrout/Pipelines/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/mytrout/Pipelines.svg)](https://github.com/mytrout/Pipelines/network)
[![License: MIT](https://img.shields.io/github/license/mytrout/Pipelines.svg)](https://licenses.nuget.org/MIT)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.Azure.Blobs&metric=alert_status)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.Azure.Blobs)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.Azure.Blobs&metric=coverage)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.Azure.Blobs)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.Azure.Blobs&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.Azure.Blobs)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.Azure.Blobs&metric=security_rating)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.Azure.Blobs)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.Azure.Blobs&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.Azure.Blobs)


## Introduction
MyTrout.Pipelines.Steps.Azure.Blobs provides Pipeline steps to read, write, and delete blobs on the Azure Storage.

MyTrout.Pipelines.Steps.Azure.Blobs targets [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)

For more details on Pipelines, see [Pipelines.Core](../../../Core/README.md)

For more details on Pipelines.Hosting, see [Pipelines.Hosting](../../../Hosting/README.md)

For a list of available steps, see [Available Steps](../../README.md)

## Installing via NuGet

    Install-Package MyTrout.Pipelines.Steps.Azure.Blobs

## Software dependencies

    1. Azure.Storage.Blobs 12.9.0
    2. MyTrout.Pipelines 3.0.1 minimum.

All software dependencies listed above use the [MIT License](https://licenses.nuget.org/MIT).

## How do I use the steps in this library?

### sample C# code

```csharp

    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Hosting;
    using MyTrout.Pipelines.Steps.Azure.Blobs;
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
                                        // The first step doesn't exist and  must be user-provided.
                                            .AddStep<LoadTargetFileNameStep>()
                                            .AddStep<DeleteBlobStep>()
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

## How do I use Pipelines.Hosting with different configurations for different instances of the same step.

If Step1 prints the Step1Options value with a trailing space to the Console when each step is called, then the following code will generate "Moe, Larry & Curly ".

```csharp

    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Hosting;
    using MyTrout.Pipelines.Steps.Azure.Blobs;
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
                                            .AddStep<LoadTheFirstBlobLocationStep>()
                                            .AddStep<DeleteBlobStep>("context-A")
                                            .AddStep<LoadTheSecondBlobLocationStep>()
                                            .AddStep<DeleteBlobStep>("context-B")
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

                await host.StopAsync().ConfigureAwait(false);

                return 0;
            }
        }
    }
}
```

## Build the software locally.
    1. Clone the software from the Pipelines repository.
    2. Build the software in Visual Studio 2019 to pull down all of the dependencies from nuget.org.
    3. Create an Azure Storage Account in the Azure Portal and capture the a Connection String to the storage account.
    4a. On Windows, Create a Machine Environment Variable named "PIPELINE_TEST_AZURE_BLOB_CONNECTION_STRING" and put the Azure Blob Storage Connection String in it.
    4b. On Linux, create an environment variable in the Test Project named "PIPELINE_TEST_AZURE_BLOB_CONNECTION_STRING" and put the Azure Blob Storage Connection String in it.
    5. In Visual Studio, run all tests.  All of the should pass.
    6. If you have Visual Studio Enterprise 2019, analyze the code coverage; it should be 100%.
    
    IMPORTANT NOTE: If you are running on Linux, do not check in the Process Level Environment variable used for tests.

## Build the software in GitHub Actions
    1. Navigate to Organization Secrets.
    2. Add an Organization Secret named 'MYTROUT_AZURE_SUBSCRIPTION' that contains the GUID that identifies the Azure Subscription used for Integration Testing.
    3. Add an Organization Secret named 'MYTROUT_AZURE_TOKEN' that contains the Azure Token used to access the Azure Subscription used for Integration Testing.
    4. Add an Organization Secret named 'MYTROUT_NUGET_API_KEY' that contains the API Key used to publish to nuget.org.
    5. Add an Organization Secret named 'MYTROUT_SONARQUBE_API_KEY' that contains the API Key used to publish results to the SonarCloud.io instance.
    6. Add an Organization Secret named 'MYTROUT_SONARQUBE_HOST_URL' that contains the URL where SonarQube is hosted.
    7. Add an Organization Secret named 'MYTROUT_SONARQUBE_ORGANIZATION' that contains the organization to which one is publishing in SonarCloud.io.
    8. Run the build-pipelines-steps.azure.blobs.yaml pipeline.

## Testing
These tests are a mixture of unit and integration tests necessary to perform blob storage tasks.
To ensure all tests run correctly, an environment variable named "PIPELINE_TEST_AZURE_BLOB_CONNECTION_STRING" must be created with full rights on an existing blob container location in Azure.
