namespace Liberty.Reservation.Employee.WebAPI.Application.Mappings;

public class ConsumptionTaxMapperProfile : Profile
{
    public ConsumptionTaxMapperProfile()
    {
        CreateMap<ConsumptionTax, ConsumptionTaxResponse>();
    }
}
