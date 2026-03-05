namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record FacilityDetailClassificationResponse
{
    public string? Code { get; init; }
    public List<long>? Allergens { get; init; }
    public List<long>? Features { get; init; }
    public List<long>? Equipments { get; init; }
    public List<long>? Services { get; init; }
    public List<long>? Baths { get; init; }
    public List<long>? Sceneries { get; init; }
    public List<long>? Amenities { get; init; }
    public List<long>? Meals { get; init; }
}
