namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record DestinationCreateRequest(
    string? Name,
    string? ShortName,
    string? PrefixName,
    string? Url
);
