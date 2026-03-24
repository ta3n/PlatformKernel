using SharedKernel.ElasticSearch.Abstractions;
using SharedKernel.ElasticSearch.Attributes;
using SharedKernel.ElasticSearch.Options;

namespace SharedKernel.ElasticSearch.Services;

/// <summary>
/// Resolves index names from explicit input, attributes, configured mappings, or defaults.
/// </summary>
public sealed class ElasticSearchIndexNameResolver(
    ElasticSearchOptions options
) : IElasticSearchIndexNameResolver
{
    /// <inheritdoc />
    public string Resolve<TDocument>(
        string? explicitIndex = null
    ) => Resolve(typeof(TDocument), explicitIndex);

    /// <inheritdoc />
    public string Resolve(
        Type documentType,
        string? explicitIndex = null
    )
    {
        if (!string.IsNullOrWhiteSpace(explicitIndex))
        {
            return explicitIndex;
        }

        var attribute = documentType.GetCustomAttributes(typeof(ElasticSearchIndexAttribute), true)
            .OfType<ElasticSearchIndexAttribute>()
            .FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(attribute?.IndexName))
        {
            return attribute.IndexName;
        }

        if (TryResolveConfiguredIndex(documentType, out var configuredIndex))
        {
            return configuredIndex;
        }

        if (!string.IsNullOrWhiteSpace(options.DefaultIndex))
        {
            return options.DefaultIndex;
        }

        throw new InvalidOperationException(
            $"No ElasticSearch index was resolved for document type '{documentType.FullName}'. " +
            "Provide an explicit index, configure ElasticSearch:DefaultIndex, set ElasticSearch:Indexes, " +
            "or decorate the document with [ElasticSearchIndex]."
        );
    }

    private bool TryResolveConfiguredIndex(
        Type documentType,
        out string index
    )
    {
        index = string.Empty;

        if (options.Indexes.Count == 0)
        {
            return false;
        }

        var keys = new[]
        {
            documentType.AssemblyQualifiedName,
            documentType.FullName,
            documentType.Name
        };

        foreach (var key in keys)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                continue;
            }

            if (TryGetCaseInsensitiveValue(key, out index))
            {
                return true;
            }
        }

        return false;
    }

    private bool TryGetCaseInsensitiveValue(
        string key,
        out string value
    )
    {
        if (options.Indexes.TryGetValue(key, out value!))
        {
            return true;
        }

        var match = options.Indexes.FirstOrDefault(
            item => string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase)
        );

        if (!string.IsNullOrWhiteSpace(match.Value))
        {
            value = match.Value;
            return true;
        }

        value = string.Empty;
        return false;
    }
}
