namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record DestinationDetailResponse(
    long Id,
    string? Code,
    string? Name,
    string? ShortName,
    string? PrefixName,
    string? Url,
    bool IsEnabled
);
