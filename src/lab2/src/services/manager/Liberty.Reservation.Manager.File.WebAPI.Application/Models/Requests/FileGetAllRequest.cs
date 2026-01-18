namespace Liberty.Reservation.Manager.File.WebAPI.Application.Models.Requests;

public record FileGetAllRequest(
    string[]? Codes,
    string[]? Records,
    FilePurposeTypes[]? FilePurposeTypes
);
