using System.Runtime.CompilerServices;
using Liberty.Cache.Options;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Configuration;

namespace Liberty.Cache.Utils;

/// <summary>
/// Provides extension methods to work with Redis configuration options.
/// </summary>
public static class RedisExtension
{
    // https://github.com/imperugo/StackExchange.Redis.Extensions
    /// <summary>
    /// Converts CacheOptions to a RedisConfiguration instance for use with the StackExchange.Redis.Extensions library.
    /// </summary>
    /// <param name="redisOptions">
    /// An instance of <see cref="CacheOptions"/> that contains the Redis cache configurations,
    /// such as connection string, timeout settings, SSL, and pool size.
    /// </param>
    /// <returns>
    /// A <see cref="RedisConfiguration"/> instance, which includes the mapped settings from the provided CacheOptions.
    /// </returns>
    public static RedisConfiguration ToExtensionRedisConfiguration(
        this CacheOptions redisOptions
    )
    {
        var urlConfiguration = redisOptions.UrlConfiguration;

        var (hosts, password, hasOptionalParameters) = CacheHelper.ParseRedisConnectionString(urlConfiguration);

        var redisConfiguration = new RedisConfiguration
        {
            AbortOnConnectFail = false,
            ConnectTimeout = redisOptions.ConnectTimeout,
            SyncTimeout = redisOptions.SyncTimeout,
            Database = 0,
            Ssl = redisOptions.Ssl,
            ServerEnumerationStrategy = new ServerEnumerationStrategy
            {
                Mode = ServerEnumerationStrategy.ModeOptions.All,
                TargetRole = ServerEnumerationStrategy.TargetRoleOptions.Any,
                UnreachableServerAction = ServerEnumerationStrategy.UnreachableServerActionOptions.Throw
            },
            PoolSize = redisOptions.PoolSize,
            Hosts = [..hosts],
            Password = password
        };

        if (hasOptionalParameters)
        {
            redisConfiguration.ConnectionString = urlConfiguration;
        }

        return redisConfiguration;
    }

    /// <summary>
    /// Converts the given <see cref="CacheOptions"/> instance into a <see cref="ConfigurationOptions"/>
    /// object, applying additional configuration settings for Redis connectivity.
    /// </summary>
    /// <param name="redisOptions">
    /// The <see cref="CacheOptions"/> instance containing initial Redis configuration values.
    /// </param>
    /// <returns>
    /// A <see cref="ConfigurationOptions"/> object representing the configured Redis connection options.
    /// </returns>
    public static ConfigurationOptions ToExtensionConfigureOptions(
        this CacheOptions redisOptions
    )
    {
        var redisConfiguration = redisOptions.ToExtensionRedisConfiguration();
        var configurationOptions = redisConfiguration.ConfigurationOptions;

        configurationOptions.KeepAlive = 300;
        configurationOptions.AsyncTimeout = redisOptions.AsyncTimeout;
        configurationOptions.ConnectRetry = 3;
        configurationOptions.ReconnectRetryPolicy = new ExponentialRetry(2000);
        configurationOptions.AllowAdmin = false;
        configurationOptions.AbortOnConnectFail = false;
        configurationOptions.AllowAdmin = false;
        configurationOptions.SocketManager = SocketManager.ThreadPool;

        return configurationOptions;
    }

    /// <summary>
    /// Asynchronously scans Redis keys matching a specified pattern, yielding results in a paginated manner.
    /// </summary>
    /// <param name="muxer">
    /// The Redis connection multiplexer used to interact with the Redis server.
    /// </param>
    /// <param name="pattern">
    /// The pattern to match Redis keys against (e.g., "prefix:*").
    /// </param>
    /// <param name="pageSize">
    /// The number of keys to retrieve per page. Defaults to 200.
    /// </param>
    /// <param name="maxKeys">
    /// The maximum number of keys to yield. Defaults to 5,000.
    /// </param>
    /// <param name="maxIters">
    /// The maximum number of iterations to perform when scanning keys. Defaults to 100.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests. Defaults to <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// An asynchronous enumerable of Redis keys matching the specified pattern.
    /// </returns>
    public static async IAsyncEnumerable<RedisKey> ScanKeysAsync(
        IConnectionMultiplexer muxer,
        string pattern,
        int pageSize = 200,
        int maxKeys = 5_000,
        int maxIters = 100,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        var yielded = 0;
        var iters = 0;

        foreach (var endpoint in muxer.GetEndPoints())
        {
            var server = muxer.GetServer(endpoint);
            if (!server.IsConnected)
            {
                continue;
            }

            await foreach (var key in server.KeysAsync(
                    pattern: pattern,
                    pageSize: pageSize
                )
                .WithCancellation(cancellationToken))
            {
                yield return key;
                yielded++;

                if (yielded >= maxKeys)
                {
                    yield break;
                }
            }

            iters++;
            if (iters >= maxIters)
            {
                yield break;
            }
        }
    }

