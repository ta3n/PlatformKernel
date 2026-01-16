using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace PlatformKernel.UnitOfWork.Interceptors;

/// <summary>
/// Provides an implementation of <see cref="DbConnectionInterceptor"/> that intercepts
/// database connection opening and closing events for DbContext pooling scenarios.
/// </summary>
/// <remarks>
/// This class is designed to log information about the opening and closing of database
/// connections when using a pooled DbContext. It primarily functions to provide detailed
/// diagnostics and monitoring of connection activities.
/// </remarks>
/// <example>
/// This class can be used to track connection lifecycle activities, such as logging when
/// connections are opened or closed. This is helpful in understanding connection usage
/// patterns and diagnosing potential connection-related issues.
/// </example>
public class DbContextPoolingInterceptor(
    ILogger<DbContextPoolingInterceptor> logger
) : DbConnectionInterceptor
{
    /// <summary>
    /// Intercepts the process of opening a database connection, allowing custom logic to execute before the connection is opened.
    /// </summary>
    /// <param name="connection">The database connection that is being opened.</param>
    /// <param name="eventData">Contextual information about the connection event.</param>
    /// <param name="result">The interception result which determines whether the operation should proceed, terminate, or be suppressed.</param>
    /// <returns>
    /// An <see cref="InterceptionResult"/> specifying the result of the operation after interception.
    /// </returns>
    public override InterceptionResult ConnectionOpening(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result
    )
    {
        logger.LogInformation("Opening connection: {ConnectionId}", connection.GetHashCode());

        return base.ConnectionOpening(connection, eventData, result);
    }

    /// <summary>
    /// Intercepts the event of closing a database connection.
    /// Logs the connection closure details.
    /// </summary>
    /// <param name="connection">The database connection that is being closed.</param>
    /// <param name="eventData">The event data associated with the connection closure.</param>
    public override void ConnectionClosed(
        DbConnection connection,
        ConnectionEndEventData eventData
    )
    {
        logger.LogInformation("Closing connection: {ConnectionId}", connection.GetHashCode());

        base.ConnectionClosed(connection, eventData);
    }
}
