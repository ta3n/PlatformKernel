namespace Liberty.Reservation.Manager.File.WebAPI.Application.Models.Requests;

public record FileCreateRequest(
    IFormFile File,
    FilePurposeTypes? FilePurposeType,
    List<long>? FileCategoryIds
);
