using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DurableFunction.Demo.Func.Scenario1;

public class GetWeatherData
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GetWeatherData> _logger;

    public GetWeatherData(HttpClient httpClient, ILogger<GetWeatherData> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    [Function(nameof(GetWeatherData))]
    public async Task<GetWeatherDataResponse> Run([ActivityTrigger] GetWeatherDataRequest request)
    {
        var weatherData = await _httpClient.GetStringAsync(request.Uri);
        _logger.LogInformation(weatherData);
        var response = JsonSerializer.Deserialize<GetWeatherDataResponse>(weatherData);
        return response;
    }
}

public record GetWeatherDataRequest(string Uri);
public record GetWeatherDataResponse([property: JsonPropertyName("latitude")] double Latitude, [property: JsonPropertyName("longitude")] double Longitude, [property: JsonPropertyName("elevation")] double Elevation);