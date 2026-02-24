using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace SharedKernel.Pagination.Utilities;

public static class PaginationUtil
{
    private const string TotalCountHeaderName = "X-Total-Count";
    private const string PaginationHeaderName = "X-Pagination";

    public static IHeaderDictionary GeneratePaginationHttpHeaders<T>(
        this IPage<T> page
    )
        where T : class
    {
        IHeaderDictionary headers = new HeaderDictionary();
        PageResponse pageDto = new()
        {
            TotalCount = page.TotalElements,
            TotalPages = page.TotalPages,
            Page = page.Number,
            Size = page.Size,
            HasNext = page.HasNext,
            HasPrevious = page.HasPrevious,
            IsFirst = page.IsFirst,
            IsLast = page.IsLast
        };
        headers.Append(TotalCountHeaderName, page.TotalElements.ToString());
        headers.Append(PaginationHeaderName, JsonSerializer.Serialize(pageDto));
        return headers;
    }
}
