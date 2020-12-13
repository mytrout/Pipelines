# MyTrout.Pipelines.Core Change Log

## 2.0.8
 - Add the pr tag to the azure-pipelines.yml due to differences between Azure DevOps repos and GitHub repos.

## 2.0.7
 - Remove the Pipelines.Steps.Core project from building and analyzing during the Azure DevOps Pipelines.Core project build.
 
## 2.0.4
- Change the project and repository urls from dev.azure.com/mytrout to github.com.

## 2.0.1
- Alter StepActivator to allow null parameters to allow object construction to blow up which reduces method complexity.
- Update azure-pipelines.yaml to use the build-nuget-template.

## 2.0.0
- Making the documentation more clear about what the exceution order will be in readme.md.
- Changed the Target Framework to .NET 5.0.
- Changed StepWithContext to a an immutable record, instead of a class.
- Changed NoOpStep to sealed to eliminate need to call GC.SuppressFinalize() to correct a warning.
- Removed Warning Suppressions from GlobalSuppressions.cs because the warning no longer applied to the code in .NET 5.0.
- Removed derefence null and null reference warnings in StepActivator.
- Removed CA1031 suppression for catching System.Exception in StepActivator by adding the list of exceptions thrown by ConstructorInfo.Invoke().
- Removed unnecessary initialzation of PipelineContext.CancellationToken property.
- Updated TYPE_FAILED_TO_INITIALIZE resource to reflect all of the cases where the StepActivator could fail to initialize a constructor parameter.
- Removed GlobalSuppressions.cs as the file does not contain any suppressions.
- Updated the Major, Minor, and Patch values to reflect the 2.0.0 version.
- Removed comments that stated GB localization tests did not work.
- Corrected the Returns_Valid_Step_Instance_From_CreateInstance() test after stricter requirements were applied to object construction in the StepActivator class.

## 1.1.1
- Remove unused parameter validations from AddStep<T> method.
- Add unit tests to bring this library up to 100% Code Coverage.

## 1.1.0
- Correct 'Cannot convert null literal to non-nullable reference type.' warnings because of nullable reference type configuration.
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
