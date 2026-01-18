namespace SharedKernel.Entity.Auditing;

public interface ILoginHistory
{
    long Id { get; }
    string? IpAddress { get; }
    string? Device { get; }
    bool? IsMobile { get; }
    DateTime DateTime { get; }
}
