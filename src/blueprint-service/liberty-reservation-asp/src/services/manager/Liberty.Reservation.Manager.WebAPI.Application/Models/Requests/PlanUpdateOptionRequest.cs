namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record PlanUpdateOptionRequest(
    bool? UseFixedOptionItem,
    List<long>? OptionIds
);
