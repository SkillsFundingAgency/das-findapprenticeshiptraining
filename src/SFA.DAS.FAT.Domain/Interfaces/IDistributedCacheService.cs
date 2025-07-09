using System;
using System.Threading.Tasks;

namespace SFA.DAS.FAT.Domain.Interfaces;

public interface IDistributedCacheService
{
    Task<T> GetAsync<T>(string key);
    Task<T> GetOrSetAsync<T>(string cacheKey, Func<Task<T>> getData, TimeSpan cacheDuration);
    Task RemoveAsync(string cacheKey);
    Task SetAsync<T>(string key, T value, TimeSpan cacheDuration);
}
