using SharedKernel.BulkInsertPipeline.Abstractions;

namespace SharedKernel.BulkInsertPipeline.Test.Service.Configuration;

public sealed class BulkInsertPipelineLabOptions
{
    public const string SectionName = "BulkInsertPipelineLab";

    public BulkInsertProviderType HostedProvider { get; init; } = BulkInsertProviderType.NpgsqlBinaryCopy;

    public BulkInsertProviderType FallbackProvider { get; init; } = BulkInsertProviderType.Dapper;

    public int WorkerCount { get; init; } = 2;

    public int ChannelCapacity { get; init; } = 2_048;

    public int WaitForDrainMilliseconds { get; init; } = 500;
}
