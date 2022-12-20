# MyTrout.Pipelines.Steps.IO.Compression

## 6.1.0
- Change default branch from master to main.
- Upgrade C# Language Version from 9.0 to 10.0 across all src projects.
- Standardize the first &lt;Property Group&gt; section in the src csproj files across all src projects.
- Standardize the NoWarn options within the src csproj files across all src projects.
- Standardize the Neutral Language options to en-US across all src projects.
- Standardize the Copyright to include 2022 across all src projects.
- Standardize all .editorconfig file inclusion across all src projects.
- Standardize inclusion of README.md file across all src projects.
- Force upgrade to MyTrout.Pipelines v4.0.3 minimum.

## 6.0.0
### BREAKING CHANGES:
- [#160](https://github.com/mytrout/Pipelines/issues/160) Remove support for .NET 5.0. 
- [#162](https://github.com/mytrout/Pipelines/issues/162) Upgrade to MyTrout.Pipelines 4.0.0 
- [#161](https://github.com/mytrout/Pipelines/issues/161) Refactor all ~Step and ~Options to use user-configurable context names for any value read from or written to IPipelineContext.Items.
- [#161](https://github.com/mytrout/Pipelines/issues/161) Add additional unit test to test that ContextName values are used when Steps executes.
### NON-BREAKING CHANGES:
- [#160](https://github.com/mytrout/Pipelines/issues/160) Add support for .NET 7.0
- [#148](https://github.com/mytrout/Pipelines/issues/148) Refactor ~Step to use AbstractCachingPipelineStep<TStep, TOptions> to guarantee that existing INPUT_STREAM values are restored after execution of this step.
- [#148](https://github.com/mytrout/Pipelines/issues/148) Add additional unit test to ensure that ~Step restores PipelineContext.Items to its original state after execution.
- Move parameter and PipelineContext.Items validation to the InvokeBeforeCacheAsync() method for classes implementing AbstractCachingPipelineStep&lt;TStep&gt;
- Add .editorconfig to enforce rules in Visual Studio 2022.
- Update Microsoft.CodeAnalysis.NetAnalyzers to .NET 7.0 preview version to eliminate build warnings.
- Update Microsoft.CodeAnalysis.Analyzers to .NET 7.0 preview version to eliminate build warnings.
- Update Microsoft.VisualStudio.ThreadingAnalyzers to 17.2.32
- Upgrade SonarAnalyzer.CSharp 8.40.0.48530
- Upgrade StyleCop.Analyzers to 1.2.0-beta.435
- Remove pramga lines in favor of null-forgiving operators and an explanation comment.

## 5.0.0
### BREAKING CHANGES:
- On OpenExistingZipArchiveFromStreamStep, INPUT_STREAM is no longer copied to OUTPUT_STREAM on the response side.
### NON-BREAKING CHANGES:
- Suppress SA1636 because three file headers have copyrights changed to 2020 - 2022.
- Alter the build.yaml to include the new version and uncomment the Upload Nuget step.
- Update Resources.tt to use the new NamespaceHint code so that the Resources.tt can be copied into any project.
- Update unit tests because two are failing due to OUTPUT_STREAM Breaking Change.
- NOTE TO DEVELOPERS: Resources.tt cannot be linked into the project yet due to issues with this.Host.Template file location at the root.
- Upgrade CreateNewZipArchiveStep to use AbstractCachingPipelineStep.
- Added CloseZipArchiveStep to populate the OUTPUT_STREAM from the ZIP_ARCHIVE for CreateNewZipArchiveStep and any changes made after OpenExistingZipArchiveStep.

## 4.0.0
- Not sure why this version was pushed out the public, nor what is in it.

## 3.1.1
- Suppress CA2254 to prevent false positives in SonarCloud.io with culture-aware logging messages.

## 3.1.0
 - Upgrade to .NET 6.0 while maintaining 5.0 support.
 - Update Resources.tt to reflect .NET 6.0 instead of .NET 5.0
 - Update C# Language Version from 9.0 to 10.0.
 - Change code to match C# 10.0 language constructs.
 - Upgrade Tests to .NET 6.0 only.
 - Upgrade documentation to reflect .NET 6.0.
 - Upgrade MyTrout.Pipelines from 3.0.1 to 3.2.0
 - Install Microsoft.CodeAnalysis.Analyzers from 3.3.2 to 3.3.3
 - Install Microsoft.CodeAnalysis.NetAnalyzers 6.0.0
 - Upgrade Microsoft.VisualStudio.Threading.Analyzers from 16.10.5617.0.64
 - Upgrade SonarAnalyzer.CSharp from 8.25.0.33663 to 8.33.0.40503
 - Upgrade StyleCop.Analyzers from 1.2.0-beta.354 to 1.2.0-beta.376
 - Upgrade all Test dependencies to ensure Unit Tests continue to operate.
  - Add work_dispatch element to allow this library to to built manually in Github.

## 3.0.0
### BREAKING CHANGES:
- Upgrade to MyTrout.Pipelines v3.0.1
### NON-BREAKING CHANGES:
- Change Resources.tt from .NET Standard 2.1 to .NET 5.0 in the comments.
- Change the nuget PackageURL and RepositoryURL from Azure DevOps to GitHub.
- Change Build Status badge from Azure DevOps to GitHub Actions.
- Convert Azure DevOps build to Github Actions build yaml.
- Update existing SonarQube Warnings and Issues.

## 2.0.1
- Upgrade MyTrout.Pipelines.Steps.Core from 1.x to 2.0.6.

## 2.0.0
- Upgrade to .NET 5.0
- Add nuget, build, and sonarqube badges to README.md.
- Change C# Language Version from 8.0 to 9.0
- Change copyright from 2020 to 2020-2021.
- Update azure-pipelines.xml to add pr element to enable CI/CD builds in Github.
- Move the version data to an Azure DevOps Variable group.
- Update azure-pipelines.xml to add artifactPublishEnabled property.

## 1.0.0
### BREAKING CHANGES:
- Upgrade to MyTrout.Pipelines.Steps v1.0.0 (including MyTrout.Pipelines v1.1.0)
### NON-BREAKING CHANGES:
- Generate and publish to Azure DevOps Artifacts the snupkg file for symbols.
- Implement nullable reference types.
- Suppress SonarQube S5042 security warnings as they are false positives - 'this may be a problem' rather than 'this *IS* a problem'.

## 0.2.0-beta
- Correct documentation on CompressionConstants and PipelineContextValidationExtensions.
- Adding unit testing to MyTrout.Pipelines.Steps.IO.Compression classes.
- Change from MyTrout.Pipelines v0.27.0-beta to MyTrout.Pipelines.Steps v0.1.0-beta.

## 0.1.2-beta
- Correct minor SonarQube issues on RemoveZipArchiveEntryStep.

## 0.1.1-beta
- Correct minor SonarQube issues on CreateNewArchiveStep, OpenExistingArchiveFromStreamStep, and UpdateZipArchiveEntryStep.

## 0.1.0-beta
- Initial implementation of AddZipArchiveEntryStep, CreateNewArchiveStep, OpenExistingArchiveFromStreamStep, ReadZipArchiveEntriesFromZipArchiveStep, RemoveZipArchiveEntryStep, and UpdateZipArchiveEntryStep.
