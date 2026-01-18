namespace Liberty.Reservation.Employee.WebAPI.Application.Mappings;

public class PermissionMapperProfile : Profile
{
    public PermissionMapperProfile()
    {
        CreateMap<Permission, PermissionCreateRequest>().ReverseMap();
    }
}
