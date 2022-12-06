# MyTrout.Pipelines.Steps.Azure.ServiceBus Change Log

## 5.1.0
- Change default branch from master to main.
- Upgrade C# Language Version from 9.0 to 10.0 across all src projects.
- Standardize the first &lt;Property Group&gt; section in the src csproj files across all src projects.
- Standardize the NoWarn options within the src csproj files across all src projects.
- Standardize the Neutral Language options to en-US across all src projects.
- Standardize the Copyright to include 2022 across all src projects.
- Standardize all .editorconfig file inclusion across all src projects.
- Standardize inclusion of README.md file across all src projects.
- Force upgrade to MyTrout.Pipelines v4.0.3 minimum.

## 5.0.0
### BREAKING CHANGES:
- [#160](https://github.com/mytrout/Pipelines/issues/160) Remove support for .NET 5.0. 
- [#162](https://github.com/mytrout/Pipelines/issues/162) Upgrade to MyTrout.Pipelines 4.0.0 
### NON-BREAKING CHANGES:
- [# 85](https://github.com/mytrout/Pipelines/issues/85)  Update Resources.tt to use NamespaceHint instead of a hard-coded namespace.
- [# 94](https://github.com/mytrout/Pipelines/issues/94)  Add "if-no-files-found: error" to the nuget publishing step.
- [#148](https://github.com/mytrout/Pipelines/issues/148) Refactor ReadMessageFromAzureStep to use AbstractCachingPipelineStep<TStep, TOptions> to guarantee that existing INPUT_STREAM values are restored after execution of this step.
- [#148](https://github.com/mytrout/Pipelines/issues/148) Add additional unit test to ensure that ReadMessageFromAzureStep restores PipelineContext.Items to its original state after execution.
- [#160](https://github.com/mytrout/Pipelines/issues/160) Add support for .NET 7.0
- [#161](https://github.com/mytrout/Pipelines/issues/161) Refactor ReadMessageFromAzureStep and ReadMessageFromAzureOptions to use user-configurable context names for any value read from or written to IPipelineContext.Items.
- [#161](https://github.com/mytrout/Pipelines/issues/161) Refactor WriteMessageToAzureStep and WriteMessageToAzureOptions to use user-configurable context names for any value read from or written to IPipelineContext.Items.
- Uncomment all of the nuget publish steps to allow a new version to be published.
- Add .editorconfig to enforce rules in Visual Studio 2022.

## 4.0.0
 - (BREAKING CHANGE) - ReadMessageFromAzureStep will always copy all ApplicationProperties values from a message into PipelineContext.Items with a prefix provided by ReadMessageFromAzureOptions.
 - (BREAKING CHANGE) - ReadMessageFromAzureOptions.ApplicationProperties has been removed permanently.
 - (BREAKING CHANGE) - ReadMessageFromAzureStep.EvaluateCancellationOfMessageAsync method was refactored to allow integration testing to use a change in this method instead of a protected setter on the ServiceBusReceiver property.
 - (BREAKING CHANGE) - ReadMessageFromAzureStep.ServiceBusReceiver is no longer a protected setter; it is read-only after construction.
 - (BREAKING CHANGE) - WriteMessageToAzureOptions.ApplicationProperties has been removed permanently.
 - (BREAKING CHANGE) - WriteMessageToAzureStep will always copy all PipelineContext.Items values prefixed with WriteMessageToAzureOptions.MessageContextItemsPrefix to the message being sent.
 - Correction of StyleCop and other analyzers to ensure consistency across projects.

## 3.1.0
 - Upgrade to .NET 6.0 while maintaining 5.0 support.
 - Update C# Language Version from 9.0 to 10.0.
 - Change code to match C# 10.0 language constructs.
 - Refactor code with warnings or informational issues.
 - Upgrade Tests to .NET 6.0 only.
 - Upgrade documentation to reflect .NET 6.0.
 - Upgrade Azure.Messaging.ServiceBus from 7.2.0 to 7.5.1
 - Upgrade MyTrout.Pipelines from 3.0.1 to 3.2.0
 - Install Microsoft.CodeAnalysis.Analyzers 3.3.3
 - Install Microsoft.CodeAnalysis.NetAnalyzers 6.0.0
 - Upgrade Microsoft.VisualStudio.Threading.Analyzers from 16.10.56 to 17.0.64
 - Install Roslynator.Analyzers 3.3.0
 - Upgrade SonarAnalyzer.CSharp from 8.26.0.34506 to 8.33.0.40503
 - Upgrade StyleCop.Analyzers from 1.1.118 to 1.20.0-beta.376
 - Upgrade all Test dependencies to ensure Unit Tests continue to operate.
 - Add work_dispatch element to allow this library to to built manually in Github.
 - Suppress the CA2254 warning because it doesn't handle Resource files for logging messages.
 - Suppress SA1636 Header Warning because ReadMessageFromAzure.cs and GlobalSuppressions.cs are now copyright 2019-2021.

##3.0.0
- Convert from Azure DevOps to Github Actions.
- Delete azure-pipelines.yml and remove from the solution.
- Upgrade Azure.Azure.ServiceBus library from 7.0.x to 7.2.x.
- Upgrade Microsoft.VisualStudio.Threading.Analyzers library from 16.8.x to 16.10.x.
- Remove MyTrout.Pipelines.Steps library v2.0.6.
- Add MyTrout.Pipelines library v3.0.1.
- Upgrade SonarAnalyzer.CSharp library from 8.16 to 8.26.
- Alter all Unit and Integration Tests to support Linux.

## 2.0.1
- Limit MyTrout.Pipelines.Steps.Core to a maximum of 3.x rather than 32.x.

## 2.0.0
 - Upgrade from .NET Standard 2.1 to .NET 5.0
 - Upgrade to MyTrout.Pipelines.Steps 2.0.6
 - Change from Microsoft.Azure.ServiceBus 4.1.3 to Azure.Messaging.ServiceBus 7.0.0
 - Move the CORRELATION_ID constant from the WriteMessageToAzureTopicStep to a MessagingConstants file.
 - Change ReadMessage~Step name to reflect new capability of using either Subscriptions or Queues.
 - Change WriteMessage~Step name to reflect new capability of using either Topics or Queues.
 - Reordering all constructor parameters to be consistent with parameter ordering in AbstractPipelineStep<TStep, TOptions>

## 1.0.0
- Upgrade to MyTrout.Pipelines.Steps v1.0.0 (including MyTrout.Pipelines v1.1.0)
- Generate and publish to Azure DevOps Artifacts the snupkg file for symbols.
- Automatically provision and deprovision a Service Bus instance during the build stage of the pipeline to enable unit/integration testing without incurring 24-7 costs of maintaining a Service Bus instance.

- NOTE: No longer upgrading Step libraries to nullable reference types because it adds too much warning suppression noise to the code.

## 0.7.0-beta
- Update to "MyTrout.Pipelines.Steps v0.1.0-beta (including MyTrout.Pipelines v1.0.0)
- Correct documentation references to MyTrout.Pipelines.Core.
- Add using statements for MyTrout.Pipelines.Core.

## 0.6.0-beta
- Upgrade all libraries to MyTrout.Pipelines v0.27.0-beta
- Update library to use AssertParameterIsNotNull<T> method.
- Remove unused Resource entries.

## 0.5.0-beta
- Upgrade all libraries to MyTrout.Pipelines v0.25.0-beta

## 0.4.0-beta
- Upgrade all libraries to MyTrout.Pipelines v0.24.0-beta

## 0.3.0-beta
- Upgrade all libraries to MyTrout.Pipelines v0.23.0-beta
- Correct documentation issues in README.md.
- Upgrade FxCop, StyleCop, SonarQube, and CodeQuality issues.

## 0.2.0-beta
- Change SonarQube ProjectName and ProjectKey to standardized names for the Pipelines projects.
- Upgrade all libraries to MyTrout.Pipelines v0.22.0-beta
- Use IPipelineRequest abstraction rather than the PipelineRequest delegate definition.
- Update changelog in README.md file.

## 0.1.0-beta
- Add name element to the azure-pipelines.yml file.
- Set Environment Variable in azure-pipelines.yml to support test configuration of the Azure Service Bus ConnectionString.
- Initial commit of ReadMessageFromAzureStep and WriteMessageToAzureTopicStep.

