using Newtonsoft.Json;

namespace Liberty.Reservation.Site.Public.WebAPI.Models.Requests;

public record BookingOptionRequest(
    [property: JsonRequired] long AppDate
);
