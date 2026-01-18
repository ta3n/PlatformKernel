using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;

public record GetBookingSearchRequest(
    [property: JsonRequired] int Person,
    [property: JsonRequired] int RestNumber,
    [property: JsonRequired] int RoomNumber,
    [property: JsonRequired] string[] FacilityIds,
    [property: JsonRequired] long CheckInDate
);
