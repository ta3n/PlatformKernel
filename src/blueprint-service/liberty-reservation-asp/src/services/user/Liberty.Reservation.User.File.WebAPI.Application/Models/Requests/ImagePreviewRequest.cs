namespace Liberty.Reservation.User.File.WebAPI.Application.Models.Requests;

public record ImagePreviewRequest(
    string Code,
    string? SizeType
);
