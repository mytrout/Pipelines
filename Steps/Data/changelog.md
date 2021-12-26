# MyTrout.Pipelines.Steps.Data Change Log

## 2.1.0
 - Upgrade to .NET 6.0 while maintaining 5.0 support.
 - Update Resources.tt to reflect .NET 6.0 instead of .NET 5.0
 - Update C# Language Version from 9.0 to 10.0.
 - Change code to match C# 10.0 language constructs.
 - Upgrade Tests to .NET 6.0 only.
 - Upgrade documentation to reflect .NET 6.0.
 - Upgrade Dapper 2.0.78 to 2.0.123
 - Upgrade MyTrout.Pipelines from 3.0.1 to 3.2.0
 - Install Microsoft.CodeAnalysis.Analyzers from 3.3.2 to 3.3.3
 - Install Microsoft.CodeAnalysis.NetAnalyzers 6.0.0
 - Upgrade Microsoft.VisualStudio.Threading.Analyzers from 16.8.55 to 17.0.64
 - Upgrade SonarAnalyzer.CSharp from 8.16.0.25740 to 8.33.0.40503
 - Upgrade StyleCop.Analyzers from 1.1.118 to 1.2.0-beta.376
 - Upgrade all Test dependencies to ensure Unit Tests continue to operate.
  - Add work_dispatch element to allow this library to to built manually in Github.

## 2.0.0
- Upgrade to MyTrout.Pipelines v3.0.1
- Change the nuget PackageURL and RepositoryURL from Azure DevOps to GitHub.
- Change Build Status badge from Azure DevOps to GitHub Actions.
- Convert Azure DevOps build to Github Actions build yaml.

## 1.0.0
- Upgrade from .NET Standard 2.1 to .NET 5.0
- Upgrade to MyTrout.Pipelines.Steps 2.0.6
- Change from Dapper requirements to >= 2.0.78 and < 3.0.
- Add Nullable reference types back with appropriate suppressions for Assert~ methods.
- Refactor azure-pipelines.yml to use an Azure DevOps controlled version number.
- Update Resources.tt to use pragma to suppress warnings.
- Update Resources.tt to reflect the new copyright 2020-2021.
- Change copyright from 2020 to 2020-2021.
- Update README.md to provide actual documentation and bring it up-to-date.
- Update the Project and Repository urls to GitHub from Azure DevOps.

 IMPORTANT NOTE: The MyTrout.Pipelines.Steps.Data.Database project was not upgraded to .NET 5.0 because I could not find a targeting pack for .NET 5.0.  It remains at .NET 4.8 as of this version.

## 0.2.0-beta
- Clean up data from SupplementContextWithDatabaseRecordStepTests to ensure subsequent runs on the same database will work.
- Initial commit of SaveContextToDatabaseStep.
- Reduce the Cognitive Complexity on SupplementContextWithDatabaseRecordStep.InvokeAsync method to 15 or below.

## 0.1.0-beta
- Initial commit of SupplementContextWithDatabaseRecordStep.
- Create Azure ARM Template for SQL Server deployment for integration testing.
- Update azure-pipelines.yml to use appropriate ARM templates.
- Create tests/MyTrout.Pipelines.Steps.Data.Database project to allow creation of DACPAC for use with integration testing database.
- Nullable reference types will not be implenented due to warning suppression noise.
- Add Resources to enable localization and suppress analyzer warnings.
