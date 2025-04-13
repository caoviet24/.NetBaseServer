using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Infrastructure.Redis;

public class RedisHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisHealthCheck(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var database = _connectionMultiplexer.GetDatabase();
            var result = await database.PingAsync();
            
            return result.TotalMilliseconds switch
            {
                < 100 => HealthCheckResult.Healthy($"Redis responds in {result.TotalMilliseconds}ms"),
                < 200 => HealthCheckResult.Degraded($"Redis responds in {result.TotalMilliseconds}ms"),
                _ => HealthCheckResult.Unhealthy($"Redis responds in {result.TotalMilliseconds}ms")
            };
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis connection failed", ex);
        }
    }
}