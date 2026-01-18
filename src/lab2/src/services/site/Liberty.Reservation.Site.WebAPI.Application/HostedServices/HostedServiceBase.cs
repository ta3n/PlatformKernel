using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Site.WebAPI.Application.HostedServices;

public abstract class HostedServiceBase(
    ILogger logger,
    TimeSpan period,
    TimeSpan delay
)
    : IHostedService,
        IDisposable
{
    /// <summary>
    ///     The default delay to use for recurring tasks for the first run after application start-up if no alternative is
    ///     configured.
    /// </summary>
    protected static readonly TimeSpan DefaultDelay = TimeSpan.FromMinutes(3);

    protected readonly ILogger Logger = logger;
    private bool _disposedValue;
    private TimeSpan _period = period;
    private Timer? _timer;

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public virtual Task StartAsync(
        CancellationToken cancellationToken
    )
    {
        using (
            !ExecutionContext.IsFlowSuppressed()
                ? (IDisposable)ExecutionContext.SuppressFlow()
                : null
        )
        {
            _timer = new Timer(ExecuteAsync, null, delay, _period);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task StopAsync(
        CancellationToken cancellationToken
    )
    {
        _period = Timeout.InfiniteTimeSpan;
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Executes the task.
    /// </summary>
    public async void ExecuteAsync(
        object? state
    )
    {
        try
        {
            // First, stop the timer, we do not want tasks to execute in parallel
            _timer?.Change(Timeout.Infinite, 0);

            // Delegate work to method returning a task, that can be called and asserted in a unit test.
            // Without this there can be behaviour where tests pass, but an error within them causes the test
            // running process to crash.
            // Hat-tip: https://stackoverflow.com/a/14207615/489433
            await HandlerExecuteAsync(state);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unhandled exception in recurring hosted service");
        }
        finally
        {
            // Resume now that the task is complete - Note we use period in both because we don't want to execute again after the delay.
            // So first execution is after _delay, and the we wait _period between each
            _timer?.Change(_period, _period);
        }
    }

    protected abstract Task HandlerExecuteAsync(
        object? state
    );

    /// <summary>
    ///     Change the period between operations.
    /// </summary>
    protected void ChangePeriod(
        TimeSpan newPeriod
    )
    {
        _period = newPeriod;
    }

    protected virtual void Dispose(
        bool disposing
    )
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            _timer?.Dispose();
        }

        _disposedValue = true;
    }
}
