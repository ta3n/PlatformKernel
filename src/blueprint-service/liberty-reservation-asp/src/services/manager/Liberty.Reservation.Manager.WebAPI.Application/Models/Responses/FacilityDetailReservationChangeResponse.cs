namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record FacilityDetailReservationChangeResponse
{
    public string? Code { get; init; }
    public bool CanAddRoomOnModify { get; init; }
}
