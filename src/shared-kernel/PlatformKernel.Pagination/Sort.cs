namespace PlatformKernel.Pagination;

/// <summary>
/// Represents sorting metadata to be used for query pagination and sorting.
/// </summary>
public class Sort(
    IList<Order>? orders
)
{
    /// <summary>
    /// Represents the default sorting direction used in the <see cref="Sort"/> class when no specific
    /// direction is provided. The default value is <see cref="Direction.Asc"/>.
    /// </summary>
    public const Direction DefaultDirection = Direction.Asc;

    /// <summary>
    /// Represents an unsorted state for the <see cref="Sort"/> class.
    /// This static field indicates the absence of any sort orders
    /// and is commonly used as the default sort configuration.
    /// </summary>
    /// <remarks>
    /// The <see cref="Sort.Unsorted"/> instance is immutable and shared.
    /// It signifies a no-operation sorting state, meaning no sorting should be applied.
    /// </remarks>
    public static readonly Sort Unsorted = new();

    /// <summary>
    /// Gets the list of sorting orders that define the property and sort direction for sorting operations.
    /// </summary>
    /// <remarks>
    /// Each <see cref="Order"/> in the list specifies a property to sort by and the direction of the sort.
    /// </remarks>
    public IList<Order> Orders { get; } = orders ?? [];

    /// <summary>
    /// Represents sorting information for pagination, allowing sorting by one or more properties
    /// with a specified direction (ascending or descending).
    /// </summary>
    public Sort(
        Direction direction,
        IEnumerable<string> properties
    ) : this(
        properties
            .Select(it => new Order(direction, it))
            .ToList()
    )
    {
    }

    /// Represents sorting information for ordered data.
    /// Provides utilities for specifying, combining, and manipulating ordering strategies across various properties or fields.
    public Sort(
        IEnumerable<string> properties
    ) : this(DefaultDirection, properties)
    {
    }

    /// Represents a sorting abstraction to be used for ordering data, commonly in pagination scenarios.
    public Sort(
        Direction direction,
        params string[] properties
    ) : this(
        direction,
        properties as IEnumerable<string>
    )
    {
    }

    /// <summary>
    /// Represents a sorting abstraction that encapsulates sorting logic based on one or more properties.
    /// </summary>
    public Sort(
        params string[] properties
    ) : this(DefaultDirection, properties as IEnumerable<string>)
    {
    }

    /// <summary>
    /// Returns a new Sort object with all current sorting orders set to ascending direction.
    /// </summary>
    /// <returns>A new Sort instance with the direction of all orders set to ascending.</returns>
    public Sort Ascending()
    {
        return WithDirection(Direction.Asc);
    }

    /// <summary>
    /// Returns a new <see cref="Sort"/> instance with all orders set to descending direction.
    /// </summary>
    /// <returns>A new <see cref="Sort"/> instance with descending direction for all orders.</returns>
    public Sort Descending()
    {
        return WithDirection(Direction.Desc);
    }

    /// <summary>
    /// Determines if the current Sort instance contains any sorting orders.
    /// </summary>
    /// <returns>
    /// True if the Sort contains at least one sorting order; otherwise, false.
    /// </returns>
    public bool IsSorted()
    {
        return Orders.Any();
    }

    /// <summary>
    /// Determines whether the current sort instance is unsorted.
    /// </summary>
    /// <returns>
    /// Returns true if the current sort instance has no sorting orders, otherwise false.
    /// </returns>
    public bool IsUnsorted()
    {
        return !IsSorted();
    }

    /// <summary>
    /// Creates a new <see cref="Sort"/> instance with the given direction applied to all orders.
    /// </summary>
    /// <param name="direction">The direction to be applied to all orders. It can be <see cref="Direction.Asc"/> or <see cref="Direction.Desc"/>.</param>
    /// <returns>A new <see cref="Sort"/> instance with updated order directions.</returns>
    public Sort WithDirection(
        Direction direction
    )
    {
        return new Sort(Orders.Select(it => new Order(direction, it.Property)).ToList());
    }

    /// Combines the current `Sort` instance with another `Sort` instance by merging their `Orders` lists into a single `Sort` instance.
    /// <param name="these">The `Sort` instance to combine with the current `Sort` instance.</param>
    /// <returns>A new `Sort` instance containing the combined list of `Orders` from both the current and the provided `Sort` instances.</returns>
    public Sort And(
        Sort these
    )
    {
        var listOfOrder = new List<Order>();
        listOfOrder.AddRange(Orders);
        listOfOrder.AddRange(these.Orders);
        return new Sort(listOfOrder);
    }

    /// <summary>
    /// Returns the hash code for the current instance of the Sort class.
    /// </summary>
    /// <returns>
    /// An integer hash code representing the current Sort instance. The hash code is computed based on the Orders list.
    /// </returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Orders.GetHashCode());
    }

    /// <summary>
    /// Compares the current Sort object with another object to determine equality.
    /// </summary>
    /// <param name="obj">The object to compare with the current Sort instance.</param>
    /// <returns>
    /// Returns true if the specified object is equal to the current Sort instance; otherwise, false.
    /// </returns>
    public override bool Equals(
        object? obj
    )
    {
        if (this == obj)
        {
            return true;
        }

        if (obj is null || GetType() != obj.GetType())
        {
            return false;
        }

        var sort = obj as Sort;
        return Orders.Equals(sort?.Orders);
    }
}
