# MyTrout.Pipelines.Steps.IO.Directories Change Log

## 1.1.0
- Change default branch from master to main.
- Upgrade C# Language Version from 9.0 to 10.0 across all src projects.
- Standardize the first &lt;Property Group&gt; section in the src csproj files across all src projects.
- Standardize the NoWarn options within the src csproj files across all src projects.
- Standardize the Neutral Language options to en-US across all src projects.
- Standardize the Copyright to include 2022 across all src projects.
- Standardize all .editorconfig file inclusion across all src projects.
- Standardize inclusion of README.md file across all src projects.
- Remove Moq references from src project.
- Force upgrade to MyTrout.Pipelines.Steps.IO.Files v4.1.0 minimum.

## 1.0.2
- Update SonarCloud with latest statistics.
- This version will NOT be uploaded to nuget or the github internal nuget package repository.

## 1.0.1
- Correct the v1.0.0 changelog text.
- Remove the Microsoft.Extensions.Logging.Abstractions v7.0.0-preview.5.22301.12 reference to remove build warning and because it is unused.

## 1.0.0
- Initial implementation of CreateDirectoryStep, DeleteDirectoryStep,EnumerateFilesInDirectoryStep, and MoveDirectoryStep based on Pipelines v4.0.0 guidelines.