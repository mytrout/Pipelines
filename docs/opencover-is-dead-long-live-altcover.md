# OpenCover is DEAD!  Long Live AltCover!

As I was research a bug with Pipelines.Steps.Data where this line cause 2 of 4 code conditions to be covered:

```csharp
DbConnection? connection = null;
```

I noticed that the OpenCover Github project had been archived and was no longer accepting issues.

https://github.com/OpenCover/opencover#readme

Based on the feedback from the repository owner, I have started this page to capture the refactoring of all test coverage reporting to AltCover
https://github.com/SteveGilham/altcover.

## Requirements
My requirements are to 
1. convert the existing solutions to use AltCover.
2. refactor the existing builds to use AltCover.
3. run on Linux.
4. collect code coverage.
5. publish code coverage results to SonarCloud.
6. determine if code coverage results are more accurate using AltCover.
7. open a bug if the code coverage results have the same issue as OpenCover did.

## Challenge #1 - TODO: Alter the Pipelines.Steps.Data solution and build to make it work.
TO BE DETERMINED!
