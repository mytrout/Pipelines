# MyTrout.Pipelines.Steps.Data Change Log

## 0.2.0-beta
- Clean up data from SupplementContextWithDatabaseRecordStepTests to ensure subsequent runs on the same database will work.
- Initial commit of SaveContextToDatabaseStep.
- Reduce the Cognitive Complexity on SupplementContextWithDatabaseRecordStep.InvokeAsync method to 15 or below.

## 0.1.0-beta
- Initial commit of SupplementContextWithDatabaseRecordStep.
- Create Azure ARM Template for SQL Server deployment for integration testing.
- Update azure-pipelines.yml to use appropriate ARM templates.
- Create tests/MyTrout.Pipelines.Steps.Data.Database project to allow creation of DACPAC for use with integration testing database.
- Nullable reference types will not be implenented due to warning suppression noise.
- Add Resources to enable localization and suppress analyzer warnings.
