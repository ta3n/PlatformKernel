namespace Liberty.Reservation.Manager.WebAPI.Application.Mappings;

public class PriceTypeMapperProfile : Profile
{
    public PriceTypeMapperProfile()
    {
        CreateMap<PriceTypeCreateRequest, AppDateType>();

        CreateMap<PriceTypeUpdateRequest, AppDateType>();

        CreateMap<FacilityAppDateType, PriceTypeResponse>()
            .ConstructUsing(
                src => new PriceTypeResponse(
                    src.AppDateType!.Id,
                    src.AppDateType!.Name,
                    src.AppDateType!.ShortName,
                    src.AppDateType!.Color,
                    src.AppDateType!.DisplayOrder,
                    src.AppDateType!.IsEnabled
                )
            );

        CreateMap<AppDateType, PriceTypeResponse>()
            .ConstructUsing(
                src => new PriceTypeResponse(
                    src.Id,
                    src.Name,
                    src.ShortName,
                    src.Color,
                    src.DisplayOrder,
                    src.IsEnabled
                )
            );
    }
}
