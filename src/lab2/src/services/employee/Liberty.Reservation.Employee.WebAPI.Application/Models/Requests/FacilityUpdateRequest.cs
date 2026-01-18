using Newtonsoft.Json;

namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record FacilityUpdateRequest(
    long[]? SiteIds,
    [property: JsonRequired] bool CanOnLinePayment,
    string? SystemEMail,
    string? Memo
)
{
    public long? Id { get; set; }
}
