using Liberty.Reservation.Application.Domains.Repositories.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class RoomGroupAppDateService(
    ILogger<RoomGroupAppDateService> logger,
    IRoomGroupAppDateRepository roomGroupAppDateRepository
) : BaseServiceRelation<RoomGroupAppDate>(logger, roomGroupAppDateRepository), IRoomGroupAppDateService
{
    public async Task<IEnumerable<RoomGroupAppDate>> FindAllByRoomGroupIdsAsync(
        IEnumerable<long> roomGroupIds,
        IEnumerable<long> appDateIds,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = roomGroupAppDateRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => roomGroupIds.Contains(x.RoomGroupId))
            .Where(x => appDateIds.Contains(x.AppDateId));

        var data = await queryable.ToListAsync(cancellationToken);

        return data;
    }
}
