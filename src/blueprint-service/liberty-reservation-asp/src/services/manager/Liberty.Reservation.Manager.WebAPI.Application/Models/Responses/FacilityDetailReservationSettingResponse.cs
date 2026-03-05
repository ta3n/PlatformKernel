namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record FacilityDetailReservationSettingResponse
{
    public string? Code { get; init; }
    public bool UseDailyPerson { get; init; }
}
