using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroup;

public class RoomGroupGetAllPublishedInQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IRoomGroupRepository roomGroupRepository
) : QueryPageBaseHandler<RoomGroupGetAllPublishedInQuery, SiteOfRoomGroupDetailPublicationSettingResponse>(
    mapper,
    cacheService
)
{
    protected override string GetCacheKey(
        RoomGroupGetAllPublishedInQuery request
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        return CacheHelper.GetCacheKeyByEntity(
            nameof(Reservation.Application.Contexts.DataContexts.Entities.Data.RoomGroup),
            $"{string.Format(CacheKeys.FacilityPrefixKey, facilityId)}",
            CacheHelper.ComputeHash(
                [
                    nameof(RoomGroupGetAllPublishedInQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, IEnumerable<SiteOfRoomGroupDetailPublicationSettingResponse>)>
        HandleAsync(
            RoomGroupGetAllPublishedInQuery request,
            CancellationToken cancellationToken
        )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = roomGroupRepository
            .GetQueryableWithAsNoTracking()
            .Include(x => x.RoomGroupSites!)
            .ThenInclude(x => x.Site)
            .Include(x => x.FacilityRoomGroups)
            .Where(x => x.Id == request.Id)
            .Where(
                x => x.FacilityRoomGroups!.Any(
                    t => t.FacilityId == facilityId
                )
            )
            .SelectMany(x => x.RoomGroupSites!)
            .Select(x => x.Site)
            .Select(
                x => new SiteOfRoomGroupDetailPublicationSettingResponse(
                    x!.Id,
                    x.Name!.GetValueByHeader()
                )
            );

        var page = await queryable.UsePageableAsync(
            request.Pageable,
            cancellationToken: cancellationToken
        );

        var data = page.Content;
        var headers = page.GeneratePaginationHttpHeaders();

        return (headers, data);
    }
}
