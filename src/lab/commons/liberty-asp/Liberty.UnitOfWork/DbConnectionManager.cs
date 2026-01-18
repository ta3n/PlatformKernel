using Serilog;

namespace Liberty.UnitOfWork;

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
        Func<Task> func
    )
    {
        Log.Information("Semaphore current count {SemaphoreCurrentCount}", _semaphore.CurrentCount);
        await _semaphore.WaitAsync();

        try
        {
            await func();
        }
        catch (Exception e)
        {
            Log.Information("DbConnectionManager > WaitAsync > Error: {Message}", e.Message);
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task WaitAsync()
    {
        Log.Information("Semaphore current count {SemaphoreCurrentCount}", _semaphore.CurrentCount);
        await _semaphore.WaitAsync();
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
