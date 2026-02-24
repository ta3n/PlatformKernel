using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using SharedKernel.AppShared.Utils;
using StackExchange.Redis.Extensions.Core.Configuration;

namespace SharedKernel.Cache.Utils;

/// <summary>
/// Provides helper methods for working with caching, including serialization, deserialization,
/// generating cache keys, and computing hashes.
/// </summary>
public static class CacheHelper
{
    /// <summary>
    /// Serializes an object to its JSON representation using camel case property naming and ensures that reference loops are ignored.
    /// </summary>
    /// <param name="obj">The object to serialize. Can be null.</param>
    /// <returns>A JSON string representation of the object, or null if the input object is null.</returns>
    public static string? Serialize(
        object? obj
    )
    {
        return obj is null
            ? null
            : JsonSerializer.Serialize(obj, JsonSettings.OptimizedSystemTextJson);
    }

    /// <summary>
    /// Deserializes a JSON string to an object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the object to which the JSON string will be deserialized.</typeparam>
    /// <param name="json">The JSON string to deserialize. If the string is null, empty, or whitespace, the default value of the specified type is returned.</param>
    /// <returns>The deserialized object of the specified type, or the default value of the specified type if the input string is null, empty, or whitespace.</returns>
    public static T? Deserialize<T>(
        string json
    )
    {
        return string.IsNullOrWhiteSpace(json)
            ? default
            : JsonSerializer.Deserialize<T>(json, JsonSettings.OptimizedSystemTextJson);
    }

    /// <summary>
    /// Parses a Redis connection string into its components, including hosts, password, and a flag indicating
    /// the presence of optional parameters in the connection string.
    /// </summary>
    /// <param name="connectionString">
    /// The Redis connection string to parse. This string may include host and port pairs, a password,
    /// and any optional parameters.
    /// </param>
    /// <returns>
    /// A tuple containing the following:
    /// 1. A list of <see cref="RedisHost"/> objects, where each object represents a host and its corresponding port.
    /// 2. A string representing the password, if provided in the connection string.
    /// 3. A boolean indicating whether the connection string contains optional parameters.
    /// </returns>
    public static (List<RedisHost> Hosts, string? Password, bool HasOptionalParameters) ParseRedisConnectionString(
        string? connectionString
    )
    {
        var hosts = new List<RedisHost>();
        string? password = null;
        var hasOptionalParameters = false;

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return (hosts, password, hasOptionalParameters);
        }

        var parts = connectionString.Split(',');

        foreach (var part in parts)
        {
            if (part.StartsWith("password=", StringComparison.OrdinalIgnoreCase))
            {
                password = part.Split('=')[1];
            }
            else if (!part.Contains('='))
            {
                var hostParts = part.Split(':');
                hosts.Add(
                    new RedisHost
                    {
                        Host = hostParts[0],
                        Port = int.Parse(hostParts[1])
                    }
                );
            }
            else
            {
                hasOptionalParameters = true;
            }
        }

        return (hosts, password, hasOptionalParameters);
    }

    /// <summary>
    /// Generates a cache key string based on the entity name and additional keys provided.
    /// </summary>
    /// <param name="entityName">The name of the entity for which the cache key is generated.</param>
    /// <param name="keys">Additional keys or identifiers to include in the cache key.</param>
    /// <returns>A string representing the generated cache key.</returns>
    public static string GetCacheKeyByEntity(
        string entityName,
        params string[] keys
    )
    {
        return $"{entityName}:{string.Join(':', keys)}";
    }

    /// <summary>
    /// Generates a cache key by combining the provided string parameters with a delimiter.
    /// </summary>
    /// <param name="keys">A collection of string parameters used to construct the cache key.</param>
    /// <returns>A concatenated string that serves as the unique cache key.</returns>
    public static string GetCacheKeyByParameters(
        params string[] keys
    )
    {
        var cacheKeyData = string.Join(':', keys);

        return cacheKeyData;
    }

    /// <summary>
    /// Computes a hash for the specified input string with an optional length.
    /// </summary>
    /// <param name="input">The input string to compute the hash for.</param>
    /// <param name="length">The length of the resulting hash. Defaults to 16.</param>
    /// <returns>A string representing the computed hash of the input.</returns>
    public static string ComputeHash(
        string input,
        int length = 16
    )
    {
        return ComputeSha256Hash(input, length);
    }

    /// <summary>
    /// Computes a hash based on the provided input strings, separator, and length.
    /// The resulting hash is typically used for generating unique keys.
    /// </summary>
    /// <param name="inputs">An array of input strings used to generate the hash.</param>
    /// <param name="separator">A character used to join the input strings before hashing. The default value is '-'.</param>
    /// <param name="length">The desired length of the computed hash. The default value is 16.</param>
    /// <returns>A string representing the computed hash with the specified length.</returns>
    public static string ComputeHash(
        string[] inputs,
        char separator = '-',
        int length = 16
    )
    {
        return ComputeSha256Hash(inputs, separator, length);
    }

    /// <summary>
    /// Computes the SHA-256 hash for the given input string and returns a truncated hexadecimal representation of the hash.
    /// </summary>
    /// <param name="input">The input string to compute the hash for.</param>
    /// <param name="length">The length of the returned hash string. Defaults to 16 if not provided.</param>
    /// <returns>A string representing the truncated hexadecimal SHA-256 hash of the input.</returns>
    private static string ComputeSha256Hash(
        string input,
        int length = 16
    )
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLower()[..length];
    }

    /// <summary>
    /// Computes the SHA-256 hash for a concatenated string of the provided inputs, separated by the specified character,
    /// and returns a hexadecimal representation of the hash truncated to the specified length.
    /// </summary>
    /// <param name="inputs">An array of input strings to be concatenated and hashed.</param>
    /// <param name="separator">The character used to join the input strings. Defaults to '-' if not provided.</param>
    /// <param name="length">The length of the resulting hash string. Defaults to 16 if not provided.</param>
    /// <returns>A hexadecimal string representation of the truncated SHA-256 hash computed from the concatenated inputs.</returns>
    private static string ComputeSha256Hash(
        string[] inputs,
        char separator = '-',
        int length = 16
    )
    {
        var cacheKeyData = string.Join(separator, inputs);

        return ComputeSha256Hash(cacheKeyData, length);
    }
}
