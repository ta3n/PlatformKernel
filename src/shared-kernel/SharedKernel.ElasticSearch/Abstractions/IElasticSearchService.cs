using Elastic.Clients.Elasticsearch;

namespace SharedKernel.ElasticSearch.Abstractions;

/// <summary>
/// Defines a high-level ElasticSearch service for common document operations.
/// </summary>
public interface IElasticSearchService
{
    /// <summary>
    /// Gets the underlying official ElasticSearch client for advanced scenarios.
    /// </summary>
    ElasticsearchClient Client { get; }

    /// <summary>
    /// Resolves the effective index name for the document type.
    /// </summary>
    string ResolveIndex<TDocument>(
        string? index = null
    );

    /// <summary>
    /// Sends a lightweight ping request to ElasticSearch.
    /// </summary>
    Task<bool> PingAsync(
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Ensures the given index exists.
    /// </summary>
    Task<bool> EnsureIndexAsync(
        string index,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Indexes a single document.
    /// </summary>
    Task<IndexResponse> IndexAsync<TDocument>(
        TDocument document,
        string? index = null,
        CancellationToken cancellationToken = default
    ) where TDocument : class;

    /// <summary>
    /// Retrieves a document by id.
    /// </summary>
    Task<GetResponse<TDocument>> GetAsync<TDocument>(
        string id,
        string? index = null,
        CancellationToken cancellationToken = default
    ) where TDocument : class;

    /// <summary>
    /// Executes a typed search query.
    /// </summary>
    Task<SearchResponse<TDocument>> SearchAsync<TDocument>(
        Func<SearchRequestDescriptor<TDocument>, SearchRequestDescriptor<TDocument>> selector,
        string? index = null,
        CancellationToken cancellationToken = default
    ) where TDocument : class;

    /// <summary>
    /// Creates or updates a document using the update API with <c>doc_as_upsert</c>.
    /// </summary>
    Task<UpdateResponse<TDocument>> UpsertAsync<TDocument, TPartialDocument>(
        string id,
        TPartialDocument partialDocument,
        string? index = null,
        CancellationToken cancellationToken = default
    )
        where TDocument : class
        where TPartialDocument : class;

    /// <summary>
    /// Deletes a document by id.
    /// </summary>
    Task<DeleteResponse> DeleteAsync<TDocument>(
        string id,
        string? index = null,
        CancellationToken cancellationToken = default
    ) where TDocument : class;

    /// <summary>
    /// Bulk indexes many documents using the official helper extension.
    /// </summary>
    Task<BulkResponse> BulkIndexAsync<TDocument>(
        IEnumerable<TDocument> documents,
        string? index = null,
        CancellationToken cancellationToken = default
    ) where TDocument : class;

    /// <summary>
    /// Refreshes an index so recently written documents become searchable.
    /// </summary>
    Task RefreshAsync(
        string? index = null,
        CancellationToken cancellationToken = default
    );
}
