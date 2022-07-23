# MyTrout.Pipelines.Steps.IO.Directories Change Log

## 2.0.0
### BREAKING CHANGES
- Change the name of EnumerateFilesInDirectoryStep.TargetBaseDirectoryPathContextName to EnumerateFilesInDirectoryStep.TargetDirectoryContextName.  Value remains the same.
- Change the value in EnumerateFilesInDirectoryStep.TargetFileContextName from the file name with extension to the full path and file name of the target file to conform with the usage of the context name in Steps.IO.Files.
### NON-BREAKING-CHANGES
- Add EnumerateFilesInDirectoryStep.TargetFileNameWithExtensionContextName to provide the same value that EnumerateFilesInDirectoryStep.TargetFileContextName provided in v1.0.0, which is file name with extension.
- Add EnumerateFilesInDirectoryStep.TargetFileNameWithoutExtensionContextName to provide the file name without extension.

## 1.0.0
- Initial implementation of ReadFileFromFileSystemStep and WriteFileToFileSystemStep based on Pipelines v4.0.0 guidelines.