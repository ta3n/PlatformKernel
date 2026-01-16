namespace PlatformKernel.Entity.Auditing;

public interface ILoginHistory
{
    long Id { get; }
    string? IpAddress { get; }
    string? Device { get; }
    bool? IsMobile { get; }

    /// <summary>
    /// 日付
    /// </summary>
    DateTime DateTime { get; }
}
