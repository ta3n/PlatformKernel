namespace Liberty.Reservation.Manager.File.WebAPI.Application.Models.Responses;

public record FileResponse(
    long Id,
    string? Code,
    string[]? Tags,
    string? Description,
    bool IsEnabled,
    int Index,
    long DisplayOrder,
    FilePurposeTypes FilePurposeType,
    IEnumerable<ImageCategoryOfFileResponse>? ImageCategories,
    IEnumerable<ImageCategoryOfFileResponse>? MasterCategories
);

public record ImageCategoryOfFileResponse(
    long Id,
    string Name
);
