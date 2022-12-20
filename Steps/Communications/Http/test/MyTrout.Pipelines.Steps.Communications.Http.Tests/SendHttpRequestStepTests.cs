// <copyright file="SendHttpRequestStep.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2021-2022 Chris Trout
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

namespace MyTrout.Pipelines.Steps.Communications.Http.Tests
{
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines.Core;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class SendHttpRequestStepTests
    {
        public static readonly EnvironmentVariableTarget ENVIRONMENT_TARGET = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? EnvironmentVariableTarget.Machine : EnvironmentVariableTarget.Process;

        public static readonly string PIPELINE_TEST_GITHUB_API_TOKEN = Environment.GetEnvironmentVariable("PIPELINE_TEST_GITHUB_API_TOKEN", ENVIRONMENT_TARGET);

        public static readonly string PIPELINE_TEST_GITHUB_QUERY = Environment.GetEnvironmentVariable("PIPELINE_TEST_GITHUB_QUERY", ENVIRONMENT_TARGET);

        public static readonly string PIPELINE_TEST_GITHUB_QUERY_RESULT = Environment.GetEnvironmentVariable("PIPELINE_TEST_GITHUB_QUERY_RESULT", ENVIRONMENT_TARGET);

        [TestMethod]
        public void Constructs_SendHttpRequestStep_Successfully()
        {
            // arrange
            var logger = new Mock<ILogger<SendHttpRequestStep>>().Object;
            var options = new SendHttpRequestOptions();

            var next = new Mock<IPipelineRequest>().Object;

            // act
            using (var result = new SendHttpRequestStep(logger, options, next))
            {
                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(logger, result.Logger);
                Assert.AreEqual(next, result.Next);
                Assert.AreEqual(options, result.Options);
            }
        }


        [TestMethod]
        public async Task Returns_Failed_Response_From_Fake_HTTP_GET_Request_When_ReasonPhrase_Is_Null()
        {
            // arrange
            var logger = new Mock<ILogger<SendHttpRequestStep>>().Object;
            using (var httpClient = HttpClientFactory.Create(new ReplaceReasonPhraseHandler()))
            {
                var options = new SendHttpRequestOptions()
                {
                    HttpClient = httpClient,
                    HttpEndpoint = new Uri("https://swapi.dev/api/"),
                    HttpMethod = "GET"
                };

                string expectedResult = string.Empty;

                var mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(It.IsAny<PipelineContext>())).Callback((IPipelineContext context) =>
                {
                    using (var reader = new StreamReader(context.Items[PipelineContextConstants.OUTPUT_STREAM] as Stream))
                    {
                        string result = reader.ReadToEnd();

                        // assert
                        Assert.AreEqual((HttpStatusCode)999, context.Items[HttpCommunicationConstants.HTTP_STATUS_CODE], "HttpStatusCode should be 999.");
                        Assert.IsTrue(context.Items.ContainsKey(HttpCommunicationConstants.HTTP_IS_SUCCESSFUL_STATUS_CODE), "IsSuccessfulStatusCode should have a value in context.Items.");
                        Assert.IsFalse(((bool)context.Items[HttpCommunicationConstants.HTTP_IS_SUCCESSFUL_STATUS_CODE]), "IsSuccessfulHttpStatusCode should be false.");
                        Assert.AreEqual(string.Empty, context.Items[HttpCommunicationConstants.HTTP_REASON_PHRASE], "ReasonPhrase should match string.Empty.");
                        Assert.IsTrue(context.Items.ContainsKey(HttpCommunicationConstants.HTTP_RESPONSE_HEADERS), "ResponseHeaders should exists in context.Items.");
                        Assert.IsTrue(context.Items.ContainsKey(HttpCommunicationConstants.HTTP_RESPONSE_TRAILING_HEADERS), "TrailingHeaders should exist in context.Items");
                        Assert.AreEqual(expectedResult, result);
                    }
                });

                var next = mockNext.Object;

                var context = new PipelineContext();

                using (var step = new SendHttpRequestStep(logger, options, next))
                {
                    // act
                    await step.InvokeAsync(context);
                }

                // assert - Phase 2
                if (context.Errors.Count > 0)
                {
                    throw context.Errors[0];
                }
            }
        }

