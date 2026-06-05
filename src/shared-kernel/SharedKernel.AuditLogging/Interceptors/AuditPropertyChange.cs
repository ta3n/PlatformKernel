namespace SharedKernel.AuditLogging.Interceptors;

internal sealed class AuditPropertyChange
{
    public object? Old { get; init; }

    public object? New { get; init; }
}
