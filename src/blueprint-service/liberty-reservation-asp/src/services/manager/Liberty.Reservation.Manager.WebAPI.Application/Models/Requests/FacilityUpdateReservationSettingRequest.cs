using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record FacilityUpdateReservationSettingRequest(
    [property: JsonRequired] bool UseDailyPerson
);
