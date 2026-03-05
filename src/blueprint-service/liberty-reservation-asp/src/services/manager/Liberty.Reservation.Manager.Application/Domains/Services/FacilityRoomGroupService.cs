namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class FacilityRoomGroupService(
    ILogger<FacilityRoomGroupService> logger,
    IFacilityRoomGroupRepository facilityRoomGroupRepository
) : BaseServiceRelation<FacilityRoomGroup>(logger, facilityRoomGroupRepository), IFacilityRoomGroupService
{
    public async Task<IEnumerable<FacilityRoomGroup>> FindAllByRoomGroupIdAsync(
        long roomGroupId,
        CancellationToken cancellationToken = default
    )
    {
        var data = await facilityRoomGroupRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.RoomGroupId == roomGroupId)
            .ToListAsync(cancellationToken);

        return data;
    }

    public async Task<IEnumerable<FacilityRoomGroup>> FindAllByFacilityIdAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    )
    {
        var data = await facilityRoomGroupRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.FacilityId == facilityId)
            .ToListAsync(cancellationToken);

        return data;
    }

    public async Task<bool> CheckGroupNameExisting(
        long facilityId,
        long roomGroupId,
        string? groupName,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrEmpty(groupName))
        {
            return false;
        }

        var queryable = facilityRoomGroupRepository
            .GetQueryableWithAsNoTracking()
            .Include(x => x.RoomGroup)
            .Where(x => x.FacilityId == facilityId)
            .Where(x => x.RoomGroupId != roomGroupId)
            .Where(x => x.RoomGroup!.GroupName == groupName);

        var isExisting = await queryable.AnyAsync(cancellationToken);

        return isExisting;
    }
}
