namespace Liberty.UnitOfWork;

public interface IDbConnectionManager
{
    bool Enabled { get; }

    Task WaitAsync();

    Task WaitAsync(
        Func<Task> func
    );

    void Wait();

    void Release();
}
