namespace Liberty.Pagination;

public class Sort(
    IList<Order>? orders
)
{
    public const Direction DefaultDirection = Direction.Asc;
    public static readonly Sort Unsorted = new();
    public IList<Order> Orders { get; } = orders ?? new List<Order>();

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

    public Sort(
        IEnumerable<string> properties
    ) : this(DefaultDirection, properties)
    {
    }

    public Sort(
        Direction direction,
        params string[] properties
    ) : this(
        direction,
        properties as IEnumerable<string>
    )
    {
    }

    public Sort(
        params string[] properties
    ) : this(DefaultDirection, properties as IEnumerable<string>)
    {
    }

    public Sort Ascending()
    {
        return WithDirection(Direction.Asc);
    }

    public Sort Descending()
    {
        return WithDirection(Direction.Desc);
    }

    public bool IsSorted()
    {
        return Orders.Any();
    }

    public bool IsUnsorted()
    {
        return !IsSorted();
    }

    public Sort WithDirection(
        Direction direction
    )
    {
        return new Sort(Orders.Select(it => new Order(direction, it.Property)).ToList());
    }

    public Sort And(
        Sort these
    )
    {
        var orders = new List<Order>();
        orders.AddRange(Orders);
        orders.AddRange(these.Orders);
        return new Sort(orders);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Orders.GetHashCode());
    }

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
