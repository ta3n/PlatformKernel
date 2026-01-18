namespace Liberty.ApplicationShared.Wrappers;

public class ResponseWrapper<T>
{
    public T? Data { get; set; }

    protected ResponseWrapper()
    {
    }

    public ResponseWrapper(
        T data
    )
    {
        Data = data;
    }
}

public class PagedResponseWrapper<T> : ResponseWrapper<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }

    public PagedResponseWrapper(
        T data,
        int pageNumber,
        int pageSize,
        int total
    )
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        Data = data;
        Total = total;
    }
}
