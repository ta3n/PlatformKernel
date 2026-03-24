using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SharedKernel.ElasticSearch.Abstractions;

namespace SharedKernel.ElasticSearch.Test.Integration;

public sealed class ElasticSearchServiceIntegrationTests(
    ElasticSearchContainerFixture fixture
) : ElasticSearchIntegrationTestBase(fixture)
{
    [Fact]
    public async Task PingAndHealthCheck_ReturnHealthy()
    {
        Assert.True(await ElasticSearchService.PingAsync());

        var result = await HealthCheckService.CheckHealthAsync();

        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Contains(result.Entries, entry => entry.Key == "elasticsearch");
    }

    [Fact]
    public async Task EnsureIndexAndDocumentOperations_WorkEndToEnd()
    {
        var index = CreateIndexName();

        try
        {
            Assert.True(await ElasticSearchService.EnsureIndexAsync(index));
            Assert.False(await ElasticSearchService.EnsureIndexAsync(index));

            var document = new IntegrationDocument
            {
                Id = "doc-1",
                Name = "Platform Kernel",
                Category = "search",
                Version = 1
            };

            var indexResponse = await ElasticSearchService.IndexAsync(document, index);
            await ElasticSearchService.RefreshAsync(index);

            var getResponse = await ElasticSearchService.GetAsync<IntegrationDocument>(document.Id, index);
            var searchResponse = await ElasticSearchService.SearchAsync<IntegrationDocument>(
                search => search.Size(10),
                index
            );

            Assert.True(indexResponse.IsValidResponse);
            Assert.True(getResponse.Found);
            Assert.NotNull(getResponse.Source);
            Assert.Equal(document.Name, getResponse.Source!.Name);
            Assert.True(searchResponse.IsValidResponse);
            Assert.Contains(searchResponse.Documents, item => item.Id == document.Id);
        }
        finally
        {
            await Fixture.DeleteIndexIfExistsAsync(index);
        }
    }

    [Fact]
    public async Task UpsertBulkDeleteAndFactoryClient_WorkReliably()
    {
        var index = CreateIndexName();

        try
        {
            await ElasticSearchService.EnsureIndexAsync(index);

            var upsertResponse = await ElasticSearchService.UpsertAsync<IntegrationDocument, IntegrationDocument>(
                "upsert-1",
                new IntegrationDocument
                {
                    Id = "upsert-1",
                    Name = "First version",
                    Category = "upsert",
                    Version = 1
                },
                index
            );

            var bulkResponse = await ElasticSearchService.BulkIndexAsync(
            [
                new IntegrationDocument
                {
                    Id = "bulk-1",
                    Name = "Bulk one",
                    Category = "bulk",
                    Version = 1
                },
                new IntegrationDocument
                {
                    Id = "bulk-2",
                    Name = "Bulk two",
                    Category = "bulk",
                    Version = 2
                }
            ],
                index
            );

            await ElasticSearchService.RefreshAsync(index);

            var searchResponse = await ElasticSearchService.SearchAsync<IntegrationDocument>(
                search => search.Size(20),
                index
            );

            var deleteResponse = await ElasticSearchService.DeleteAsync<IntegrationDocument>("upsert-1", index);
            await ElasticSearchService.RefreshAsync(index);
            var deletedDocumentResponse = await ElasticSearchService.GetAsync<IntegrationDocument>("upsert-1", index);

            await using var provider = CreateServiceProvider();
            var clientFromFactory = provider
                .GetRequiredService<IElasticSearchClientFactory>()
                .CreateClient();
            var pingResponse = await clientFromFactory.PingAsync();

            Assert.True(upsertResponse.IsValidResponse);
            Assert.True(bulkResponse.IsValidResponse);
            Assert.False(bulkResponse.Errors);
            Assert.Equal(3, searchResponse.Documents.Count);
            Assert.True(deleteResponse.IsValidResponse);
            Assert.False(deletedDocumentResponse.Found);
            Assert.True(pingResponse.IsValidResponse);
        }
        finally
        {
            await Fixture.DeleteIndexIfExistsAsync(index);
        }
    }

    [Fact]
    public async Task DefaultIndexResolution_WithoutExplicitIndex_WorksAgainstContainer()
    {
        var index = CreateIndexName();

        await using var provider = CreateServiceProvider(defaultIndex: index);
        var service = provider.GetRequiredService<IElasticSearchService>();

        try
        {
            await service.EnsureIndexAsync(index);

            var response = await service.IndexAsync(
                new DefaultIndexDocument
                {
                    Id = "default-1",
                    Name = "Default index document",
                    Category = "default",
                    Version = 1
                });

            await service.RefreshAsync(index);

            var getResponse = await service.GetAsync<DefaultIndexDocument>("default-1");

            Assert.True(response.IsValidResponse);
            Assert.True(getResponse.Found);
            Assert.Equal(index, service.ResolveIndex<DefaultIndexDocument>());
        }
        finally
        {
            await Fixture.DeleteIndexIfExistsAsync(index);
        }
    }

    private sealed class DefaultIndexDocument
    {
        public string Id { get; init; } = string.Empty;

        public string Name { get; init; } = string.Empty;

        public string Category { get; init; } = string.Empty;

        public int Version { get; init; }
    }

    private sealed class IntegrationDocument
    {
        public string Id { get; init; } = string.Empty;

        public string Name { get; init; } = string.Empty;

        public string Category { get; init; } = string.Empty;

        public int Version { get; init; }
    }
}
