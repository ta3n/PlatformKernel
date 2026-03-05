namespace Liberty.Reservation.Application.Models.Responses;

public record AlertMessageResponse(
    long Id,
    string? Title,
    string Content,
    string? Icon,
    string? Color
);
