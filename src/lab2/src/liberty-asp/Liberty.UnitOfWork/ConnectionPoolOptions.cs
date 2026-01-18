namespace Liberty.UnitOfWork;

/// <summary>
/// Represents configuration options for managing a connection pool in a database context.
/// </summary>
public class ConnectionPoolOptions
{
    /// <summary>
    /// Gets or sets the maximum number of concurrent entries allowed in the connection pool.
    /// This property defines the upper limit on the number of simultaneous connections or operations
    /// that can be handled concurrently, ensuring resource control and preventing overload scenarios.
    /// </summary>
    public int MaximumNumberOfConcurrentEntries { get; set; } = 50;

    /// <summary>
    /// Indicates whether the connection pool is enabled or disabled.
    /// When set to <c>true</c>, the connection pooling feature is activated, allowing concurrency control
    /// for database connections. When set to <c>false</c>, connection pooling is disabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Specifies the maximum time, in milliseconds, to wait for a semaphore slot to become available
    /// before a timeout occurs when attempting to acquire a connection from the pool.
    /// </summary>
    public int WaitTimeout { get; set; } = 3000;
}
