using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;

namespace Liberty.Reservation.Employee.WebAPI.Application.Mappings;

public class CategoryMapperProfile : Profile
{
    public CategoryMapperProfile()
    {
        CreateMap<CategoryCreateRequest, Category>()
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Name ?? string.Empty } }
                )
            )
            .ForMember(
                dest => dest.Description,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Description ?? string.Empty } }
                )
            );

        CreateMap<CategoryUpdateRequest, Category>()
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Name ?? string.Empty } }
                )
            )
            .ForMember(
                dest => dest.Description,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Description ?? string.Empty } }
                )
            );

        CreateMap<Category, CategoryResponse>();
    }
}
