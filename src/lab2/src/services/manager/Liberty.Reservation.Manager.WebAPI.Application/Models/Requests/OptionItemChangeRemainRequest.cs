using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record OptionItemChangeRemainRequest(
    [property: JsonRequired] long AppDateId,
    [property: JsonRequired] long OptionItemId,
    [property: JsonRequired] int SellNumber,
    [property: JsonRequired] bool IsNotSold
);
