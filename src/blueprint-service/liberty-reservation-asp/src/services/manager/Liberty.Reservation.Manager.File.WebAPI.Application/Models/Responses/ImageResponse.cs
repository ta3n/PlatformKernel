namespace Liberty.Reservation.Manager.File.WebAPI.Application.Models.Responses;

public record ImageResponse(
    long Id,
    string? Code,
    string[]? Tags,
    string? Description,
    bool IsEnabled,
    int Index,
    float SizeBytes,
    string? Encrypt,
    long DisplayOrder,
    FilePurposeTypes FilePurposeType,
    List<long>? ImageCategoryIds,
    List<long>? MasterCategoryIds
)
{
    public List<FilePurposeTypes>? FilePurposeTypes { get; set; }
    public float SizeKiloBytes => SizeBytes / 1024;
    public float SizeMegaBytes => SizeKiloBytes / 1024;
    public float SizeGigaBytes => SizeMegaBytes / 1024;
};

public record ImagePreviewResponse(
    string ContentType,
    Stream? Image
);
