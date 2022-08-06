# Breaking Changes for 4.0.0
LTS stands for Long-Term Support, or even numbered releases of .NET.

## Remove support for .NET 5.0 and on-going understanding of versioning with new .NET versions.
The removal of support for .NET 5.0 coincides with the end-of-life happening on 10 MAY 2022.

As each .NET version reaches end-of-life, the project will release a major version release to remove support for that version of .NET.
At most, three .NET versions will be supported simultaneously.

.NET 8.0 will release in NOVEMBER 2023.
The libraries should be upgraded with a MINOR Release to include .NET 8.0 in NOVEMBER/DECEMBER 2023.
This Minor Release will support .NET 6.0, .NET 7.0, and .NET 8.0

.NET 7.0 will end-of-life in MAY 2024.
The libraries should be upgraded with a MINOR Release to remove .NET 7.0 in MAY 2024.
This Minor Release will support .NET 6.0 and .NET 8.0.

.NET 6.0 will end-of-life in NOVEMBER 2024
.NET 9.0 will release in NOVEMBER 2024.
The libraries will be upgraded with a MAJOR release to remove .NET 6.0 and add .NET 9.x in NOVEMBER/DECEMBER 2024.
This MAJOR release will support .NET 8.0 and .NET 9.0

.NET 10 will release in NOVEMBER 2025.
The libraries should be upgraded with a MINOR Release to include .NET 10.0 in NOVEMBER/DECEMBER 2025.
This Minor Release will support .NET 8.0, .NET 9.0, and .NET 10.0


## Namespace Changes to enable StepActivator Pipeline Build-time alterations
IStepActivator.ParameterCreators property was added to allow developers to alter the Parameter Creation functionality at Pipeline Build time.

Because this property was a listing of ParameterCreatorDelegates, the IStepActivator interface defined in the MyTrout.Pipelines namespace could not rely 
on a delegate definition that was located in deeper in the namespace hierarchy at MyTrout.Pipelines.Steps.
Ergo, ParameterCreationDelegate moved to the MyTrout.Pipelines namespace.

ParameterCreationDelegate also relied on the class ParameterCreationResult, so that class moved with ParameterCreationDelegate to the MyTrout.Pipelines namespace.

The logger parameter of ParameterCreationDelegate was defined in terms of ILogger<StepActivator>.  StepActivator was located in the MyTrout.Pipelines.Core which again
required a change. ParameterCreationDelegate's logger parameter was altered from ILogger<StepActivator> to ILogger<IStepActivator>.
  
Once the ParameterCreationDelegate's parameters were changed, all of the currently implemented methods that matched the ParameterCreationDelegates parameter list
were changed:
  StepActivator.CreateParameterFromConfiguration
  StepActivator.CreateParameterForNextPipelineRequest
  StepActivator.CreateParameterFromStepFactory
  StepActivator.CreateParameterFromStepInstance
  
## Dependency Injection into ~Options class.
  All of the Namespace Changes and StepActivator changes allowed for fully-constructed services to be injected into the ~Options classes using the [FromServicesAttribute].
  
  Static Analysis tools such as SonarQube will flag classes with more than 7 parameters.
  Using the sub-classes provided by Pipelines, only 4-5 interfaces can be injected into the Step without triggering the issue.
  
  Use the [FromServices] attribute on the property and it will be injected from the DI stack used by the Generic Host.
    
  ### sample C# code

```csharp
  
    namespace MyTrout.Pipeline.Steps.Options
    {
        public class SampleOptions
        {
            [FromServices]
            public ISomeService SomeSevice { get; init; }
        }
    }
  ```
  
  RECOMMENDATION: ALWAYS USE THE [FromServices] attribute for DI-constructed dependencies to allow your Step constructor tests to be standardized across all Steps.
