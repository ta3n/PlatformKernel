using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace Liberty.Application.Cache;

/// <summary>
/// Redis, SQL Server, Memory Cache
/// </summary>
public interface IDistributedCacheService
{
    void AddCacheKey(string cacheKey);
    Task AddCacheKeyAsync(string cacheKey);

    void DelByPattern(string key);
    Task DelByPatternAsync(string key);

    void DelCacheKey(string cacheKey);
    Task DelCacheKeyAsync(string cacheKey);

    bool Exists(string cacheKey);
    Task<bool> ExistsAsync(string cacheKey);

    List<string>? GetAllCacheKeys();
    Task<List<string>?> GetAllCacheKeysAsync();

    T Get<T>(string cacheKey);
    Task<T?> GetAsync<T>(string cacheKey);

    object? Get(Type type, string cacheKey);
    Task<object?> GetAsync(Type type, string cacheKey);

    string? GetString(string cacheKey);
    Task<string?> GetStringAsync(string cacheKey);

    void Remove(string key);
    Task RemoveAsync(string key);

    void RemoveAll();
    Task RemoveAllAsync();

    void Set<T>(string cacheKey, T value, TimeSpan? expire = null);
    Task SetAsync<T>(string cacheKey, T value);
    Task SetAsync<T>(string cacheKey, T value, TimeSpan expire);

    void SetPermanent<T>(string cacheKey, T value);
    Task SetPermanentAsync<T>(string cacheKey, T value);

    void SetString(string cacheKey, string value, TimeSpan? expire = null);
    Task SetStringAsync(string cacheKey, string value);
    Task SetStringAsync(string cacheKey, string value, TimeSpan expire);

    Task DelByParentKeyAsync(string key);
}

public class DistributedCacheService : IDistributedCacheService
{
    private readonly IDistributedCache _cache;

