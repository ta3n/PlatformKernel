using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IFacilityRoomGroupService : IBaseServiceRelation<FacilityRoomGroup>
{
    Task<IEnumerable<FacilityRoomGroup>> FindAllByRoomGroupIdAsync(
        long roomGroupId,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<FacilityRoomGroup>> FindAllByFacilityIdAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    );

    Task<bool> CheckGroupNameExisting(
        long facilityId,
        long roomGroupId,
        string? groupName,
        CancellationToken cancellationToken = default
    );
}
