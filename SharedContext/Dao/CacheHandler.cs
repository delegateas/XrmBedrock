using Microsoft.Extensions.Logging;
using System;
using System.Runtime.Caching;

namespace SharedContext.Dao;

internal class CacheHandler
{
    public const int _cacheExpirationTimeInMinutes = 60;
    private readonly MemoryCache _cache;
    private readonly ILogger logger;

    public CacheHandler(MemoryCache cache, ILogger logger)
    {
        _cache = cache;
        this.logger = logger;
    }

    public T GetOrCreate<T>(string key, Func<T> factory)
    {
        T value = GetCachedValue<T>(key);
        if (value == null)
        {
            // Stop logging as soon as confidence in the caching mekanism is established among the team
            logger.LogTrace($"Cache miss on '{key}'");
            value = factory.Invoke();
            if (value != null)
                _cache.Add(key, value, DateTime.Now.AddMinutes(_cacheExpirationTimeInMinutes));
        }
        else
            // Stop logging as soon as confidence in the caching mekanism is established among the team
            logger.LogTrace($"Cache hit on '{key}'");
        return value;
    }

    private T GetCachedValue<T>(string key)
    {
        T value;
        try
        {
            value = (T)_cache.Get(key);
        }
        catch (InvalidCastException ice)
        {
            logger?.LogWarning($"InvalidCastException thrown on getting value for key '{key}' from the cache. Details: {ice.Message}");
            // InvalidCastException may occur if the plugin assembly is loaded multiple times on the same XRM node.
            // Types from different copies of the same plugin are considered different even though they are basically the same.
            // See https://stackoverflow.com/questions/2500280/invalidcastexception-for-two-objects-of-the-same-type
            // By setting value = default (which is typically null), we force an update of the cached value
            value = default(T);
        }
        return value;
    }

    /// <summary>
    /// The use of this method is solely for testing purposes
    /// </summary>
    /// <param name="key"></param>
    public void ClearItemFromCache(string key)
    {
        _cache.Remove(key);
    }

    /// <summary>
    /// The use of this method is solely for testing purposes
    /// </summary>
    /// <returns></returns>
    public string GetGeneralCacheInfo()
    {
        return $"Number of items in the cache: {_cache.GetCount()}. CacheMemoryLimit (MB): {_cache.CacheMemoryLimit / 1024 / 1024}";
    }

    /// <summary>
    /// The use of this method is solely for testing purposes
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string PeekAnItemInTheCache(string key)
    {
        var value = _cache.Get(key);
        if (value == null) return $"No value for key '{key}' found in cache";
        string valueStr = null;
        if (value.GetType() == typeof(string)) valueStr = value.ToString();
        if (string.IsNullOrEmpty(valueStr)) return $"Key '{key}' found in cache with type '{value.GetType()}'";
        return $"Key '{key}' found in cache with type '{value.GetType()}' and value '{valueStr}'";
    }
}
