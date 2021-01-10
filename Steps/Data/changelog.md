# MyTrout.Pipelines.Steps.Data Change Log

## 1.0.0
- Upgrade from .NET Standard 2.1 to .NET 5.0
- Upgrade to MyTrout.Pipelines.Steps 2.0.6
- Change from Dapper requirements to >= 2.0.78 and < 3.0.
- Add Nullable reference types back with appropriate suppressions for Assert~ methods.
- Refactor azure-pipelines.yml to use an Azure DevOps controlled version number.
- Update Resources.tt to use pragma to suppress warnings.
- Update Resources.tt to reflect the new copyright 2020-2021.
- Change copyright from 2020 to 2020-2021.
- Update README.md to provide actual documentation and bring it up-to-date.

 IMPORTANT NOTE: The MyTrout.Pipelines.Steps.Data.Database project was not upgraded to .NET 5.0 because I could not find a targeting pack for .NET 5.0.  It remains at .NET 4.8 as of this version.

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
