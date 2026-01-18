using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Liberty.Pagination;

public class Pageable : IPageable
{
    public Pageable(
        int pageNumber,
        int pageSize,
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
    }

    [ValidateNever]
    private IPageable Previous => new Pageable(PageNumber - 1, PageSize, Sort);

    public bool IsPaged => true;

    public int PageNumber { get; }
    public int PageSize { get; }

    public int Offset => PageNumber * PageSize;
    public Sort Sort { get; }

    [ValidateNever]
    public IPageable Next => new Pageable(PageNumber + 1, PageSize, Sort);

    [ValidateNever]
    public IPageable PreviousOrFirst => HasPrevious ? Previous : First;

    [ValidateNever]
    public IPageable First => new Pageable(0, PageSize, Sort);

    public bool HasPrevious => PageNumber > 0;

    public static Pageable Of(
        int pageNumber,
        int pageSize,
        Sort? sort = null
    )
    {
        return new Pageable(pageNumber, pageSize, sort);
    }

    public static Pageable Of(
        int pageNumber,
        int pageSize,
        Direction direction,
        params string[] properties
    )
    {
        return new Pageable(pageNumber, pageSize, new Sort(direction, properties));
    }
}
