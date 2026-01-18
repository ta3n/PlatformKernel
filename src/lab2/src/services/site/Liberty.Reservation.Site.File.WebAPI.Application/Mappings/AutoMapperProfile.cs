using Liberty.Entity.ValueObjects;

namespace Liberty.Reservation.Site.File.WebAPI.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<MultilingualText, string>().ConvertUsing(src => src == null ? string.Empty : src.GetValueByHeader());
    }
}
