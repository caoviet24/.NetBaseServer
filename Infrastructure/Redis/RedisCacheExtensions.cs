using Application.Common.Interfaces;
using System.Text.RegularExpressions;

namespace Infrastructure.Redis;

public static class RedisCacheExtensions
{
    /// <summary>
    /// Caches the result of a query for a specified time
    /// </summary>
    public static async Task<T> CacheQueryAsync<T>(
        this IRedisService redisService,
        string cacheKey,
        Func<Task<T>> queryFunc,
        TimeSpan? expiry = null) where T : class
    {
        // Try to get from cache first
        var cachedResult = await redisService.GetStringAsync<T>(cacheKey);
        if (cachedResult != null)
        {
            return cachedResult;
        }

        // If not in cache, execute query
        var result = await queryFunc();

        // Cache result with expiry (default 5 minutes if not specified)
        await redisService.SetStringAsync(cacheKey, result, expiry ?? TimeSpan.FromMinutes(5));

        return result;
    }

    /// <summary>
    /// Invalidates all cache keys matching a pattern
    /// </summary>
    public static async Task InvalidateCachePatternAsync(
        this IRedisService redisService,
        string pattern)
    {
        // Implementation would depend on the specific Redis client library
        // This is a placeholder that would need to be implemented based on your Redis client
        
        // Example implementation concept:
        // 1. Get all keys matching pattern
        // 2. Delete all these keys
        
        // Since we're using a simplified Redis service, this would need to be 
        // implemented with direct connection multiplexer operations
    }

    /// <summary>
    /// Generates a cache key based on object type and identifier
    /// </summary>
    public static string GenerateCacheKey(string prefix, string identifier)
    {
        // Sanitize the identifier to remove any characters that shouldn't be in a key
        var sanitizedId = Regex.Replace(identifier, "[^a-zA-Z0-9_]", "");
        return $"{prefix}:{sanitizedId}";
    }

    /// <summary>
    /// Generates a cache key for a collection
    /// </summary>
    public static string GenerateCollectionCacheKey(string prefix, string filter = "")
    {
        if (string.IsNullOrEmpty(filter))
        {
            return $"{prefix}:all";
        }
        var sanitizedFilter = Regex.Replace(filter, "[^a-zA-Z0-9_]", "");
        return $"{prefix}:filter:{sanitizedFilter}";
    }
}