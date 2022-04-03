# MyTrout.Pipelines.Steps.Cryptography Change Log

## 3.2.0
 - Add unit test because AltCover shows a branch code coverage discrepancy causing coverage to fall to 96.6%
 - Use the null-forgiving operator to remove known null suppression false positives.
 - Suppress SA1636 because Steps are being upgraded to use null-forgiving operator in known good cases.
 - Change DecryptStreamWithAes256Step to AbstractCachingPipelineStep to eliminate an unnecessary context item removal which caused code coveraget decrease.
     IMPORTANT NOTE: Any change *WILL NOT* be considered breaking when the public/protected interface is not changed, including marking an item Obsolete.
            

## 3.1.0
 - Upgrade to .NET 6.0 while maintaining 5.0 support.
 - Update C# Language Version from 9.0 to 10.0.
 - Change code to match C# 10.0 language constructs.
 - Refactor code deprecated by .NET 6.0.
 - Upgrade Tests to .NET 6.0 only.
 - Upgrade documentation to reflect .NET 6.0.
 - Upgrade MyTrout.Pipelines from 3.0.1 to 3.2.0
 - Install Microsoft.CodeAnalysis.Analyzers 3.3.3
 - Install Microsoft.CodeAnalysis.NetAnalyzers 6.0.0
 - Upgrade Microsoft.VisualStudio.Threading.Analyzers from 17.0.17-alpha to 17.0.64
 - Upgrade SonarAnalyzer.CSharp from 8.25.0.33663 to 8.33.0.40503
 - Upgrade StyleCop.Analyzers from 1.2.0-beta-354 to 1.20.0-beta.376
 - Upgrade all Test dependencies to ensure Unit Tests continue to operate.
 - Add work_dispatch element to allow this library to to built manually in Github.

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
