namespace Liberty.Reservation.Manager.File.WebAPI.Application.Models.Requests;

public record ImageCreateRequest(
    IFormFile File,
    List<FilePurposeTypes>? FilePurposeTypes,
    List<long>? ImageCategoryIds
);
