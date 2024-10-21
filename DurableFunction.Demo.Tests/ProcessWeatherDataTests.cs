using DurableFunction.Demo.Func.Scenario1;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using Moq;

namespace DurableFunction.Demo.Tests;

public class ProcessWeatherDataTests
{
    [Fact]
    public async Task RunOrchestrator_Should_Call_Activities()
    {
        // Arrange
        var contextMock = new Mock<TaskOrchestrationContext>();
        var loggerMock = new Mock<ILogger<ProcessWeatherData>>();
        contextMock.Setup(c => c.CreateReplaySafeLogger<ProcessWeatherData>()).Returns(loggerMock.Object);

        contextMock.Setup(c => c.GetInput<string>()).Returns("http://example.com/weather");
        contextMock.Setup(c => c.CallActivityAsync<GetWeatherDataResponse>(
            nameof(GetWeatherData), It.IsAny<GetWeatherDataRequest>(), It.IsAny<TaskOptions>()))
            .ReturnsAsync(new GetWeatherDataResponse(35.5, 139, 234));
        contextMock.Setup(c => c.CallActivityAsync<SaveWeatherDataResponse>(
            nameof(SaveWeatherData), It.IsAny<SaveWeatherDataRequest>(), It.IsAny<TaskOptions>()))
            .ReturnsAsync(new SaveWeatherDataResponse("Success"));

        // Act
        var result = await ProcessWeatherData.RunOrchestrator(contextMock.Object);

        // Assert
        Assert.NotNull(result);
        contextMock.Verify(c => c.CallActivityAsync<GetWeatherDataResponse>(
            nameof(GetWeatherData), It.IsAny<GetWeatherDataRequest>(), It.IsAny<TaskOptions>()), Times.Once);
        contextMock.Verify(c => c.CallActivityAsync<SaveWeatherDataResponse>(
            nameof(SaveWeatherData), It.IsAny<SaveWeatherDataRequest>(), It.IsAny<TaskOptions>()), Times.Once);
    }
}