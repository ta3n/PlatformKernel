using Elastic.Clients.Elasticsearch;
using SharedKernel.ElasticSearch.Abstractions;

namespace SharedKernel.ElasticSearch.Services;

/// <summary>
/// Default implementation of <see cref="IElasticSearchClientFactory"/>.
/// </summary>
public sealed class ElasticSearchClientFactory(
    ElasticsearchClient client
) : IElasticSearchClientFactory
{
    /// <inheritdoc />
    public ElasticsearchClient CreateClient()
    {
        return client;
    }
}
