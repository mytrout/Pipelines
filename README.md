# Pipelines

## Introduction
Provides a non-HTTP pipeline similar to the ASP.NET Core request pipeline.

MyTrout.Pipelines targets [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0)

If three steps named M1, M2, and M3 were added to the Pipeline, here is the execution path for the code.

![](./Core/pipeline-drawing.jpg)

The Pipeline automatically adds the NoOpStep as the last step in the Pipeline.

## BREAKING CHANGES INTRODUCED WITH 3.0.0

See [Breaking Changes for 3.0.0](./docs/pipelines-core-breaking-changes-3-0-0.md)

## Installing via NuGet

    Install-Package MyTrout.Pipelines

## Software dependencies
    1. Microsoft.Extensions.Configuration.Abstractions 6.0.0
    2. Microsoft.Extensions.Configuration.Binder 6.0.0
    3. Microsoft.Extensions.DependencyInjection.Abstractions 6.0.0
    4. Microsoft.Extensions.Logging.Abstractions 6.0.0

All software dependencies listed above use the [MIT License](https://licenses.nuget.org/MIT).

## How do I use Pipelines?
Please refer to the [Pipelines.Core](./Core/README.md) for details on the basics of how to use Pipelines.

## How do I write Steps?
Please refer to the [Steps](./Steps/README.md) for more details on how to write steps.

## How do I use Pipelines with Console Applications / Generic Host?
Please refer to the [Pipeline.Hosting](./Hosting/README.md) for more details on how to use Pipelines with Console applications.

## Build the software locally.
    1. Clone the software from the Pipelines repository.
    2. Build the software in Visual Studio 2019 v16.8 or higher to pull down all of the dependencies from nuget.org.
    3. In Visual Studio, run all tests.  All of the should pass.
    4. If you have Visual Studio Enterprise 2019, analyze the code coverage; it should be 100%.

## Build the software in Azure DevOps.
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
    12. Create a New Pipeline and reference the azure-pipelines.yml file in the /Core directory.
    13. Run the newly created pipeline.
