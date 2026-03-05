using Liberty.Entity;
using static Liberty.Reservation.Application.Models.Responses.SetRoomsResponse;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

public class AdjustmentResult : EntityData
{
    public string HotelId { get; set; } = string.Empty;

    public string RoomId { get; set; } = string.Empty;

    public FailureReason? Reason { get; set; }

    public long RoomAdjustmentStatusId { get; set; }

    public RoomAdjustmentStatus? RoomAdjustmentStatus { get; set; }

    public bool IsSuccess { get; set; }
}
