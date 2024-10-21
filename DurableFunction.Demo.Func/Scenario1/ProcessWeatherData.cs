using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using System;

namespace DurableFunction.Demo.Func.Scenario1;

public class ProcessWeatherData
{
    private static RetryPolicy retryPolicy = new RetryPolicy(
         maxNumberOfAttempts: 2,
         firstRetryInterval: TimeSpan.FromSeconds(1),
         backoffCoefficient: 2.0);

    // [OrchestrationTrigger] attribute is used to mark a function as an orchestrator function.
    // This function will be triggered by the Durable Task framework (Either via DurableTaskClient/HttpAPI)
    // when an orchestration instance is started or when it needs to process an event.
    // [TaskOrchestrationContext] is used to interact with the Durable Task framework from within the orchestrator function.
    [Function(nameof(ProcessWeatherData))]
    public static async Task<bool> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var url = context.GetInput<string>();
        
        // IMPORTANT: Returns an instance of ILogger that is replay safe, ensuring the logger logs only when the orchestrator is not replaying that line of code.
        ILogger logger = context.CreateReplaySafeLogger<ProcessWeatherData>();
        logger.LogInformation("Saying hello.");
        
        var weatherData = await context.CallActivityAsync<GetWeatherDataResponse>(
            nameof(GetWeatherData), 
            new GetWeatherDataRequest(url), 
            new TaskOptions() { Retry = new TaskRetryOptions(retryPolicy) });

        var saveResonse = await context.CallActivityAsync<SaveWeatherDataResponse>(nameof(SaveWeatherData), new SaveWeatherDataRequest(weatherData.Latitude, weatherData.Longitude, weatherData.Elevation));

        // this can be void or any value you would want to track as output of the orchestrator
        return true;
    }
}
