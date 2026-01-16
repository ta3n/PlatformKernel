using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace PlatformKernel.Service.Base.Application.Contexts.Interceptors;

/// <summary>
/// An EF Core command interceptor that monitors the performance of database queries and logs warnings
/// for queries that execute longer than a specified threshold.
/// </summary>
/// <remarks>
/// This class extends <see cref="DbCommandInterceptor"/> and is primarily used to identify long-running database queries,
/// enabling performance optimization and monitoring within an application.
/// </remarks>
public class PerformanceInterceptor(
    ILogger<PerformanceInterceptor> logger
) : DbCommandInterceptor
{
    /// <summary>
    /// Intercepts the execution of a database command and logs a warning if the command execution exceeds a predefined duration threshold.
    /// </summary>
    /// <param name="command">The database command being executed.</param>
    /// <param name="eventData">Event data related to the executed command, including timing details.</param>
    /// <param name="result">The data reader resulting from the executed command.</param>
    /// <param name="cancellationToken">An optional token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the executed command.</returns>
    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default
    )
    {
        if (eventData.Duration.TotalMilliseconds > 2000)
        {
            LogLongQuery(command, eventData);
        }

        var response = base.ReaderExecutedAsync(
            command,
            eventData,
            result,
            cancellationToken
        );

        return response;
    }

    /// <summary>
    /// Intercepts a database command execution after the command has been executed and the reader is returned.
    /// It can optionally log queries that exceed a specified duration threshold.
    /// </summary>
    /// <param name="command">The <see cref="DbCommand"/> object representing the database command that was executed.</param>
    /// <param name="eventData">The event data containing information about the executed command.</param>
    /// <param name="result">The resulting <see cref="DbDataReader"/> returned from the executed command.</param>
    /// <returns>The <see cref="DbDataReader"/> that represents the result of the executed command.</returns>
    public override DbDataReader ReaderExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result
    )
    {
        if (eventData.Duration.TotalMilliseconds > 2000)
        {
            LogLongQuery(command, eventData);
        }

        var response = base.ReaderExecuted(
            command,
            eventData,
            result
        );

        return response;
    }

    /// <summary>
    /// Logs a warning message for queries that exceed a predefined execution duration threshold.
    /// </summary>
    /// <param name="command">The <see cref="DbCommand"/> instance representing the query being executed.</param>
    /// <param name="eventData">The <see cref="CommandExecutedEventData"/> containing details about the executed command and its duration.</param>
    private void LogLongQuery(
        DbCommand command,
        CommandExecutedEventData eventData
    )
    {
        logger.LogWarning(
            "Long query: {CommandText}. Duration: {TotalMilliseconds} ms",
            command.CommandText,
            eventData.Duration.TotalMilliseconds
        );
    }
}
