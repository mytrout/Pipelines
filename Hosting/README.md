# MyTrout.Pipelines.Hosting

MyTrout.Pipelines.Hosting provides helper classes to run a pipeline using Microsoft's Generic Host.

MyTrout.Pipelines.Hosting targets [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)

For more details on Pipelines, see [Pipelines.Core](../Core/README.md)

For more details on Pipelines.Steps, see [Pipelines.Steps.Core](../Steps/Core/README.md)

# Installing via NuGet

    Install-Package MyTrout.Pipelines.Hosting

# Software dependencies
    1. Microsoft.Hosting 5.0.0
    2. Microsoft.Hosting.Abstractions 5.0.0
    3. MyTrout.Pipelines 2.0.2 up to but not including 3.0.0

All software dependencies listed above use the [MIT License](https://licenses.nuget.org/MIT).

# How do I use Pipelines.Hosting?

```csharp

    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Hosting;
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
                                        builder.AddStep<Step1>()
                                                .AddStep<Step2>()
                                                .AddStep<Step3>();
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

# How do I use Pipelines.Hosting with different configurations for different instances of the same step.

If Step1 prints the Step1Options value with a trailing space to the Console when each step is called, then the following code will generate "Moe, Larry & Curly ".

```csharp

    using MyTrout.Pipelines.Core;
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
                                    .AddStepDependency("context-1", new Step1Options("Moe,"))
                                    .AddStepDependency("context-2", new Step1Options("Larry"))
                                    .AddStepDependency("context-3", new Step1Options("&"))
                                    .AddStepDependency("context-4", new Step1Options("Curly"))
                                    .UsePipeline(builder => 
                                    {
                                        builder.AddStep<Step1>("context-1")
                                            .AddStep<Step1>("context-2")
                                            .AddStep<Step1>("context-3")
                                            .AddStep<Step1>("context-4")
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

# Build the software locally.
    1. Clone the software from the Pipelines repository.
    2. Build the software in Visual Studio 2019 to pull down all of the dependencies from nuget.org.
    3. In Visual Studio, run all tests.  All of them should pass.
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
    12. Create a New Pipeline and reference the azure-pipelines.yml file in the /Hosting directory.
    13. Run the newly created pipeline.


# Contribute
No contributions are being accepted at this time.