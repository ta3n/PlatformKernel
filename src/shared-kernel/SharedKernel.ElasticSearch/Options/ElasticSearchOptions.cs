namespace SharedKernel.ElasticSearch.Options;

/// <summary>
/// Represents configuration options for the ElasticSearch plugin.
/// </summary>
public sealed class ElasticSearchOptions
{
    /// <summary>
    /// Gets the default configuration section name for the plugin.
    /// </summary>
    public const string SectionName = "ElasticSearch";

    /// <summary>
    /// Gets or sets a value indicating whether the ElasticSearch integration is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the ElasticSearch endpoint used for self-hosted clusters.
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the Elastic Cloud identifier.
    /// </summary>
    public string? CloudId { get; set; }

    /// <summary>
    /// Gets or sets the username used for basic authentication.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the password used for basic authentication.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the raw API key used for token authentication.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the certificate fingerprint used to validate the server certificate.
    /// </summary>
    public string? CertificateFingerprint { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether server certificate validation should be bypassed.
    /// Intended for local development and integration tests only.
    /// </summary>
    public bool AllowUnsafeServerCertificate { get; set; }

    /// <summary>
    /// Gets or sets the fallback default index used when no explicit index is supplied.
    /// </summary>
    public string? DefaultIndex { get; set; }

    /// <summary>
    /// Gets or sets type-to-index mappings keyed by CLR full name or type name.
    /// </summary>
    public Dictionary<string, string> Indexes { get; set; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether debug mode is enabled.
    /// </summary>
    public bool EnableDebugMode { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether pretty JSON output is requested.
    /// </summary>
    public bool PrettyJson { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether request and response payloads should be buffered.
    /// </summary>
    public bool DisableDirectStreaming { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether gzip compression is enabled.
    /// </summary>
    public bool EnableHttpCompression { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether transport exceptions should be thrown immediately.
    /// </summary>
    public bool ThrowExceptions { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the transport should skip pinging nodes before requests.
    /// </summary>
    public bool DisablePing { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of transport retries.
    /// </summary>
    public int MaximumRetries { get; set; } = 2;

    /// <summary>
    /// Gets or sets the maximum concurrent connection limit.
    /// </summary>
    public int ConnectionLimit { get; set; } = 80;

    /// <summary>
    /// Gets or sets the request timeout in seconds.
    /// </summary>
    public int RequestTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Gets or sets the ping timeout in seconds.
    /// </summary>
    public int PingTimeoutSeconds { get; set; } = 5;

    /// <summary>
    /// Gets or sets the time a dead node stays out of rotation in seconds.
    /// </summary>
    public int DeadTimeoutSeconds { get; set; } = 60;

    /// <summary>
    /// Gets or sets the maximum retry timeout in seconds.
    /// </summary>
    public int MaxRetryTimeoutSeconds { get; set; } = 60;

    /// <summary>
    /// Gets or sets the maximum dead timeout in seconds.
    /// </summary>
    public int MaxDeadTimeoutSeconds { get; set; } = 180;

    /// <summary>
    /// Gets or sets a value indicating whether sniffing should run on startup.
    /// </summary>
    public bool SniffOnStartup { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether sniffing should run after connection faults.
    /// </summary>
    public bool SniffOnConnectionFault { get; set; }

    /// <summary>
    /// Gets or sets the TCP keep alive probe time in seconds.
    /// </summary>
    public int TcpKeepAliveTimeSeconds { get; set; } = 30;

    /// <summary>
    /// Gets or sets the TCP keep alive retry interval in seconds.
    /// </summary>
    public int TcpKeepAliveIntervalSeconds { get; set; } = 10;

    /// <summary>
    /// Gets or sets health check settings for the ElasticSearch client.
    /// </summary>
    public ElasticSearchHealthCheckOptions HealthCheck { get; set; } = new();
}
