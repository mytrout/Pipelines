# MyTrout.Pipelines.Core Change Log

## 3.0.2
- Correct DisposeAsync() method implementation.

## 3.0.1
- Add the Steps library to the Pipelines.Core library.
- Update the implemented Step Tests to use IDisposable and IAsyncDisposable.
- Remove CS8604 from editorconfig.
- Change Tests/Steps/test.txt to Always Copy.

## 3.0.0 - BREAKING CHANGES
- Move separate ~Steps library into the Pipelines.Core.
- Refactor StepActivator to inject Step dependencies into PipelineBuilder.
- Add StepDependencyType to the StepWithContext class.
- Add ConfigKeys to the StepWithContext class.
- Add StepWithFactory{TStep,TOptions} class to handle factory method instantiation of the TOptions class.
- Add StepWitnInstance{TStep,TOptions} class to handle a pre-built instance of the TOptions class.
- Add IStepWithFactory interface to enable 100% code coverage during the usage of StepWithFactory{TStep,TOptions} instance in the StepActivator implementation.
- Add IStepWithInstance interface to enable 100% code coverage during the usage of StepWithInstance{TStep,TOptions} intance in the StepActivator implementation.
- The project has been compiled and tested against .NET 6.0 Preview 4 to ensure future compatibility.
- Move all helper method AddStep implementations from PipelineBuilder to PipelineBuilderExtensions.
- Add additional PipelineBuilder.AddStep extension methods to simplify the usage of adding dependencies.
- Refactor the StepActivator.RetrieveParameter() method to reduce Cognitive Complexity from 29 to 15 or below.
- Upgrade the StyleCop Analyzer to 1.2.0-beta.261 to alleviate the following exception when building with GitHub Actions:
	CSC : warning AD0001: Analyzer 'StyleCop.Analyzers.DocumentationRules.SA1649FileNameMustMatchTypeName' threw an exception of type 'System.ArgumentException' with message 'Unhandled declaration kind: RecordDeclaration'. [/home/runner/work/Pipelines/Pipelines/Core/test/MyTrout.Pipelines.Tests.csproj]
- Alter build-pipelines-core.yaml to publish code coverage statistics to SonarCloud.io from GitHub Actions.
- Document process of all GitHub actions corrections named github-actions-and-net-5.0.md in the root folder for the repository.
- Disable stylecop check on copyright headers due to lack of ability to have multiple year ranges in the same project.
- Remove GlobalSuppressions.cs because all of the rules are either deprecated or suppressed elsewhere.
- Delete PipelineRequestDelegate as it is no longer used.
- Add ParameterCreationDelegate, ParameterCreationResult and StepActivator.ParameterCreators to allow parameter creation behavior in StepActivator to be changed.
- Due to the change in parameter creation behavior, the virtual StepActivator.RetrieveParameter method is no longer required.  Finer-grained control is available via StepActivator.ParameterCreators.

## 2.1.1
- Remove extra stylecop.json file as it isn't being respected by Github Actions dotnet build step.
- Change copyright headers in PipelineContext and StepActivator back to 2019-2020 year range.
- Change copyright headers for PipelineBuilder to 2019-2021 range.
- Add StyleCop and warning suppressions to PipelineBuilder.cs for the 2019-2021 range.
- Remove ExcludeFromCodeCoverage attribute on the PipelineBuilder constructor.
- Add virtual keyword to PipelineBuilder.Build() method.
- Remove Microsoft.CSharp 4.7.0 from the Software Dependencies section of README.md.
- Correct stars badge link to point to mytrout/Pipelines on github.
- Correct forks badge link to point to mytrout/Pipelines on github.
- Change Build Status to point to Github Actions.

## 2.1.0
- Remove capability to build and publish library on Azure DevOps.
- Add capability to build and publish library on Github.
- Eliminate all code rules suppressions in GlobalSuppressions.cs as they are no longer required.
- Change all copyright headers to copyright year range ending on 2021 for every file in src/Core directory.
- Add new stylecop.json file with copyright year range ending on 2021.
- Change version in project to 2.1.0.
- New files have code suppressions to allow the copyright year only on 2021.
- Remove Microsoft.CSharp 4.7.0 Dependency as it is no longer required.
- Add StepAddedEventArgs to allow StepBuilding event to be invoked.
- Add StepAddedEventHandler event to PipelineBuilder to allow other activities occur when steps are added.

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
