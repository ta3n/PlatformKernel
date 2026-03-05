namespace Liberty.Reservation.Employee.WebAPI.Application.Mappings;

public class FaxServiceMapperProfile : Profile
{
    public FaxServiceMapperProfile()
    {
        CreateMap<FaxService, FaxServiceResponse>();
    }
}
