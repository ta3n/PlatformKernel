using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Application.Mappings;

public class AlertMessageMapperProfile : Profile
{
    protected AlertMessageMapperProfile()
    {
        CreateMap<AlertMessage, AlertMessageResponse>()
            .ForMember(
                dest => dest.Title,
                opt => opt.MapFrom(
                    src => src.Title != null
                        ? src.Title.GetValueByHeader()
                        : string.Empty
                )
            )
            .ForMember(
                dest => dest.Content,
                opt => opt.MapFrom(
                    src => src.Content != null
                        ? src.Content.GetValueByHeader()
                        : string.Empty
                )
            );
    }
}
