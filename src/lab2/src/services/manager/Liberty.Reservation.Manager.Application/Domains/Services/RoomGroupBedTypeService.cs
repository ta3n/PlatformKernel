using Liberty.ApplicationShared.Utils;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class RoomGroupBedTypeService(
    ILogger<RoomGroupBedTypeService> logger,
    IRoomGroupBedTypeRepository roomGroupBedTypeRepository
) : BaseServiceRelation<RoomGroupBedType>(logger, roomGroupBedTypeRepository), IRoomGroupBedTypeService
{
    public async Task<IEnumerable<RoomGroupBedType>> FindAllByRoomGroupIdAsync(
        long roomGroupId,
        CancellationToken cancellationToken = default
    )
    {
        var data = await roomGroupBedTypeRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.RoomGroupId == roomGroupId)
            .ToListAsync(cancellationToken);

        return data;
    }

    public async Task<(IEnumerable<RoomGroupBedType> adds, IEnumerable<RoomGroupBedType> removes)>
        ChangeBedTypesOfRoomGroupAsync(
            long roomGroupId,
            IEnumerable<(long bedTypeId, int number)> adjustBedTypes,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        )
    {
        var existingBedTypesOfRoomGroup = (await FindAllByRoomGroupIdAsync(
            roomGroupId,
            cancellationToken
        )).ToList();
        var updateBedTypesOfRoomGroup = adjustBedTypes
            .Select(
                x => new RoomGroupBedType
                {
                    RoomGroupId = roomGroupId,
                    BedTypeId = x.bedTypeId,
                    Number = x.number
                }
            )
            .ToList();

        var comparer = new DelegateEqualityComparer<RoomGroupBedType>(
            (
                x,
                y
            ) => x?.RoomGroupId == y?.RoomGroupId && x?.BedTypeId == y?.BedTypeId && x?.Number == y?.Number,
            obj => obj.RoomGroupId.GetHashCode() ^ obj.BedTypeId.GetHashCode() ^ obj.Number.GetHashCode()
        );

        var removeBedTypesInRoomGroup = existingBedTypesOfRoomGroup.Except(
            updateBedTypesOfRoomGroup,
            comparer
        );
        var addBedTypesInRoomGroup = updateBedTypesOfRoomGroup.Except(
            existingBedTypesOfRoomGroup,
            comparer
        );

        var removes = await DeleteRangeAsync(
            removeBedTypesInRoomGroup,
            false,
            cancellationToken
        );

        var adds = await CreateRangeAsync(
            addBedTypesInRoomGroup,
            false,
            cancellationToken
        );

        return (adds, removes);
    }
}
