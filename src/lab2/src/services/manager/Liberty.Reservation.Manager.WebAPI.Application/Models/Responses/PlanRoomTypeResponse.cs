namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record PlanRoomTypeResponse
{
    public long? Id { get; init; }
    public IEnumerable<RoomGroupOfPlanRoomTypeResponse>? RoomGroups { get; init; }
}

public record RoomGroupOfPlanRoomTypeResponse(
    long Id,
    string? Name,
    bool IsEnabled
);
