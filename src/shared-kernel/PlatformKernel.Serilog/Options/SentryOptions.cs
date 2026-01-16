namespace PlatformKernel.Serilog.Options;

/// <summary>
///
/// </summary>
/// <param name="Enabled">Enable integration sentry in the serilog.</param>
/// <param name="Dsn">The DSN can also be set via environment variable.</param>
/// <param name="TracesSampleRate">Indicates the percentage of the tracing data that is collected.</param>
/// <param name="SendDefaultPii">Sends Cookies, User ID when one is logged on and user IP address to sentry. It's turned off by default.</param>
/// <param name="AttachStackTrace">Send the stack trace of captured messages (e.g: a LogWarning without an exception).</param>
/// <param name="Debug">The flag below can be used to see the internal logs of the SDK in the applications log (it's off by default).</param>
/// <param name="DiagnosticLevel">By default, the level is Debug, but it can be changed to any level of SentryLevel enum.</param>
public record SentryOptions(
    bool Enabled,
    string Dsn,
    string DiagnosticLevel = "Error",
    double TracesSampleRate = 1.0,
    double ProfilesSampleRate = 1.0,
    bool SendDefaultPii = true,
    bool Debug = true,
    bool AttachStackTrace = true
);
