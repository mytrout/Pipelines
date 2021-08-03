# MyTrout.Pipelines.Steps.Azure.Blobs Change Log

## 2.0.0
- Convert from Azure DevOps to Github Actions.
- Delete azure-pipelines.yml and remove from the solution.

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