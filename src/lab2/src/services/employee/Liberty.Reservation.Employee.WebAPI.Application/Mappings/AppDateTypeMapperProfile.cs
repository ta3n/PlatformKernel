namespace Liberty.Reservation.Employee.WebAPI.Application.Mappings;

public class AppDateTypeMapperProfile : Profile
{
    public AppDateTypeMapperProfile()
    {
        CreateMap<AppDateTypeCreateRequest, AppDateType>().ReverseMap();
        CreateMap<AppDateTypeUpdateRequest, AppDateType>().ReverseMap();
        CreateMap<AppDateTypeResponse, AppDateType>().ReverseMap();
    }
}
