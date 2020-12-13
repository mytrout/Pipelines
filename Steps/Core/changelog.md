# MyTrout.Pipelines.Steps.Core Change Log

## 2.0.6
 - Upgrade to .NET 5.0
 - Change the project and repository urls from dev.azure.com/mytrout to github.com.
 - Move versioning out of the azure-pipelines.yml and into Azure DevOps.
 - Remove deprecated Microsoft.CodeAnalysis.NetAnalyzers library.
 - Remove deprecated Microsoft.CodeAnalysis.FxCopAnalyzers library.

## 1.0.1
- Limit MyTrout.Pipelines versions to >= 1.1 and < 2.0 in the nuget package.

## 1.0.0
- Upgrade Hosting to MyTrout.Pipelines v1.1.*
- Generate and publish to Azure DevOps Artifacts the snupkg file for symbols.
- Implement nullable reference types.
- Support a single step type being configured multiple times in the same pipeline via StepContext.
- Move PipelineContextConstants to the MyTrout.Pipelines.Steps.Core assembly.
- Move PipelineContextValidationExtensions to the MyTrout.Pipelines.Steps.Core assembly.
- Move StreamExtensions to the MyTrout.Pipelines.Steps.Core assembly.

## 0.27.0-beta
- Refactor PipelineContext and Parameter Validation Extension methods into two different classes.

## 0.26.3-beta
- Add AssertParameterIsNotWhiteSpace to the Parameter Validation Extensions.

## 0.26.2-beta
- Correct minor warnings and stylecop issues.

## 0.26.1-beta
- Added StreamExtensions to prevent duplication and allow additional testing in MyTrout.Pipelines.Steps.Crytography.

## 0.26.0-beta
- Correct issues with unit tests introduced by v0.25.0-beta.
- Add the MoveOutputStreamToInputStreamStep to allow for additional testing in MyTrout.Pipelines.Steps.Crytography.

## 0.25.0-beta
- Refactor MoveInputStreamToOutputStreamStep to enable simpler testing.
- Correct minor warnings and stylecop issues with MoveInputStreamToOuputStreamStep.
- Add new ParameterValidationExtensions method.

Related work items: #11

## 0.24.0-beta
- Add MoveInputStreamToOutputStreamStep.
- Reorganize the namespaces on other classes to preserve the correct dependency structures.
