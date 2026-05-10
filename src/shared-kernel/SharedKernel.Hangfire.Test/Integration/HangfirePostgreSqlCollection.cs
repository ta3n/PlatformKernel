namespace SharedKernel.Hangfire.Test.Integration;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class HangfirePostgreSqlCollection
    : ICollectionFixture<HangfirePostgreSqlContainerFixture>
{
    public const string Name = "hangfire-postgresql";
}
