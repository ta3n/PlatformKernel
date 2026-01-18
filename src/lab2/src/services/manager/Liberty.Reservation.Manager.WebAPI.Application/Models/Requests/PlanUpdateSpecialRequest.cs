namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record PlanUpdateSpecialRequest(
    bool? IsSecret,
    string? SecretWord
);
