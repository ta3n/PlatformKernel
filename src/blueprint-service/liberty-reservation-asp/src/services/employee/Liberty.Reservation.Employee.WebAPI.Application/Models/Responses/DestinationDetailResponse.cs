namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record DestinationDetailResponse(
    long Id,
    string? Code,
    string? Name,
    string? ShortName,
    string? Url,
    bool IsEnabled
);
