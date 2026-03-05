using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.User.WebAPI.Application.Models.Responses;

namespace Liberty.Reservation.User.WebAPI.Application.Mappings;

public class SystemConfigMapperProfile : Profile
{
    public SystemConfigMapperProfile()
    {
        CreateMap<SystemConfig, SystemConfigResponse>().ReverseMap();
    }
}
