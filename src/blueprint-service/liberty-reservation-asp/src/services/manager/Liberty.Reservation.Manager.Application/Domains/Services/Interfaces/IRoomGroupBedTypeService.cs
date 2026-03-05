using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IRoomGroupBedTypeService : IBaseServiceRelation<RoomGroupBedType>
{
    Task<IEnumerable<RoomGroupBedType>> FindAllByRoomGroupIdAsync(
        long roomGroupId,
        CancellationToken cancellationToken = default
    );

    Task<(IEnumerable<RoomGroupBedType> adds, IEnumerable<RoomGroupBedType> removes)>
        ChangeBedTypesOfRoomGroupAsync(
            long roomGroupId,
            IEnumerable<(long bedTypeId, int number)> adjustBedTypes,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        );
}
