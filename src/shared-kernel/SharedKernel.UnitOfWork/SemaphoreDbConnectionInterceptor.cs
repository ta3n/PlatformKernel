using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace SharedKernel.UnitOfWork;

/// <summary>
/// A database connection interceptor that utilizes a <see cref="SemaphoreSlim"/> to control
/// the number of concurrent database connection openings. This is often used to manage and
/// limit the database connection pool usage in resource-constrained scenarios.
/// </summary>
/// <remarks>
/// This interceptor integrates with the Entity Framework Core database connection pipeline,
/// ensuring that a semaphore is used to throttle connection opening operations while properly
/// handling exceptions and cancellations.
/// </remarks>
public sealed class SemaphoreDbConnectionInterceptor(
    SemaphoreSlim semaphore
) : DbConnectionInterceptor
{
    /// <summary>
    /// Intercepts the asynchronous opening of a database connection, ensuring the operation
    /// respects a semaphore for limiting access and concurrently handles exceptions.
    /// </summary>
    /// <param name="connection">The database connection being opened.</param>
    /// <param name="eventData">Event data associated with the connection opening.</param>
    /// <param name="result">The result of the connection opening interception.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the connection opening operation to complete.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains an <see cref="InterceptionResult"/>.</returns>
    public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var acquired = false;
        try
        {
            await semaphore.WaitAsync(
                TimeSpan.FromSeconds(30),
                cancellationToken
            );
            acquired = true;

            return await base.ConnectionOpeningAsync(
                connection,
                eventData,
                result,
                cancellationToken
            );
        }
        catch (OperationCanceledException)
        {
            if (acquired)
            {
                semaphore.Release();
            }

            throw;
        }
        catch (Exception)
        {
            if (acquired)
            {
                semaphore.Release();
            }

            throw;
        }
    }

    /// <summary>
    /// Intercepts the process of opening a database connection, ensuring that a semaphore is acquired before proceeding.
    /// </summary>
    /// <param name="connection">The database connection to be opened.</param>
    /// <param name="eventData">The event data associated with the connection opening operation.</param>
    /// <param name="result">An instance of <see cref="InterceptionResult"/> that allows suppression or modification of the operation.</param>
    /// <returns>An <see cref="InterceptionResult"/> indicating the outcome of the interception, allowing the original operation to continue or be modified.</returns>
    public override InterceptionResult ConnectionOpening(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result
    )
    {
        var acquired = false;
        try
        {
            acquired = semaphore.Wait(TimeSpan.FromSeconds(30));
            if (!acquired)
            {
                throw new TimeoutException(
                    "Unable to acquire database connection semaphore within the timeout period."
                );
            }

            return base.ConnectionOpening(
                connection,
                eventData,
                result
            );
        }
        catch (OperationCanceledException)
        {
            if (acquired)
            {
                semaphore.Release();
            }

            throw;
        }
        catch (Exception)
        {
            if (acquired)
            {
                semaphore.Release();
            }

            throw;
        }
    }

    /// <summary>
    /// Asynchronously handles the event when a database connection is closed,
    /// releasing a semaphore to allow other operations access to the connection pool.
    /// </summary>
    /// <param name="connection">The database connection that has been closed.</param>
    /// <param name="eventData">The event data related to the connection closure.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public override async Task ConnectionClosedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData
    )
    {
        await base.ConnectionClosedAsync(
            connection,
            eventData
        );

        semaphore.Release();
    }

    /// <summary>
    /// Triggered when a database connection is closed, and releases the semaphore used for connection management.
    /// </summary>
    /// <param name="connection">The database connection that was closed.</param>
    /// <param name="eventData">The event data associated with the connection being closed.</param>
    public override void ConnectionClosed(
        DbConnection connection,
        ConnectionEndEventData eventData
    )
    {
        base.ConnectionClosed(
            connection,
            eventData
        );

        semaphore.Release();
    }
}
