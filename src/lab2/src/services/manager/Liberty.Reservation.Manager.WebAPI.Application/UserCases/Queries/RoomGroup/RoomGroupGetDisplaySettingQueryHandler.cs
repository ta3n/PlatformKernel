using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroup;

public class RoomGroupGetDisplaySettingQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IRoomGroupRepository roomGroupRepository
) : QuerySingleBaseHandler<RoomGroupGetDisplaySettingQuery, RoomGroupDetailDisplaySettingResponse>(
    mapper,
    cacheService
)
{
    protected override string GetCacheKey(
        RoomGroupGetDisplaySettingQuery request
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        return CacheHelper.GetCacheKeyByEntity(
            nameof(Reservation.Application.Contexts.DataContexts.Entities.Data.RoomGroup),
            $"{string.Format(CacheKeys.FacilityPrefixKey, facilityId)}",
            CacheHelper.ComputeHash(
                [
                    nameof(RoomGroupGetDisplaySettingQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, RoomGroupDetailDisplaySettingResponse)> HandleAsync(
        RoomGroupGetDisplaySettingQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = roomGroupRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == request.Id)
            .Where(x => x.FacilityRoomGroups!.Any(y => y.FacilityId == facilityId))
            .ProjectTo<RoomGroupDetailDisplaySettingResponse>(Mapper.ConfigurationProvider);

        var data = await queryable.SingleOrDefaultAsync(
                cancellationToken
            )
            ?? throw new RoomGroupNotfoundException();

        return (new HeaderDictionary(), data);
    }
}
