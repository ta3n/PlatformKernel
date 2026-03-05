namespace Liberty.Reservation.Manager.WebAPI.Application.Mappings;

public class SystemConfigMapperProfile : Profile
{
    public SystemConfigMapperProfile()
    {
        CreateMap<SystemConfig, SystemConfigResponse>();
    }
}
