using AutoMapper.QueryableExtensions;
using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Destination;

public class DestinationGetQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISiteRepository siteRepository
) : QuerySingleBaseHandler<DestinationGetQuery, DestinationDetailResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        DestinationGetQuery request
    )
    {
        return CacheHelper.GetCacheKeyByEntity(
            nameof(Site),
            CacheHelper.ComputeHash(
                [
                    nameof(DestinationGetQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, DestinationDetailResponse)> HandleAsync(
        DestinationGetQuery request,
        CancellationToken cancellationToken
    )
    {
        var queryable = siteRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == request.Id)
            .ProjectTo<DestinationDetailResponse>(Mapper.ConfigurationProvider);

        var data = await queryable.SingleOrDefaultAsync(
                cancellationToken
            )
            ?? throw new SiteNotfoundException();

        return (new HeaderDictionary(), data);
    }
}
