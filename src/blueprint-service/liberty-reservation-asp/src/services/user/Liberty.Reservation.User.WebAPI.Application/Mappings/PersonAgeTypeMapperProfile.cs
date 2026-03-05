using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.User.WebAPI.Application.Models.Responses;

namespace Liberty.Reservation.User.WebAPI.Application.Mappings;

public class PersonAgeTypeMapperProfile : Profile
{
    public PersonAgeTypeMapperProfile()
    {
        CreateMap<PersonAgeType, PersonAgeTypeResponse>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(x => x.Id)
            )
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(x => x.Name)
            )
            .ForMember(
                dest => dest.IsMain,
                opt => opt.MapFrom(x => x.IsMain)
            )
            .ForMember(
                dest => dest.AgeMin,
                opt => opt.MapFrom(x => x.AgeMin)
            )
            .ForMember(
                dest => dest.AgeMax,
                opt => opt.MapFrom(x => x.AgeMax)
            );
    }
}
