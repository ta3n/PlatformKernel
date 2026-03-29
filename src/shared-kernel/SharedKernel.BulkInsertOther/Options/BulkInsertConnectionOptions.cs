namespace SharedKernel.BulkInsertOther.Options;

public sealed class BulkInsertConnectionOptions
{
    public string? DirectConnectionString { get; set; }

    public string? OltpConnectionString { get; set; }

    public TimeSpan ImportTimeout { get; set; } = TimeSpan.FromSeconds(30);
}
