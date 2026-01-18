namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record CreateEmployeeRequest(
    string Name,
    string? Kana,
    string Email,
    string? Tel,
    string? Mobile,
    string? Gender,
    DateOnly? BirthDay
);
