# MyTrout.Pipelines.Steps.Data Change Log

## 3.2.0
- Update documentation on SqlStatement.CommandType to reflect how the value should be loaded from Configuration.
- Change SqlStatement.Parameters from an IEnumerable&lt;string&gt; to a List&lt;string&gt; to enable the value to be loaded from Configuration.
- Remove link to README.md and stylecop.json from Solution Items [#236]
- Upgrade .NET 7.0 from preview to fully supported version.
- IMPORTANT NOTE: All versions of this library prior to this one will have problems loading any SqlStatement from Configuration.
- IMPORTANT NOTE: All versions of this library prior to 2.1.0 will be marked deprecated in nuget.org due to unsupported .NET versions.

## 3.1.0
- Change default branch from master to main.
- Upgrade C# Language Version from 9.0 to 10.0 across all src projects.
- Standardize the first &lt;Property Group&gt; section in the src csproj files across all src projects.
- Standardize the NoWarn options within the src csproj files across all src projects.
- Standardize the Neutral Language options to en-US across all src projects.
- Standardize the Copyright to include 2022 across all src projects.
- Standardize all .editorconfig file inclusion across all src projects.
- Standardize inclusion of README.md file across all src projects.
- Force upgrade to MyTrout.Pipelines v4.0.3 minimum.

## 3.0.0
### BREAKING CHANGES:
- [#160](https://github.com/mytrout/Pipelines/issues/160) Remove support for .NET 5.0. 
- [#162](https://github.com/mytrout/Pipelines/issues/162) Upgrade to MyTrout.Pipelines 4.0.0 
### NON-BREAKING CHANGES:
- [# 85](https://github.com/mytrout/Pipelines/issues/85)  Update Resources.tt to use NamespaceHint instead of a hard-coded namespace.
- [# 94](https://github.com/mytrout/Pipelines/issues/94)  Add "if-no-files-found: error" to the nuget publishing step.
- [#160](https://github.com/mytrout/Pipelines/issues/160) Add support for .NET 7.0
- [#161](https://github.com/mytrout/Pipelines/issues/161) Refactor ~Steps and ~Options to use user-configurable context names for any value read from or written to IPipelineContext.Items.
- Uncomment all of the nuget publish steps to allow a new version to be published.
- Add .editorconfig to enforce rules in Visual Studio 2022.
- Update Microsoft.CodeAnalysis.NetAnalyzers to .NET 7.0 preview version to eliminate build warnings.
- Update Microsoft.CodeAnalysis.Analyzers to .NET 7.0 preview version to eliminate build warnings.
- Update Microsoft.VisualStudio.ThreadingAnalyzers to 17.2.32
- Upgrade SonarAnalyzer.CSharp 8.40.0.48530
- Upgrade StyleCop.Analyzers to 1.2.0-beta.435

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
- Nullable reference types will not be implemented due to warning suppression noise.
- Add Resources to enable localization and suppress analyzer warnings.
