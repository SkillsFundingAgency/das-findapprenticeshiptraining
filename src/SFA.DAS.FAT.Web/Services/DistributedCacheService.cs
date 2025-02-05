using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Web.Services;

public class DistributedCacheService : IDistributedCacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly IMemoryCache _memoryCache;

    public DistributedCacheService(IDistributedCache distributedCache, IMemoryCache memoryCache)
    {
        _distributedCache = distributedCache;
        _memoryCache = memoryCache;
    }

    public async Task<T> GetOrSetAsync<T>(string cacheKey, Func<Task<T>> getData, TimeSpan cacheDuration)
    {
        if (_memoryCache.TryGetValue(cacheKey, out T localCacheData))
        {
            return localCacheData;
        }

        var cachedData = await _distributedCache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            var distributedCacheData = JsonSerializer.Deserialize<T>(cachedData);

            _memoryCache.Set(cacheKey, distributedCacheData, cacheDuration);

            return distributedCacheData;
        }

        var data = await getData();
        var serializedData = JsonSerializer.Serialize(data);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = cacheDuration
        };

        await _distributedCache.SetStringAsync(cacheKey, serializedData, options);

        _memoryCache.Set(cacheKey, data, cacheDuration);

        return data;
    }

    public async Task RemoveAsync(string cacheKey)
    {
        _memoryCache.Remove(cacheKey);

        await _distributedCache.RemoveAsync(cacheKey);
    }
}
