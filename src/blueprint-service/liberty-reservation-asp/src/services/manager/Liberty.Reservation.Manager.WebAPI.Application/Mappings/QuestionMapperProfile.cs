using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;

namespace Liberty.Reservation.Manager.WebAPI.Application.Mappings;

public class QuestionMapperProfile : Profile
{
    public QuestionMapperProfile()
    {
        CreateMap<QuestionCreateRequest, Question>()
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
            )
            .ForMember(
                des => des.QuestionType,
                opt => opt.MapFrom(
                    src => QuestionTypes.Unknown
                )
            )
            .ForMember(
                des => des.FormData,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), string.Empty } }
                )
            );

        CreateMap<QuestionUpdateRequest, Question>()
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
            )
            .ForMember(
                dest => dest.FormData,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.FormData ?? string.Empty } }
                )
            )
            .ForMember(
                des => des.QuestionType,
                opt => opt.MapFrom(
                    src => src.Type
                )
            );

        CreateMap<Question, QuestionResponse>()
            .ConstructUsing(
                src => new QuestionResponse(
                    src.Id,
                    src.Name!.GetValueByHeader(),
                    src.Description == null ? null : src.Description!.GetValueByHeader(),
                    src.IsEnabled,
                    src.PlanQuestions!.Any(
                        x => x.Plan!.PlanType == PlanTypes.Combo
                            || x.Plan!.PlanType == PlanTypes.RoomOnly
                    ),
                    src.OptionItemQuestions!.Count > 0,
                    src.HasContent
                )
            );

        CreateMap<Question, QuestionDetailResponse>()
            .ForMember(
                des => des.Type,
                opt => opt.MapFrom(
                    src => src.QuestionType
                )
            );
    }
}
