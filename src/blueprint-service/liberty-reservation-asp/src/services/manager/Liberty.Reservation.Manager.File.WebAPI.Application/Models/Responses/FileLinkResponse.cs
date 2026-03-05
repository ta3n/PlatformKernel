namespace Liberty.Reservation.Manager.File.WebAPI.Application.Models.Responses;

public record FileLinkResponse(
    long RecordCode,
    string? Code,
    int Index,
    string? Type,
    string? ContentType,
    bool IsEnabled
);
