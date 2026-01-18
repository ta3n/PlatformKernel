namespace Liberty.Reservation.Employee.WebAPI.Application.Mappings;

public class SystemConfigMapperProfile : Profile
{
    public SystemConfigMapperProfile()
    {
        CreateMap<SystemConfig, SystemConfigResponse>().ReverseMap();
    }
}
