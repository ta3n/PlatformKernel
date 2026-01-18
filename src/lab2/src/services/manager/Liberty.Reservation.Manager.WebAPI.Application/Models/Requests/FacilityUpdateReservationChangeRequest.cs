using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record FacilityUpdateReservationChangeRequest(
    [property: JsonRequired] bool CanAddRoomOnModify,
    [property: JsonRequired] bool IsExtendedStayOnModify
);
