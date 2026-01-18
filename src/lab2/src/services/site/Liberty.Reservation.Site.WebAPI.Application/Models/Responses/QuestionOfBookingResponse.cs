namespace Liberty.Reservation.Site.WebAPI.Application.Models.Responses;

public record QuestionOfBookingResponse(
    long Id,
    string? Name,
    string? Description,
    string? FormData,
    QuestionTypes Type,
    bool IsRequired
);
