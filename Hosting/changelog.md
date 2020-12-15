# MyTrout.Pipelines.Hosting Change Log

## 2.0.0
 - Upgrade to .NET 5.0
 - Change the project and repository urls from dev.azure.com/mytrout to github.com.
 - Move versioning out of the azure-pipelines.yml and into Azure DevOps.
 - Remove deprecated Microsoft.CodeAnalysis.NetAnalyzers library.
 - Remove deprecated Microsoft.CodeAnalysis.FxCopAnalyzers library.
 - Use primarySourceBranch variable for builds to allow SonarQube analysis to be performed on feature branches.
 - Update AddStepDependencyExtensions to comply with non-null reference type constraints.

## 1.2.1
- Limit MyTrout.Pipelines versions to >= 1.1 and < 2.0 in the nuget package.

## 1.2.0
- Upgrade Hosting to MyTrout.Pipelines v1.1.*
- Generate the snupkg file for symbols for MyTrout.Pipelines.Hosting.
- Upgrade MyTrout.Pipelines.Hosting to nullable reference types.

## 1.1.0
- Upgrade Hosting to MyTrout.Pipelines v1.0.1
- Support a step being configured multiple times with different configurations in the same pipeline via AddStepDependency.
- Rename PipelineHostBuilderExtensions to UsePipelineExtensions.
- Split the UsePipelineExtensions into AddStepDependencyExtensions and UsePipelineExtensions.
- Changed '<' to &lt; and '>' to &gt; to correct warnings in the Resources T4 template documentation.

## 1.0.0
- Upgrade Hosting to MyTrout.Pipelines v0.27.0-beta

## 0.21.0-beta
- Upgrade Hosting to MyTrout.Pipelines v0.26.0-beta 

## 0.20.0-beta
- Upgrade Hosting to MyTrout.Pipelines v0.25.0-beta

## 0.19.0-beta
- Upgrade Hosting to use MyTrout.Pipelines v0.24.0-beta

## 0.18.0-beta
- Upgrade Hosting to use MyTrout.Pipelines v0.23.0-beta.

## 0.17.0-beta
- Correcting all style analyzer and compiler warnings with Hosting, including a potential threading issue in SonarCloud.

## 0.15.1-beta
- Correct project and git repository urls for this package.