# MyTrout.Pipelines.Steps.Azure.ServiceBus

[![Build Status](https://github.com/mytrout/Pipelines/actions/workflows/build-pipelines-steps-azure-servicebus.yaml/badge.svg)](https://github.com/mytrout/Pipelines/actions/workflows/build-pipelines-steps-azure-servicebus.yaml)
[![nuget](https://buildstats.info/nuget/MyTrout.Pipelines.Steps.Azure.ServiceBus?includePreReleases=true)](https://www.nuget.org/packages/MyTrout.Pipelines.Steps.Azure.ServiceBus/)
[![GitHub stars](https://img.shields.io/github/stars/mytrout/Pipelines.svg)](https://github.com/mytrout/Pipelines/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/mytrout/Pipelines.svg)](https://github.com/mytrout/Pipelines/network)
[![License: MIT](https://img.shields.io/github/license/mytrout/Pipelines.svg)](https://licenses.nuget.org/MIT)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.Azure.ServiceBus&metric=alert_status)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.Azure.ServiceBus)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.Azure.ServiceBus&metric=coverage)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.Azure.ServiceBus)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.Azure.ServiceBus&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.Azure.ServiceBus)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.Azure.ServiceBus&metric=security_rating)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.Azure.ServiceBus)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=Pipelines.Steps.Azure.ServiceBus&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=Pipelines.Steps.Azure.ServiceBus)

## Introduction
MyTrout.Pipelines.Steps.Azure.ServiceBus provides Pipeline steps to receive and send messages using Azure ServiceBus.

MyTrout.Pipelines.Steps.Azure.ServiceBus targets [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0)

[Pipelines.Hosting](../../Hosting/README.md) runs the Pipeline in a Console Application using Generic Host.

[List of Available Steps](../README.md) 

## Installing via NuGet

    Install-Package MyTrout.Pipelines.Steps.Azure.ServiceBus

## Software dependencies

    1. Azure.Messaging.ServiceBus 7.5.1 minimum, less than 7.6.x
    2. MyTrout.Pipelines.Steps 2.3.0 minimum, less than 2.4.x
    
All software dependencies listed above use the [MIT License](https://licenses.nuget.org/MIT).


## BREAKING CHANGES FROM v1.x to V2.0
- Update from .NET Standard 2.1 to .NET 5.0

## How do I use the steps in this library ([ReadMessageFromAzureStep](./src/ReadMessageFromAzureStep.cs)) ?

### sample C# code

```csharp

    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Hosting;
    using MyTrout.Pipelines.Steps.Azure.ServiceBus
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    namespace MyTrout.Pipeline.Azure.ServiceBus.Samples
    {
        public class Program
        {
            public static async Task Main(string[] args)
            {

                var host = Host.CreateDefaultBuilder(args)
                                    .AddStepDependency<ReadMessageFromAzureOptions>()
                                    .UsePipeline(builder => 
                                    {
                                        builder
                                            // NOTE: ReadMessageFromAzureStep will continue to read (and process) messages 
                                            //       one at a time until there are no more messages on the subscription.
                                            .AddStep<ReadMessageFromAzureStep>()
                                            .AddStep<UserDefinedStepThatProcessesMessageBodyIntoPipelineContext>()
                                            .AddStep<UserDefinedStepThatDoesSomethingWithPipelineContext>();
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

ApplicationrProperties are values that should be copied from the ApplicationProperties collection of the message to the PipelineContext.

```json
{
    "AzureServiceBusConnectionString": "user supplied ASB ConnectionString",
    "EntityName": "topic/subscription"
    "ApplicationProperties": [ "Year", "Month", "Id" ]
}
```

## How do I configure TimeSpan via the .NET ConfigurationManager.

The appropriate format for configuring a timespan using the .NET Configuration abstraction is ````D.HH.mm.nn````.

For example, the default timespan configurations for ReadMessageFromAzureOptions would be:

```json
{
    "TimeToWaitForNewMessage": "0.00:01:00",
    "TimeToWaitBetweenMessageChecks": "0.00:00:02"
}
```

## Build the software locally.
    1. Clone the software from the Pipelines repository.
    2. Build the software in Visual Studio 2019 to pull down all of the dependencies from nuget.org.
    3. Deploy the Azure ResourceGroup Template located at [azure-arm-resourcegroup.json](./azure-arm-resourcegroup.json))  to create the Resource Group.
    3. Deploy the Azure ServiceBus Template located at [azure-arm-servicebus.json](./azure-arm-servicebus.json))  to initialize the Azure Service Bus connectivity.
    4. Get the Primary Connection String for the Azure Service Bus Instance.
    5. Create a new machine-level environment variable named 'TEST_PIPELINE_AZURE_SERVICE_BUS_CONNECTION_STRING' and set the value to the Azure Service Bus Connection string.
    6. In Visual Studio, run all tests.  All of the should pass.
    7. If you have Visual Studio Enterprise 2019, analyze the code coverage; it should be 100%.

## Build the software in Azure DevOps.
    1. In Organization Settings, select Extensions option.
    2. Install the SonarCloud Extension.
    3. Login to the SonarQube instance and generate a SonarQube token with the user account to use for running analysis.
    4. In Project Settings, select Service Connections option.
    5. Add a Service Connection for SonarQube and enter the token.
       - Make sure you check the 'Grant access permission to all pipelines' checkbox or configure appropriate security to this connection.
    6. Add a Service Connection for an Azure Subscription 
       - The Azure Service Principal must have ability to create and delete Resource Groups/Azure Service Bus namespaces.
       - Make sure you check the 'Grant access permission to all pipelines' checkbox or configure appropriate security to this connection.
    7. In Artifacts, add a new Feed named mytrout.
    8. On the mytrout Artifacts feed, select the gear icon to configure the feed.
    9. Select the Permissions tab, and click the ...
    10. Click on Allow builds and Releases (which will add Project Collection Build Services as a Contributor).
    11. Click on Allow project-scoped builds (which will add Pipeline Build Service as a Contributor)
    12. Create a New Pipeline and reference the azure-pipelines.yml file in the /Steps/Azure/ServiceBus directory.
    13. In Pipelines....Library, set up a Variable group named 'SonarQube Analysis'
        a. Add a variable named 'sonarCloudConnectionName' with the name of the SonarQube Service Connection created in Step 5.
        b. Add a variable named 'sonarCloudEnabled' with the value of 'YES'.
        c. Add a variable named 'sonarCloudOrganization' with the value of your SonarCloud organization.
    14. In Pipelines....Library, set up a Variable Group named 'Pipelines Artifacts Feed'.
        a. Add a variable named 'publishVstsFeed' with the value of the feed to which output should be published.
    15. Run the newly created pipeline.


## Testing
These tests are a mixture of unit and integration tests necessary to read an Azure Service Bus Message into the pipeline and write an Azure Service Bus Message from the pipeline.

## API references

ReadMessageFromAzureStep
* Reads a multiple messages from an Azure Service queue or subscription and sends them to the request pipeline one message at a time.
* Copies any Application Properties from the Message into the PipelineContext based on the Options.ApplicationProperties configured.
* Creates a PipelineContext entry named INPUT_STREAM containing a MemoryStream that is wrapping the ServiceBusMessage.Body stream.

WriteMessageToAzureStep
* Writes a single message to an Azure Service queue or topic.
* Copies any Application Properties from the PipelineContext into the ServiceBusMessage based on the Options.ApplicationProperties configured.
* UTF8 encodes the Stream contained in OUTPUT_STREAM and writes it to the Message.Body.


TO DEVELOPERS:
All of the tests with a ReadMessageFromAzureOptions class are written to use the options defined in TODO: ?? this class ?? for all subsequent Topic or Subscription calls.
All Pull Requests should follow this pattern to enable a "one location" alteration within each test to be used throughout the rest of the test.
