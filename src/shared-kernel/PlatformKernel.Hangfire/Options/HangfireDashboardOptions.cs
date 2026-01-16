namespace PlatformKernel.Hangfire.Options;

/// <summary>
/// Represents configuration options for the Hangfire dashboard integration.
/// </summary>
public record HangfireDashboardOptions(
    bool Enabled,
    string? DashboardUrl,
    string? Username,
    string? Password,
    bool IsReadOnly
);
