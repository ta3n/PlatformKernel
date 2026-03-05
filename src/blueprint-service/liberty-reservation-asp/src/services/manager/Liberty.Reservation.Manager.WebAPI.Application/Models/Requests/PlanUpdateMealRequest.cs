namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record PlanUpdateMealRequest(
    List<PlanMealTypeRequest>? MealTypes
);

public record PlanMealTypeRequest(
    long Id,
    MealTypeEatTypes MealTypeEatType
);
