using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroup;

public class RoomGroupGetPublicationSettingQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IRoomGroupRepository roomGroupRepository
) : QuerySingleBaseHandler<RoomGroupGetPublicationSettingQuery, RoomGroupDetailPublicationSettingResponse>(
    mapper,
    cacheService
)
{
    protected override string GetCacheKey(
        RoomGroupGetPublicationSettingQuery request
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        return CacheHelper.GetCacheKeyByEntity(
            nameof(Reservation.Application.Contexts.DataContexts.Entities.Data.Plan),
            $"{string.Format(CacheKeys.FacilityPrefixKey, facilityId)}",
            CacheHelper.ComputeHash(
                [
                    nameof(RoomGroupGetPublicationSettingQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, RoomGroupDetailPublicationSettingResponse)> HandleAsync(
        RoomGroupGetPublicationSettingQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = roomGroupRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == request.Id)
            .Where(x => x.FacilityRoomGroups!.Any(y => y.FacilityId == facilityId))
            .ProjectTo<RoomGroupDetailPublicationSettingResponse>(Mapper.ConfigurationProvider);

        var data = await queryable.SingleOrDefaultAsync(
                cancellationToken
            )
            ?? throw new RoomGroupNotfoundException();

        return (new HeaderDictionary(), data);
    }
}
