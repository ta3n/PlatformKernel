namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record PlanMealResponse
{
    public long Id { get; init; }
    public IEnumerable<TypeOfPlanMealResponse>? MealTypes { get; init; }
}

public record TypeOfPlanMealResponse(
    long Id,
    MealTypeEatTypes MealTypeEatType
);
