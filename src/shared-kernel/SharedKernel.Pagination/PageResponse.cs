using Newtonsoft.Json;

namespace SharedKernel.Pagination;

/// <summary>
/// Represents a standardized response for paginated data.
/// </summary>
public class PageResponse
{
    /// <summary>
    /// Gets or sets the total number of elements in a paginated result set.
    /// </summary>
    /// <remarks>
    /// This property represents the total count of items regardless of the current page being viewed.
    /// It is typically used for pagination purposes to inform about the overall size of the dataset.
    /// </remarks>
    [JsonProperty("totalCount")]
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the size of the page for pagination, representing the number of items
    /// included in a single page.
    /// </summary>
    [JsonProperty("size")]
    public int Size { get; set; }

    /// <summary>
    /// Gets or sets the current page number in a paginated response.
    /// </summary>
    /// <remarks>
    /// Represents the index of the current page for the given paginated dataset.
    /// Typically used in conjunction with other pagination details such as total elements,
    /// total pages, and page size to generate or interpret pagination headers or responses.
    /// </remarks>
    [JsonProperty("page")]
    public int Page { get; set; }

    /// <summary>
    /// Gets or sets the total number of pages available in the paginated response.
    /// This value represents the total number of discrete pages for the given collection
    /// when divided according to the page size.
    /// </summary>
    [JsonProperty("totalPages")]
    public int TotalPages { get; set; }

    /// <summary>
    /// Indicates whether the current page is the first page in a paginated response.
    /// </summary>
    [JsonProperty("isFirst")]
    public bool IsFirst { get; set; }

    /// <summary>
    /// Indicates whether the current page is the last page in a paginated response.
    /// </summary>
    [JsonProperty("isLast")]
    public bool IsLast { get; set; }

    /// <summary>
    /// Indicates whether there is a subsequent page available in the paginated response.
    /// </summary>
    [JsonProperty("hasNext")]
    public bool HasNext { get; set; }

    /// Indicates whether there is a preceding page available in the paginated result set.
    /// This property is typically used to determine if navigation to a previous page is possible.
    [JsonProperty("hasPrevious")]
    public bool HasPrevious { get; set; }
}
