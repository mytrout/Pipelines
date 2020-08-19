# MyTrout.Pipelines.Steps.IO.Files Change Log

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