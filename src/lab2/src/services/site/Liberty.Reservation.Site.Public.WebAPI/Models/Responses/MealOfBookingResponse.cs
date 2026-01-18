namespace Liberty.Reservation.Site.Public.WebAPI.Models.Responses;

public record MealOfBookingResponse(
    long Id,
    bool? IsSelected,
    MealTypeEatTypes MealTypeEatType,
    string? Name
);