        [TestMethod]
        public async Task Returns_Successful_Response_From_HTTP_GET_Request()
        {
            // arrange
            var logger = new Mock<ILogger<SendHttpRequestStep>>().Object;
            using (var httpClient = new HttpClient())
            {
                var options = new SendHttpRequestOptions()
                {
                    HttpClient = httpClient,
                    HttpEndpoint = new Uri("https://swapi.dev/api/"),
                    HttpMethod = "GET"
                };

                string expectedResult = "{\"people\":\"https://swapi.dev/api/people/\",\"planets\":\"https://swapi.dev/api/planets/\",\"films\":\"https://swapi.dev/api/films/\",\"species\":\"https://swapi.dev/api/species/\",\"vehicles\":\"https://swapi.dev/api/vehicles/\",\"starships\":\"https://swapi.dev/api/starships/\"}";

                var mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(It.IsAny<PipelineContext>())).Callback((IPipelineContext context) =>
                {
                    using (var reader = new StreamReader(context.Items[PipelineContextConstants.OUTPUT_STREAM] as Stream))
                    {
                        string result = reader.ReadToEnd();

                        // assert
                        Assert.AreEqual(HttpStatusCode.OK, context.Items[HttpCommunicationConstants.HTTP_STATUS_CODE]);
                        Assert.IsTrue(context.Items.ContainsKey(HttpCommunicationConstants.HTTP_IS_SUCCESSFUL_STATUS_CODE) && ((bool)context.Items[HttpCommunicationConstants.HTTP_IS_SUCCESSFUL_STATUS_CODE]));
                        Assert.AreEqual("OK", context.Items[HttpCommunicationConstants.HTTP_REASON_PHRASE]);
                        Assert.IsTrue(context.Items.ContainsKey(HttpCommunicationConstants.HTTP_RESPONSE_HEADERS));
                        Assert.IsTrue(context.Items.ContainsKey(HttpCommunicationConstants.HTTP_RESPONSE_TRAILING_HEADERS));
                        Assert.AreEqual(expectedResult, result);
                    }
                });

                var next = mockNext.Object;

                var context = new PipelineContext();

                using (var step = new SendHttpRequestStep(logger, options, next))
                {
                    // act
                    await step.InvokeAsync(context);
                }

                // assert - Phase 2
                if (context.Errors.Count > 0)
                {
                    throw context.Errors[0];
                }
            }
        }

        [TestMethod]
        public async Task Returns_Successful_Response_From_HTTP_GET_Request_When_Header_Does_Not_Exist()
        {
            // arrange
            var logger = new Mock<ILogger<SendHttpRequestStep>>().Object;
            using (var httpClient = new HttpClient())
            {
                var options = new SendHttpRequestOptions()
                {
                    HeaderNames = new List<string>() { "Accepted" },
                    HttpClient = httpClient,
                    HttpEndpoint = new Uri("https://swapi.dev/api/"),
                    HttpMethod = "GET"
                };

                string expectedResult = "{\"people\":\"https://swapi.dev/api/people/\",\"planets\":\"https://swapi.dev/api/planets/\",\"films\":\"https://swapi.dev/api/films/\",\"species\":\"https://swapi.dev/api/species/\",\"vehicles\":\"https://swapi.dev/api/vehicles/\",\"starships\":\"https://swapi.dev/api/starships/\"}";

                var mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(It.IsAny<PipelineContext>())).Callback((IPipelineContext context) =>
                {
                    using (var reader = new StreamReader(context.Items[PipelineContextConstants.OUTPUT_STREAM] as Stream))
                    {
                        string result = reader.ReadToEnd();

                        // assert
                        Assert.AreEqual(HttpStatusCode.OK, context.Items[HttpCommunicationConstants.HTTP_STATUS_CODE]);
                        Assert.IsTrue(context.Items.ContainsKey(HttpCommunicationConstants.HTTP_IS_SUCCESSFUL_STATUS_CODE) && ((bool)context.Items[HttpCommunicationConstants.HTTP_IS_SUCCESSFUL_STATUS_CODE]));
                        Assert.AreEqual("OK", context.Items[HttpCommunicationConstants.HTTP_REASON_PHRASE]);
                        Assert.IsTrue(context.Items.ContainsKey(HttpCommunicationConstants.HTTP_RESPONSE_HEADERS));
                        Assert.IsTrue(context.Items.ContainsKey(HttpCommunicationConstants.HTTP_RESPONSE_TRAILING_HEADERS));
                        Assert.AreEqual(expectedResult, result);
                    }
                });

                var next = mockNext.Object;

                var context = new PipelineContext();
                using (var step = new SendHttpRequestStep(logger, options, next))
                {
                    // act
                    await step.InvokeAsync(context);
                }


                // assert - Phase 2
                if (context.Errors.Count > 0)
                {
                    throw context.Errors[0];
                }
            }
        }

        [TestMethod]
        public async Task Returns_Successful_Response_From_HTTP_GET_Request_When_HttpOptions_Are_Set()
        {
            // arrange
            var logger = new Mock<ILogger<SendHttpRequestStep>>().Object;
            var requestOptions = new HttpRequestOptions();
            requestOptions.TryAdd("Accept", "application/json");
            using (var httpClient = new HttpClient())
            {
                var options = new SendHttpRequestOptions()
                {
                    HttpClient = httpClient,
                    HttpEndpoint = new Uri("https://swapi.dev/api/"),
                    HttpMethod = "GET",
                    RequestOptions = requestOptions
                };

                string expectedResult = "{\"people\":\"https://swapi.dev/api/people/\",\"planets\":\"https://swapi.dev/api/planets/\",\"films\":\"https://swapi.dev/api/films/\",\"species\":\"https://swapi.dev/api/species/\",\"vehicles\":\"https://swapi.dev/api/vehicles/\",\"starships\":\"https://swapi.dev/api/starships/\"}";

                var mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(It.IsAny<PipelineContext>())).Callback((IPipelineContext context) =>
                {
                    using (var reader = new StreamReader(context.Items[PipelineContextConstants.OUTPUT_STREAM] as Stream))
                    {
                        string result = reader.ReadToEnd();

                        // assert
                        Assert.AreEqual(HttpStatusCode.OK, context.Items[HttpCommunicationConstants.HTTP_STATUS_CODE]);
                        Assert.IsTrue(context.Items.ContainsKey(HttpCommunicationConstants.HTTP_IS_SUCCESSFUL_STATUS_CODE) && ((bool)context.Items[HttpCommunicationConstants.HTTP_IS_SUCCESSFUL_STATUS_CODE]));
                        Assert.AreEqual("OK", context.Items[HttpCommunicationConstants.HTTP_REASON_PHRASE]);
                        Assert.IsTrue(context.Items.ContainsKey(HttpCommunicationConstants.HTTP_RESPONSE_HEADERS));
                        Assert.IsTrue(context.Items.ContainsKey(HttpCommunicationConstants.HTTP_RESPONSE_TRAILING_HEADERS));
                        Assert.AreEqual(expectedResult, result);
                    }
                });

                var next = mockNext.Object;

                var context = new PipelineContext();

                using (var step = new SendHttpRequestStep(logger, options, next))
                {
                    // act
                    await step.InvokeAsync(context);
                }

                // assert - Phase 2
                Assert.AreEqual(0, context.Errors.Count);
            }
        }

        [TestMethod]
        public async Task Returns_Successful_Response_From_HTTP_GET_Request_When_HttpEndpointReplacementKeys_Are_Set()
        {
            // arrange
            var logger = new Mock<ILogger<SendHttpRequestStep>>().Object;
            var requestOptions = new HttpRequestOptions();
            requestOptions.TryAdd("Accept", "application/json");
            using (var httpClient = new HttpClient())
            {
                var options = new SendHttpRequestOptions()
                {
                    HttpClient = httpClient,
                    HttpEndpoint = new Uri("https://swapi.dev/api/people/peopleId/"),
                    HttpEndpointReplacementKeys = new List<string>() { "peopleId" },
                    HttpMethod = "GET",
                    RequestOptions = requestOptions
                };

                string expectedResult = "{\"name\":\"Luke Skywalker\",";

                var mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(It.IsAny<PipelineContext>())).Callback((IPipelineContext context) =>
                {
                    using (var reader = new StreamReader(context.Items[PipelineContextConstants.OUTPUT_STREAM] as Stream))
                    {
                        string result = reader.ReadToEnd();

                        // assert
                        Assert.AreEqual(HttpStatusCode.OK, context.Items[HttpCommunicationConstants.HTTP_STATUS_CODE]);
                        Assert.IsTrue(context.Items.ContainsKey(HttpCommunicationConstants.HTTP_IS_SUCCESSFUL_STATUS_CODE) && ((bool)context.Items[HttpCommunicationConstants.HTTP_IS_SUCCESSFUL_STATUS_CODE]));
                        Assert.AreEqual("OK", context.Items[HttpCommunicationConstants.HTTP_REASON_PHRASE]);
                        Assert.IsTrue(context.Items.ContainsKey(HttpCommunicationConstants.HTTP_RESPONSE_HEADERS));
                        Assert.IsTrue(context.Items.ContainsKey(HttpCommunicationConstants.HTTP_RESPONSE_TRAILING_HEADERS));
                        StringAssert.StartsWith(result, expectedResult);
                    }
                });

                var next = mockNext.Object;

                var context = new PipelineContext();
                context.Items.Add("peopleId", 1);

                using (var step = new SendHttpRequestStep(logger, options, next))
                {
                    // act
                    await step.InvokeAsync(context);
                }

                // assert - Phase 2
                Assert.AreEqual(0, context.Errors.Count);
            }
        }
        //GET 
        [TestMethod]
        public async Task Returns_Successful_Response_From_HTTP_POST_Request_When_Authentication_Is_Required()
        {
            // arrange
            var logger = new Mock<ILogger<SendHttpRequestStep>>().Object;
            using (var httpClient = new HttpClient())
            {
                var options = new SendHttpRequestOptions()
                {
                    ContentTypeHeaderValue = "application/graphql",
                    HttpClient = httpClient,
                    HttpEndpoint = new Uri("https://api.github.com/graphql"),
                    HttpMethod = "POST",
                    HeaderNames = new List<string>() { "Accept", "Authorization", "User-Agent" },
                    UploadInputStreamAsContent = true
                };

                var mockNext = new Mock<IPipelineRequest>();

                mockNext.Setup(x => x.InvokeAsync(It.IsAny<PipelineContext>())).Callback((IPipelineContext context) =>
                {
                    using (var reader = new StreamReader(context.Items[PipelineContextConstants.OUTPUT_STREAM] as Stream))
                    {
                        string result = reader.ReadToEnd();

                        // assert
                        Assert.AreEqual(HttpStatusCode.OK, context.Items[HttpCommunicationConstants.HTTP_STATUS_CODE]);
                        Assert.IsTrue(context.Items.ContainsKey(HttpCommunicationConstants.HTTP_IS_SUCCESSFUL_STATUS_CODE) && ((bool)context.Items[HttpCommunicationConstants.HTTP_IS_SUCCESSFUL_STATUS_CODE]));
                        Assert.AreEqual("OK", context.Items[HttpCommunicationConstants.HTTP_REASON_PHRASE]);
                        Assert.IsTrue(context.Items.ContainsKey(HttpCommunicationConstants.HTTP_RESPONSE_HEADERS));
                        Assert.IsTrue(context.Items.ContainsKey(HttpCommunicationConstants.HTTP_RESPONSE_TRAILING_HEADERS));
                        Assert.AreEqual(SendHttpRequestStepTests.PIPELINE_TEST_GITHUB_QUERY_RESULT, result);
                    }
                });

                var next = mockNext.Object;

                var context = new PipelineContext();
                context.Items.Add("Accept", "application/vnd.github.v4.idl");
                context.Items.Add("Authorization", $"Bearer {SendHttpRequestStepTests.PIPELINE_TEST_GITHUB_API_TOKEN}");
                context.Items.Add("User-Agent", "MyTrout.Pipelines.Steps.Communications.Http.v1.1");
                context.Items.Add("Content-Type", "application/graphql");


                using (var stream = new MemoryStream())
                {
                    stream.Write(Encoding.UTF8.GetBytes(SendHttpRequestStepTests.PIPELINE_TEST_GITHUB_QUERY));

                    stream.Position = 0;

                    context.Items.Add(PipelineContextConstants.INPUT_STREAM, stream);


                    using (var step = new SendHttpRequestStep(logger, options, next))
                    {
                        // act
                        await step.InvokeAsync(context);
                    }
                }

                // assert - Phase 2
                if (context.Errors.Count > 0)
                {
                    throw context.Errors[0];
                }
            }
        }

        [TestMethod]
        public async Task Returns_Successful_Response_From_HTTP_POST_Request_When_ContextNames_Are_Changed()
        {
            // arrange
            var logger = new Mock<ILogger<SendHttpRequestStep>>().Object;
            using (var httpClient = new HttpClient())
            {
                var options = new SendHttpRequestOptions()
                {
                    ContentTypeHeaderValue = "application/graphql",
                    HttpClient = httpClient,
                    HttpReasonPhraseContextName = "TESTING_REASON_PHRASE",
                    HttpResponseHeadersContextName = "TESTING_RESPONSE_HEADERS",
                    HttpResponseTrailingHeadersContextName = "TESTING_TRAILING_RESPONSE_HEADERS",
                    HttpStatusCodeContextName = "TESTING_STATUS_CODE",
                    HttpEndpoint = new Uri("https://api.github.com/graphql"),
                    InputStreamContextName = "TESTING_INPUT_STREAM",
                    IsSuccessfulStatusCodeContextName = "TESTING_IS_SUCCESSFUL_RESPONSE_CODE",
                    HttpMethod = "POST",
                    HeaderNames = new List<string>() { "Accept", "Authorization", "User-Agent" },
                    OutputStreamContextName = "TESTING_OUTPUT_STREAM",
                    UploadInputStreamAsContent = true
                };

                var mockNext = new Mock<IPipelineRequest>();

                mockNext.Setup(x => x.InvokeAsync(It.IsAny<PipelineContext>())).Callback((IPipelineContext context) =>
                {
                    using (var reader = new StreamReader(context.Items[options.OutputStreamContextName] as Stream))
                    {
                        string result = reader.ReadToEnd();

                        // assert
                        Assert.AreEqual(HttpStatusCode.OK, context.Items[options.HttpStatusCodeContextName]);
                        Assert.IsTrue(context.Items.ContainsKey(options.IsSuccessfulStatusCodeContextName) && ((bool)context.Items[options.IsSuccessfulStatusCodeContextName]));
                        Assert.AreEqual("OK", context.Items[options.HttpReasonPhraseContextName]);
                        Assert.IsTrue(context.Items.ContainsKey(options.HttpResponseHeadersContextName));
                        Assert.IsTrue(context.Items.ContainsKey(options.HttpResponseTrailingHeadersContextName));
                        Assert.AreEqual(SendHttpRequestStepTests.PIPELINE_TEST_GITHUB_QUERY_RESULT, result);
                    }
                });

                var next = mockNext.Object;

                var context = new PipelineContext();
                context.Items.Add("Accept", "application/vnd.github.v4.idl");
                context.Items.Add("Authorization", $"Bearer {SendHttpRequestStepTests.PIPELINE_TEST_GITHUB_API_TOKEN}");
                context.Items.Add("User-Agent", "MyTrout.Pipelines.Steps.Communications.Http.v1.1");
                context.Items.Add("Content-Type", "application/graphql");


                using (var stream = new MemoryStream())
                {
                    stream.Write(Encoding.UTF8.GetBytes(SendHttpRequestStepTests.PIPELINE_TEST_GITHUB_QUERY));

                    stream.Position = 0;

                    context.Items.Add(options.InputStreamContextName, stream);


                    using (var step = new SendHttpRequestStep(logger, options, next))
                    {
                        // act
                        await step.InvokeAsync(context);
                    }
                }

                // assert - Phase 2
                if (context.Errors.Count > 0)
                {
                    throw context.Errors[0];
                }
            }
        }

        [TestMethod]
        public async Task Returns_PipelineContext_With_No_Alterations_From_InvokeAsync_After_Execution()
        {
            // arrange
            var logger = new Mock<ILogger<SendHttpRequestStep>>().Object;
            using (var httpClient = new HttpClient())
            {
                var options = new SendHttpRequestOptions()
                {
                    HttpClient = httpClient,
                    HttpEndpoint = new Uri("https://swapi.dev/api/"),
                    HttpMethod = "GET"
                };

                var mockNext = new Mock<IPipelineRequest>();
                mockNext.Setup(x => x.InvokeAsync(It.IsAny<PipelineContext>())).Returns(Task.CompletedTask);
                var next = mockNext.Object;

                var context = new PipelineContext();
                var expectedItemCount = context.Items.Count;

                using (var step = new SendHttpRequestStep(logger, options, next))
                {
                    // act
                    await step.InvokeAsync(context);
                }

                // assert - Phase 2
                if (context.Errors.Count > 0)
                {
                    throw context.Errors[0];
                }
                Assert.AreEqual(expectedItemCount, context.Items.Count, "context.Items was not cleaned up during step execution. It contains {0} items and should contain {1}.", context.Items.Count, expectedItemCount);
            }
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Logger_Is_Null()
        {
            // arrange
            ILogger<SendHttpRequestStep> logger = null;
            var options = new SendHttpRequestOptions();
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            string expectedParamName = nameof(logger);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new SendHttpRequestStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Next_Is_Null()
        {
            // arrange

            ILogger<SendHttpRequestStep> logger = new Mock<ILogger<SendHttpRequestStep>>().Object;
            var options = new SendHttpRequestOptions();
            IPipelineRequest next = null;

            string expectedParamName = nameof(next);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new SendHttpRequestStep(logger, options, next));


            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Options_Is_Null()
        {
            // arrange
            ILogger<SendHttpRequestStep> logger = new Mock<ILogger<SendHttpRequestStep>>().Object;
            SendHttpRequestOptions options = null;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            string expectedParamName = nameof(options);

            // act
            var result = Assert.ThrowsException<ArgumentNullException>(() => new SendHttpRequestStep(logger, options, next));

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedParamName, result.ParamName);
        }
    }
}