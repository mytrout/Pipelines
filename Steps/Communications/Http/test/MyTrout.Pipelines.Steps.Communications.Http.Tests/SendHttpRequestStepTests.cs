// <copyright file="SendHttpRequestStep.cs" company="Chris Trout">
// MIT License
//
// Copyright(c) 2021 Chris Trout
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
            var httpClient = new HttpClient();
            
            var next = new Mock<IPipelineRequest>().Object;

            // act
            using (var result = new SendHttpRequestStep(logger, httpClient, options, next))
            {
                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(httpClient, result.HttpClient);
                Assert.AreEqual(logger, result.Logger);
                Assert.AreEqual(next, result.Next);
                Assert.AreEqual(options, result.Options);
            }
        }

        [TestMethod]
        public async Task Returns_Successful_Response_From_HTTP_GET_Request_When_Header_Does_Not_Exist()
        {
            // arrange
            var logger = new Mock<ILogger<SendHttpRequestStep>>().Object;
            var options = new SendHttpRequestOptions()
            {
                HeaderNames = new List<string>() { "Accepted" },
                HttpEndpoint = new Uri("https://swapi.dev/api/"),
                Method = System.Net.Http.HttpMethod.Get
            };

            string expectedResult = "{\"people\":\"https://swapi.dev/api/people/\",\"planets\":\"https://swapi.dev/api/planets/\",\"films\":\"https://swapi.dev/api/films/\",\"species\":\"https://swapi.dev/api/species/\",\"vehicles\":\"https://swapi.dev/api/vehicles/\",\"starships\":\"https://swapi.dev/api/starships/\"}";

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(It.IsAny<PipelineContext>())).Callback((IPipelineContext context) =>
            {
                using (var reader = new StreamReader(context.Items[PipelineContextConstants.INPUT_STREAM] as Stream))
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
            using (var httpClient = new HttpClient())
            {
                context.Items.Add(HttpCommunicationConstants.HTTP_CLIENT, httpClient);

                using (var step = new SendHttpRequestStep(logger, httpClient, options, next))
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

        [TestMethod]
        public async Task Returns_Successful_Response_From_HTTP_GET_Request()
        {
            // arrange
            var logger = new Mock<ILogger<SendHttpRequestStep>>().Object;
            var options = new SendHttpRequestOptions()
            {
                HttpEndpoint = new Uri("https://swapi.dev/api/"),
                Method = System.Net.Http.HttpMethod.Get
            };

            string expectedResult = "{\"people\":\"https://swapi.dev/api/people/\",\"planets\":\"https://swapi.dev/api/planets/\",\"films\":\"https://swapi.dev/api/films/\",\"species\":\"https://swapi.dev/api/species/\",\"vehicles\":\"https://swapi.dev/api/vehicles/\",\"starships\":\"https://swapi.dev/api/starships/\"}";

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(It.IsAny<PipelineContext>())).Callback((IPipelineContext context) =>
            {
                using(var reader = new StreamReader(context.Items[PipelineContextConstants.INPUT_STREAM] as Stream))
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
            using (var httpClient = new HttpClient())
            {
                using (var step = new SendHttpRequestStep(logger, httpClient, options, next))
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

        [TestMethod]
        public async Task Returns_Failed_Response_From_Fake_HTTP_GET_Request_When_ReasonPhrase_Is_Null()
        {
            // arrange
            var logger = new Mock<ILogger<SendHttpRequestStep>>().Object;
            var options = new SendHttpRequestOptions()
            {
                HttpEndpoint = new Uri("https://swapi.dev/api/"),
                Method = System.Net.Http.HttpMethod.Get
            };

            string expectedResult = string.Empty;

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(It.IsAny<PipelineContext>())).Callback((IPipelineContext context) =>
            {
                using (var reader = new StreamReader(context.Items[PipelineContextConstants.INPUT_STREAM] as Stream))
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
            using (var httpClient = HttpClientFactory.Create( new ReplaceReasonPhraseHandler()))
            {
                using (var step = new SendHttpRequestStep(logger, httpClient, options, next))
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

        [TestMethod]
        public async Task Returns_Successful_Response_From_HTTP_GET_Request_When_HttpOptions_Are_Set()
        {
            // arrange
            var logger = new Mock<ILogger<SendHttpRequestStep>>().Object;
            var requestOptions = new HttpRequestOptions();
            requestOptions.TryAdd("Accept", "aaplication/json");

            var options = new SendHttpRequestOptions()
            {
                HttpEndpoint = new Uri("https://swapi.dev/api/"),
                Method = System.Net.Http.HttpMethod.Get,
                RequestOptions = requestOptions
            };

            string expectedResult = "{\"people\":\"https://swapi.dev/api/people/\",\"planets\":\"https://swapi.dev/api/planets/\",\"films\":\"https://swapi.dev/api/films/\",\"species\":\"https://swapi.dev/api/species/\",\"vehicles\":\"https://swapi.dev/api/vehicles/\",\"starships\":\"https://swapi.dev/api/starships/\"}";

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(It.IsAny<PipelineContext>())).Callback((IPipelineContext context) =>
            {
                using (var reader = new StreamReader(context.Items[PipelineContextConstants.INPUT_STREAM] as Stream))
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
            using (var httpClient = new HttpClient())
            {
                context.Items.Add(HttpCommunicationConstants.HTTP_CLIENT, httpClient);

                using (var step = new SendHttpRequestStep(logger, httpClient, options, next))
                {
                    // act
                    await step.InvokeAsync(context);
                }
            }

            // assert - Phase 2
            Assert.AreEqual(0, context.Errors.Count);
        }
        [TestMethod]
        public async Task Returns_Successful_Response_From_HTTP_POST_Request_When_Authentication_Is_Required()
        {
            // arrange
            var logger = new Mock<ILogger<SendHttpRequestStep>>().Object;
            var options = new SendHttpRequestOptions()
            {
                HttpEndpoint = new Uri("https://api.github.com/graphql"),
                Method = System.Net.Http.HttpMethod.Post,
                HeaderNames = new List<string>() { "Accept", "Authorization", "User-Agent" },
                UploadInputStreamAsContent = true
            };

            var mockNext = new Mock<IPipelineRequest>();

            mockNext.Setup(x => x.InvokeAsync(It.IsAny<PipelineContext>())).Callback((IPipelineContext context) =>
            {
                using (var reader = new StreamReader(context.Items[PipelineContextConstants.INPUT_STREAM] as Stream))
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
            context.Items.Add("User-Agent", "MyTrout.Pipelines.Steps.Communications.Http.v1.0");
            context.Items.Add("Content-Type", "application/graphql");


            using (var stream = new MemoryStream())
            {
                stream.Write(Encoding.UTF8.GetBytes(SendHttpRequestStepTests.PIPELINE_TEST_GITHUB_QUERY));
                    
                stream.Position = 0;

                context.Items.Add(PipelineContextConstants.OUTPUT_STREAM, stream);

                using (var httpClient = new HttpClient())
                {
                    using (var step = new SendHttpRequestStep(logger, httpClient, options, next))
                    {
                        // act
                        await step.InvokeAsync(context);
                    }
                }
            }

            // assert - Phase 2
            if (context.Errors.Count > 0)
            {
                throw context.Errors[0];
            }
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_HttpClient_Is_Null()
        {
            // arrange
            ILogger<SendHttpRequestStep> logger = new Mock<ILogger<SendHttpRequestStep>>().Object;
            var options = new SendHttpRequestOptions();
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            using (var httpClient = null as HttpClient)
            {
                string expectedParamName = nameof(httpClient);

                // act
                var result = Assert.ThrowsException<ArgumentNullException>(() => new SendHttpRequestStep(logger, httpClient, options, next));

                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedParamName, result.ParamName);
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

            using (var httpClient = new HttpClient())
            {
                // act
                var result = Assert.ThrowsException<ArgumentNullException>(() => new SendHttpRequestStep(logger, httpClient, options, next));

                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedParamName, result.ParamName);
            }
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Next_Is_Null()
        {
            // arrange
            
            ILogger<SendHttpRequestStep> logger = new Mock<ILogger<SendHttpRequestStep>>().Object;
            var options = new SendHttpRequestOptions();
            IPipelineRequest next = null;

            string expectedParamName = nameof(next);

            using (var httpClient = new HttpClient())
            {
                // act
                var result = Assert.ThrowsException<ArgumentNullException>(() => new SendHttpRequestStep(logger, httpClient, options, next));
            
            
                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedParamName, result.ParamName);
            }
        }

        [TestMethod]
        public void Throws_ArgumentNullException_From_Constructor_When_Options_Is_Null()
        {
            // arrange
            ILogger<SendHttpRequestStep> logger = new Mock<ILogger<SendHttpRequestStep>>().Object;
            SendHttpRequestOptions options = null;
            IPipelineRequest next = new Mock<IPipelineRequest>().Object;

            string expectedParamName = nameof(options);

            using (var httpClient = new HttpClient())
            {
                // act
                var result = Assert.ThrowsException<ArgumentNullException>(() => new SendHttpRequestStep(logger, httpClient, options, next));

                // assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedParamName, result.ParamName);
            }
        }
    }
}
