# MyTrout.Pipelines.Steps.Cryptography

MyTrout.Pipelines.Steps.Cryptography provides Pipeline steps to encrypt, hash, and decrypt streams.

[![Build Status](https://dev.azure.com/mytrout/Pipelines/_apis/build/status/mytrout.Pipelines.Steps.Cryptography?branchName=master)](https://dev.azure.com/mytrout/Pipelines/_build/latest?definitionId=17&branchName=master)
[![nuget](https://buildstats.info/nuget/MyTrout.Pipelines.Steps.Cryptography?includePreReleases=true)](https://www.nuget.org/packages/MyTrout.Pipelines.Steps.Cryptography/)
[![GitHub stars](https://img.shields.io/github/stars/mytrout/Pipelines.svg)](https://github.com/mytrout/Pipelines/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/mytrout/Pipelines.svg)](https://github.com/mytrout/Pipelines/network)
[![License: MIT](https://img.shields.io/github/license/mytrout/Pipelines.svg)](https://licenses.nuget.org/MIT)

MyTrout.Pipelines.Steps.Cryptography targets [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)

For more details on Pipelines, see [Pipelines.Core](../../Core/README.md)

For more details on Pipelines.Hosting, see [Pipelines.Hosting](../../Hosting/README.md)

For a list of available steps, see [Available Steps](../)

## Installing via NuGet

    Install-Package MyTrout.Pipelines.Steps.Cryptography

## Software dependencies

    1. MyTrout.Pipelines.Steps 2.0.*

All software dependencies listed above use the [MIT License](https://licenses.nuget.org/MIT).

## How do I use the steps in this library [DecryptStreamWithAes256Step](./src/DecryptStreamWithAes256Step.cs) ?

### sample C# code

```csharp

    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Hosting;
    using MyTrout.Pipelines.Steps.Cryptography
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    namespace MyTrout.Pipeline.Hosting.Samples
    {
        public class Program
        {
            public static async Task Main(string[] args)
            {

                var host = Host.CreateDefaultBuilder(args)
                                    .AddStepDependency<DecryptStreamWithAes256Options>()
                                    .UsePipeline(builder => 
                                    {
                                        builder
                                            .AddStep<StepThatLoadsStream>()
                                            .AddStep<DecryptStreamWithAes256Step>()
                                            .AddStep<StepThatProcessesTheStream>();
                                    })
                                    .Build();

                //
                // IMPORTANT NOTE FOR DEVELOPERS:
                // 
                // Use StartAsync() to allow the caller to review the PipelineContext after execution.
                //
                await host.StartAsync().ConfigureAwait(false);

                var context = host.Services.GetService<PipelineContext>();

                if(context.Errors.Any())
                {
                    // TODO: Errors have already been logged, do any special error processing here.
                }

                return 0;
            }
        }
    }
}

```
### sample appsettings.json file

```json
{
    "DecryptionInitializationVector": "user supplied initialization vector",
    "DecryptionKey: "user supplied decryption key"
}
```

## How do I use Pipelines.Hosting with different configurations for different instances of the same step.

Each decrypt step would use a different configuration to 

```csharp

    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Hosting;
    using MyTrout.Pipelines.Steps.Cryptography
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    namespace MyTrout.Pipeline.Hosting.Samples
    {
        public class Program
        {
            public static async Task Main(string[] args)
            {
                // IMPORTANT NOTE FOR DEVELOPERS !
                // 
                // Step Dependencies with context must be defined BEFORE UsePipelines() to load the dependencies correctly.
                //

                var host = Host.CreateDefaultBuilder(args)
                                    .AddStepDependency<DecryptStreamWithAes256Options>("context-A")
                                    .AddStepDependency<DecryptStreamWithAes256Options>("context-B")
                                    .UsePipeline(builder => 
                                    {
                                        builder
                                            .AddStep<LoadStreamToProcess>()
                                            .AddStep<DecryptStreamWithAes256Step>("context-A")
                                            .AddStep<TakeDataFromFirstStreamAndCreateANewStream>()
                                            .AddStep<DecryptStreamWithAes256Step>("context-B")
                                            .AddStep<ProcessSecondDecryptedStream>()
                                    })
                                    .Build();
                
                // IMPORTANT NOTE FOR DEVELOPERS !
                // 
                // Use StartAsync() to allow the caller to review the PipelineContext after execution.
                //

                await host.StartAsync().ConfigureAwait(false);

                var context = host.Services.GetService<PipelineContext>();

                if(context.Errors.Any())
                {
                    // TODO: Errors have already been logged, do any special error processing here.
                }

                return 0;
            }
        }
    }
}
```

### sample appsettings.json file

```json
{
    "context-A": {
        "DecryptionInitializationVector": "user supplied initialization vector #1",
        "DecryptionKey: "user supplied decryption key #1"
    },
    "context-B": {
        "DecryptionInitializationVector": "user supplied initialization vector #2",
        "DecryptionKey: "user supplied decryption key #2"
    }
}
```

## How do I provide secrets such as DecryptionInitializationVector and DecryptionKey to the ~Options classes.

All of the ~Options classes now use the Retrieve~ methods for secrets to allow callers to reconfigure how secrets are stored.

1. For simple non-secure implementations, a configuration value named the same as the ~Options property can be defined in any IConfigurationProvider and the default Retrieve~ method will be used.
2. For a more secure implementation, encrypt the configuration value and override the Retrieve~ method with your implementation for decrypting the value; add your new ~Options class to your DI Injection using the base ~Options class type.
3. For a more secure implementation, override the Retrieve~ methods and retrieve your secrets from a location such as Azure Key Vault, Hashicorp Vault, or AWS Secrets Manager; and add your new ~Options class to your DI injection using the base ~Options class type.
4. For a more secure implementation, create an factory method used by the DI framework that only loads the secrets values when the ~Options class is needed; add this factory method under the base ~Options class type

DISCLAIMER: ALL SECURITY SUGGESTIONS IN THIS SECTIONS ARE COVERED UNDER THE SAME 'USE AT YOUR OWN RISK' LICENSE AS THE SOFTWARE.
