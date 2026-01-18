namespace Liberty.Serilog.Options;

/// <summary>
/// Configuration options for connecting to Seq for logging purposes.
/// </summary>
public record SeqOptions(
    bool Enabled,
    string Url,
    string ApiKey
);
