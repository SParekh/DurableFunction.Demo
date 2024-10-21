using DurableFunction.Demo.Func.Scenario1;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DurableFunction.Demo.Func;

// To Test this endpoint
// Endpiont:  http://localhost:7218/api/GetWeatherDataEndpoint
// Method: POST
// Body: https://api.open-meteo.com/v1/forecast?latitude=35&longitude=139&hourly=temperature_2m
public static class LocalTestEndpoints
{
#if DEBUG
    // OPTIONS to start, query , and manage orchestration instances
    // 1) [DurableClient] attribute is used in Azure Durable Functions to inject an instance of DurableTaskClient into your function.
    // This client is essential for interacting with the Durable Task framework,
    // allowing you to start, query, and manage orchestration instances.
    // DURABLECLIENT can be injected in any Function TRIGGER
    // 2) Durable Functions HTTP API - http://localhost:7218/runtime/webhooks/durabletask/instances/5000564b77ee4cedb73151a64c03c83e
    // The Durable Functions extension provides an HTTP API that allows you to interact with orchestrations.
    // This API can be used to start, query, and manage orchestrations from external systems without directly using DurableTaskClient.
    [Function("GetWeatherDataEndpoint")]
    public static async Task<HttpResponseData> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger(nameof(LocalTestEndpoints));
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        // Pass StartOrchestrationsOptions to specify instanceId if you want to ensure only a single instance of given scenario is running
        string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(ProcessWeatherData), requestBody);


        logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

        return await client.CreateCheckStatusResponseAsync(req, instanceId);
    }
#endif
}