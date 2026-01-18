namespace Liberty.Reservation.Site.Public.WebAPI.Models.Responses;

public record QuestionOfBookingResponse(
    long Id,
    string? Name,
    string? Description,
    string? FormData,
    QuestionTypes Type
);
