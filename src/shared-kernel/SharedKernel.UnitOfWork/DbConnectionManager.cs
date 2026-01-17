using Serilog;

namespace SharedKernel.UnitOfWork;

public sealed class DbConnectionManager : IDbConnectionManager
{
    private readonly ConnectionPoolOptions _connectionPoolOptions;
    private readonly SemaphoreSlim _semaphore;

    public bool Enabled => _connectionPoolOptions.Enabled;

    public DbConnectionManager(
        ConnectionPoolOptions connectionPoolOptions
    )
    {
        _connectionPoolOptions = connectionPoolOptions;
        _semaphore = new SemaphoreSlim(
            connectionPoolOptions.MaximumNumberOfConcurrentEntries,
            connectionPoolOptions.MaximumNumberOfConcurrentEntries
        );

        Log.Information(
            "Semaphore MaximumNumberOfConcurrentEntries: {MaximumNumberOfConcurrentEntries}",
            connectionPoolOptions.MaximumNumberOfConcurrentEntries
        );
    }

    public async Task WaitAsync(
        Func<Task> func,
        CancellationToken cancellationToken = default
    )
    {
        if (!await _semaphore.WaitAsync(_connectionPoolOptions.WaitTimeout, cancellationToken))
        {
            throw new TimeoutException("Timeout waiting for a database connection.");
        }

        try
        {
            await func();
        }
        catch (Exception ex)
        {
            Log.Error(
                ex,
                "DbConnectionManager > WaitAsync > Error: {Message}",
                ex.Message
            );
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task WaitAsync(
        CancellationToken cancellationToken = default
    )
    {
        Log.Information(
            "Semaphore current count {SemaphoreCurrentCount}",
            _semaphore.CurrentCount
        );
        if (!await _semaphore.WaitAsync(_connectionPoolOptions.WaitTimeout, cancellationToken))
        {
            throw new TimeoutException("Timeout waiting for a database connection.");
        }
    }

    public void Wait()
    {
        Log.Information("Semaphore current count {SemaphoreCurrentCount}", _semaphore.CurrentCount);
        _semaphore.Wait();
    }

    public void Release()
    {
        _semaphore.Release();
    }
}
