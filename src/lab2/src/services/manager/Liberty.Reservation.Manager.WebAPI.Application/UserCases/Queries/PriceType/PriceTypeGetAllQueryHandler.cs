using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PriceType;

public class PriceTypeGetAllQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IAppDateTypeRepository appDateTypeRepository
) : QueryPageBaseHandler<PriceTypeGetAllQuery, PriceTypeResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        PriceTypeGetAllQuery request
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        return CacheHelper.GetCacheKeyByEntity(
            nameof(AppDateType),
            $"{string.Format(CacheKeys.FacilityPrefixKey, facilityId)}",
            CacheHelper.ComputeHash(
                [
                    nameof(PriceTypeGetAllQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, IEnumerable<PriceTypeResponse>)> HandleAsync(
        PriceTypeGetAllQuery request,
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
            .OrderByDescending(x => x.DisplayOrder);

        var page = await queryable.UsePageableAsDtoAsync<AppDateType, PriceTypeResponse>(
            request.Pageable,
            Mapper,
            cancellationToken: cancellationToken
        );

        var data = page.Content;
        var headers = page.GeneratePaginationHttpHeaders();

        return (headers, data);
    }
}
