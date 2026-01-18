namespace Liberty.Reservation.Manager.File.WebAPI.Application.Mappings;

public class ImageMapperProfile : Profile
{
    public ImageMapperProfile()
    {
        CreateMap<FacilityFile, ImageResponse>()
            .ConvertUsing(
                src =>
                    new ImageResponse(
                        src.FileId,
                        src.File!.Code,
                        string.IsNullOrEmpty(src.File!.Tag)
                            ? Array.Empty<string>()
                            : src.File!.Tag.Split(',', StringSplitOptions.RemoveEmptyEntries),
                        src.File!.Description,
                        src.File!.IsEnabled,
                        src.Index,
                        src.File!.FileSize,
                        src.File!.Encrypt,
                        src.File!.DisplayOrder,
                        src.FilePurposeType,
                        src.File!.FileCategories != null
                            ? src.File!.FileCategories.Where(x => x.Category!.CategoryType == CategoryTypes.File && !x.Category!.IsMaster)
                                .Select(fc => fc.CategoryId)
                                .ToList()
                            : new List<long>(),
                        src.File!.FileCategories != null
                            ? src.File!.FileCategories.Where(x => x.Category!.CategoryType == CategoryTypes.File && x.Category!.IsMaster)
                                .Select(fc => fc.CategoryId)
                                .ToList()
                            : new List<long>()
                    )
            );
    }
}
