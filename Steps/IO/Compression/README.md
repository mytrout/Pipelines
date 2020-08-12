# MyTrout.Pipelines.Steps.IO.Compression

MyTrout.Pipelines.Steps.IO.Compression provides Pipeline steps to manage zip archives and zip archive entries.

MyTrout.Pipelines.Steps.IO.Compression targets [.NET Standard 2.1](https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support)

For more details on Pipelines, see [Pipelines.Core](../../Core/README.md)

For more details on Pipelines.Hosting, see [Pipelines.Hosting](../../Hosting/README.md)

For a list of available steps, see [Available Steps](../README.md)

# Installing via NuGet

    Install-Package MyTrout.Pipelines.Steps.IO.Compression

# Software dependencies

    1. MyTrout.Pipelines.Steps 1.0.0

All software dependencies listed above use the [MIT License](https://licenses.nuget.org/MIT).

# How do I create a new zip archive and add entries using this library ?

```csharp

    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Hosting;
    using MyTrout.Pipelines.Steps.IO.Compression
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
                                            // This step functionality adds an INPUT_STREAM and loops. (It doesn't exist)
                                            .AddStep<GatherEachFileInADirectoryStep>()
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

# Build the software locally.
    1. Clone the software from the Pipelines repository.
    2. Build the software in Visual Studio 2019 to pull down all of the dependencies from nuget.org.
    3. In Visual Studio, run all tests.  All of the should pass.
    4. If you have Visual Studio Enterprise 2019, analyze the code coverage; it should be 100%.

# Build the software in Azure DevOps.
    1. In Organization Settings, select Extensions option.
    2. Install the SonarCloud Extension.
    3. Login to the SonarQube instance and generate a SonarQube token with the user account to use for running analysis.
    4. In Project Settings, select Service Connections option.
    5. Add a Service Connection for SonarQube and enter the token.
    6. Make sure you check the 'Grant access permission to all pipelines' checkbox or configure appropriate security to this connection.
    7. In Artifacts, add a new Feed named mytrout.
    8. On the mytrout Artifacts feed, select the gear icon to configure the feed.
    9. Select the Permissions tab, and click the ...
    10. Click on Allow builds and Releases (which will add Project Collection Build Services as a Contributor).
    11. Click on Allow project-scoped builds (which will add Pipeline Build Service as a Contributor)
    12. Create a New Pipeline and reference the azure-pipelines.yml file in the /Steps/IO.Compression directory.
    13. In Pipelines....Library, set up a Variable group named 'SonarQube Analysis'
        a. Add a variable named 'sonarCloudConnectionName' with the name of the SonarQube Service Connection created in Step 5.
        b. Add a variable named 'sonarCloudEnabled' with the value of 'YES'.
        c. Add a variable named 'sonarCloudOrganization' with the value of your SonarCloud organization.
    14. In Pipelines....Library, set up a Variable Group named 'Pipelines Artifacts Feed'.
        a. Add a variable named 'publishVstsFeed' with the value of the feed to which output should be published.
    13. Run the newly created pipeline.

# Contribute
No contributions are being accepted at this time.