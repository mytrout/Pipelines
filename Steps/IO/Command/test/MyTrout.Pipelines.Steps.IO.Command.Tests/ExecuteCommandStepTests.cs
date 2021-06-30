// <copyright file="ExecuteCommandStepTests.cs" company="Chris Trout">
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

namespace MyTrout.Pipelines.Steps.IO.Command.Tests
{
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrout.Pipelines.Core;
    using MyTrout.Pipelines.Steps.IO.Command;
    using MyTrout.Pipelines.Steps.IO.Files;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;

    [TestClass]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ExecuteCommandStepTests
    {
        [TestMethod]
        public void Constructs_ExecuteCommandStep_Successfully()
        {
            // arrange
            var mockLogger = new Mock<ILogger<ExecuteCommandStep>>();
            var logger = mockLogger.Object;

            var options = new WindowsDefenderAntivirusOptions()
            {
                ExpectedResult = "Failed with hr = 0x80004005"
            };

            var next = new Mock<IPipelineRequest>().Object;

            // act
            var result = new ExecuteCommandStep(logger, options, next);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(logger, result.Logger);
            Assert.AreEqual(next, result.Next);
            Assert.AreEqual(options, result.Options);
        }

        [TestMethod]
        public async Task Returns_Exception_From_InvokeCoreAsync_When_Exception_Text_Is_Thrown()
        {
            // arrange
            var mockLogger = new Mock<ILogger<ExecuteCommandStep>>();
            var logger = mockLogger.Object;

            var options = new ExecuteCommandOptions()
            {
                Arguments = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}{Path.DirectorySeparatorChar}ExceptionConsole.dll --exception",
                CommandString = "dotnet",
                ExpectedResult = "Nothing",
                IncludeFileNameTransformInArguments = false
            };

            PipelineContext context = new PipelineContext();

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            var next = mockNext.Object;

            var source = new ExecuteCommandStep(logger, options, next);

            var exceptionCount = 1;
            var exceptionType = typeof(InvalidOperationException);
            var expectedMessage = "Unhandled exception. System.InvalidOperationException: Hello World!";
            

            // act
            await source.InvokeAsync(context);

            // assert
            Assert.AreEqual(exceptionCount, context.Errors.Count);
            Assert.IsInstanceOfType(context.Errors[0], exceptionType, context.Errors[0].Message);
            StringAssert.StartsWith(context.Errors[0].Message, expectedMessage);
        }

        [TestMethod]
        public async Task Returns_Successful_Status_From_InvokeCoreAsync_When_ExpectedResult_Matches()
        {
            // arrange
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            var logger = loggerFactory.CreateLogger<ExecuteCommandStep>();
            
            // This test also includes the Arguments replacement for File Systems.
            var options = new ExecuteCommandOptions()
            {
                Arguments = "{0}",
                CommandString = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}{Path.DirectorySeparatorChar}ExceptionConsole.exe",
                ExpectedResult = "Nothing",
                IncludeFileNameTransformInArguments = true
            };

            PipelineContext context = new PipelineContext();
            context.Items.Add(FileConstants.TARGET_FILE, options.ExpectedResult);

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            var next = mockNext.Object;

            var source = new ExecuteCommandStep(logger, options, next);

            var expectedStatus = CommandLineConstants.COMMAND_LINE_SUCCESS;

            // act
            await source.InvokeAsync(context);

            // assert
            Assert.IsTrue(context.Items.ContainsKey(CommandLineConstants.COMMAND_LINE_STATUS), "PipelineContext does not contain a COMMAND_LINE_STATUS entry.");
            Assert.AreEqual(expectedStatus, context.Items[CommandLineConstants.COMMAND_LINE_STATUS]);
        }

        [TestMethod]
        public async Task Returns_Failed_Status_From_InvokeCoreAsync_When_ExpectedResult_Does_Not_Match()
        {
            // arrange
            var mockLogger = new Mock<ILogger<ExecuteCommandStep>>();
            var logger = mockLogger.Object;

            var options = new ExecuteCommandOptions()
            {
                Arguments = "Something",
                CommandString = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}{Path.DirectorySeparatorChar}ExceptionConsole.exe",
                ExpectedResult = "Nothing",
                IncludeFileNameTransformInArguments = false
            };

            PipelineContext context = new PipelineContext();
            context.Items.Add(FileConstants.SOURCE_FILE, $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}{Path.DirectorySeparatorChar}ExecuteCommandStepTests.cs");

            var mockNext = new Mock<IPipelineRequest>();
            mockNext.Setup(x => x.InvokeAsync(context)).Returns(Task.CompletedTask);
            var next = mockNext.Object;

            var source = new ExecuteCommandStep(logger, options, next);

            var expectedStatus = CommandLineConstants.COMMAND_LINE_FAILED;

            // act
            await source.InvokeAsync(context);

            // assert
            Assert.IsTrue(context.Items.ContainsKey(CommandLineConstants.COMMAND_LINE_STATUS), "PipelineContext does not contain a COMMAND_LINE_STATUS entry.");
            Assert.AreEqual(expectedStatus, context.Items[CommandLineConstants.COMMAND_LINE_STATUS]);
        }
    }
}
