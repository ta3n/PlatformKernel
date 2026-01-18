using Newtonsoft.Json;

namespace Liberty.Reservation.Site.Public.WebAPI.Models.Requests;

public record BookingCheckNightNumberRequest(
    [property: JsonRequired] long CheckInDate,
    [property: JsonRequired] int RestNumber
);
