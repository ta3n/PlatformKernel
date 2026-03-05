using Newtonsoft.Json;

namespace Liberty.Reservation.Site.WebAPI.Application.Models.Requests;

public record BookingOptionRequest(
    [property: JsonRequired] long AppDate
);
