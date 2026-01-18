using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Employee.WebAPI.Application.Mappings;

public class DestinationMapperProfile : Profile
{
    public DestinationMapperProfile()
    {
        CreateMap<DestinationCreateRequest, Site>()
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Name ?? string.Empty } }
                )
            );

        CreateMap<DestinationUpdateRequest, Site>()
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Name ?? string.Empty } }
                )
            );

        CreateMap<Site, DestinationResponse>()
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(src => src.Name!.GetValueByHeader())
            );

        CreateMap<Site, DestinationDetailResponse>()
            .ConstructUsing(
                src => new DestinationDetailResponse(
                    src.Id,
                    src.Code,
                    src.Name!.GetValueByHeader(),
                    src.ShortName,
                    src.PrefixName,
                    src.Url,
                    src.IsEnabled
                )
            );

        CreateMap<FacilitySite, DestinationResponse>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.Site!.Id)
            )
            .ForMember(
                dest => dest.Code,
                opt => opt.MapFrom(src => src.Site!.Code)
            )
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(src => src.Site!.Name!.GetValueByHeader())
            )
            .ForMember(
                dest => dest.ShortName,
                opt => opt.MapFrom(src => src.Site!.ShortName)
            )
            .ForMember(
                dest => dest.PrefixName,
                opt => opt.MapFrom(src => src.Site!.PrefixName)
            )
            .ForMember(
                dest => dest.Url,
                opt => opt.MapFrom(src => src.Site!.Url)
            )
            .ForMember(
                dest => dest.DisplayOrder,
                opt => opt.MapFrom(src => src.Site!.DisplayOrder)
            )
            .ForMember(
                dest => dest.IsEnabled,
                opt => opt.MapFrom(src => src.Site!.IsEnabled)
            );
    }
}
