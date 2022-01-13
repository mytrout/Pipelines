# MyTrout.Pipelines.Steps.Communications.Http Change Log

## 1.1.0 - SonarCloud UPDATE ONLY
Suppress SA1009 to prevent false positives in SonarCloud.io with after paranthesis spacing.
Commented out ability to publish to nuget or github to ensure this update does not cause a build failure.
Add "if-no-files-found: error" to the nuget publishing step. (see Issue #94)
NOTE TO DEVELOPERS: Developers must reverse the removal of nuget and github publishing when the next version is ready to be released

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
