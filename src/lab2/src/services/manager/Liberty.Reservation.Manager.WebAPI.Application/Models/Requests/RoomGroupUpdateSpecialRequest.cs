namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record RoomGroupUpdateSpecialRequest(
    bool? IsSecret,
    string? SecretWord
) : PlanUpdateSpecialRequest(
    IsSecret,
    SecretWord
);
