using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Destination;

public class DestinationGetAllQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISiteRepository siteRepository
) : QueryPageBaseHandler<DestinationGetAllQuery, DestinationResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        DestinationGetAllQuery request
    )
    {
        return CacheHelper.GetCacheKeyByEntity(
            nameof(Site),
            CacheHelper.ComputeHash(
                [
                    nameof(DestinationGetAllQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, IEnumerable<DestinationResponse>)> HandleAsync(
        DestinationGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var queryable = siteRepository
            .GetQueryableWithAsNoTracking()
            .OrderByDescending(x => x.DisplayOrder);

        var page = await queryable.UsePageableAsDtoAsync<Site, DestinationResponse>(
            request.Pageable,
            Mapper,
            cancellationToken: cancellationToken
        );

        var data = page.Content;
        var headers = page.GeneratePaginationHttpHeaders();

        return (headers, data);
    }
}
