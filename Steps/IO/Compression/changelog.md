## 1.0.0
- Upgrade to MyTrout.Pipelines.Steps v1.0.0 (including MyTrout.Pipelines v1.1.0)
- Generate and publish to Azure DevOps Artifacts the snupkg file for symbols.
- Implement nullable reference types.
- Suppress SonarQube S5042 security warnings as they are false positives - 'this may be a problem' rather than 'this *IS* a problem'.

## 0.2.0-beta
- Correct documentation on CompressionConstants and PipelineContextValidationExtensions.
- Adding unit testing to MyTrout.Pipelines.Steps.IO.Compression classes.
- Change from MyTrout.Pipelines v0.27.0-beta to MyTrout.Pipelines.Steps v0.1.0-beta.

## 0.1.2-beta
- Correct minor SonarQube issues on RemoveZipArchiveEntryStep.

## 0.1.1-beta
- Correct minor SonarQube issues on CreateNewArchiveStep, OpenExistingArchiveFromStreamStep, and UpdateZipArchiveEntryStep.

## 0.1.0-beta
- Initial implementation of AddZipArchiveEntryStep, CreateNewArchiveStep, OpenExistingArchiveFromStreamStep, ReadZipArchiveEntriesFromZipArchiveStep, RemoveZipArchiveEntryStep, and UpdateZipArchiveEntryStep.