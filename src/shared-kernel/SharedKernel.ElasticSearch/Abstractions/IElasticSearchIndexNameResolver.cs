namespace SharedKernel.ElasticSearch.Abstractions;

/// <summary>
/// Resolves an ElasticSearch index name for a CLR document type.
/// </summary>
public interface IElasticSearchIndexNameResolver
{
    /// <summary>
    /// Resolves an index name for the given document type.
    /// </summary>
    string Resolve<TDocument>(
        string? explicitIndex = null
    );

    /// <summary>
    /// Resolves an index name for the given document type.
    /// </summary>
    string Resolve(
        Type documentType,
        string? explicitIndex = null
    );
}
