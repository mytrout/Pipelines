# MyTrout.Pipelines.Steps.Azure.Blobs Change Log

## 3.1.0
- Upgrade Azure.Storage.Blobs from 12.10.0 to 12.13.0 to address security vulnerability.

## 3.0.0
### BREAKING CHANGES:
- [#160](https://github.com/mytrout/Pipelines/issues/160) Remove support for .NET 5.0. 
- [#162](https://github.com/mytrout/Pipelines/issues/162) Upgrade to MyTrout.Pipelines 4.0.0 
### NON-BREAKING CHANGES:
- [# 85](https://github.com/mytrout/Pipelines/issues/85)  Update Resources.tt to use NamespaceHint instead of a hard-coded namespace.
- [# 94](https://github.com/mytrout/Pipelines/issues/94)  Add "if-no-files-found: error" to the nuget publishing step.
- [#148](https://github.com/mytrout/Pipelines/issues/148) Refactor ReadStreamFromBlobStorageStep to use AbstractCachingPipelineStep<TStep, TOptions> to guarantee that existing INPUT_STREAM values are restored after execution of this step.
- [#148](https://github.com/mytrout/Pipelines/issues/148) Add additional unit test to ensure that ReadStreamFromBlobStorageStep restores PipelineContext.Items to its original state after execution.
- [#160](https://github.com/mytrout/Pipelines/issues/160) Add support for .NET 7.0
- [#161](https://github.com/mytrout/Pipelines/issues/161) Refactor DeleteBlobStep and DeleteBlobOptions to use user-configurable context names for any value read or written to IPipelineContext.Items.
- [#161](https://github.com/mytrout/Pipelines/issues/161) Refactor ReadStreamFromBlobStorageStep and ReadStreamFromBlobStorageOptions to use user-configurable context names for any value read or written to IPipelineContext.Items.
- [#161](https://github.com/mytrout/Pipelines/issues/161) Refactor WriteStreamToBlobStorageStep and WriteStreamToBlobStorageOptions to use user-configurable context names for any value read or written to IPipelineContext.Items.
- Uncomment all of the nuget publish steps to allow a new version to be published.
- Add .editorconfig to enforce rules in Visual Studio 2022.

## 2.1.0
 - Upgrade to .NET 6.0 while maintaining 5.0 support.
 - Update C# Language Version from 9.0 to 10.0.
 - Change code to match C# 10.0 language constructs.
 - Refactor code with warnings or informational issues.
 - Upgrade Tests to .NET 6.0 only.
 - Upgrade documentation to reflect .NET 6.0.
 - Upgrade Azure.Storage.Blobs from 12.9.1 to 12.10.0
 - Upgrade MyTrout.Pipelines from 3.0.1 to 3.2.0
 - Install Microsoft.CodeAnalysis.Analyzers 3.3.3
 - Install Microsoft.CodeAnalysis.NetAnalyzers 6.0.0
 - Upgrade Microsoft.VisualStudio.Threading.Analyzers from 16.10.56 to 17.0.64
 - Upgrade SonarAnalyzer.CSharp from 8.26.0.34506 to 8.33.0.40503
 - Upgrade StyleCop.Analyzers from 1.1.118 to 1.20.0-beta.376
 - Upgrade all Test dependencies to ensure Unit Tests continue to operate.
 - Add work_dispatch element to allow this library to to built manually in Github.

## 2.0.0
- Convert from Azure DevOps to Github Actions.
- Delete azure-pipelines.yml and remove from the solution.
- Upgrade Azure.Azure.Blobs library from 12.7.x to 12.9.x.
- Upgrade Microsoft.VisualStudio.Threading.Analyzers library from 16.8.x to 16.10.x.
- Remove MyTrout.Pipelines.Steps library v2.0.6.
- Add MyTrout.Pipelines library v3.0.1.
- Upgrade SonarAnalyzer.CSharp library from 8.16 to 8.26.
- Alter all Unit and Integration Tests to support Linux.

## 1.0.0
 - Upgrade from .NET Standard 2.1 to .NET 5.0
 - Upgrade to MyTrout.Pipelines.Steps 2.0.6
 - Change from Azure.Storage.Blobs 12.5.1 to Azure.Storage.Blobs 12.7.0
 - Reordering all constructor parameters to be consistent with parameter ordering in AbstractPipelineStep<TStep, TOptions>
 - Add Nullable reference types back with appropriate suppressions for Assert~ methods.
 - Refactor RetrieveConnectionStringAsync methods to use a local Connection String value loaded by Configuration.
 - Remove ExcludeFromCodeCoverage on all ~Options classes and added unit tests because of new functionality.
 - Refactor azure-pipelines.yml to use an Azure DevOps controlled version number.
 - Change copyright from 2020 to 2020-2021.
 - Update README.md to reflect these changes.
 - Add sonarqube, nuget, and build badges to README.md.
 - Change README.md root entries to be second level headers.
 - Remove Contribution restrictions from the project.

## 0.1.1-beta
- Correct Project Name and Project Key for SonarQube analysis.
- Delay DeleteBlobStep test execution by one second to prevent failures due to timing issues in Azure.
- This version will not be published to the public.

## 0.1.0-beta
- Initial implementation of DeleteBlobStep, ReadStreamFromBlobStorageStep and WriteStreamToBlobStorageStep.
- Create Azure ARM Template for Azure Blob Storage deployment for integration testing.
- Update azure-pipelines.yml to use appropriate ARM templates.
- Nullable reference types will not be implemented due to warning suppression noise.