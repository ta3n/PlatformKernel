namespace Liberty.Application.Cache.Redis;

public class RedisOptions
{
    /// <summary>
    /// enable
    /// </summary>
    public bool Enable { get; set; }

    /// <summary>
    /// Redis connection
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Key value prefix
    /// </summary>
    public string InstanceName { get; set; }
}
