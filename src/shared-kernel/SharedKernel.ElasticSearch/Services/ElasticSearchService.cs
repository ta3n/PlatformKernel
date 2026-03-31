using Elastic.Clients.Elasticsearch;
using SharedKernel.ElasticSearch.Abstractions;
using SharedKernel.ElasticSearch.Options;

namespace SharedKernel.ElasticSearch.Services;

/// <summary>
/// Default high-level ElasticSearch service implementation.
/// </summary>
public sealed class ElasticSearchService(
    ElasticsearchClient client,
    IElasticSearchIndexNameResolver indexNameResolver,
    ElasticSearchOptions options
) : IElasticSearchService
{
    /// <inheritdoc />
    public ElasticsearchClient Client { get; } = client;

    /// <inheritdoc />
    public string ResolveIndex<TDocument>(
        string? index = null
    )
    {
        return indexNameResolver.Resolve<TDocument>(index);
    }

    /// <inheritdoc />
    public async Task<bool> PingAsync(
        CancellationToken cancellationToken = default
    )
    {
        var response = await Client.PingAsync(cancellationToken).ConfigureAwait(false);
        return response.IsValidResponse;
    }

    /// <inheritdoc />
    public async Task<bool> EnsureIndexAsync(
        string index,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(index);

        var existsResponse = await Client.Indices.ExistsAsync(index, cancellationToken).ConfigureAwait(false);

        if (existsResponse.Exists)
        {
            return false;
        }

        var createResponse = await Client.Indices.CreateAsync(index, cancellationToken).ConfigureAwait(false);
        return createResponse.IsValidResponse;
    }

    /// <inheritdoc />
    public Task<IndexResponse> IndexAsync<TDocument>(
        TDocument document,
        string? index = null,
        CancellationToken cancellationToken = default
    ) where TDocument : class
    {
        ArgumentNullException.ThrowIfNull(document);

        var resolvedIndex = indexNameResolver.Resolve<TDocument>(index);
        return Client.IndexAsync(
            document,
            request => request.Index(resolvedIndex),
            cancellationToken
        );
    }

    /// <inheritdoc />
    public Task<GetResponse<TDocument>> GetAsync<TDocument>(
        string id,
        string? index = null,
        CancellationToken cancellationToken = default
    ) where TDocument : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var resolvedIndex = indexNameResolver.Resolve<TDocument>(index);
        return Client.GetAsync<TDocument>(
            id,
            request => request.Index(resolvedIndex),
            cancellationToken
        );
    }

    /// <inheritdoc />
    public Task<SearchResponse<TDocument>> SearchAsync<TDocument>(
        Func<SearchRequestDescriptor<TDocument>, SearchRequestDescriptor<TDocument>> selector,
        string? index = null,
        CancellationToken cancellationToken = default
    ) where TDocument : class
    {
        ArgumentNullException.ThrowIfNull(selector);

        var resolvedIndex = indexNameResolver.Resolve<TDocument>(index);
        return Client.SearchAsync<TDocument>(
            descriptor => selector(descriptor.Indices(resolvedIndex)),
            cancellationToken
        );
    }

    /// <inheritdoc />
    public Task<UpdateResponse<TDocument>> UpsertAsync<TDocument, TPartialDocument>(
        string id,
        TPartialDocument partialDocument,
        string? index = null,
        CancellationToken cancellationToken = default
    )
        where TDocument : class
        where TPartialDocument : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(partialDocument);

        var resolvedIndex = indexNameResolver.Resolve<TDocument>(index);
        return Client.UpdateAsync<TDocument, TPartialDocument>(
            resolvedIndex,
            id,
            request => request
                .Doc(partialDocument)
                .DocAsUpsert(),
            cancellationToken
        );
    }

    /// <inheritdoc />
    public Task<DeleteResponse> DeleteAsync<TDocument>(
        string id,
        string? index = null,
        CancellationToken cancellationToken = default
    ) where TDocument : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var resolvedIndex = indexNameResolver.Resolve<TDocument>(index);
        return Client.DeleteAsync(resolvedIndex, id, cancellationToken);
    }

    /// <inheritdoc />
    public Task<BulkResponse> BulkIndexAsync<TDocument>(
        IEnumerable<TDocument> documents,
        string? index = null,
        CancellationToken cancellationToken = default
    ) where TDocument : class
    {
        ArgumentNullException.ThrowIfNull(documents);

        var resolvedIndex = indexNameResolver.Resolve<TDocument>(index);
        return Client.IndexManyAsync(documents, resolvedIndex, cancellationToken);
    }

    /// <inheritdoc />
    public Task RefreshAsync(
        string? index = null,
        CancellationToken cancellationToken = default
    )
    {
        var resolvedIndex = !string.IsNullOrWhiteSpace(index)
            ? index
            : options.DefaultIndex;

        if (string.IsNullOrWhiteSpace(resolvedIndex))
        {
            throw new InvalidOperationException(
                "No ElasticSearch index was provided for refresh. Supply an explicit index or configure ElasticSearch:DefaultIndex."
            );
        }

        return Client.Indices.RefreshAsync(resolvedIndex, cancellationToken);
    }
}
