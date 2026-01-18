namespace Liberty.Reservation.Site.File.WebAPI.Application.Models.Requests;

public record ImagePreviewRequest(
    string Code,
    string? SizeType
);
