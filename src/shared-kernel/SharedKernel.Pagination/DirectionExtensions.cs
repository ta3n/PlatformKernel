namespace SharedKernel.Pagination;

public static class DirectionExtensions
{
    public static bool IsAscending(
        this Direction direction
    )
    {
        return direction.Equals(Direction.Asc);
    }

    public static bool IsDescending(
        this Direction direction
    )
    {
        return direction.Equals(Direction.Desc);
    }
}
