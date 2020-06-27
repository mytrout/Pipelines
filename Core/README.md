MyTrout.Pipelines

# Introduction 
These tests are unit tests necessary to test the core components of the MyTrout.Pipelines library.

# Getting Started
1.	Installation process (Local)
    1. Clone the software from the Pipelines repository.
    2. Build the software in Visual Studio 2019 to pull down all of the dependencies from nuget.org.
    3. In Visual Studio, run all tests.  All of the should pass.
    4. If you have Visual Studio Enterprise 2019, analyze the code coverage; it should be 100%.


2.	Installation process (Azure DevOps)
    1. Login to the SonarQube instance and generate a SonarQube token with the user account you want to use for running analysis.
    2. In Organization Settings, Add a Service Connection for SonarQube and enter the token.
    3. Make sure you check the 'Grant access permission to all pipelines' checkbox or configure appropriate security to this connection.
    4. Open a New Pipeline and reference the azure-pipelines.yml file in the /Core directory.


2.	Software dependencies
    1. Microsoft.Extensions.Configuration.Abstractions 3.1.5
    2. Microsoft.Extensions.Logging.Abstractions 3.1.5
    3. MyTrout.Pipelines 0.22.0-beta


3.	Latest releases

| Version    | Release Date | Details                                    |
| 0.25.0-beta | 27 JUNE 2020 | Adding the MoveOutputStreamToInputStreamStep to support capabilities need for testing MyTrout.Pipelines.Steps.Cryptography.  |

4.	API references
TODO: Future documentation will contain API references.

# Build and Test
An azure-pipelines.yml exists in the /Core directory.  It is usable with any Azure DevOps instance.


# Contribute
No contributions are being accepted at this time.