# MyTrout.Pipelines.Steps.Azure.ServiceBus Change Log

## 3.1.0
- Upgrade to .NET 6.0

## 3.0.0
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

