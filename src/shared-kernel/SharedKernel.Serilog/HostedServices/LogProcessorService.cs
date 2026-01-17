using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SharedKernel.Serilog.HostedServices;

/// <summary>
/// A background service that processes and logs messages dequeued
/// from a shared <see cref="ConcurrentQueue{T}"/>.
/// </summary>
/// <remarks>
/// The <see cref="LogProcessorService"/> runs on a hosted background thread
/// and continuously retrieves log messages from the provided
/// <see cref="ConcurrentQueue{T}"/>. Each log message is logged using
/// an instance of <see cref="ILogger{TCategoryName}"/>. If no messages
/// are available in the queue, the service waits for a specified delay
/// before attempting to dequeue messages again.
/// </remarks>
/// <example>
/// Designed to integrate with the application's dependency injection container.
/// Can be configured and registered through the extension methods, such as
/// `ConfigureLogProcessorService(IServiceCollection services)` in the associated codebase.
/// </example>
/// <threadsafety>
/// The log queue used by this service is threadsafe and allows concurrent access
/// across multiple producers (enqueue operations) while the consumer (this service)
/// dequeues messages.
/// </threadsafety>
/// <seealso cref="BackgroundService" />
public class LogProcessorService(
    ConcurrentQueue<string> logQueue,
    ILogger<LogProcessorService> logger
)
    : BackgroundService
{
    /// <summary>
    /// Executes the background processing of dequeuing log messages from a concurrent queue
    /// and logging them, until the task is canceled.
    /// </summary>
    /// <param name="stoppingToken">A token that signals cancellation of the background processing.</param>
    /// <returns>A Task representing the asynchronous execution of the method.</returns>
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken
    )
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            while (logQueue.TryDequeue(out var logMessage))
            {
                await Task.Run(
                    () => logger.LogInformation("{LogMessage}", logMessage),
                    stoppingToken
                );
            }

            await Task.Delay(500, stoppingToken);
        }
    }
}
