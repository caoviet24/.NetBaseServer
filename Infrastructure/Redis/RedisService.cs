using System.Text.Json;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Infrastructure.Redis;

public class RedisService : IRedisService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _database;
    private readonly string _instanceName;

    public RedisService(IConnectionMultiplexer connectionMultiplexer, IConfiguration configuration)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _database = connectionMultiplexer.GetDatabase();
        _instanceName = configuration["Redis:InstanceName"] ?? "elearning:";
    }

    private string GetKey(string key) => $"{_instanceName}{key}";

    // String operations
    public async Task<bool> SetStringAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        string serializedValue = JsonSerializer.Serialize(value);
        return await _database.StringSetAsync(GetKey(key), serializedValue, expiry);
    }

    public async Task<T> GetStringAsync<T>(string key)
    {
        var value = await _database.StringGetAsync(GetKey(key));
        if (value.IsNullOrEmpty)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(value);
    }

    // Hash operations
    public async Task<bool> SetHashAsync<T>(string key, string field, T value)
    {
        string serializedValue = JsonSerializer.Serialize(value);
        return await _database.HashSetAsync(GetKey(key), field, serializedValue);
    }

    public async Task<T> GetHashAsync<T>(string key, string field)
    {
        var value = await _database.HashGetAsync(GetKey(key), field);
        if (value.IsNullOrEmpty)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(value);
    }

    public async Task<IDictionary<string, T>> GetAllHashAsync<T>(string key)
    {
        var entries = await _database.HashGetAllAsync(GetKey(key));
        var result = new Dictionary<string, T>();

        foreach (var entry in entries)
        {
            result.Add(entry.Name.ToString(), JsonSerializer.Deserialize<T>(entry.Value));
        }

        return result;
    }

    public async Task<bool> RemoveHashAsync(string key, string field)
    {
        return await _database.HashDeleteAsync(GetKey(key), field);
    }

    // List operations
    public async Task<long> AddToListAsync<T>(string key, T item)
    {
        string serializedValue = JsonSerializer.Serialize(item);
        return await _database.ListRightPushAsync(GetKey(key), serializedValue);
    }

    public async Task<IEnumerable<T>> GetListAsync<T>(string key)
    {
        var values = await _database.ListRangeAsync(GetKey(key));
        var result = new List<T>();

        foreach (var value in values)
        {
            result.Add(JsonSerializer.Deserialize<T>(value));
        }

        return result;
    }

    // Key operations
    public async Task<bool> KeyExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(GetKey(key));
    }

    public async Task<bool> SetExpiryAsync(string key, TimeSpan expiry)
    {
        return await _database.KeyExpireAsync(GetKey(key), expiry);
    }

    public async Task<bool> RemoveAsync(string key)
    {
        return await _database.KeyDeleteAsync(GetKey(key));
    }

    // Cache operations
    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null)
    {
        var value = await GetStringAsync<T>(key);
        if (value != null)
        {
            return value;
        }

        value = await factory();
        await SetStringAsync(key, value, expiry);
        return value;
    }
}