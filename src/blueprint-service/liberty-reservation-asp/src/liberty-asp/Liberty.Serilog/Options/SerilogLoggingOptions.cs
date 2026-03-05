namespace Liberty.Serilog.Options;

/// <summary>
/// Represents configuration options for setting up logging using Serilog.
/// </summary>
public record SerilogLoggingOptions(
    bool Enabled,
    bool WriteToConsole,
    bool WriteToFile,
    SeqOptions? WriteToSeq = null,
    SentryOptions? WriteToSentry = null,
    AwsCloudWatchOptions? WriteToAwsCloudWatch = null
);