    /// <summary>
    /// Asynchronously scans Redis hash fields for keys matching a specified pattern and retrieves their values.
    /// </summary>
    /// <param name="muxer">
    /// The Redis connection multiplexer used to interact with the Redis server.
    /// </param>
    /// <param name="pattern">
    /// The pattern to match Redis keys against (e.g., "prefix:*").
    /// </param>
    /// <param name="field">
    /// The specific hash field to retrieve values for.
    /// </param>
    /// <param name="pageSize">
    /// The number of keys to retrieve per page. Defaults to 200.
    /// </param>
    /// <param name="maxKeys">
    /// The maximum number of keys to process. Defaults to 3,000.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests. Defaults to <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A dictionary mapping Redis keys to their corresponding hash field values.
    /// </returns>
    public static async Task<Dictionary<string, RedisValue>> ScanHashFieldAsync(
        IConnectionMultiplexer muxer,
        string pattern,
        RedisValue field,
        int pageSize = 200,
        int maxKeys = 3_000,
        CancellationToken cancellationToken = default
    )
    {
        var db = muxer.GetDatabase();
        var result = new Dictionary<string, RedisValue>();
        var count = 0;

        await foreach (var key in ScanKeysAsync(
                muxer,
                pattern,
                pageSize,
                maxKeys,
                cancellationToken: cancellationToken
            ))
        {
            var value = await db.HashGetAsync(key, field);
            result[key!] = value;
            count++;

            if (count >= maxKeys)
            {
                break;
            }
        }

        return result;
    }

    /// <summary>
    /// Asynchronously deletes Redis keys matching a specified pattern in batches.
    /// </summary>
    /// <param name="muxer">
    /// The Redis connection multiplexer used to interact with the Redis server.
    /// </param>
    /// <param name="pattern">
    /// The pattern to match Redis keys against (e.g., "prefix:*").
    /// </param>
    /// <param name="pageSize">
    /// The number of keys to retrieve per page. Defaults to 500.
    /// </param>
    /// <param name="deleteBatchSize">
    /// The number of keys to delete in each batch. Defaults to 200.
    /// </param>
    /// <param name="maxKeys">
    /// The maximum number of keys to delete. Defaults to 10,000.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests. Defaults to <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// The total number of keys deleted.
    /// </returns>
    public static async Task<long> ClearByPatternAsync(
        IConnectionMultiplexer muxer,
        string pattern,
        int pageSize = 500,
        int deleteBatchSize = 200,
        int maxKeys = 10_000,
        CancellationToken cancellationToken = default
    )
    {
        var db = muxer.GetDatabase();
        var deleted = 0L;
        var batch = new List<RedisKey>(deleteBatchSize);

        await foreach (var key in ScanKeysAsync(
                muxer,
                pattern,
                pageSize,
                maxKeys,
                cancellationToken: cancellationToken
            ))
        {
            batch.Add(key);

            if (batch.Count < deleteBatchSize)
            {
                continue;
            }

            deleted += await UnlinkBatchAsync(db, batch);
            batch.Clear();
        }

        if (batch.Count > 0)
        {
            deleted += await UnlinkBatchAsync(db, batch);
        }

        return deleted;

        // Deletes a batch of Redis keys using the UNLINK command, falling back to DEL if UNLINK fails.
        async Task<long> UnlinkBatchAsync(
            IDatabase database,
            List<RedisKey> keys
        )
        {
            try
            {
                // UNLINK (non-blocking)
                return await database.ExecuteAsync("UNLINK", keys.Select(k => k).ToArray())
                    .ContinueWith(t => (long)t.Result, cancellationToken);
            }
            catch
            {
                // Fallback DEL
                return await database.KeyDeleteAsync(keys.ToArray());
            }
        }
    }
}
