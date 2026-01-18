namespace Liberty.Pagination;

public interface ISlice<out T> where T : class
{
    int Number { get; }

    int Size { get; }

    int NumberOfElements { get; }

    IEnumerable<T> Content { get; }

    Sort Sort { get; }

    bool IsFirst { get; }

    bool IsLast { get; }

    bool HasNext { get; }

    bool HasPrevious { get; }

    IPageable Pageable { get; }

    IPageable NextPageable { get; }

    IPageable PreviousPageable { get; }
}
