using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IRoomGroupSiteService : IBaseServiceRelation<RoomGroupSite>
{
    Task<IEnumerable<RoomGroupSite>> FindAllByRoomGroupIdAsync(
        long roomGroupId,
        CancellationToken cancellationToken = default
    );

    Task<(IEnumerable<RoomGroupSite> adds, IEnumerable<RoomGroupSite> removes)>
        ChangeSitesOfRoomGroupAsync(
            long roomGroupId,
            IEnumerable<long> siteIds,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        );
}
