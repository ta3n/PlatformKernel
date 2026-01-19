using JHipsterNet.Core.Pagination;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BlueprintCqrs.Infrastructure.Web.Rest.Utilities;

public static class PaginationUtil
{
    private const string XTotalCountHeaderName = "X-Total-Count";
    private const string XPaginationHeaderName = "X-Pagination";

    public static IHeaderDictionary GeneratePaginationHttpHeaders<T>(
        this IPage<T> page
    )
        where T : class
    {
        IHeaderDictionary headers = new HeaderDictionary();
        var pageDto = new PageResponse
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
        headers.Add(XTotalCountHeaderName, page.TotalElements.ToString());
        headers.Add(XPaginationHeaderName, JsonConvert.SerializeObject(pageDto));
        return headers;
    }
}
