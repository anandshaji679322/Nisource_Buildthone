using SmartBillAi.Api.Models;

namespace SmartBillAi.Api.Services;

public interface IWeatherService
{
    Task<WeatherMetricDto> GetWeatherMetricsAsync(string region, string periodId, CancellationToken cancellationToken = default);
}
