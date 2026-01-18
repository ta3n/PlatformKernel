namespace Liberty.Application.Wrappers
{
    public class ResponseWrapper<T>
    {
        public ResponseWrapper()
        {

        }
        public ResponseWrapper(T data)
        {
            Data = data;
        }
        public T Data { get; set; }
    }

    public class PagedResponseWrapper<T> : ResponseWrapper<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }

        public PagedResponseWrapper(T data, int pageNumber, int pageSize, int total)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Data = data;
            Total = total;
        }
    }
}
