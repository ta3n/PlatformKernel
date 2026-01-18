using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IRoomGroupService : IBaseService<RoomGroup>
{
    Task<RoomGroup> UpdateBasicConfigurationOfRoomGroupAsync(
        RoomGroup entityToUpdate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    Task<RoomGroup> UpdateDisplaySettingOfRoomGroupAsync(
        RoomGroup entityToUpdate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    Task<RoomGroup> CreateRoomGroupWithFacilityAsync(
        RoomGroup entityToCreate,
        long facilityId,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    Task<(IEnumerable<RoomGroupCategory> adds, IEnumerable<RoomGroupCategory> removes)>
        ChangeCategoriesOfRoomGroupAsync(
            long roomGroupId,
            IEnumerable<long> categoryIds,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        );

    Task<(IEnumerable<RoomGroupSite> adds, IEnumerable<RoomGroupSite> removes)>
        ChangeSitesOfRoomGroupAsync(
            long roomGroupId,
            IEnumerable<long> siteIds,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        );

    Task<(IEnumerable<RoomGroupBedType> adds, IEnumerable<RoomGroupBedType> removes)>
        ChangeBedTypesOfRoomGroupAsync(
            long roomGroupId,
            IEnumerable<(long bedTypeId, int number)> adjustBedTypes,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        );

    Task<bool> CheckRoomGroupNameExistAsync(
        long roomGroupId,
        string? groupName,
        CancellationToken cancellationToken = default
    );

    Task UpdateLastModifiedAsync(
        long id,
        CancellationToken cancellationToken = default
    );

    Task<long> GetRoomGroupByGroupNameAsync(
        string groupName,
        CancellationToken cancellationToken = default
    );
}
