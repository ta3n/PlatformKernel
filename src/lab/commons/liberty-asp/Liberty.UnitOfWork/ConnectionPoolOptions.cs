namespace Liberty.UnitOfWork;

public class ConnectionPoolOptions
{
    public int MaximumNumberOfConcurrentEntries { get; set; } = 50;
    public bool Enabled { get; set; } = true;
}
