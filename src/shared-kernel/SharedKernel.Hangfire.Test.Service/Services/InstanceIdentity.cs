namespace SharedKernel.Hangfire.Test.Service.Services;

internal static class InstanceIdentity
{
    public static string Resolve(
        string? configuredInstanceId
    )
    {
        if (!string.IsNullOrWhiteSpace(configuredInstanceId))
        {
            return configuredInstanceId;
        }

        return $"{Environment.MachineName}-{Environment.ProcessId}";
    }
}
