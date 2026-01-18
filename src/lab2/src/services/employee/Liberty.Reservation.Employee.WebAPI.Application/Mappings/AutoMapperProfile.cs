using Liberty.Entity.ValueObjects;

namespace Liberty.Reservation.Employee.WebAPI.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<MultilingualText, string>().ConvertUsing(src => src == null ? string.Empty : src.GetValueByHeader());
    }
}
