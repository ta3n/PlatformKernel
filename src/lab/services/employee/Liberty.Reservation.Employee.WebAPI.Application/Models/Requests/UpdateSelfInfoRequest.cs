namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record UpdateSelfInfoRequest(
    string Name,
    string? Kana
);
