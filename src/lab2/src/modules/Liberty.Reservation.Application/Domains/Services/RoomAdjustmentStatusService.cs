using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Application.Domains.Services;

public class RoomAdjustmentStatusService(
    ILogger<RoomAdjustmentStatusService> logger,
    IRoomAdjustmentStatusRepository roomAdjustmentRepository
) : BaseService<RoomAdjustmentStatus>(logger, roomAdjustmentRepository, new RoomAdjustmentStatusNotfoundException()),
    IRoomAdjustmentStatusService
{
    public async Task<RoomAdjustmentStatus> GetRoomAdjustmentStatusByCodeAsync(
        string? code,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = roomAdjustmentRepository
            .GetQueryableWithAsNoTracking();

        var existingRoomAdjustment = await queryable
            .FirstOrDefaultAsync(
                x => x.Code == code,
                cancellationToken
            );

        return existingRoomAdjustment ?? throw new RoomAdjustmentStatusNotfoundException();
    }
};
