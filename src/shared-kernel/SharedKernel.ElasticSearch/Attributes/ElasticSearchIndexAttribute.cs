namespace SharedKernel.ElasticSearch.Attributes;

/// <summary>
/// Declares the ElasticSearch index associated with a CLR type.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class ElasticSearchIndexAttribute(
    string indexName
) : Attribute
{
    /// <summary>
    /// Gets the configured index name.
    /// </summary>
    public string IndexName { get; } = indexName;
}
