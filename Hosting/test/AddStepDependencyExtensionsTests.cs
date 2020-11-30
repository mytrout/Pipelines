// <copyright file="AddStepDependencyExtensionsTests.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2019-2020 Chris Trout
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>

namespace MyTrout.Pipelines.Hosting.Tests
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines.Core;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class AddStepDependencyExtensionsTests
    {
        [TestMethod]
        public async Task Returns_Correct_Message_From_PipelineBuilder_When_Step_Dependency_With_Configuration_Is_Provided()
        {
            // arrange
            var host = Host.CreateDefaultBuilder()
                                .ConfigureAppConfiguration(config =>
                                {
                                    config.AddJsonFile("config-test-root.json", optional: false, reloadOnChange: false);
                                })
                                .AddStepDependency<TestingOptions>()
                                .UsePipeline(builder =>
                                {
                                    builder.AddStep<TestingStep5>();
                                })
                                .Build();

            string expectedMessage = "Moe ";

            // act
            await host.StartAsync().ConfigureAwait(false);

            var context = host.Services.GetService<PipelineContext>();

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            Assert.AreEqual(expectedMessage, context.Items["MESSAGE"]);
        }

        [TestMethod]
        public async Task Returns_Correct_Message_From_PipelineBuilder_When_Step_Dependency_With_Configuration_ConfigKeys_Is_Provided()
        {
            // arrange
            var host = Host.CreateDefaultBuilder()
                                .ConfigureAppConfiguration(config =>
                                {
                                    config.AddJsonFile("config-test-configKeys.json", optional: false, reloadOnChange: false);
                                })
                                .AddStepDependency<TestingOptions>(new string[1] { "context-8" })
                                .UsePipeline(builder =>
                                {
                                    builder.AddStep<TestingStep5>();
                                })
                                .Build();

            string expectedMessage = "Curly ";

            // act
            await host.StartAsync().ConfigureAwait(false);

            var context = host.Services.GetService<PipelineContext>();

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            Assert.AreEqual(expectedMessage, context.Items["MESSAGE"]);
        }

        [TestMethod]
        public async Task Returns_Correct_Message_From_PipelineBuilder_When_Step_Dependency_With_Configuration_Type_Is_Provided()
        {
            // arrange
            var host = Host.CreateDefaultBuilder()
                                .ConfigureAppConfiguration(config =>
                                {
                                    config.AddJsonFile("config-test-type.json", optional: false, reloadOnChange: false);
                                })
                                .AddStepDependency<TestingOptions>()
                                .UsePipeline(builder =>
                                {
                                    builder.AddStep<TestingStep5>();
                                })
                                .Build();

            string expectedMessage = "Larry ";

            // act
            await host.StartAsync().ConfigureAwait(false);

            var context = host.Services.GetService<PipelineContext>();

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            Assert.AreEqual(expectedMessage, context.Items["MESSAGE"]);
        }

        [TestMethod]
        public async Task Returns_Correct_Message_From_PipelineBuilder_When_Step_Dependency_With_Context_And_Configuration_Are_Provided()
        {
            // arrange
            var host = Host.CreateDefaultBuilder()
                                .ConfigureAppConfiguration(config =>
                                {
                                    config.AddJsonFile("config-test-context.json", optional: false, reloadOnChange: false);
                                })
                                .AddStepDependency<TestingOptions>("context-1")
                                .AddStepDependency<TestingOptions>("context-2")
                                .AddStepDependency<TestingOptions>("context-3")
                                .AddStepDependency<TestingOptions>("context-4")
                                .UsePipeline(builder =>
                                {
                                    builder.AddStep<TestingStep5>("context-1")
                                            .AddStep<TestingStep5>("context-2")
                                            .AddStep<TestingStep5>("context-3")
                                            .AddStep<TestingStep5>("context-4");
                                })
                                .Build();

            string expectedMessage = "Moe Larry & Curly ";

            // act
            await host.StartAsync().ConfigureAwait(false);

            var context = host.Services.GetService<PipelineContext>();

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            Assert.AreEqual(expectedMessage, context.Items["MESSAGE"]);
        }

        [TestMethod]
        public async Task Returns_Correct_Message_From_PipelineBuilder_When_Step_Dependency_With_Context_And_Configuration_ConfigKeys_Are_Provided()
        {
            // arrange
            var host = Host.CreateDefaultBuilder()
                                .ConfigureAppConfiguration(config =>
                                {
                                    config.AddJsonFile("config-test-configkeys.json", optional: false, reloadOnChange: false);
                                })
                                .AddStepDependency<TestingOptions>("context-1", new string[1] { "context-5" })
                                .AddStepDependency<TestingOptions>("context-2", new string[1] { "context-6" })
                                .AddStepDependency<TestingOptions>("context-3", new string[1] { "context-7" })
                                .AddStepDependency<TestingOptions>("context-4", new string[1] { "context-8" })
                                .UsePipeline(builder =>
                                {
                                    builder.AddStep<TestingStep5>("context-1")
                                            .AddStep<TestingStep5>("context-2")
                                            .AddStep<TestingStep5>("context-3")
                                            .AddStep<TestingStep5>("context-4");
                                })
                                .Build();

            string expectedMessage = "Moe Larry & Curly ";

            // act
            await host.StartAsync().ConfigureAwait(false);

            var context = host.Services.GetService<PipelineContext>();

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            Assert.AreEqual(expectedMessage, context.Items["MESSAGE"]);
        }

        [TestMethod]
        public async Task Returns_Correct_Message_From_PipelineBuilder_When_Step_Dependency_With_Context_And_Configuration_Root_Are_Provided()
        {
            // arrange
            var host = Host.CreateDefaultBuilder()
                                .ConfigureAppConfiguration(config =>
                                {
                                    config.AddJsonFile("config-test-root.json", optional: false, reloadOnChange: false);
                                })
                                .AddStepDependency<TestingOptions>("context-1")
                                .AddStepDependency<TestingOptions>("context-2")
                                .AddStepDependency<TestingOptions>("context-3")
                                .AddStepDependency<TestingOptions>("context-4")
                                .UsePipeline(builder =>
                                {
                                    builder.AddStep<TestingStep5>("context-1")
                                            .AddStep<TestingStep5>("context-2")
                                            .AddStep<TestingStep5>("context-3")
                                            .AddStep<TestingStep5>("context-4");
                                })
                                .Build();

            string expectedMessage = "Moe Larry & Curly ";

            // act
            await host.StartAsync().ConfigureAwait(false);

            var context = host.Services.GetService<PipelineContext>();

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            Assert.AreEqual(expectedMessage, context.Items["MESSAGE"]);
        }

        [TestMethod]
        public async Task Returns_Correct_Message_From_PipelineBuilder_When_Step_Dependency_With_Context_And_Configuration_Type_Are_Provided()
        {
            // arrange
            var host = Host.CreateDefaultBuilder()
                                .ConfigureAppConfiguration(config =>
                                {
                                    config.AddJsonFile("config-test-type.json", optional: false, reloadOnChange: false);
                                })
                                .AddStepDependency<TestingOptions>("context-1")
                                .AddStepDependency<TestingOptions>("context-2")
                                .AddStepDependency<TestingOptions>("context-3")
                                .AddStepDependency<TestingOptions>("context-4")
                                .UsePipeline(builder =>
                                {
                                    builder.AddStep<TestingStep5>("context-1")
                                            .AddStep<TestingStep5>("context-2")
                                            .AddStep<TestingStep5>("context-3")
                                            .AddStep<TestingStep5>("context-4");
                                })
                                .Build();

            string expectedMessage = "Moe Larry & Curly ";

            // act
            await host.StartAsync().ConfigureAwait(false);

            var context = host.Services.GetService<PipelineContext>();

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            Assert.AreEqual(expectedMessage, context.Items["MESSAGE"]);
        }

        [TestMethod]
        public async Task Returns_Correct_Message_From_PipelineBuilder_When_Step_Dependency_With_Context_And_Function_Are_Provided()
        {
            // arrange
            var host = Host.CreateDefaultBuilder()
                                .AddStepDependency<TestingOptions>("context-1", (IServiceProvider services) => { return new TestingOptions() { Key = "Moe" }; })
                                .AddStepDependency<TestingOptions>("context-2", (IServiceProvider services) => { return new TestingOptions() { Key = "Larry" };  })
                                .AddStepDependency<TestingOptions>("context-3", (IServiceProvider services) => { return new TestingOptions() { Key = "&" };  })
                                .AddStepDependency<TestingOptions>("context-4", (IServiceProvider services) => { return new TestingOptions() { Key = "Curly" };  })
                                .UsePipeline(builder =>
                                {
                                    builder.AddStep<TestingStep5>("context-1")
                                            .AddStep<TestingStep5>("context-2")
                                            .AddStep<TestingStep5>("context-3")
                                            .AddStep<TestingStep5>("context-4");
                                })
                                .Build();

            string expectedMessage = "Moe Larry & Curly ";

            // act
            await host.StartAsync().ConfigureAwait(false);

            var context = host.Services.GetService<PipelineContext>();

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            Assert.AreEqual(expectedMessage, context.Items["MESSAGE"]);
        }

        [TestMethod]
        public async Task Returns_Correct_Message_From_PipelineBuilder_When_Step_Dependency_With_Context_And_Instance_Are_Provided()
        {
            // arrange
            var host = Host.CreateDefaultBuilder()
                                .AddStepDependency<TestingOptions>("context-1", new TestingOptions() { Key = "Moe" })
                                .AddStepDependency<TestingOptions>("context-2", new TestingOptions() { Key = "Larry" })
                                .AddStepDependency<TestingOptions>("context-3", new TestingOptions() { Key = "&" })
                                .AddStepDependency<TestingOptions>("context-4", new TestingOptions() { Key = "Curly" })
                                .UsePipeline(builder =>
                                {
                                    builder.AddStep<TestingStep5>("context-1")
                                            .AddStep<TestingStep5>("context-2")
                                            .AddStep<TestingStep5>("context-3")
                                            .AddStep<TestingStep5>("context-4");
                                })
                                .Build();

            string expectedMessage = "Moe Larry & Curly ";

            // act
            await host.StartAsync().ConfigureAwait(false);

            var context = host.Services.GetService<PipelineContext>();

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            Assert.AreEqual(expectedMessage, context.Items["MESSAGE"]);
        }

        [TestMethod]
        public async Task Returns_Correct_Message_From_PipelineBuilder_When_Step_Dependency_Function_Is_Provided()
        {
            // arrange
            var host = Host.CreateDefaultBuilder()
                                .AddStepDependency<TestingOptions>((IServiceProvider services) => { return new TestingOptions() { Key = "&" }; })
                                .UsePipeline(builder =>
                                {
                                    builder.AddStep<TestingStep5>();
                                })
                                .Build();

            string expectedMessage = "& ";

            // act
            await host.StartAsync().ConfigureAwait(false);

            var context = host.Services.GetService<PipelineContext>();

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            Assert.AreEqual(expectedMessage, context.Items["MESSAGE"]);
        }

        [TestMethod]
        public async Task Returns_Correct_Message_From_PipelineBuilder_When_Step_Dependency_Instance_Is_Provided()
        {
            // arrange
            var host = Host.CreateDefaultBuilder()
                                .AddStepDependency<TestingOptions>(new TestingOptions() { Key = "Shemp" })
                                .UsePipeline(builder =>
                                {
                                    builder.AddStep<TestingStep5>();
                                })
                                .Build();

            string expectedMessage = "Shemp ";

            // act
            await host.StartAsync().ConfigureAwait(false);

            var context = host.Services.GetService<PipelineContext>();

            // assert
            if (context.Errors.Any())
            {
                throw context.Errors[0];
            }

            Assert.AreEqual(expectedMessage, context.Items["MESSAGE"]);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_PipelineBuilder_When_HostBuilder_Properties_Contains_Invalid_Type_For_AddStep_Dependency_key()
        {
            // arrange
            var hostBuilder = Host.CreateDefaultBuilder();

            string keyName = $"{AddStepDependencyExtensions.STEP_CONFIG_CONTEXT}-{typeof(TestingOptions).FullName}";
            hostBuilder.Properties[keyName] = new object();

            string expectedMessage = Resources.CONTEXT_WRONG_TYPE(CultureInfo.CurrentCulture, typeof(TestingOptions).FullName);
            
            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() => 
                            hostBuilder.AddStepDependency<TestingOptions>("context-1", new TestingOptions() { Key = "Shemp" }));
            
            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_PipelineBuilder_When_HostBuilder_Properties_Contains_Null_Value_For_Type_In_The_Dictionary()
        {
            // arrange
            var hostBuilder = Host.CreateDefaultBuilder();

            string keyName = $"{AddStepDependencyExtensions.STEP_CONFIG_CONTEXT}-{typeof(TestingOptions).FullName}";
            var dictionary = new Dictionary<string, Dictionary<string, Func<IServiceProvider, TestingOptions>>>()
                                {
                                    { string.Empty, new Dictionary<string, Func<IServiceProvider, TestingOptions>>() }
                                };

            hostBuilder.Properties[keyName] = dictionary;

            string expectedMessage = Resources.CONTEXT_IS_NOT_CORRECT(CultureInfo.CurrentCulture, keyName);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() =>
                                                    hostBuilder.ConfigureServices(services =>
                                                    {
                                                        services.AddSingleton(new TestingOptions() { Key = "Moe" });
                                                    })
                                                    .UsePipeline(builder =>
                                                    {
                                                        builder.AddStep<TestingStep5>();
                                                    }));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_PipelineBuilder_When_HostBuilder_Properties_Contains_Non_Enumerable_Object_For_Type_In_The_Dictionary()
        {
            // arrange
            var hostBuilder = Host.CreateDefaultBuilder();

            string keyName = $"{AddStepDependencyExtensions.STEP_CONFIG_CONTEXT}-{typeof(FailedTestingOptions).FullName}";
            var dictionary = new FailedTestingOptions();

            hostBuilder.Properties[keyName] = dictionary;

            string expectedMessage = Resources.CONTEXT_IS_NOT_CORRECT(CultureInfo.CurrentCulture, keyName);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() =>
                                                    hostBuilder.ConfigureServices(services =>
                                                    {
                                                        services.AddSingleton(new FailedTestingOptions());
                                                    })
                                                    .UsePipeline(builder =>
                                                    {
                                                        builder.AddStep<TestingStep5>();
                                                    }));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [TestMethod]
        public void Throws_InvalidOperationException_From_PipelineBuilder_When_HostBuilder_Properties_Contains_Type_With_No_Keys_Property_In_The_Dictionary()
        {
            // arrange
            var hostBuilder = Host.CreateDefaultBuilder();

            string keyName = $"{AddStepDependencyExtensions.STEP_CONFIG_CONTEXT}-{typeof(FailedTestingOptions).FullName}";
            var dictionary = new TestingOptions();

            hostBuilder.Properties[keyName] = dictionary;

            string expectedMessage = Resources.CONTEXT_IS_NOT_CORRECT(CultureInfo.CurrentCulture, keyName);

            // act
            var result = Assert.ThrowsException<InvalidOperationException>(() =>
                                                    hostBuilder.ConfigureServices(services =>
                                                    {
                                                        services.AddSingleton(new TestingOptions());
                                                    })
                                                    .UsePipeline(builder =>
                                                    {
                                                        builder.AddStep<TestingStep5>();
                                                    }));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.Message);
        }
    }
}
