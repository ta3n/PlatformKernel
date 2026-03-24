namespace SharedKernel.Cache.Test.Integration;

[CollectionDefinition(CollectionName)]
public sealed class RedisIntegrationCollection : ICollectionFixture<RedisContainerFixture>
{
    public const string CollectionName = "shared-kernel-cache-redis";
}
