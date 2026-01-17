namespace SharedKernel.Serilog.Options;

/// <summary>
/// Represents configuration options for logging to AWS CloudWatch using Serilog.
/// </summary>
public record AwsCloudWatchOptions(
    bool Enabled,
    string LogGroupName,
    string LogStreamPrefix,
    AwsSetting AwsSetting
);

/// <summary>
/// Represents AWS connection settings including credentials necessary for accessing AWS services.
/// </summary>
public record AwsSetting(
    string AccessKeyId,
    string SecretAccessKey
);
