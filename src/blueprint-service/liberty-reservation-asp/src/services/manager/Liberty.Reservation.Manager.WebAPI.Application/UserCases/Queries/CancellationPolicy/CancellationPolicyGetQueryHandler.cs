using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.CancellationPolicy;

public class CancellationPolicyGetQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    ICancellationRepository cancellationRepository
) : QuerySingleBaseHandler<CancellationPolicyGetQuery, CancellationPolicyDetailResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        CancellationPolicyGetQuery request
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        return CacheHelper.GetCacheKeyByEntity(
            nameof(Cancellation),
            $"{string.Format(CacheKeys.FacilityPrefixKey, facilityId)}",
            CacheHelper.ComputeHash(
                [
                    nameof(CancellationPolicyGetQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, CancellationPolicyDetailResponse)> HandleAsync(
        CancellationPolicyGetQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = cancellationRepository
            .GetQueryableWithAsNoTracking()
            .Include(x => x.FacilityCancellations)
            .Include(x => x.CancellationCancellationDatas!)
            .ThenInclude(x => x.CancellationData)
            .Where(
                x => x.Id == request.Id
                    && x.FacilityCancellations!.Any(
                        t => t.FacilityId == facilityId
                    )
            )
            .ProjectTo<CancellationPolicyDetailResponse>(Mapper.ConfigurationProvider);

        var data = await queryable.SingleOrDefaultAsync(
                cancellationToken
            )
            ?? throw new CancellationNotfoundException();

        return (new HeaderDictionary(), data);
    }
}
