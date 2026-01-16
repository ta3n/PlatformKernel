using Newtonsoft.Json;
using PlatformKernel.Pagination;

namespace PlatformKernel.Service.WebApi.Application.Web.Rest.Utilities;

public static class PaginationUtil
{
    private const string TotalCountHeaderName = "X-Total-Count";
    private const string PaginationHeaderName = "X-Pagination";

    public static IHeaderDictionary GeneratePaginationHttpHeaders<T>(
        this IPage<T> page
    )
        where T : class
    {

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

        IHeaderDictionary headers = new HeaderDictionary();
        headers.Append(TotalCountHeaderName, page.TotalElements.ToString());
        headers.Append(PaginationHeaderName, JsonConvert.SerializeObject(pageDto));

        return headers;
    }
}
