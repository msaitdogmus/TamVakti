using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using TamVakti.Sample.Models;

namespace TamVakti.Sample.Services;

public sealed class WeatherClient
{
    private readonly HttpClient httpClient;

    public WeatherClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<WeatherSnapshot?> GetCurrentAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default)
    {
        var lat = latitude.ToString("0.####", CultureInfo.InvariantCulture);
        var lon = longitude.ToString("0.####", CultureInfo.InvariantCulture);
        var url = "https://api.open-meteo.com/v1/forecast" +
                  $"?latitude={lat}&longitude={lon}" +
                  "&current=temperature_2m,weather_code" +
                  "&timezone=auto";

        var response = await httpClient.GetFromJsonAsync<WeatherResponse>(
            url,
            cancellationToken);

        if (response?.Current is null)
        {
            return null;
        }

        return new WeatherSnapshot(
            response.Current.Temperature,
            response.Units?.Temperature ?? "°C",
            response.Current.WeatherCode,
            DateTimeOffset.Now);
    }

    private sealed class WeatherResponse
    {
        [JsonPropertyName("current")]
        public CurrentWeather? Current { get; init; }

        [JsonPropertyName("current_units")]
        public CurrentUnits? Units { get; init; }
    }

    private sealed class CurrentWeather
    {
        [JsonPropertyName("temperature_2m")]
        public double Temperature { get; init; }

        [JsonPropertyName("weather_code")]
        public int WeatherCode { get; init; }
    }

    private sealed class CurrentUnits
    {
        [JsonPropertyName("temperature_2m")]
        public string Temperature { get; init; } = "°C";
    }
}
