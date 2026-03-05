namespace Liberty.Reservation.User.File.WebAPI.Application.Models.Responses;

public record ImagePreviewResponse(
    string ContentType,
    Stream? Image
);
