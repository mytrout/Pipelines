# How did I repair DACPAC build failures that started happening in March 2022?

1. First identified that Github had changed the windows-latest tag to use windows-2022 which has significant build problems with .NET 6.0 in Github.
https://github.com/actions/virtual-environments/issues/4884

2. This Github issue lead me to the resolution of the CopyRefAssembly build issue.
https://github.com/dotnet/runtime/issues/42318

## Requirements
My requirements were to create a GitHub actions yaml file that would 
1. reorder the DACPAC build to avoid provisioning Azure resources, if it fails.
2. restore DACPAC build capabilities.

## Accomplishment #1 - Change the order of operations so DACPAC builds before Azure Login.
The new step ordering prevents Azure provisioning from happening if the DACPAC build fails.

## Accomplishment #2 - Determining why .NET 6.0 was not loading correctly on Windows 2022 servers.
See the first link above, so the first thing that was done to resolve was rollback to windows-2019 for the azure-pre-processing step on Pipelines.Steps.Data.

## Accomplishment #3 - Ubuntu DACPAC build still failed trying to look up the .NET 6.0 runtime.
Changed the DACPAC build to use net5.0 rather than netstandard2.1 which specifies the exact .NET version to use since .netstandard2.1 isn't really used with .NET 6.0 and above.

## Accomplishment #4 - Ubuntu DACPAC build failed when attempting to copy reference assemblies.
Add the element <ProduceReferenceAssembly>false</ProduceReferenceAssembly> as referenced in Link #2 above to prevent reference assemblies from being generated.
DACPAC now builds successfully.

## Accomplishment #5 - DACPAC location reference still refereneces netstandard2.0 instead of net5.0
Change the dacpac-package location in the sql-action@v1 step that applies the DACPAC.

SUCCESS!
