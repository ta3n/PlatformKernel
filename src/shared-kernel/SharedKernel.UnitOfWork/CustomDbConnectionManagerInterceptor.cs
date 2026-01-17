using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace SharedKernel.UnitOfWork;

/// <summary>
/// A custom database connection interceptor that integrates with a connection manager
/// to control connection concurrency and ensure proper handling during connection lifecycle events.
/// </summary>
/// <remarks>
/// This class extends <see cref="Microsoft.EntityFrameworkCore.Diagnostics.DbConnectionInterceptor"/>
/// and overrides relevant methods to interact with an <see cref="IDbConnectionManager"/>
/// for controlling database connection access. It provides mechanisms to handle connection opening,
/// failure, and closing events in both synchronous and asynchronous contexts, ensuring that
/// connection operations respect concurrency limits or resource constraints managed by the
/// <see cref="IDbConnectionManager"/>.
/// </remarks>
public sealed class CustomDbConnectionManagerInterceptor(
    IDbConnectionManager dbConnectionManager
) : DbConnectionInterceptor
{
    /// <summary>
    /// Intercepts the asynchronous opening of a database connection.
    /// If connection management is enabled, it waits asynchronously
    /// for permission to proceed with the connection operation.
    /// </summary>
    /// <param name="connection">
    /// The <see cref="DbConnection"/> being opened.
    /// </param>
    /// <param name="eventData">
    /// Contextual information about the connection event.
    /// </param>
    /// <param name="result">
    /// The initial <see cref="InterceptionResult"/> for the operation.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> that can be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> containing the <see cref="InterceptionResult"/>
    /// which may be modified by the interceptor.
    /// </returns>
    public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result,
        CancellationToken cancellationToken = default
    )
    {
        if (dbConnectionManager.Enabled)
        {
            await dbConnectionManager.WaitAsync(cancellationToken);
        }

        return await base.ConnectionOpeningAsync(
            connection,
            eventData,
            result,
            cancellationToken
        );
    }

    /// <summary>
    /// Executes logic when a database connection is being opened via a database interceptor.
    /// </summary>
    /// <param name="connection">The database connection being opened.</param>
    /// <param name="eventData">The event data associated with the connection opening.</param>
    /// <param name="result">The interception result allowing modification or suppression of the connection opening.</param>
    /// <returns>The interception result, potentially modified by the logic in the interceptor.</returns>
    public override InterceptionResult ConnectionOpening(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result
    )
    {
        if (dbConnectionManager.Enabled)
        {
            dbConnectionManager.Wait();
        }

        return base.ConnectionOpening(
            connection,
            eventData,
            result
        );
    }

    /// <summary>
    /// Handles the asynchronous operation when a database connection attempt fails.
    /// Releases the resources held by the connection manager if enabled.
    /// </summary>
    /// <param name="connection">The database connection that failed.</param>
    /// <param name="eventData">The details of the connection failure event.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task ConnectionFailedAsync(
        DbConnection connection,
        ConnectionErrorEventData eventData,
        CancellationToken cancellationToken = new()
    )
    {
        if (dbConnectionManager.Enabled)
        {
            dbConnectionManager.Release();
        }

        await base.ConnectionFailedAsync(
            connection,
            eventData,
            cancellationToken
        );
    }

    /// <summary>
    /// Invoked when a connection attempt to the database fails.
    /// This method allows custom behavior to handle failed connection scenarios.
    /// </summary>
    /// <param name="connection">The database connection that encountered the failure.</param>
    /// <param name="eventData">The error event data that provides details about the connection failure.</param>
    public override void ConnectionFailed(
        DbConnection connection,
        ConnectionErrorEventData eventData
    )
    {
        if (dbConnectionManager.Enabled)
        {
            dbConnectionManager.Release();
        }

        base.ConnectionFailed(connection, eventData);
    }

    /// <summary>
    /// Invoked asynchronously when a database connection is closed.
    /// Releases resources managed by the connection manager if enabled.
    /// </summary>
    /// <param name="connection">The database connection that has been closed.</param>
    /// <param name="eventData">The event data associated with the connection closure.</param>
    /// <returns>A task that represents the asynchronous completion of the operation.</returns>
    public override async Task ConnectionClosedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData
    )
    {
        if (dbConnectionManager.Enabled)
        {
            dbConnectionManager.Release();
        }

        await base.ConnectionClosedAsync(connection, eventData);
    }

    /// <summary>
    /// Handles the closure of a database connection and releases the semaphore control
    /// when the custom connection manager is enabled.
    /// </summary>
    /// <param name="connection">The database connection that is being closed.</param>
    /// <param name="eventData">Contextual event data related to the connection being closed.</param>
    public override void ConnectionClosed(
        DbConnection connection,
        ConnectionEndEventData eventData
    )
    {
        if (dbConnectionManager.Enabled)
        {
            dbConnectionManager.Release();
        }

        base.ConnectionClosed(connection, eventData);
    }
}
