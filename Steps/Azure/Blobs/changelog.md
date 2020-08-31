# MyTrout.Pipelines.Steps.Azure.Blobs Change Log

## 0.1.1-beta
- Correct Project Name and Project Key for SonarQube analysis.
- Delay DeleteBlobStep test execution by one second to prevent failures due to timing issues in Azure.
- This version will not be published to the public.

## 0.1.0-beta
- Initial implementation of DeleteBlobStep, ReadStreamFromBlobStorageStep and WriteStreamToBlobStorageStep.
- Create Azure ARM Template for Azure Blob Storage deployment for integration testing.
- Update azure-pipelines.yml to use appropriate ARM templates.
- Nullable reference types will not be implenented due to warning suppression noise.