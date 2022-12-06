# MyTrout.Pipelines.Steps.Serialization.Json Change Log

## 1.2.0
- Change default branch from master to main.
- Upgrade C# Language Version from 9.0 to 10.0 across all src projects.
- Standardize the first &lt;Property Group&gt; section in the src csproj files across all src projects.
- Standardize the NoWarn options within the src csproj files across all src projects.
- Standardize the Neutral Language options to en-US across all src projects.
- Standardize the Copyright to include 2022 across all src projects.
- Standardize all .editorconfig file inclusion across all src projects.
- Standardize inclusion of README.md file across all src projects.
- Force upgrade to MyTrout.Pipelines.Steps.Serialization.Core v1.1.0 minimum.

## 1.1.1
- Correct 1 SonarQube issue.

## 1.1.0
- Mark all existing Steps and Options with the Obsolete attribute to encourage usage of the new functionality.
- Add DeserializeJsonObjectFromStreamOptions and SerializeJsonObjectToStreamOptions for use with functionality in Pipelines.Steps.Serialization.Core library.

## 1.0.0
- Initial implementation of DeserializeObjectFromStreamStep and SerializeObjectToStreamStep.
