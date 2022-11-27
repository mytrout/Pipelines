# MyTrout.Pipelines.Steps.Communications.Http Change Log

## 2.1.0
### NON-BREAKING CHANGES:
- [#198](https://github.com/mytrout/Pipelines/issues/198) Allow context values to be injected dynamically into the HttpEndpoint to allow GET verbs to use query string values.

## 2.0.0
### BREAKING CHANGES:
- [#160](https://github.com/mytrout/Pipelines/issues/160) Remove support for .NET 5.0. 
- [#162](https://github.com/mytrout/Pipelines/issues/162) Upgrade to MyTrout.Pipelines 4.0.0 
### NON-BREAKING CHANGES:
- [#148](https://github.com/mytrout/Pipelines/issues/148) Refactor SendHttpRequestStep to use AbstractCachingPipelineStep<TStep, TOptions> to guarantee that existing values are restored after execution of this step.
- [#148](https://github.com/mytrout/Pipelines/issues/148) Add additional unit test to ensure that SendHttpRequestStep restores PipelineContext.Items to its original state after execution.
- [#160](https://github.com/mytrout/Pipelines/issues/160) Add support for .NET 7.0
- [#161](https://github.com/mytrout/Pipelines/issues/161) Refactor SendHttpRequestStep and SendHttpRequestOptions to use user-configurable context names for any value read from or written to IPipelineContext.Items.
- [#161](https://github.com/mytrout/Pipelines/issues/161) Add additional unit test to test that SendHttpRequestOptions ~ContextName values are used when SendHttpRequestStep executes.
- Uncomment all of the nuget publish steps to allow a new version to be published.
- Add .editorconfig to enforce rules in Visual Studio 2022.
- Update Microsoft.CodeAnalysis.NetAnalyzers to .NET 7.0 preview version to eliminate build warnings.

## 1.1.0 - SonarCloud UPDATE ONLY
 - Removed the null check when returning HttpResponseMessage.Content because it is non-nullable in .NET 5.0 and higher.
 - Suppress SA1636 Copyright Header violations since SendHttpRequestStep.cs is now 2021-2022 Copyright.

## 1.1.0 - SonarCloud UPDATE ONLY
 - Suppress SA1009 to prevent false positives in SonarCloud.io with after paranthesis spacing.
 - Commented out ability to publish to nuget or github to ensure this update does not cause a build failure.
 - Add "if-no-files-found: error" to the nuget publishing step. (see Issue #94)
 - NOTE TO DEVELOPERS: Developers must reverse the removal of nuget and github publishing when the next version is ready to be released

## 1.1.0
 - Upgrade to .NET 6.0 while maintaining 5.0 support.
 - Update C# Language Version from 9.0 to 10.0.
 - Change code to match C# 10.0 language constructs.
 - Upgrade Tests to .NET 6.0 only.
 - Upgrade documentation to reflect .NET 6.0.
 - Upgrade MyTrout.Pipelines from 3.1.0 to 3.2.0
 - Install Microsoft.CodeAnalysis.Analyzers 3.3.3
 - Install Microsoft.CodeAnalysis.NetAnalyzers 6.0.0
 - Install Roslynator.Analyzers 3.3.0
 - Upgrade Microsoft.VisualStudio.Threading.Analyzers from 17.0.17-alpha to 17.0.64
 - Upgrade SonarAnalyzer.CSharp from 8.25.0.33663 to 8.33.0.40503
 - Upgrade StyleCop.Analyzers from 1.2.0-beta.354 to 1.20.0-beta.376
 - Upgrade all Test dependencies to ensure Unit Tests continue to operate.
 - Add yaml build file to the solution.
 - Add work_dispatch element to allow this library to to built manually in Github.

## 1.0.0
- Initial implementation of SendHttpRequestStep
