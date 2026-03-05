using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

public interface IRoomAdjustmentStatusService : IBaseService<RoomAdjustmentStatus>
{
    Task<RoomAdjustmentStatus> GetRoomAdjustmentStatusByCodeAsync(
        string? code,
        CancellationToken cancellationToken = default
    );
};
