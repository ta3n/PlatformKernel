using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.CancellationPolicy;

public class CancellationPolicyGetAllQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    ICancellationRepository cancellationRepository
) : QueryPageBaseHandler<CancellationPolicyGetAllQuery, CancellationPolicyResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        CancellationPolicyGetAllQuery request
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        return CacheHelper.GetCacheKeyByEntity(
            nameof(Cancellation),
            $"{string.Format(CacheKeys.FacilityPrefixKey, facilityId)}",
            CacheHelper.ComputeHash(
                [
                    nameof(CancellationPolicyGetAllQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, IEnumerable<CancellationPolicyResponse>)> HandleAsync(
        CancellationPolicyGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = cancellationRepository
            .GetQueryableWithAsNoTracking()
            .Include(x => x.FacilityCancellations)
            .Where(
                x => x.FacilityCancellations!.Any(
                    t => t.FacilityId == facilityId
                )
            )
            .OrderByDescending(x => x.DisplayOrder);

        var page = await queryable.UsePageableAsDtoAsync<Cancellation, CancellationPolicyResponse>(
            request.Pageable,
            Mapper,
            cancellationToken: cancellationToken
        );

        var data = page.Content;
        var headers = page.GeneratePaginationHttpHeaders();

        return (headers, data);
    }
}
