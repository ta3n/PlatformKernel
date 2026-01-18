namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record FacilityUpdateClassificationRequest(
    List<long>? Allergens,
    List<long>? Features,
    List<long>? Equipments,
    List<long>? Services,
    List<long>? Baths,
    List<long>? Sceneries,
    List<long>? Amenities,
    List<long>? Meals
);
