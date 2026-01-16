namespace PlatformKernel.Pagination;

public class Order(
    Direction direction,
    string property
)
{
    public Direction Direction { get; } = direction;
    public string Property { get; } = property;
}
