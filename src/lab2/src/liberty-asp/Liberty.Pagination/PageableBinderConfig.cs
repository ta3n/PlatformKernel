namespace Liberty.Pagination;

/// <summary>
/// Configuration class for pageable binding used to facilitate pagination.
/// </summary>
public class PageableBinderConfig
{
    /// <summary>
    /// The default parameter name used to specify the page number in paginated requests.
    /// </summary>
    /// <remarks>
    /// This constant is typically used in conjunction with pagination frameworks
    /// to enforce a standardized query parameter for identifying the requested page.
    /// The default value is "page".
    /// </remarks>
    public const string DefaultPageParameter = "page";

    /// <summary>
    /// Represents the default query parameter name used to specify the page size in pagination.
    /// </summary>
    /// <remarks>
    /// This constant is used to configure the size parameter in pageable queries, which dictates the
    /// number of items displayed per page during data pagination. The default value is "size".
    /// </remarks>
    public const string DefaultSizeParameter = "size";

    /// <summary>
    /// Represents the default parameter name for enabling or disabling pagination functionality.
    /// </summary>
    /// <remarks>
    /// The value of this constant is used to define the query parameter
    /// for enabling pagination in the application. It serves as a default value
    /// that can be overridden by providing a custom name in the configuration.
    /// </remarks>
    public const string DefaultIsEnabledParameter = "enabled";

    /// <summary>
    /// Represents the default query parameter name used to specify sorting options
    /// for pageable requests in the pagination configuration.
    /// </summary>
    public const string DefaultSortParameter = "sort";

    /// <summary>
    /// Specifies the default delimiter used to separate multiple sort criteria
    /// in query parameters for the pageable configuration.
    /// </summary>
    public const string DefaultSortDelimiter = ",";

    /// <summary>
    /// Represents the default prefix value used in parameterized query string keys for pageable requests.
    /// This serves as a convention for prefixing request parameter names such as page, size, sort, etc.
    /// </summary>
    public const string DefaultPrefix = "";

    /// <summary>
    /// The default delimiter used to separate qualifiers in parameterized route or query components.
    /// </summary>
    public const string DefaultQualifierDelimiter = "_";

    /// <summary>
    /// Represents the default maximum page size for pagination.
    /// </summary>
    /// <remarks>
    /// This constant defines the upper limit for the number of items
    /// that can be requested per page in a pagination query. It ensures
    /// that queries do not exceed predefined resource limits.
    /// </remarks>
    public const int DefaultMaxPageSize = 2000;

    /// <summary>
    /// Represents the default instance of an <see cref="IPageable"/> configuration used system-wide.
    /// This value defines the default pagination settings, including the page number and page size.
    /// </summary>
    public static readonly IPageable DefaultPageable = Pageable.Of(1, 20);

    /// <summary>
    /// Gets or sets the fallback <see cref="IPageable"/> to be used
    /// when no specific paging information is provided in a query.
    /// This property ensures a default paging behavior, such as default
    /// page number and size, when paging parameters are missing or invalid.
    /// </summary>
    public IPageable FallbackPageable { get; set; } = DefaultPageable;

    /// <summary>
    /// Gets or sets the query string parameter name used to specify the page number
    /// when retrieving paginated data. The default value is "page".
    /// </summary>
    public string PageParameterName { get; set; } = DefaultPageParameter;

    /// <summary>
    /// Gets or sets the name of the query string parameter used to specify the page size in pageable requests.
    /// </summary>
    /// <remarks>
    /// This property defines the key used to retrieve the page size value from the query string when processing
    /// pagination requests. The default value is "size". Changing this value allows customization of the query
    /// parameter name to suit different API designs.
    /// </remarks>
    public string SizeParameterName { get; set; } = DefaultSizeParameter;

    /// <summary>
    /// Gets or sets the name of the query string parameter used to determine if a
    /// specific functionality is enabled.
    /// </summary>
    /// <remarks>
    /// The default value is "enabled". This property allows customization of the
    /// parameter name based on application requirements. It is commonly used in
    /// query string parsing to toggle or control certain features dynamically.
    /// </remarks>
    public string IsEnabledParameterName { get; set; } = DefaultIsEnabledParameter;

    /// <summary>
    /// Represents the query string parameter name used for specifying sort criteria.
    /// Defaults to the value of <c>"sort"</c>.
    /// </summary>
    /// <remarks>
    /// This property is configurable and can be set to a custom value to adapt
    /// to different API query parameter naming conventions.
    /// The value specified determines which query string parameter the system
    /// examines to extract sorting information for data pagination.
    /// </remarks>
    public string SortParameterName { get; set; } = DefaultSortParameter;

    /// <summary>
    /// Gets or sets the delimiter used to separate multiple sort parameters in query strings.
    /// </summary>
    /// <remarks>
    /// This property defines the character or string that separates sorting information
    /// within a single sort parameter. It is utilized during the parsing of query strings
    /// to extract and process sorting rules.
    /// </remarks>
    public string SortDelimiter { get; set; } = DefaultSortDelimiter;

    /// <summary>
    /// Gets or sets the prefix used for query parameters when constructing requests.
    /// This can be useful for namespacing query parameters or avoiding collisions with other query parameters.
    /// </summary>
    public string Prefix { get; set; } = DefaultPrefix;

    /// <summary>
    /// Gets or sets the delimiter to use between qualifiers in pageable query parameters.
    /// This property is used to configure how qualifier parameters are concatenated in a pagination context.
    /// </summary>
    public string QualifierDelimiter { get; set; } = DefaultQualifierDelimiter;

    /// <summary>
    /// Gets or sets the maximum allowed page size for pagination requests.
    /// </summary>
    /// <remarks>
    /// This property enforces an upper limit on the number of items that can be retrieved in a single page
    /// during pagination. If a client requests a page size exceeding this value, the limit defined by
    /// <c>MaxPageSize</c> will be applied to ensure performance and prevent excessive data transfer.
    /// </remarks>
    public int MaxPageSize { get; set; } = DefaultMaxPageSize;
}
