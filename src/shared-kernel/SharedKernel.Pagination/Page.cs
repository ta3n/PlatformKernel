namespace SharedKernel.Pagination;

public class Page<T>(
    List<T> content,
    IPageable pageable,
    int total
) : Chunk<T>(content, pageable), IPage<T>
    where T : class
{
    public int Total { get; set; } = total;

    public override bool HasNext => Number + 1 <= TotalPages;

    public new bool IsLast => !HasNext;

    public int TotalPages => Size == 0 ? 1 : (int)Math.Ceiling(Total / (float)Size);

    public int TotalElements => Total;
}
