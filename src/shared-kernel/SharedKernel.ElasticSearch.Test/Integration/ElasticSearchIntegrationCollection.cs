namespace SharedKernel.ElasticSearch.Test.Integration;

[CollectionDefinition(CollectionName)]
public sealed class ElasticSearchIntegrationCollection : ICollectionFixture<ElasticSearchContainerFixture>
{
    public const string CollectionName = "shared-kernel-elasticsearch";
}
