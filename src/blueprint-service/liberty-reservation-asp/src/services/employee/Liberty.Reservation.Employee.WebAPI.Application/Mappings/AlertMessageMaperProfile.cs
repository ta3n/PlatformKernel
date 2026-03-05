using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;

namespace Liberty.Reservation.Employee.WebAPI.Application.Mappings;

public class AlertMessageMaperProfile : Profile
{
    public AlertMessageMaperProfile()
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

        CreateMap<AlertMessageUpdateRequest, AlertMessage>()
            .ForMember(
                dest => dest.Title,
                opt => opt.MapFrom(
                    src =>
                        new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Title ?? string.Empty } }
                )
            )
            .ForMember(
                dest => dest.Content,
                opt => opt.MapFrom(
                    src =>
                        new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Content } }
                )
            );
    }
}
