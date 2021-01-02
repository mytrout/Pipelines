# Contributing to the MyTrout.Pipelines projects.

## Setting up a Development environment
The easiest way is to download Visual Studio 2019 Community Edition and install all patches.
The project is always upgrading to the latest long-term release version of .NET and will support all .NET versions for their lifetime, starting with .NET 5.0.

## Getting the code
1. Navigate to the mytrout/Pipelines GitHub repository and press "Fork" in the upper right hand corner.

2. Clone that fork to your local machine.

3. Checkout the master branch.
```
$ git checkout master
$ git pull --rebase.
```

## New Step Project Structure

### Where does a new Step fit in the overall repository?
The Steps part of the repository is structured by Cloud Provider.
Typically, developers who are using one Clound Provider are more likely to use other features of that Cloud Provider rather than a mix-and-match implementation.

By Functionality links (such as Storage, Messaging, Database
We will provide multiple linked lists at the ./Steps/README.md to ensure that the "By Functionality" perspectives are able to self-service.

  By Cloud Provider Example
    ./Steps/Amazon/SQS
    ./Steps/Amazon/S3
    ./Steps/Azure/ServiceBus
    ./Steps/Azure/Blobs
    ./Steps/Google/PubSub
    ./Steps/Google/CloudStorage


<MODIFIERS> represents systems and descriptors for your library mentioned above.

### How do I structure a new project?
  ./Steps/<MODIFIERS>/src
  ./Steps/<MODIFIERS>/src/GlobalSuppressions.cs
  ./Steps/<MODIFIERS>/src/<root namespace of project>.csproj
  ./Steps/<MODIFIERS>/src/Resources.cs
  ./Steps/<MODIFIERS>/src/Resources.resx
  ./Steps/<MODIFIERS>/src/Resources.tt
  ./Steps/<MODIFIERS>/test
  ./Steps/<MODIFIERS>/test/<root namespace of project>.Tests.csproj
  ./Steps/<MODIFIERS>/azure-pipelines.yml - builds the new library
  ./Steps/<MODIFIERS>/CHANGELOG.md - Change Log for the new library.
  ./Steps/<MODIFIERS>/LICENSE - MIT License documentation with your name and year delivered.
  ./Steps/<MODIFIERS>/README.md - Read Me documentation describing how to use your Step.
  ./Steps/<MODIFIERS>/stylecop.json - stylecop file with 
  ./Steps/<MODIFIERS>/<root namespace of project>.sln


### Details for New Projects..

1. For new projects, please use the stylecop.json as a starting point for rules.

```json
{
  "$schema": "https://raw.githubusercontent.com/DotNetAnalyzers/StyleCopAnalyzers/master/StyleCop.Analyzers/StyleCop.Analyzers/Settings/stylecop.schema.json",
  "settings": {
    "documentationRules": {
      "companyName": "<INSERT YOUR NAME HERE>",
      "copyrightText": "MIT License\r\n\r\nCopyright(c) <YEAR DELIVERED> <INSERT YOUR NAME HERE>\r\n\r\nPermission is hereby granted, free of charge, to any person obtaining a copy\r\nof this software and associated documentation files (the \"Software\"), to deal\r\nin the Software without restriction, including without limitation the rights\r\nto use, copy, modify, merge, publish, distribute, sublicense, and/or sell\r\ncopies of the Software, and to permit persons to whom the Software is\r\nfurnished to do so, subject to the following conditions:\r\n\r\nThe above copyright notice and this permission notice shall be included in all\r\ncopies or substantial portions of the Software.\r\n\r\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR\r\nIMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,\r\nFITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE\r\nAUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER\r\nLIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,\r\nOUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE\r\nSOFTWARE."
    },
    "orderingRules": {
      "systemUsingDirectivesFirst": false
    }
  }
}
```json
2. Change the <INSERT YOUR NAME HERE> to your name.
3. Change the <YEAR DELIVERED> to the current year.
4. If the new project is a newly implemented Step, it should be under the Steps directory.

### Write your code
Once you have established a fork of the repository on your machine, add/change code to your heart's content!

The code base is divided into two sets of rules, one for projects where implementation work is done to be used by consumers of the libraries and one for test projects.

The following rules apply to implementation projects:
1. Constants should be all upper case with underscores between words.  
  a. SA1310 should be suppressed in a Global Suppressions file with the following explanation "Constant names should contain underscores."

2. Use the following analyzers to ensure a consistent code base.
  a. Enable .NET Analyzers in the csproj file for .NET 5.0 projects.
  b. SonarQube - used to catch some additional issues that other analyzers may miss.
  c. StyleCop - used to ensure that code look-and-feel remains consistent across all projects.
  d. Microsoft.VisualStudio.Threading.Analyzers - used to ensure that static Threading issues are caught during development.

3. These rules can be suppressed permanently in the csproj file
  a. SA1413 - UseTrailingCommasInMultiLineInitializers 
     RATIONALE: The confusion about whether or not something has been deleted (and subsequent research) outweighs the ability to reorder items and changes on unrelated lines rationale provided by the implementors.

  b. IDE0063 - Use simple 'using' statement.  
     RATIONALE: Having deterministic boundaries for using statements ensures that the developer knows when a variable is going out-of-scope instead of non-deterministic behavior that could change from version to version of the compiler.

4. All new or updated reference and value types must have 100% Code Coverage by unit and integration tests that assert correct execution within the type.

5. All unit and integration tests must follow include the Arrange..Act..Assert pattern for happy path testing.

6. All unit and integration tests must use Assert.ThrowsException<>() for exception path testing.  
   RATIONALE: The team is aware that Assert.ThrowsException<> violates the separation between Act and Assert.  
	      Using an Assert.IsNotNull() as the first Assert takes care of some of the issues with this approach.
              We aren't 100% perfect, but we try to be as close as we can be without re-implementing functionality that we don't want to support.

7. Any class implementation that has only automatic properties with or without initialization and a default constructor generated by the compiler can be marked with [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage].

8. Any future updates to classes marked with [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] may require unit or integration tests to be written, if the above items change.