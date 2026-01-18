using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.User.WebAPI.Application.Models.Responses;

public record QuestionOfBookingResponse(
    long Id,
    string? Name,
    string? Description,
    string? FormData,
    QuestionTypes Type,
    bool IsRequired
);
