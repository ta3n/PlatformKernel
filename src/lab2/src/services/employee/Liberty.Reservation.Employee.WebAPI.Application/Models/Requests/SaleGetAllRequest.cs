using Newtonsoft.Json;

namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record SaleGetAllRequest(
    [property: JsonRequired] long StartAppDateId,
    [property: JsonRequired] long EndAppDateId,
    IEnumerable<long> FacilityIds
);
