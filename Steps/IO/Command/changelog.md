# MyTrout.Pipelines.Steps.IO.Command

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