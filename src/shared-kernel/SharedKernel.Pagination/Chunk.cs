using System.Text.Json.Serialization;

namespace SharedKernel.Pagination;

public abstract class Chunk<T> : ISlice<T> where T : class
{
    protected Chunk(
        List<T>? content,
        IPageable? pageable
    )
    {
        Content = [];
        if (content != null)
        {
            ((List<T>)Content).AddRange(content);
        }

        Pageable = pageable;
        if (Pageable is null)
        {
            return;
        }

        Number = Pageable.IsPaged ? Pageable.PageNumber : 0;
        Size = Pageable.IsPaged ? Pageable.PageSize : 0;
    }

    public IList<T> Content { get; }

    public int Number { get; set; }

    public int Size { get; set; }

    public int NumberOfElements => Content.Count;

    IEnumerable<T> ISlice<T>.Content => Content;

    [JsonIgnore]
    public Sort? Sort => Pageable?.Sort;

    public bool IsFirst => !HasPrevious;

    public bool IsLast => !HasNext;

    public abstract bool HasNext { get; }
    public bool HasPrevious => Number > 0;

    [JsonIgnore]
    public IPageable? Pageable { get; }

    [JsonIgnore]
    public IPageable? NextPageable => HasNext ? Pageable?.Next : PageableConstants.UnPaged;

    [JsonIgnore]
    public IPageable? PreviousPageable => HasPrevious ? Pageable?.PreviousOrFirst : PageableConstants.UnPaged;
}
