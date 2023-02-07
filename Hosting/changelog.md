# MyTrout.Pipelines.Hosting Change Log

## 4.2.0
- Add additional debug logging to ensure Production support can be performed on this application by altering logging level.
- Set the PipelineContext.PipelineName from the HostingContext to ensure a standardized value is set.
- Add PipelineContext.PipelineName to the "generic" "Pipeline did x" logging messages to make it easier to read in external logs.
- Update "generic" log messages to include the PipelineContext.PipelineName.
- Ensure that successful execution logging message is written to the logger prior to exiting.
- Ensure that end execution logging message is written in the finally block prior to exiting.
- Change from IHosting
## 4.1.0
- Change default branch from master to main.
- Upgrade C# Language Version from 9.0 to 10.0 across all src projects.
- Standardize the first &lt;Property Group&gt; section in the src csproj files across all src projects.
- Standardize the NoWarn options within the src csproj files across all src projects.
- Standardize the Neutral Language options to en-US across all src projects.
- Standardize the Copyright to include 2022 across all src projects.
- Standardize all .editorconfig file inclusion across all src projects.
- Standardize inclusion of README.md file across all src projects.
- Force upgrade to MyTrout.Pipelines v4.0.3 minimum.

## 4.0.0 - BREAKING CHANGES
- BREAKING CHANGE: Upgrade Hosting to MyTrout.Pipelines 4.x
- BREAKING CHANGE: Remove support for .NET 5.0.
- Add support for .NET 7.0.100-preview.3.22179.4
- Uncomment the nuget and github publishing to allow v4.x to be published.
- Change Resources.tt to automatically read the namespace from NamespaceHint and change copyright year.

## 3.1.0 - SonarCloud UPDATE ONLY
- Correct 3 CA1816 on Test classes
- Commented out ability to publish to nuget or github to ensure this update does not cause a build failure.
- Add "if-no-files-found: error" to the nuget publishing step. (see Issue #94)
- NOTE TO DEVELOPERS: Developers must reverse the removal of nuget and github publishing when the next version is ready to be released

## 3.1.0
 - Upgrade to .NET 6.0 while maintaining 5.0 support.
 - Update Resources.tt to reflect .NET 6.0 instead of .NET 5.0
 - Update C# Language Version from 9.0 to 10.0.
 - Change code to match C# 10.0 language constructs.
 - Upgrade Tests to .NET 6.0 only.
 - Upgrade documentation to reflect .NET 6.0.
 - Upgrade MyTrout.Pipelines from 3.0.0 to 3.2.0
 - Upgrade Microsoft.Extensions.Hosting from 5.0.0 to 6.0.0
 - Upgrade Microsoft.Extensions.Hosting.Abstractions from 5.0.0 to 6.0.0
 - Upgrade Microsoft.CodeAnalysis.Analyzers from 3.3.2 to 3.3.3
 - Upgrade Microsoft.VisualStudio.Threading.Analyzers from 16.8.55 to 17.0.64
 - Upgrade SonarAnalyzer.CSharp from 8.15.0.24505 to 8.33.0.40503
 - Upgrade all Test dependencies to ensure Unit Tests continue to operate.
 - Suppress the CA2254 analyzer message because it does not recognize Resource strings as static strings.
 - Add work_dispatch element to allow this library to to built manually in Github.

## 3.0.0
 - Use refactored MyTrout.Pipelines 3.x.
 - Remove azure-pipelines.yml due to GitHub Actions conversion.
 - Add build-pipelines-hosting.yml due to GitHub Actions conversion.
 - Alter samples to ensure compliance with new MyTrout.Pipelines 3.x changes.
 - Remove AddStepDependencyExtensions and all usages because they are no longer used.
 - Correct the IDisposable implementations in TestSteps to comply with MyTrout.Pipelines 3.x changes.
 - Update Resources.tt to reflect .NET 5.0 instead of .NET Standard 2.1

## 2.0.0
 - Upgrade to .NET 5.0
 - Change the project and repository urls from dev.azure.com/mytrout to github.com.
 - Move versioning out of the azure-pipelines.yml and into Azure DevOps.
 - Remove deprecated Microsoft.CodeAnalysis.NetAnalyzers library.
 - Remove deprecated Microsoft.CodeAnalysis.FxCopAnalyzers library.
 - Use primarySourceBranch variable for builds to allow SonarQube analysis to be performed on feature branches.
 - Add HostBuilderExtensions to verify the sourceDictionary to ensure mucking around with Host.Properties doesn't break the system.
 - Alter AddStepDependecyExtensions to allow better and easier testing.

## 1.2.1
- Limit MyTrout.Pipelines versions to >= 1.1 and < 2.0 in the nuget package.

## 1.2.0
- Upgrade Hosting to MyTrout.Pipelines v1.1.*
- Generate the snupkg file for symbols for MyTrout.Pipelines.Hosting.
- Upgrade MyTrout.Pipelines.Hosting to nullable reference types.

## 1.1.0
- Upgrade Hosting to MyTrout.Pipelines v1.0.1
- Support a step being configured multiple times with different configurations in the same pipeline via AddStepDependency.
- Rename PipelineHostBuilderExtensions to UsePipelineExtensions.
- Split the UsePipelineExtensions into AddStepDependencyExtensions and UsePipelineExtensions.
- Changed '<' to &lt; and '>' to &gt; to correct warnings in the Resources T4 template documentation.

## 1.0.0
- Upgrade Hosting to MyTrout.Pipelines v0.27.0-beta

## 0.21.0-beta
- Upgrade Hosting to MyTrout.Pipelines v0.26.0-beta 

## 0.20.0-beta
- Upgrade Hosting to MyTrout.Pipelines v0.25.0-beta

## 0.19.0-beta
- Upgrade Hosting to use MyTrout.Pipelines v0.24.0-beta

## 0.18.0-beta
- Upgrade Hosting to use MyTrout.Pipelines v0.23.0-beta.

## 0.17.0-beta
- Correcting all style analyzer and compiler warnings with Hosting, including a potential threading issue in SonarCloud.

## 0.15.1-beta
- Correct project and git repository urls for this package.
