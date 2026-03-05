namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record RoomGroupUpdateMealRequest(
    List<PlanMealTypeRequest>? MealTypes
) : PlanUpdateMealRequest(MealTypes);
