# Breaking Changes for 3.0.0

## Auto-registration of Step Dependencies

In the earlier version of the Pipelines library, a user would have build an IDictionary<string, TOptions> for multi-context steps and add it to the IServiceProvider.
For the Hosting library, this choice caused a number of gyrations by adding configurations to the Host.Properties while the PipelineBuilder was being built and then
an additional processing of moving those Host.Properties configurations into the IDictionary<string, TOptions> and injecting it at the time of Host.Build().
This implementation required the usage of the dynamic keyword and reflection to gain access to the appropriately configured TOptions classes.

The current implementation makes use of the IStepWithFactory and IStepWithInstance interfaces to eliminate the cases where reflection was required AND handles the 
IConfiguration and IServiceProvider that were previously handled by AddStepDependencyExtensions and HostBuilderExtensions.

CAVEAT: This approach incurs the additional overhead of requiring IConfiguration to be injected, even when it isn't used.


In the simplest case where every Step has some kind of configuration, the auto-registration of step dependencies causes this code:

```csharp

    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Hosting;
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
                                    .AddStepDependency<StepOptions1>()
                                    .AddStepDependency<StepOptions2>()
                                    .AddStepDependency<StepOptions3>()
                                    .UsePipeline(builder => 
                                    {
                                        builder.AddStep<Step1>()
                                                .AddStep<Step2>()
                                                .AddStep<Step3>();
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

to use this simplified syntax...


```csharp

    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Hosting;
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
                                    .UsePipeline(builder => 
                                    {
                                        builder.AddStep<Step1, Step1Options>()
                                                .AddStep<Step2, Step2Options>()
                                                .AddStep<Step3, Step3Options>();
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

or this even more terse syntax if AbstractStepPipeline<TStep,TOptions> base class is used.

```csharp

    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Hosting;
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
                                    .UsePipeline(builder => 
                                    {
                                        builder.AddStep<Step1>()
                                                .AddStep<Step2>()
                                                .AddStep<Step3>();
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

From the Hosting library perspective, this changes the code for multi-context steps from:

```csharp

    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Hosting;
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
                                    .AddStepDependency<StepOptions1>("context-1")
                                    .AddStepDependency<StepOptions1>("context-2")
                                    .AddStepDependency<StepOptions1>("context-3")
                                    .UsePipeline(builder => 
                                    {
                                        builder.AddStep<Step1>("context-1")
                                                .AddStep<Step1>("context-2")
                                                .AddStep<Step1>("context-3");
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

to reduce the configuration lines to the following:

```csharp

    using MyTrout.Pipelines;
    using MyTrout.Pipelines.Hosting;
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
                                       .UsePipeline(builder => 
                                    {
                                        builder.AddStep<Step1, Step1Options>("context-1")
                                                .AddStep<Step1, Step1Options>("context-2")
                                                .AddStep<Step1, Step1Options>("context-3");
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