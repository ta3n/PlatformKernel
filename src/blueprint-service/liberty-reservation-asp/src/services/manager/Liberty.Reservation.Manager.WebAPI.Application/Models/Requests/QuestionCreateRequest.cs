namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record QuestionCreateRequest(
    string? Name,
    string? Description
);
