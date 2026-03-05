namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record SentFaxResponse(
    bool? IsSuccess,
    string? Result,
    string? ProcessKey,
    string? AcceptTime
);
