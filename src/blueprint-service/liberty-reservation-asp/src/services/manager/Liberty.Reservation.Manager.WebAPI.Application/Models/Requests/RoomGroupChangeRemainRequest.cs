using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record RoomGroupChangeRemainRequest(
    [property: JsonRequired] long AppDateId,
    [property: JsonRequired] long RoomGroupId,
    [property: JsonRequired] int SellNumber,
    [property: JsonRequired] bool IsNotSold
);
