# MyTrout.Pipelines.Steps.Cryptography Change Log

## 3.0.0
- Upgrade to MyTrout.Pipelines v3.0.1
- Change the nuget PackageURL and RepositoryURL from Azure DevOps to GitHub.
- Change Build Status badge from Azure DevOps to GitHub Actions.
- Convert Azure DevOps build to Github Actions build yaml.
- Correct SonarQube warnings and info.

## 2.0.0
 - Upgrade from .NET Standard 2.1 to .NET 5.0
 - Upgrade to MyTrout.Pipelines.Steps 2.0.6
 - Remove older analyzers in favor of .NET 5.0 analyzers.
 - Add header to change log.
 - Remove Major, Minor, and Iteration values into Azure DevOps variable group.
 - Remove restrictions on contributors from README.md.
 - Add instructions about how to provide secure implementations for secrets in ~Options classes.
 - Add build, nuget, and github badges to the README.md.

## 1.0.0
- Upgrade to MyTrout.Pipelines.Steps v1.0.0 (including MyTrout.Pipelines v1.1.0)
- Generate and publish to Azure DevOps Artifacts the snupkg file for symbols.
- Implement nullable reference types.

## 0.5.0-beta
- Upgrade to MyTrout.Pipelines.Steps v0.1.0-beta (including MyTrout.Pipelines v1.0.1)

## 0.4.0-beta
- Upgrade to MyTrout.Pipelines v0.27.0-beta.

## 0.3.1-beta
- Simplify the implementation of the EncrytStream step.

## 0.3.0-beta
- Correct issues that prevented DecryptStream, EncryptStream, and CreateSHA256Hash steps from working properly.

## 0.2.2-beta
- Correct SonarQube documentation issues.
- Correct SonarQube issue with closing Stream more than once.
- Remove unused using statements.

## 0.2.1-beta
- Initial implementation of DecryptStream, EncryptStream, and CreateSHA256Hash steps.
