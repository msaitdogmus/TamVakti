namespace TamVakti.Sample.Models;

public sealed record WeatherSnapshot(
    double Temperature,
    string TemperatureUnit,
    int WeatherCode,
    DateTimeOffset UpdatedAt);
