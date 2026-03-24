using Elastic.Clients.Elasticsearch;

namespace SharedKernel.ElasticSearch.Abstractions;

/// <summary>
/// Defines a factory used to access the configured ElasticSearch client.
/// </summary>
public interface IElasticSearchClientFactory
{
    /// <summary>
    /// Returns the configured <see cref="ElasticsearchClient"/> instance.
    /// </summary>
    ElasticsearchClient CreateClient();
}
