namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record UploadAAvatarOfSelfRequest(
    string FileId,
    string FileSecret
);
