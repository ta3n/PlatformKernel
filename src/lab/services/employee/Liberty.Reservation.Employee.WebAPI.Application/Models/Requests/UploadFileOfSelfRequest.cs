namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record UploadFileOfSelfRequest(
    string FileId,
    string FileSecret
);
