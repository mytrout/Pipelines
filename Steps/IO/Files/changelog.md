# MyTrout.Pipelines.Steps.IO.Files Change Log

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