MyTrout.Pipelines.Steps.Azure.ServiceBus

# Introduction 
These tests are a mixture of unit and integration tests necessary to read an Azure Service Bus Message into the pipeline and write an Azure Service Bus Message from the pipeline.

# Getting Started
1.	Installation process (Local)
    1. Clone the software from the Pipelines repository.
    2. Build the software in Visual Studio 2019 to pull down all of the dependencies from nuget.org.
    3. Deploy the Azure Resource Template located at <insert document location here> to initialize the Azure Service Bus connectivity.
    4. Get the Primary Connection String for the Azure Service Bus Instance.
    5. Create a new machine-level environment variable named 'PIPELINE_TEST_AZURE_SERVICE_BUS_CONNECTION_STRING' and set the value to the Azure Service Bus Connection string.
    6. In Visual Studio, run all tests.  All of the should pass.
    7. If you have Visual Studio Enterprise 2019, analyze the code coverage; it should be 100%.


2.	Installation process (Azure DevOps)
    1. Login to the SonarQube instance and generate a SonarQube token with the user account you want to use for running analysis.
    2. In Organization Settings, Add a Service Connection for SonarQube and enter the token.
    3. Make sure you check the 'Grant access permission to all pipelines' checkbox or configure appropriate security to this connection.
    4. Open a New Pipeline and reference the azure-pipelines.yml file in the /Steps/Azure/ServiceBus directory.


2.	Software dependencies
    1. Microsoft.Azure.ServiceBus 4.1.3
    2. Microsoft.Extensions.Configuration.Abstractions 3.1.5
    3. Microsoft.Extensions.Logging.Abstractions 3.1.5
    4. MyTrout.Pipelines 0.22.0-beta


3.	Latest releases

| Version    | Release Date | Details                                    |
| 0.1.0-beta | 20 JUNE 2020 | Initial release of the library             |
| 0.2.0-beta | 21 JUNE 2020 | Upgrade MyTrout.Pipelines to v.0.22.0-beta |
| 0.3.0-beta | 21 JUNE 2020 | Upgrade MyTrout.Pipelines to v.0.23.0-beta |

4.	API references

ReadMessageFromAzureSubscriptionStep
* Reads a single message from an Azure Service subscription.
* Copies any User Properties from the Message into the PipelineContext based on the Options.UserProperties configured.
* Creates a PipelineContext entry named INPUT_STREAM containing a MemoryStream that is wrapping the Message.Body byte array.

WriteMessageToAzureSubscriptionOptions
* Writes a single message from an Azure Service subscription.
* Copies any User Properties from the PipelineContext into the Message based on the Options.UserProperties configured.
* UTF8 encodes the Stream contained in OUTPUT_STREAM and writes it to the Message.Body.

# Build and Test
An azure-pipelines.yml exists in the /Steps/Azure/ServiceBus directory.  It is usable with any Azure DevOps instance.

TO DEVELOPERS:
All of the tests with a ReadMessageFromAzureSubscriptionOptions class are written to use the options defined in this class for all subsequent Topic or Subscription calls.
All Pull Requests should follow this pattern to enable a "one location" alteration within each test to be used throughout the rest of the test.


# Contribute
No contributions are being accepted at this time.