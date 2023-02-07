# MyTrout.Pipelines.Core Change Log

## 4.2.0
- Move some readme.md documentation to the root readme.md.
- Remove README.md and stylecop.json from the Solution Items collection.
- Correct the links to the readme.md documentation.
- Add documemtation on the root readme.md to on how to perform git remote prune origin automatically in Visual Studio.
- Implement the Optional [FromServicesAttribute] capability when a property has a default value.
- Use the Optional [FromServicesAttribute] capability on LoadValuesFromContextObjectToPipelineContextOptions for IContextNameBuilder.
- Mark obsolete the LoadValuesFromContextObjectToPipelineContextOptions.BuildContextNameFunction in favor of new ContextNameBuilder property.
- To prevent a breaking change on LoadValuesFromContextObjectToPipelineContextOptions, override the usage of ContextNameBuilder in the BuildContextName method.
- Remove ExcludeFromCodeCoverageAttribute from LoadValuesFromContextObjectToPipelineContextOptions because it was incorrectly marked.
- Alter AbstractPipelineStep{TStep} to support BeforeNextStepAsync and AfterNextStepAsync methods and predicates to allow more run-time execution control on each step.
- Adding PipelineName back to the PipelineContext to enable downstream callers to log the PipelineName.

## 4.1.0
- Upgrade from preview release of .NET 7.0 to official release.

## 4.0.3 
- Upgrade C# Language Version from 9.0 to 10.0 across all src projects.
- Standardize the first &lt;Property Group&gt; section in the src csproj files across all src projects.
- Standardize the NoWarn options within the src csproj files across all src projects.
- Standardize the Neutral Language options to en-US across all src projects.
- Standardize the Copyright to include 2022 across all src projects.
- Standardize all .editorconfig file inclusion across all src projects.
- Standardize inclusion of README.md file across all src projects.

## 4.0.2 - SONARCLOUD UPDATE ONLY
- Rebuild with a new version number after renaming the master branch in SonarCloud.
- Prevent the publish to nuget.org as it isn't necessary.
- NOTE TO DEVELOPERS: Developers must reverse the removal of nuget and github publishing when the next version is ready to be released.

## 4.0.1
- Add PipelineContextValidationExtension to check object existence without casting it.
- Add new step MoveInputObjectToOutputObjectStep to perform default renaming easily.
- Add new step MoveOutputObjectToInputObjectStep to perform default renaming easily.
- Add an Environment Variable named 'PIPELINES_PreventCollisionsWithRenamedValueNames' to control default behavior for MoveInput~ToOutput~Steps.  
- SonarQube ignored the ExcludeFromCodeCoverage on the FromServicesAttribute, so unit tests were delivered.
- Change the push branch in the build yaml from master to main.

See [Documentation around 4.0.1](../docs/pipelines-core-4-0.1.md) for more information.

## 4.0.0 - BREAKING CHANGES
- Alter IStepActivator interface to include the ParameterCreators property to allow developers to reconfigure the parameter creation behavior.
- BREAKING CHANGE: Remove support for .NET 5.0.
- BREAKING CHANGE: Move ParameterCreationDelegate and ParameterCreationResult to the MyTrout.Pipelines namespace due to IStepActivator.ParameterCreators change to preserve namespace dependencies.
- BREAKING CHANGE: Alter ParameterCreationDelegate to use ILogger&lt;IStepActivator&gt; instead of ILogger&lt;StepActivator&gt; to preserve namespace dependencies.
- BREAKING CHANGE: Alter StepActivator delegate methods to use ILogger&lt;IStepActivator&gt; instead of ILogger&lt;StepActivator&gt; to preserve namespace dependencies.
- Add new ParameterCreationDelegate that injects both IConfiguration and Dependency Injection values.
- Alter the behavior of StepActivator.ParameterCreators property to use the newly created CreateParameterFromConfigurationAndDependencyInjection delegate.
- Uncomment all of the nuget publish steps to allow a new version to be published.
- Mark the Configuration and IsConfigurationAvailable properties with the Obsolete attribute as the new LoadConfigurationToPipelineContextStep supercedes this functionality.
- Add CreateUnixEpochStep.
- Add LoadValuesFromContextObjectToPipelineContextStep&lt;TObject&gt;
- Add LoadValuesFromConfigurationToPipelineContextStep.
- Add LoadValuesFromContextObjectToPipelineContextStep&lt;TObject&gt;.
- Add EnumerateItemsinCollectionStep&lt;TParent, TObject&gt;.
- Refactor LoadValuesFromContextObjectToPipelineContextStep and LoadValuesFromOptionsToPipelineContextStep into AbstractLoadItemToPipelineContextStep&lt;TItem, TOptions&gt;.
- Add UnixEpochKind to support different epoch configurations in CreateUnixEpochStep.
- Add INPUT_OBJECT, OUTPUT_OBJECT, and UNIX_EPOCH to PipelineContextConstants.
- Refactor MoveOutputStreamToInputStreamStep and MoveInputStreamToOutputStream to use the RenameContextItemStep as the base class to eliminate duplicate code.
- Correct documentation typos on RenameContextItemStep.
- Add support for .NET 7.0
- Add unit tests to confirm that PipelineContext is restored to its previous state after InvokeAsync().

## 3.2.0 - SonarCloud UPDATE ONLY
- Suppress CA2254 to prevent false positives in SonarCloud.io with culture-aware logging messages
- Correct test project steps to conform with CA1816 warning
- Simplify loop to use LINQ expression in RenameContextItemstep to conform with S3267
= Install Microsoft.CodeAnalysis.NetAnalyzers 3.3.0
- Install Roslynator.Analyzers 3.3.0
- Install SonarAnalyzer.CSharp 8.33.0.40503
- Commented out ability to publish to nuget or github to ensure this update does not cause a build failure.
- Add "if-no-files-found: error" to the nuget publishing step. (see Issue #94)
- NOTE TO DEVELOPERS: Developers must reverse the removal of nuget and github publishing when the next version is ready to be released

## 3.2.0
- Upgrade to .NET 6.0
- Upgrade libraries to 6.0.0 versions.

## 3.1.0
- Add the AbstractCachingPipelineStep{TStep}, AbstractCachingPipelineStep{TStep,TOption}, RenameContextItemStep, and RenameContextItemOptions.
- Simplify the MoveInputStreamToOutputStreamStep to use the AbstractCachingPipelineStep{TStep} as its base class.
- Simplify the MoveOutputStreamToInputStreamStep to use the AbstractCachingPipelineStep{TStep} as its base class.

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
