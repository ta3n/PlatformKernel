using Liberty.ApplicationShared.Utils;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class RoomGroupSiteService(
    ILogger<RoomGroupSiteService> logger,
    IRoomGroupSiteRepository roomGroupSiteRepository
) : BaseServiceRelation<RoomGroupSite>(logger, roomGroupSiteRepository), IRoomGroupSiteService
{
    public async Task<IEnumerable<RoomGroupSite>> FindAllByRoomGroupIdAsync(
        long roomGroupId,
        CancellationToken cancellationToken = default
    )
    {
        var data = await roomGroupSiteRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.RoomGroupId == roomGroupId)
            .ToListAsync(cancellationToken);

        return data;
    }

    public async Task<(IEnumerable<RoomGroupSite> adds, IEnumerable<RoomGroupSite> removes)>
        ChangeSitesOfRoomGroupAsync(
            long roomGroupId,
            IEnumerable<long> siteIds,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        )
    {
        var existingSitesOfRoomGroup = (await FindAllByRoomGroupIdAsync(
            roomGroupId,
            cancellationToken
        )).ToList();
        var updateSitesOfRoomGroup = siteIds
            .Select(
                siteId => new RoomGroupSite
                {
                    RoomGroupId = roomGroupId,
                    SiteId = siteId
                }
            )
            .ToList();

        var comparer = new DelegateEqualityComparer<RoomGroupSite>(
            (
                x,
                y
            ) => x?.RoomGroupId == y?.RoomGroupId && x?.SiteId == y?.SiteId,
            obj => obj.RoomGroupId.GetHashCode() ^ obj.SiteId.GetHashCode()
        );

        var removeSitesInRoomGroup = existingSitesOfRoomGroup.Except(
            updateSitesOfRoomGroup,
            comparer
        );
        var addSitesInRoomGroup = updateSitesOfRoomGroup.Except(
            existingSitesOfRoomGroup,
            comparer
        );

        var removes = await DeleteRangeAsync(
            removeSitesInRoomGroup,
            autoSave,
            cancellationToken
        );

        var adds = await CreateRangeAsync(
            addSitesInRoomGroup,
            autoSave,
            cancellationToken
        );

        return (adds, removes);
    }
}
