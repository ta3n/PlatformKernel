namespace Liberty.Reservation.Site.WebAPI.Application.Models.Responses;

public record MealOfBookingResponse(
    long Id,
    bool? IsSelected,
    MealTypeEatTypes MealTypeEatType,
    string? Name
);
