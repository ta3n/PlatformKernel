using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PriceType;

public class PriceTypeGetQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IAppDateTypeRepository appDateTypeRepository
) : QuerySingleBaseHandler<PriceTypeGetQuery, PriceTypeResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        PriceTypeGetQuery request
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        return CacheHelper.GetCacheKeyByEntity(
            nameof(AppDateType),
            $"{string.Format(CacheKeys.FacilityPrefixKey, facilityId)}",
            CacheHelper.ComputeHash(
                [
                    nameof(PriceTypeGetQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, PriceTypeResponse)> HandleAsync(
        PriceTypeGetQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = appDateTypeRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x => x.FacilityAppDateTypes!.Any(
                    t => t.FacilityId == facilityId
                )
            )
            .Where(x => x.Id == request.Id)
            .ProjectTo<PriceTypeResponse>(Mapper.ConfigurationProvider);

        var data = await queryable.SingleOrDefaultAsync(
                cancellationToken
            )
            ?? throw new AppDateTypeNotfoundException();

        return (new HeaderDictionary(), data);
    }
}
