# MyTrout.Pipelines.Steps.IO.Files Change Log

## 4.0.0
### BREAKING CHANGES:
- [#160](https://github.com/mytrout/Pipelines/issues/160) Remove support for .NET 5.0. 
- [#162](https://github.com/mytrout/Pipelines/issues/162) Upgrade to MyTrout.Pipelines 4.0.0 
- [#169](https://github.com/mytrout/Pipelines/issues/169) Refactor all steps to conform to the ILogger<T>, ~Options, IPipelineRequest parameter order on constructors.
### NON-BREAKING CHANGES:
- [#148](https://github.com/mytrout/Pipelines/issues/148) Add additional unit test to ensure that ReadStreamFromFileSystemStep restores PipelineContext.Items to its original state after execution.
- [#160](https://github.com/mytrout/Pipelines/issues/160) Add support for .NET 7.0
- [#161](https://github.com/mytrout/Pipelines/issues/161) Refactor AppendStreamToFileStep, DeleteFileStep, MoveFileStep, ReadStreamFromFileSystemStep, WriteStreamToFileSystemStep and ~Options to use user-configurable context names for any value read from or written to IPipelineContext.Items.
- [#161](https://github.com/mytrout/Pipelines/issues/161) Add additional unit test to test that ContextName values are used when Steps executes.
- Add .editorconfig to enforce rules in Visual Studio 2022.
- Update Microsoft.CodeAnalysis.NetAnalyzers to .NET 7.0 preview version to eliminate build warnings.
- Update Microsoft.CodeAnalysis.Analyzers to .NET 7.0 preview version to eliminate build warnings.
- Update Microsoft.VisualStudio.ThreadingAnalyzers to 17.2.32
- Upgrade SonarAnalyzer.CSharp 8.40.0.48530
- Upgrade StyleCop.Analyzers to 1.2.0-beta.435
- Update all markdown files to use &amp;lt; and &amp;gt; in lieu of &lt; and &gt;

## 3.2.0
- Change from suppressions to null-forgiving operators to handle known false positive null warnings.
- Add AppendStreamToFileStep and ~Options to allow a stream to be appended to an new or existing file.
- Change the underlying base type for ReadStreamFromFileSystemStep to use the AbstractCachingPipelineStep&lt;TService, TOptions&gt; instead of implementing caching in this class.
- Uncomment the nuget publishing to allow version 3.2.0 to be published.
- Suppress SA1636 to allow for variation in the copyright headers for 2022 and range from 2019-2022.
- Add if-no-files-found to the nuget publish step in build.
- Bump actions/upload-artifact from 2 to 3.
- Uncomment the local Github artifact publish, nuget.org publish, and local Github nuget publish to allow these actions to run again.

## 3.1.0
 - Upgrade to .NET 6.0 while maintaining 5.0 support.
 - Update Resources.tt to reflect .NET 6.0 instead of .NET 5.0
 - Update C# Language Version from 9.0 to 10.0.
 - Change code to match C# 10.0 language constructs.
 - Upgrade Tests to .NET 6.0 only.
 - Upgrade documentation to reflect .NET 6.0.
 - Upgrade MyTrout.Pipelines from 3.0.1 to 3.2.0
 - Install Microsoft.CodeAnalysis.Analyzers 3.3.3
 - Install Microsoft.CodeAnalysis.NetAnalyzers 6.0.0
 - Upgrade Microsoft.VisualStudio.Threading.Analyzers from 16.8.55 to 17.0.64
 - Upgrade SonarAnalyzer.CSharp from 8.25.0.33663 to 8.33.0.40503
 - Upgrade all Test dependencies to ensure Unit Tests continue to operate.
 - Add work_dispatch element to allow this library to to built manually in Github.

## 3.0.0
- Upgrade to MyTrout.Pipelines v3.0.1
- Change Resources.tt from .NET Standard 2.1 to .NET 5.0 in the comments.
- Add 2021 to the copyright year range in Resources.tt
- Change the nuget PackageURL and RepositoryURL from Azure DevOps to GitHub.
- Change Build Status badge from Azure DevOps to GitHub Actions.
- Convert Azure DevOps build to Github Actions build yaml.
- Change GetFullyQualifiedPath to use IsPathRooted rather than GetFullyQualifiedPath; the latter works differently on Linux than on Windows.
- Change tests to support differently rooted linux paths.

## 2.1.0
- Upgrade to MyTrout.Pipelines.Steps v2.0.7
- Upgrade all analyzers and remove deprecated analyzers.
- Add the ExecutionTimings to DeleteFileOptions, MoveFileOptions, and WriteStreamToFileSystemOptions.
- Add the ability to execute on the request or response side for DeleteFileStep, MoveFileStep, and WriteStreamToFileSystemStep.
- Correct analyzer warnings.

## 2.0.0
- Upgrade to .NET 5.0
- Upgrade to MyTrout.Pipelines.Steps v2.0.0.
- Add nuget, build, and sonarqube badges to README.md.
- Add nullable requirements and C# Language Version 9.0 to the project.
- Update azure-pipelines.xml to add artifactPublishEnabled property.
- Correct broken internal documentation links 

## 1.0.0
- Upgrade to MyTrout.Pipelines.Steps v1.0.0 (including MyTrout.Pipelines v1.1.0)
- Generate and publish to Azure DevOps Artifacts the snupkg file for symbols.

- NOTE: No longer upgrading Step libraries to nullable reference types because it adds too much warning suppression noise to the code.

## 0.5.0-beta
- Upgrade MyTrout.Pipelines.Steps v0.1.0-beta (including MyTrout.Pipelines v1.0.0)

## 0.4.0-beta
- Upgrade all libraries to MyTrout.Pipelines v0.27.0-beta

## 0.3.0-beta
- Upgrade to MyTrout.Pipelines v0.26.3

## 0.2.1-beta
- Add nuget properties to the csproj file.
- Correct SonarQube warnings that were not available in the IDE at development time.

## 0.2.0-beta
- Initial implementation of DeleteFileStep, MoveFileStep, ReadStreamFromFileSystemStep, and WriteStreamToFileSystemStep.
- Remove ReadFileFromFileSystemStep and WriteFileToFileSystemStep.
- Correct all SonarQube and analyzer warnings.
- Implement 100% code coverage.

## 0.1.0-beta
- Initial implementation of ReadFileFromFileSystemStep and WriteFileToFileSystemStep.