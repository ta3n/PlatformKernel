namespace Liberty.Reservation.Manager.WebAPI.Application.Mappings;

public class AllergenMapperProfile : Profile
{
    public AllergenMapperProfile()
    {
        CreateMap<Allergen, AllergenResponse>();
    }
}
