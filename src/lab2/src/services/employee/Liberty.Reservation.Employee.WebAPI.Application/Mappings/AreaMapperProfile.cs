namespace Liberty.Reservation.Employee.WebAPI.Application.Mappings;

public class AreaMapperProfile : Profile
{
    public AreaMapperProfile()
    {
        CreateMap<AreaCreateRequest, Area>();
        CreateMap<AreaUpdateRequest, Area>();
        CreateMap<Area, AreaResponse>();
    }
}
