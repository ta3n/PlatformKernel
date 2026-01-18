using Newtonsoft.Json;

namespace Liberty.Reservation.Site.WebAPI.Application.Models.Requests;

public record BookingCheckRoomNumberRequest(
    [property: JsonRequired] long CheckInDate,
    [property: JsonRequired] int RoomNumber
);
