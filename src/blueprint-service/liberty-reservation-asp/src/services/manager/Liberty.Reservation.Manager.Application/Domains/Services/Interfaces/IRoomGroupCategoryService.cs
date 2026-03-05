using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IRoomGroupCategoryService : IBaseServiceRelation<RoomGroupCategory>
{
    Task<IEnumerable<RoomGroupCategory>> FindAllByRoomGroupIdAsync(
        long roomGroupId,
        CancellationToken cancellationToken = default
    );

    Task<(IEnumerable<RoomGroupCategory> adds, IEnumerable<RoomGroupCategory> removes)>
        ChangeCategoriesOfRoomGroupAsync(
            long roomGroupId,
            IEnumerable<long> categoryIds,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        );
}
