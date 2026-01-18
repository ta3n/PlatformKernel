using Newtonsoft.Json;

namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record MasterCalendarEditTypeRequest(
    [property: JsonRequired] long AppDate,
    [property: JsonRequired] long AppDateTypeId
);
