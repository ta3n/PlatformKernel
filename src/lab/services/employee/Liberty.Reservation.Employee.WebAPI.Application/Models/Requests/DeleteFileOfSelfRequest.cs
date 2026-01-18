namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record DeleteFileOfSelfRequest(
    string FileId,
    string FileSecret
);
