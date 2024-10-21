using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DurableFunction.Demo.Func.Scenario1;

public class SaveWeatherData
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SaveWeatherData> _logger;

    public SaveWeatherData(HttpClient httpClient, ILogger<SaveWeatherData> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    [Function(nameof(SaveWeatherData))]
    public async Task<SaveWeatherDataResponse> Run([ActivityTrigger] SaveWeatherDataRequest input)
    {
       _logger.LogInformation(input.ToString());
        return new SaveWeatherDataResponse("Success");
    }
}

public record SaveWeatherDataRequest(double Latitude, double Longitude, double Elevation);
public record SaveWeatherDataResponse(string State);