using Newtonsoft.Json;

namespace Liberty.Reservation.Site.Public.WebAPI.Models.Requests;

public record BookingCheckRoomNumberRequest(
    [property: JsonRequired] long CheckInDate,
    [property: JsonRequired] int RoomNumber
);