    public DistributedCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    private byte[] GetBytes<T>(T source)
    {
        return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(source));
    }

    public void AddCacheKey(string cacheKey)
    {
        var res = _cache.GetString(CacheConst.KeyAll);
        var allkeys = string.IsNullOrWhiteSpace(res) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(res);
        if (!allkeys.Any(m => m == cacheKey))
        {
            allkeys.Add(cacheKey);
            _cache.SetString(CacheConst.KeyAll, JsonConvert.SerializeObject(allkeys));
        }
    }

    /// <summary>
    /// Add cache Key
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <returns></returns>
    public async Task AddCacheKeyAsync(string cacheKey)
    {
        var res = await _cache.GetStringAsync(CacheConst.KeyAll);
        var allkeys = string.IsNullOrWhiteSpace(res) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(res);
        if (!allkeys.Any(m => m == cacheKey))
        {
            allkeys.Add(cacheKey);
            await _cache.SetStringAsync(CacheConst.KeyAll, JsonConvert.SerializeObject(allkeys));
        }
    }

    public void DelByPattern(string key)
    {
        var allkeys = GetAllCacheKeys();
        if (allkeys == null) return;

        var delAllkeys = allkeys.Where(u => u.Contains(key)).ToList();
        delAllkeys.ForEach(u => { _cache.Remove(u); });

        //Update all cache keys
        allkeys = allkeys.Where(u => !u.Contains(key)).ToList();
        _cache.SetString(CacheConst.KeyAll, JsonConvert.SerializeObject(allkeys));
    }

    /// <summary>
    /// Delete a certain feature keyword cache
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task DelByPatternAsync(string key)
    {
        var allkeys = await GetAllCacheKeysAsync();
        if (allkeys == null) return;

        var delAllkeys = allkeys.Where(u => u.Contains(key)).ToList();
        delAllkeys.ForEach(u => { _cache.Remove(u); });

        //Update all cache keys
        allkeys = allkeys.Where(u => !u.Contains(key)).ToList();
        await _cache.SetStringAsync(CacheConst.KeyAll, JsonConvert.SerializeObject(allkeys));
    }

    public void DelCacheKey(string cacheKey)
    {
        var res = _cache.GetString(CacheConst.KeyAll);
        var allkeys = string.IsNullOrWhiteSpace(res) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(res);
        if (allkeys.Any(m => m == cacheKey))
        {
            allkeys.Remove(cacheKey);
            _cache.SetString(CacheConst.KeyAll, JsonConvert.SerializeObject(allkeys));
        }
    }

    /// <summary>
    /// Delete cache
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <returns></returns>
    public async Task DelCacheKeyAsync(string cacheKey)
    {
        var res = await _cache.GetStringAsync(CacheConst.KeyAll);
        var allkeys = string.IsNullOrWhiteSpace(res) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(res);
        if (allkeys.Any(m => m == cacheKey))
        {
            allkeys.Remove(cacheKey);
            await _cache.SetStringAsync(CacheConst.KeyAll, JsonConvert.SerializeObject(allkeys));
        }
    }

    public bool Exists(string cacheKey)
    {
        var res = _cache.Get(cacheKey);
        return res != null;
    }

    /// <summary>
    /// Check if the given key exists
    /// </summary>
    /// <param name="cacheKey">Key</param>
    /// <returns></returns>
    public async Task<bool> ExistsAsync(string cacheKey)
    {
        var res = await _cache.GetAsync(cacheKey);
        return res != null;
    }

    public List<string>? GetAllCacheKeys()
    {
        var res = _cache.GetString(CacheConst.KeyAll);
        return string.IsNullOrWhiteSpace(res) ? null : JsonConvert.DeserializeObject<List<string>>(res) ?? null;
    }

    /// <summary>
    /// Get all cache lists
    /// </summary>
    /// <returns></returns>
    public async Task<List<string>?> GetAllCacheKeysAsync()
    {
        var res = await _cache.GetStringAsync(CacheConst.KeyAll);
        return string.IsNullOrWhiteSpace(res) ? null : JsonConvert.DeserializeObject<List<string>>(res);
    }

    public T? Get<T>(string cacheKey)
    {
        var res = _cache.Get(cacheKey);
        return res == null ? default : JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(res));
    }

    /// <summary>
    /// Get cache
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cacheKey"></param>
    /// <returns></returns>
    public async Task<T?> GetAsync<T>(string cacheKey)
    {
        var res = await _cache.GetAsync(cacheKey);
        return res == null ? default : JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(res));
    }

    public object? Get(Type type, string cacheKey)
    {
        var res = _cache.Get(cacheKey);
        return res == null ? default : JsonConvert.DeserializeObject(Encoding.UTF8.GetString(res), type);
    }

    public async Task<object?> GetAsync(Type type, string cacheKey)
    {
        var res = await _cache.GetAsync(cacheKey);
        return res == null ? default : JsonConvert.DeserializeObject(Encoding.UTF8.GetString(res), type);
    }

    public string? GetString(string cacheKey)
    {
        return _cache.GetString(cacheKey);
    }

    /// <summary>
    /// Get cache
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <returns></returns>
    public async Task<string?> GetStringAsync(string cacheKey)
    {
        return await _cache.GetStringAsync(cacheKey);
    }
    public void Remove(string key)
    {
        _cache.Remove(key);
        DelCacheKey(key);
    }

    /// <summary>
    /// Delete cache
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
        await DelCacheKeyAsync(key);
    }

    public void RemoveAll()
    {
        var catches = GetAllCacheKeys();
        foreach (var @catch in catches) Remove(@catch);

        catches.Clear();
        _cache.SetString(CacheConst.KeyAll, JsonConvert.SerializeObject(catches));
    }

    public async Task RemoveAllAsync()
    {
        var catches = await GetAllCacheKeysAsync();
        foreach (var @catch in catches) await RemoveAsync(@catch);

        catches.Clear();
        await _cache.SetStringAsync(CacheConst.KeyAll, JsonConvert.SerializeObject(catches));
    }


    public void Set<T>(string cacheKey, T value, TimeSpan? expire = null)
    {
        _cache.Set(cacheKey, GetBytes(value),
        expire == null
        ? new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6) }
        : new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = expire });

        AddCacheKey(cacheKey);
    }

    /// <summary>
    /// Increase object cache
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task SetAsync<T>(string cacheKey, T value)
    {
        await _cache.SetAsync(cacheKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)),
        new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6) });

        await AddCacheKeyAsync(cacheKey);
    }

    /// <summary>
    /// Add object cache and set expiration time
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="value"></param>
    /// <param name="expire"></param>
    /// <returns></returns>
    public async Task SetAsync<T>(string cacheKey, T value, TimeSpan expire)
    {
        await _cache.SetAsync(cacheKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)),
        new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = expire });

        await AddCacheKeyAsync(cacheKey);
    }

    public void SetPermanent<T>(string cacheKey, T value)
    {
        _cache.Set(cacheKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));
        AddCacheKey(cacheKey);
    }

    public async Task SetPermanentAsync<T>(string cacheKey, T value)
    {
        await _cache.SetAsync(cacheKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));
        await AddCacheKeyAsync(cacheKey);
    }

    public void SetString(string cacheKey, string value, TimeSpan? expire = null)
    {
        if (expire == null)
            _cache.SetString(cacheKey, value, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6) });
        else
            _cache.SetString(cacheKey, value, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = expire });

        AddCacheKey(cacheKey);
    }

    /// <summary>
    /// Add string cache
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task SetStringAsync(string cacheKey, string value)
    {
        await _cache.SetStringAsync(cacheKey, value, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6) });

        await AddCacheKeyAsync(cacheKey);
    }

    /// <summary>
    /// Add string cache and set expiration time
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="value"></param>
    /// <param name="expire"></param>
    /// <returns></returns>
    public async Task SetStringAsync(string cacheKey, string value, TimeSpan expire)
    {
        await _cache.SetStringAsync(cacheKey, value, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = expire });

        await AddCacheKeyAsync(cacheKey);
    }


    /// <summary>
    /// Cache the maximum character data range
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="dataScopeType"></param>
    /// <returns></returns>
    public async Task SetMaxDataScopeType(long userId, int dataScopeType)
    {
        var cacheKey = CacheConst.KeyMaxDataScopeType + userId;
        await SetStringAsync(cacheKey, dataScopeType.ToString());

        await AddCacheKeyAsync(cacheKey);
    }

    /// <summary>
    /// Clear based on parent key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task DelByParentKeyAsync(string key)
    {
        var allkeys = await GetAllCacheKeysAsync();
        if (allkeys == null) return;

        var delAllkeys = allkeys.Where(u => u.StartsWith(key)).ToList();
        delAllkeys.ForEach(Remove);
        //Update all cache keys
        allkeys = allkeys.Where(u => !u.StartsWith(key)).ToList();
        await SetStringAsync(CacheConst.KeyAll, JsonConvert.SerializeObject(allkeys));
    }
}
public class CacheConst
{
    /// <summary>
    /// Maximum role data range cache
    /// </summary>
    public const string KeyMaxDataScopeType = "maxDataScopeType:";

    /// <summary>
    /// All cache keyword sets
    /// </summary>
    public const string KeyAll = "keys";
}