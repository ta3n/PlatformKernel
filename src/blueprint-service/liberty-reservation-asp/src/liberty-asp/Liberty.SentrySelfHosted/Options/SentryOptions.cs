namespace Liberty.SentrySelfHosted.Options;

/// <summary>
///
/// </summary>
/// <param name="Enabled">Enable integration sentry in the serilog.</param>
/// <param name="Dsn">The DSN can also be set via environment variable.</param>
/// <param name="TracesSampleRate">Indicates the percentage of the tracing data that is collected.</param>
/// <param name="Debug">The flag below can be used to see the internal logs of the SDK in the applications log (it's off by default).</param>
public record SentryOptions(
    bool Enabled,
    string Dsn,
    bool Debug = true,
    double TracesSampleRate = 1.0
);
