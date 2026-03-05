namespace Liberty.Reservation.Site.File.WebAPI.Application.Models.Responses;

public record ImagePreviewResponse(
    string ContentType,
    Stream? Image
);
