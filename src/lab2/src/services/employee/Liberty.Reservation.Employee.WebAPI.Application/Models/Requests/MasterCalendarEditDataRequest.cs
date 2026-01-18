using Newtonsoft.Json;

namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record MasterCalendarEditDataRequest(
    [property: JsonRequired] long AppDate,
    string? Name
);
