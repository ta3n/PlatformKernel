using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroup;

public class RoomGroupGetAllQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IRoomGroupRepository roomGroupRepository
) : QueryPageBaseHandler<RoomGroupGetAllQuery, RoomGroupResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        RoomGroupGetAllQuery request
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        return CacheHelper.GetCacheKeyByEntity(
            nameof(Reservation.Application.Contexts.DataContexts.Entities.Data.RoomGroup),
            $"{string.Format(CacheKeys.FacilityPrefixKey, facilityId)}",
            CacheHelper.ComputeHash(
                [
                    nameof(RoomGroupGetAllQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, IEnumerable<RoomGroupResponse>)> HandleAsync(
        RoomGroupGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = roomGroupRepository
            .GetQueryableWithAsNoTracking()
            .Include(x => x.FileRoomGroups!)
            .ThenInclude(x => x.File)
            .Where(
                x => x.FacilityRoomGroups!.Any(
                    t => t.FacilityId == facilityId
                )
            )
            .OrderByDescending(x => x.DisplayOrder);

        var page = await queryable.UsePageableAsDtoAsync
            <Reservation.Application.Contexts.DataContexts.Entities.Data.RoomGroup, RoomGroupResponse>(
                request.Pageable,
                Mapper,
                cancellationToken: cancellationToken
            );

        var data = page.Content;
        var headers = page.GeneratePaginationHttpHeaders();

        return (headers, data);
    }
}
