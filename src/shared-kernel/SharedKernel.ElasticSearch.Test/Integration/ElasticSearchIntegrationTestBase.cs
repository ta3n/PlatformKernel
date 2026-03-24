using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SharedKernel.ElasticSearch.Abstractions;

namespace SharedKernel.ElasticSearch.Test.Integration;

[Collection(ElasticSearchIntegrationCollection.CollectionName)]
public abstract class ElasticSearchIntegrationTestBase(
    ElasticSearchContainerFixture fixture
) : IAsyncLifetime
{
    private ServiceProvider _serviceProvider = null!;

    protected ElasticSearchContainerFixture Fixture { get; } = fixture;

    protected IElasticSearchService ElasticSearchService => _serviceProvider.GetRequiredService<IElasticSearchService>();

    protected HealthCheckService HealthCheckService => _serviceProvider.GetRequiredService<HealthCheckService>();

    public virtual Task InitializeAsync()
    {
        _serviceProvider = Fixture.CreateServiceProvider();
        return Task.CompletedTask;
    }

    public virtual async Task DisposeAsync()
    {
        await _serviceProvider.DisposeAsync();
    }

    protected static string CreateIndexName()
    {
        return $"shared-kernel-es-{Guid.NewGuid():N}";
    }

    protected ServiceProvider CreateServiceProvider(
        string? defaultIndex = null
    )
    {
        return Fixture.CreateServiceProvider(defaultIndex);
    }
}
