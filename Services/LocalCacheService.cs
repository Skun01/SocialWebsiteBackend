using System;
using Microsoft.Extensions.Caching.Memory;
using SocialWebsite.Interfaces.Services;

namespace SocialWebsite.Services;

public class LocalCacheService : ICacheService
{
private readonly IMemoryCache _memoryCache;

    public LocalCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public T? Get<T>(string key)
    {
        return _memoryCache.TryGetValue(key, out T? value) ? value : default;
    }

    public bool TryGetValue<T>(string key, out T? value)
    {
        return _memoryCache.TryGetValue(key, out value);
    }

    public void Set<T>(string key, T value, TimeSpan? absoluteExpiration = null)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions();
        
        if (absoluteExpiration.HasValue)
        {
            cacheEntryOptions.SetAbsoluteExpiration(absoluteExpiration.Value);
        }
        else
        {
            // Mặc định cache 5 phút
            cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        }

        _memoryCache.Set(key, value, cacheEntryOptions);
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }
}
