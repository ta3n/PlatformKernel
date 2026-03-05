using Newtonsoft.Json;

namespace Liberty.Reservation.Site.WebAPI.Application.Models.Requests;

public record BookingCheckNightNumberRequest(
    [property: JsonRequired] long CheckInDate,
    [property: JsonRequired] int RestNumber
);
