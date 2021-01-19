
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
                Arguments = string.Empty,
                CommandString = Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "\\ExceptionConsole.exe",
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
            var expectedMessage = "Cannot use file stream for [";
            

            // act
            await source.InvokeAsync(context);

            // assert
            Assert.AreEqual(exceptionCount, context.Errors.Count);
            Assert.IsInstanceOfType(context.Errors[0], exceptionType);
            StringAssert.StartsWith(context.Errors[0].Message, expectedMessage);
        }

        [TestMethod]
        public async Task Returns_Successful_Status_From_InvokeCoreAsync_When_ExpectedResult_Matches()
        {
            // arrange
            var mockLogger = new Mock<ILogger<ExecuteCommandStep>>();
            var logger = mockLogger.Object;

            var options = new WindowsDefenderAntivirusOptions()
            {
                ExpectedResult = "Failed with hr = 0x80004005"
            };

            PipelineContext context = new PipelineContext();
            context.Items.Add(FileConstants.SOURCE_FILE, Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "\\ExecuteCommandStepTests.cs");

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

            var options = new WindowsDefenderAntivirusOptions()
            {
                ExpectedResult = "I watched TV while writing this test."
            };

            PipelineContext context = new PipelineContext();
            context.Items.Add(FileConstants.SOURCE_FILE, Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "\\ExecuteCommandStepTests.cs");

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
