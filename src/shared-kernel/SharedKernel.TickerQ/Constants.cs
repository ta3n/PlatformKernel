namespace SharedKernel.TickerQ;

/// <summary>
/// Constant values used throughout the TickerQ module.
/// </summary>
internal static class Constants
{
    /// <summary>
    /// Configuration section names.
    /// </summary>
    public static class ConfigurationSections
    {
        public const string TickerQStorage = "TickerQStorage";
        public const string TickerQServer = "TickerQServer";
        public const string TickerQDashboard = "TickerQDashboard";
        public const string TickerQBasicAuth = "TickerQBasicAuth";
    }

    /// <summary>
    /// Default configuration values.
    /// </summary>
    public static class Defaults
    {
        public const int MaxConcurrency = 4;
        public const int DefaultRetries = 3;
        public const string BootstrapLockName = "tickerq:bootstrap";
        public const int BootstrapLockTimeoutInSeconds = 180;
        public const string DashboardPath = "tickerq-dashboard";
    }

    /// <summary>
    /// Error messages.
    /// </summary>
    public static class ErrorMessages
    {
        public const string MissingConnectionString = "Missing connection string '{0}' for TickerQ {1} storage.";

        public const string UnsupportedStorageProvider =
            "Unsupported TickerQ storage provider '{0}'. Supported values are 'EntityFramework' and 'Redis'.";

        public const string DashboardUrlRequired =
            "TickerQDashboard:DashboardUrl must be configured when TickerQ dashboard is enabled.";

        public const string DashboardAuthRequired =
            "TickerQDashboard:Username and TickerQDashboard:Password must be configured when basic auth is enabled.";

        public const string InvalidJobDefinition = "Invalid job definition: {0}";
        public const string JobNotFound = "Job with ID '{0}' was not found.";
    }
}
