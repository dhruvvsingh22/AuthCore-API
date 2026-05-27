using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace UsersApi.Services;
public class RedisCacheService : IRedisCacheService
{
    private readonly IDistributedCache _cache;
    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }
    public async Task SetAsync<T> (string key,T value,TimeSpan? expiry=null)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(5)
        };
        var json = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key,json,options);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await _cache.GetStringAsync(key);
        if(json==null)
        {
            return default;
        }
        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task RemoveAsync(string key) => await _cache.RemoveAsync(key);
    public async Task<bool> ExistsAsync(string key) => await _cache.GetStringAsync(key)!=null;
}