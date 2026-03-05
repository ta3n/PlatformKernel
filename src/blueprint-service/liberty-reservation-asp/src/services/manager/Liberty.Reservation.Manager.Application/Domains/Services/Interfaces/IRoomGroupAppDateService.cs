using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IRoomGroupAppDateService : IBaseServiceRelation<RoomGroupAppDate>
{
    Task<IEnumerable<RoomGroupAppDate>> FindAllByRoomGroupIdsAsync(
        IEnumerable<long> roomGroupIds,
        IEnumerable<long> appDateIds,
        CancellationToken cancellationToken = default
    );
}
