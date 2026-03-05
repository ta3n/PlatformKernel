namespace Liberty.Pagination;

/// <summary>
/// A static class that provides constants for implementing pageable functionality.
/// </summary>
public static class PageableConstants
{
    /// <summary>
    /// Represents a constant instance of the <see cref="IPageable"/> interface,
    /// used to denote a non-paginated state or the absence of pagination.
    /// </summary>
    public static readonly IPageable UnPaged = new UnPaged();
}

/// <summary>
/// Represents a contract for pagination information and operations in a pageable system.
/// </summary>
public interface IPageable
{
    /// Indicates whether the current instance is pageable.
    /// This property determines if the `IPageable` instance
    /// has information pertaining to pagination, such as page
    /// number, page size, and offset. A value of `true` means
    /// the instance represents a pageable collection, while a
    /// value of `false` indicates that paging is not applied.
    bool IsPaged { get; }

    /// <summary>
    /// Represents the current page index within a collection of paginated results.
    /// The index is generally 1-based, meaning the first page has a PageNumber of 1.
    /// </summary>
    int PageNumber { get; }

    /// Gets the number of items contained in a single page of a paginated collection.
    /// The property defines the maximum amount of items that should be displayed or processed
    /// in a single page. It is commonly used in pagination logic to calculate total pages,
    /// determine the range of data to fetch, and validate navigation between pages.
    /// A value of -1 may indicate an unpaged state where no restrictions on the page size apply.
    int PageSize { get; }

    /// <summary>
    /// Gets a value indicating whether pagination is enabled or not. A null value implies
    /// that no specific preference is set for pagination behavior.
    /// </summary>
    bool? IsEnabled { get; }

    /// <summary>
    /// Gets the offset of the first item to be returned in the current page of results.
    /// Offset is calculated based on the page number and page size.
    /// </summary>
    /// <remarks>
    /// Offset is determined as (PageNumber - 1) * PageSize. This value may be used for
    /// paginated queries to skip a specified number of items. Negative values might indicate
    /// an unpaged state, depending on the implementation.
    /// </remarks>
    int Offset { get; }

    /// Represents sorting information used for ordering query results.
    /// This property provides the details about the sort direction
    /// and the fields on which sorting is applied.
    /// A `Sort` instance may encapsulate one or more sorting orders where
    /// each order specifies a direction (ascending or descending) along with
    /// a property to sort by. It supports operations to modify
    /// sorting orders or add additional sorting criteria.
    /// It is a core property in pagination models (e.g., `IPageable`)
    /// to define how the sorted data should be returned in a paged request.
    Sort Sort { get; }

    /// <summary>
    /// Gets the next IPageable instance representing the subsequent page based on the current page's pagination settings.
    /// </summary>
    /// <remarks>
    /// This property enables navigation to the next page of data in a paginated collection. If the current page is the last
    /// available or represents an unpaged implementation, the property returns itself or an equivalent unpaged instance.
    /// </remarks>
    /// <returns>
    /// An IPageable instance representing the next page. For a paginated implementation, this increments the page number
    /// while maintaining the same page size and sorting criteria.
    /// </returns>
    IPageable Next { get; }

    /// <summary>
    /// Gets the previous pageable instance if a previous page exists; otherwise, retrieves the first pageable instance.
    /// </summary>
    /// <remarks>
    /// This property is used to navigate to the preceding page in a paginated sequence. If there is no previous page,
    /// it will return the first pageable instance. The determination is based on the <see cref="HasPrevious"/> property.
    /// </remarks>
    IPageable PreviousOrFirst { get; }

    /// Gets the first page of the pageable request.
    /// This property is used to retrieve the first page of a pageable instance,
    /// typically representing the starting point of pagination with respect to the configured
    /// page size and additional parameters. It is often used in navigation scenarios
    /// or to reset paging back to the initial page.
    /// In an unpaged scenario, this property would return itself, indicating that paging
    /// is not applicable or relevant.
    IPageable First { get; }

    /// <summary>
    /// Indicates whether there is a previous page available based on the current page number.
    /// </summary>
    /// <remarks>
    /// Returns <c>true</c> if the current page number is greater than 0, indicating
    /// that a previous page exists. Otherwise, it returns <c>false</c>.
    /// </remarks>
    bool HasPrevious { get; }
}

/// <summary>
/// Represents a non-paged implementation of the <see cref="IPageable"/> interface.
/// This class is used to indicate that a pageable request does not have pagination enabled.
/// </summary>
internal class UnPaged : IPageable
{
    /// <summary>
    /// Indicates whether the current instance is paged.
    /// </summary>
    /// <remarks>
    /// A value of true signifies that the instance supports pagination,
    /// while false indicates that the instance is not paged.
    /// </remarks>
    public bool IsPaged => false;

    /// Represents the sorting configuration for a pageable collection or query result.
    /// Provides functionalities to determine the sorting order of items,
    /// typically used in conjunction with pagination.
    /// The static property `Unsorted` represents a state where no sorting is applied.
    public Sort Sort => Sort.Unsorted;

    /// <summary>
    /// Gets the next <see cref="IPageable"/> instance.
    /// This property is used to represent the continuation of unpaged results
    /// where no subsequent page is applicable.
    /// </summary>
    public IPageable Next => this;

    /// <summary>
    /// Gets the current instance or represents the first pageable instance when unpaged.
    /// </summary>
    /// <remarks>
    /// This property is designed to provide a consistent reference
    /// to the first pageable element in cases where paging is not applied.
    /// </remarks>
    public IPageable PreviousOrFirst => this;

    /// <summary>
    /// Gets the first pageable instance.
    /// This property returns the current instance, which represents an unpaged state.
    /// </summary>
    public IPageable First => this;

    /// <summary>
    /// Indicates whether there is a previous page available in the pagination sequence.
    /// </summary>
    /// <remarks>
    /// This property is typically used in implementations of pagination to determine
    /// if navigation to a previous page is possible.
    /// Returns <c>false</c> for unpaged implementations.
    /// </remarks>
    public bool HasPrevious => false;

    /// <summary>
    /// Represents the current page index for pagination operations.
    /// </summary>
    /// <remarks>
    /// The page number typically starts at 0 or 1, depending on the implementation,
    /// and is used in conjunction with other paging parameters to navigate through a
    /// set of data.
    /// </remarks>
    public int PageNumber => -1;

    /// <summary>
    /// Gets the size of the page for pagination purposes.
    /// </summary>
    /// <remarks>
    /// A value used to define the number of items per page in a pageable data set.
    /// For unpaged configurations, this property may return a predefined value (e.g., -1)
    /// indicating that no pagination is applied.
    /// </remarks>
    public int PageSize => -1;

    /// <summary>
    /// Gets the offset of the pageable structure, indicating the starting position of elements
    /// to be retrieved in a paginated query.
    /// </summary>
    /// <remarks>
    /// In this implementation, the value of <c>Offset</c> is set to -1,
    /// which typically signifies an unpaged state or that no offset is applied.
    /// </remarks>
    public int Offset => -1;

    /// Gets a value indicating whether the feature or functionality is enabled.
    /// This property may return null, indicating that its enabled status is not explicitly specified.
    public bool? IsEnabled => null;
}
