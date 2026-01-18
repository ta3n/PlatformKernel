namespace Liberty.Reservation.Manager.WebAPI.Application.Mappings;

public class MealTypeMapperProfile : Profile
{
    public MealTypeMapperProfile()
    {
        CreateMap<MealType, MealTypeResponse>();
    }
}
