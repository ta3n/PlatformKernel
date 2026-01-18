namespace Liberty.Pagination;

public interface IPage<out T> : ISlice<T> where T : class
{
    int TotalPages { get; }
    int TotalElements { get; }
}
