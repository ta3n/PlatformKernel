namespace SharedKernel.Hangfire.Test.Integration;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class HangfireRedisCollection : ICollectionFixture<HangfireRedisContainerFixture>
{
    public const string Name = "hangfire-redis";
}
