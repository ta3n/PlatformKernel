using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Reflection;

namespace Liberty.Application.Cache;

public interface IMemoryCacheService
{
    T GetCache<T>(string key) where T : class;
    IEnumerable<string> GetAllKey<T>() where T : class;
    void SetCache<T>(string key, T value, MemoryCacheEntryOptions options) where T : class;
    void ClearCache(string key);
}
public class MemoryCacheService : IMemoryCacheService
{
    protected IMemoryCache _memoryCache;
    public MemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }
    public T GetCache<T>(string key) where T : class
    {
        _memoryCache.TryGetValue(key, out T cachedResponse);
        return cachedResponse;
    }

    public IEnumerable<string> GetAllKey<T>() where T : class
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var coherentState = _memoryCache.GetType().GetField("_coherentState", flags).GetValue(_memoryCache);
        var entries = coherentState.GetType().GetField("_entries", flags).GetValue(coherentState);
        var cacheItems = entries as IDictionary;
        var keys = new List<string>();
        if (cacheItems == null) return keys;
        foreach (DictionaryEntry cacheItem in cacheItems)
        {
            keys.Add(cacheItem.Key.ToString());
        }
        return keys;
    }


    public void SetCache<T>(string key, T value, MemoryCacheEntryOptions options) where T : class
    {
        _memoryCache.Set(key, value, options);
    }

    public void ClearCache(string key)
    {
        _memoryCache.Remove(key);
    }
}
