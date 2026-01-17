namespace SharedKernel.UnitOfWork;

/// <summary>
/// Interface for managing database connection concurrency via a semaphore mechanism.
/// It provides functionality to control access to connections in a controlled manner
/// by limiting the number of concurrent connection operations.
/// </summary>
public interface IDbConnectionManager
{
    /// <summary>
    /// Gets a value indicating whether the functionality of the connection manager is enabled.
    /// </summary>
    /// <remarks>
    /// This property determines if specific operations related to the management of database connections
    /// (such as waiting for or releasing connections) are activated. It typically reflects an option set
    /// through the configuration.
    /// </remarks>
    bool Enabled { get; }

    /// Waits asynchronously for a database connection to become available.
    /// Utilizes a semaphore to limit the number of concurrent connections.
    /// <param name="cancellationToken">
    /// A cancellation token that can be used to cancel the wait operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous wait operation.
    /// </returns>
    Task WaitAsync(
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Waits asynchronously to acquire access to the database connection according to the configured connection pool options.
    /// </summary>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous wait operation.</returns>
    /// <exception cref="TimeoutException">
    /// Thrown when the wait timeout defined in the connection pool options is exceeded before access can be acquired.
    /// </exception>
    Task WaitAsync(
        Func<Task> func,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Blocks the calling thread until a semaphore slot becomes available.
    /// </summary>
    /// <remarks>
    /// This method decreases the count of the semaphore, blocking if no slots
    /// are available. It works in conjunction with the semaphore used for
    /// managing the concurrency limits.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">
    /// Thrown when the semaphore has been disposed.
    /// </exception>
    /// <exception cref="SemaphoreFullException">
    /// Thrown if the semaphore is released to full capacity.
    /// </exception>
    void Wait();

    /// <summary>
    /// Releases a semaphore, incrementing the semaphore's current count by one.
    /// This allows other threads or tasks that are waiting for the semaphore to enter.
    /// </summary>
    void Release();
}
