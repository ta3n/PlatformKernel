using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.User.WebAPI.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<MultilingualText, string>()
            .ConvertUsing(src => src == null ? string.Empty : src.GetValueByHeader(DefaultValues.LanguageCode));
    }
}
