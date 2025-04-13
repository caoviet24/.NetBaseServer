using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IRedisService
    {
        // String operations
        Task<bool> SetStringAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<T> GetStringAsync<T>(string key);
        Task<bool> RemoveAsync(string key);
        
        // Hash operations
        Task<bool> SetHashAsync<T>(string key, string field, T value);
        Task<T> GetHashAsync<T>(string key, string field);
        Task<IDictionary<string, T>> GetAllHashAsync<T>(string key);
        Task<bool> RemoveHashAsync(string key, string field);
        
        // List operations
        Task<long> AddToListAsync<T>(string key, T item);
        Task<IEnumerable<T>> GetListAsync<T>(string key);
        
        // Key operations
        Task<bool> KeyExistsAsync(string key);
        Task<bool> SetExpiryAsync(string key, TimeSpan expiry);
        
        // Cache operations
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null);
    }
}