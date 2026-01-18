using Liberty.Entity.ValueObjects;

namespace Liberty.Reservation.Manager.WebAPI.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<BedType, BedTypeResponse>()
            .ConstructUsing(
                src => new BedTypeResponse(
                    src.Id,
                    src.Code,
                    src.Name,
                    src.BedTypeUnitType,
                    src.IsEnabled
                )
            );

        CreateMap<MultilingualText, string>().ConvertUsing(src => src == null ? string.Empty : src.GetValueByHeader());
    }
}
