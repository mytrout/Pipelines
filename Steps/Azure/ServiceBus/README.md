# MyTrout.Pipelines.Steps.Azure.ServiceBus

MyTrout.Pipelines.Steps.Azure.ServiceBus provides Pipeline steps to encrypt, hash, and decrypt streams.

MyTrout.Pipelines.Steps.Azure.ServiceBus targets [.NET Standard 2.1](https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support)

For more details on Pipelines, see [Pipelines.Core](../../Core/README.md)

For more details on Pipelines.Hosting, see [Pipelines.Hosting](../../Hosting/README.md)

For a list of available steps, see [Available Steps](../README.md)

# Installing via NuGet

    Install-Package MyTrout.Pipelines.Steps.Azure.ServiceBus

# Software dependencies

    1. Microsoft.Azure.ServiceBus v4.1.3
    2. MyTrout.Pipelines.Steps >= v1.0.0 and < 2.0.0
    
All software dependencies listed above use the [MIT License](https://licenses.nuget.org/MIT).

# How do I use the steps in this library ([ReadMessageFromAzureSubscriptionStep](/src/ReadMessageFromAzureSubscriptionStep.cs)) ?

## sample C# code

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
                                    .AddStepDependency<ReadMessageFromAzureSubscriptionOptions>()
                                    .UsePipeline(builder => 
                                    {
                                        builder
                                            // NOTE: ReadMessageFromAzureSubscriptionStep will continue to read (and process) messages 
                                            //       one at a time until there are no more messages on the subscription.
                                            .AddStep<ReadMessageFromAzureSubscriptionStep>()
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
## sample appsettings.json file

UserProperties are values that should be copied from the UserProperties collection of the message to the PipelineContext.

```json
{
    "AzureServiceBusConnectionString": "user supplied ASB ConnectionString",
    "TopicName": "user supplied topic",
    "SubscriptionName": "user supplied subscriptionName",
    "UserProperties": [ "Year", "Month", "Id" ]
}
```


# Build the software locally.
    1. Clone the software from the Pipelines repository.
    2. Build the software in Visual Studio 2019 to pull down all of the dependencies from nuget.org.
    3. Deploy the Azure Resource Template located at <insert document location here> to initialize the Azure Service Bus connectivity.
    4. Get the Primary Connection String for the Azure Service Bus Instance.
    5. Create a new machine-level environment variable named 'PIPELINE_TEST_AZURE_SERVICE_BUS_CONNECTION_STRING' and set the value to the Azure Service Bus Connection string.
    6. In Visual Studio, run all tests.  All of the should pass.
    7. If you have Visual Studio Enterprise 2019, analyze the code coverage; it should be 100%.

# Build the software in Azure DevOps.
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

# Contribute
No contributions are being accepted at this time.

# Testing
These tests are a mixture of unit and integration tests necessary to read an Azure Service Bus Message into the pipeline and write an Azure Service Bus Message from the pipeline.

# API references

ReadMessageFromAzureSubscriptionStep
* Reads a single message from an Azure Service subscription.
* Copies any User Properties from the Message into the PipelineContext based on the Options.UserProperties configured.
* Creates a PipelineContext entry named INPUT_STREAM containing a MemoryStream that is wrapping the Message.Body byte array.

WriteMessageToAzureSubscriptionOptions
* Writes a single message from an Azure Service subscription.
* Copies any User Properties from the PipelineContext into the Message based on the Options.UserProperties configured.
* UTF8 encodes the Stream contained in OUTPUT_STREAM and writes it to the Message.Body.


TO DEVELOPERS:
All of the tests with a ReadMessageFromAzureSubscriptionOptions class are written to use the options defined in this class for all subsequent Topic or Subscription calls.
All Pull Requests should follow this pattern to enable a "one location" alteration within each test to be used throughout the rest of the test.
