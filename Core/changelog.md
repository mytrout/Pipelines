## 1.0.2
- Generate the snupkg file for symbols for MyTrout.Pipelines.Core.
- Update changelog for MyTrout.Pipelines v1.0.1.

## 1.0.1
- Minor change in Tests to correct a SonarQube issue about unused property.

## 1.0.0
- Support a single step type being configured multiple times in the same pipeline via StepContext.
- Move StepActivetor from the MyTrout.Pipelines namespace to MyTrout.Pipelines.Core namespace to better reflect its usage and dependencies.
- Move everything in the /Steps directory to the MyTrout.Pipelines.Steps.Core assembly, EXCEPT NoOpStep which is required by Core/PipelineBuilder.
- Move PipelineContext to the /Core directory.
- Move PipelineContextConstants to the MyTrout.Pipelines.Steps.Core assembly.
- Move PipelineContextValidationExtensions to the MyTrout.Pipelines.Steps.Core assembly.
- Move StreamExtensions to the MyTrout.Pipelines.Steps.Core assembly.

IMPORTANT NOTE: 
The vast majority of changes in the last 10 versions of MyTrout.Pipelines was to Step-related functionality. 
Chris Trout decided to move the Step-related functionality to another assembly to provide a more stable base package.
After MyTrout.Pipelines v1.0.0 is published, a MyTrout.Pipelines.Steps v0.1.0-beta will be published with the same classes that were previously published in earlier versions of MyTrout.Pipelines.

## 0.27.0-beta
- Refactor PIpelineComntext and Parameter Validation Extension methods into two different classes.

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
