namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record RoomGroupUpdateOptionRequest(
    bool? UseFixedOptionItem,
    List<long>? OptionIds
) : PlanUpdateOptionRequest(
    UseFixedOptionItem,
    OptionIds
);
