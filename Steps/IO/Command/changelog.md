# MyTrout.Pipelines.Steps.IO.Command

## 2.0.0
### BREAKING CHANGES:
- [#160](https://github.com/mytrout/Pipelines/issues/160) Remove support for .NET 5.0. 
- [#162](https://github.com/mytrout/Pipelines/issues/162) Upgrade to MyTrout.Pipelines 4.0.0 
### NON-BREAKING CHANGES:
- [#160](https://github.com/mytrout/Pipelines/issues/160) Add support for .NET 7.0
- Add .editorconfig to enforce rules in Visual Studio 2022.
- Update Microsoft.CodeAnalysis.NetAnalyzers to 7.0.0-preview1.22274.2
- Update Microsoft.CodeAnalysis.Analyzers to 3.3.4-beta1-22274.2
- Update Microsoft.VisualStudio.ThreadingAnalyzers to 17.2.32
- Upgrade SonarAnalyzer.CSharp 8.40.0.48530
- Upgrade StyleCop.Analyzers to 1.2.0-beta.435

## 1.1.0 - SonarCloud UPDATE ONLY
- Suppress CA2254 to prevent false positives in SonarCloud.io with culture-aware logging messages
- Install Roslynator.Analyzers 3.3.0
- Commented out ability to publish to nuget or github to ensure this update does not cause a build failure.
- Add "if-no-files-found: error" to the nuget publishing step. (see Issue #94)
- NOTE TO DEVELOPERS: Developers must reverse the removal of nuget and github publishing when the next version is ready to be released

## 1.1.0
 - Upgrade to .NET 6.0 while maintaining 5.0 support.
 - Update Resources.tt to reflect .NET 6.0 instead of .NET 5.0
 - Update C# Language Version from 9.0 to 10.0.
 - Change code to match C# 10.0 language constructs.
 - Upgrade Tests to .NET 6.0 only.
 - Upgrade documentation to reflect .NET 6.0.
 - Upgrade MyTrout.Pipelines.Steps.IO.Files from 3.0.0 to 3.1.0
 - Upgrade Microsoft.CodeAnalysis.Analyzers from 3.3.2 to 3.3.3
 - Install Microsoft.CodeAnalysis.NetAnalyzers 6.0.0
 - Install Microsoft.VisualStudio.Threading.Analyzers 17.0.64
 - Upgrade SonarAnalyzer.CSharp from 8.25.0.33663 to 8.33.0.40503
 - Upgrade StyleCop.Analyzers from 1.2.0-beta.354 to 1.2.0-beta.376
 - Upgrade all Test dependencies to ensure Unit Tests continue to operate.
 - Upgrade ExceptionConsole.exe runtime config to .NET 6.0 to support testing in .NET 6.0
 - Add work_dispatch element to allow this library to to built manually in Github.

## 1.0.0
- Upgrade to MyTrout.Pipelines.Steps.IO.Files v3.0.0
- Change the nuget PackageURL and RepositoryURL from Azure DevOps to GitHub.
- Change Build Status badge from Azure DevOps to GitHub Actions.
- Convert Azure DevOps build to Github Actions build yaml.
- Update the documentation on all AntivirusOptions classes to indicate they are designed for Windows.
- Update ExecuteCommandStepTests to work on both Windows and Linux.
- Provide updated documentation for executing ExecuteCommandStep on both Windows and Linux.

## 0.2.0-beta
- CHanging from SOURCE_FILE to TARGET_FILE as the input for the command.

## 0.1.1-beta
- Change Program to a static class and make it the Project Startup class.

## 0.1.0-beta
- Initial implementation of ExecuteFileCommandStep and options for major anti-virus providers.
- NOTE TO CALLERS: The Success Text is not correct for Avg or Norton Antivirus options.