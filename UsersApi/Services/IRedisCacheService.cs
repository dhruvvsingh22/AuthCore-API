namespace UsersApi.Services;

public interface IRedisCacheService
{
    Task SetAsync<T> (string key,T Value,TimeSpan ? expiry = null);
    Task<T?> GetAsync<T>(string key);
    Task RemoveAsync(string key);
    Task<bool>ExistsAsync(string key);
}