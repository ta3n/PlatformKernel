using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;

namespace Liberty.Reservation.Manager.WebAPI.Application.Mappings;

public class CancellationPolicyMapperProfile : Profile
{
    public CancellationPolicyMapperProfile()
    {
        CreateMap<CancellationPolicyCreateRequest, Cancellation>()
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

        CreateMap<CancellationPolicyUpdateRequest, Cancellation>()
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Name } }
                )
            )
            .ForMember(
                dest => dest.Description,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Description ?? string.Empty } }
                )
            )
            .ForMember(
                dest => dest.RuleDetail,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.RuleDetail ?? string.Empty } }
                )
            );

        CreateMap<Cancellation, CancellationPolicyResponse>()
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(
                    src => src.Name!.GetValueByHeader()
                )
            );

        CreateMap<DataOfCancellationUpdateRequest, CancellationData>()
            .ForMember(
                dest => dest.Description,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Description ?? string.Empty } }
                )
            );

        CreateMap<Cancellation, CancellationPolicyDetailResponse>()
            .ConstructUsing(
                src => new CancellationPolicyDetailResponse(
                    src.Id,
                    src.Name!.GetValueByHeader(),
                    src.Description == null ? null : src.Description!.GetValueByHeader(),
                    src.RuleDetail == null ? null : src.RuleDetail!.GetValueByHeader(),
                    new CancellationMeta(src.TableSource!.GetValueByHeader()),
                    src.CancellationCancellationDatas!
                        .OrderBy(x => x.CancellationData!.DisplayOrder)
                        .Select(
                            x => new CancellationDataResponse(
                                x.CancellationData!.Id,
                                x.CancellationData.DayStart,
                                x.CancellationData.DayEnd,
                                x.CancellationData.Rate,
                                x.CancellationData.Description!.GetValueByHeader()
                            )
                        )
                )
            );
    }
}
