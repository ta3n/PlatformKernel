namespace Liberty.Reservation.Site.WebAPI.Application.Mappings;

public class SystemConfigMapperProfile : Profile
{
    public SystemConfigMapperProfile()
    {
        CreateMap<SystemConfig, SystemConfigResponse>().ReverseMap();
    }
}
