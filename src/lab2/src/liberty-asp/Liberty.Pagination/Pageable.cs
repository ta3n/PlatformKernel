using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;

namespace Liberty.Pagination;

public class Pageable : IPageable
{
    private Pageable(
        int pageNumber,
        int pageSize,
        bool? isEnabled = null,
        Sort? sort = null
    )
    {
        if (pageNumber < 0)
        {
            throw new ArgumentNullException(nameof(pageNumber), "Page Number must not be less than zero!");
        }

        if (pageSize < 1)
        {
            throw new ArgumentNullException(nameof(pageSize), "Page Size must not be less than one!");
        }

        PageNumber = pageNumber;
        PageSize = pageSize;
        Sort = sort ?? Sort.Unsorted;
        IsEnabled = isEnabled;
    }

    [ValidateNever]
    [JsonIgnore]
    private IPageable Previous => new Pageable(PageNumber - 1, PageSize, IsEnabled, Sort);

    public bool IsPaged => true;

    public int PageNumber { get; }
    public int PageSize { get; }
    public bool? IsEnabled { get; }
    public int Offset => Math.Max(0, PageNumber - 1) * PageSize;
    public Sort Sort { get; }

    [ValidateNever]
    [JsonIgnore]
    public IPageable Next => new Pageable(PageNumber + 1, PageSize, IsEnabled, Sort);

    [ValidateNever]
    [JsonIgnore]
    public IPageable PreviousOrFirst => HasPrevious ? Previous : First;

    [ValidateNever]
    [JsonIgnore]
    public IPageable First => new Pageable(0, PageSize, IsEnabled, Sort);

    public bool HasPrevious => PageNumber > 0;

    public static Pageable Of(
        int pageNumber,
        int pageSize,
        bool? isEnabled = null,
        Sort? sort = null
    )
    {
        return new Pageable(pageNumber, pageSize, isEnabled, sort);
    }

    public static Pageable Of(
        int pageNumber,
        int pageSize,
        Direction direction,
        params string[] properties
    )
    {
        return new Pageable(pageNumber, pageSize, sort: new Sort(direction, properties));
    }
}
