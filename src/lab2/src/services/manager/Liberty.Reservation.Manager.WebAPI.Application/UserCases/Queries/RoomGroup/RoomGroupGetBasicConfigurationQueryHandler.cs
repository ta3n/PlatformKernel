using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroup;

public class RoomGroupGetBasicConfigurationQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IRoomGroupRepository roomGroupRepository
) : QuerySingleBaseHandler<RoomGroupGetBasicConfigurationQuery, RoomGroupDetailBasicConfigurationResponse>(
    mapper,
    cacheService
)
{
    protected override string GetCacheKey(
        RoomGroupGetBasicConfigurationQuery request
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        return CacheHelper.GetCacheKeyByEntity(
            nameof(Reservation.Application.Contexts.DataContexts.Entities.Data.RoomGroup),
            $"{string.Format(CacheKeys.FacilityPrefixKey, facilityId)}",
            CacheHelper.ComputeHash(
                [
                    nameof(RoomGroupGetBasicConfigurationQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, RoomGroupDetailBasicConfigurationResponse)> HandleAsync(
        RoomGroupGetBasicConfigurationQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = roomGroupRepository
            .GetQueryableWithAsNoTracking()
            .Include(x => x.RoomGroupBedTypes!)
            .ThenInclude(x => x.BedType)
            .Include(x => x.FacilityRoomGroups)
            .Where(x => x.Id == request.Id)
            .Where(x => x.FacilityRoomGroups!.Any(y => y.FacilityId == facilityId))
            .ProjectTo<RoomGroupDetailBasicConfigurationResponse>(Mapper.ConfigurationProvider);

        var data = await queryable.SingleOrDefaultAsync(
                cancellationToken
            )
            ?? throw new RoomGroupNotfoundException();

        return (new HeaderDictionary(), data);
    }
}
