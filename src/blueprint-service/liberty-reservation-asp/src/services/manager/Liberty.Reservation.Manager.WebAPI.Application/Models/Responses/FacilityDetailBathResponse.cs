namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record FacilityDetailBathResponse
{
    public string? Code { get; init; }
    public string? SpaType { get; init; }
    public string? SpaName { get; init; }
    public string? SpaDescription { get; init; }
    public string? SpaInfoComment { get; init; }
}
