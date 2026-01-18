using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;

namespace Liberty.Reservation.Manager.WebAPI.Application.Mappings;

public class OptionItemMapperProfile : Profile
{
    public OptionItemMapperProfile()
    {
        CreateMap<OptionItemCreateRequest, OptionItem>()
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Name } }
                )
            );

        CreateMap<OptionItemUpdateRequest, OptionItem>()
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Name } }
                )
            );

        CreateMap<OptionItem, OptionItemResponse>()
            .ConstructUsing(
                src => new OptionItemResponse(
                    src.Id,
                    src.Name!.GetValueByHeader(),
                    src.Description,
                    src.BaseNumber,
                    src.Price,
                    src.IsEnabled,
                    src.PlanOptionItems!.Any(
                        x => x.Plan!.PlanType == PlanTypes.Combo
                            || x.Plan!.PlanType == PlanTypes.RoomOnly
                    ),
                    src.FileOptionItems!.OrderBy(x => x.Index)
                        .Select(
                            t => new ImageOfOptionItemResponse(
                                t.FileId,
                                t.File!.Code ?? string.Empty,
                                t.Index,
                                t.File!.IsEnabled,
                                t.File!.Description
                            )
                        )
                )
            );

        CreateMap<OptionItem, OptionItemDetailResponse>()
            .ConstructUsing(
                src => new OptionItemDetailResponse(
                    src.Id,
                    src.Name!.GetValueByHeader(),
                    src.Description,
                    src.BaseNumber,
                    src.MaxSupplyNumber,
                    src.Price,
                    src.OptionItemCategories!.Where(x => x.Category!.CategoryType == CategoryTypes.OptionItem && !x.Category!.IsMaster)
                        .Select(
                            t => new CategoryOfOptionItemDetailResponse(
                                t.CategoryId,
                                t.Category!.Name!.GetValueByHeader()
                            )
                        ),
                    src.OptionItemCategories!.Where(x => x.Category!.CategoryType == CategoryTypes.OptionItem && x.Category!.IsMaster)
                        .Select(
                            t => new CategoryOfOptionItemDetailResponse(
                                t.CategoryId,
                                t.Category!.Name!.GetValueByHeader()
                            )
                        ),
                    src.OptionItemQuestions!.Select(
                        t => new QuestionOfOptionItemDetailResponse(
                            t.QuestionId,
                            t.Question!.Name!.GetValueByHeader()
                        )
                    ),
                    src.FileOptionItems!.OrderBy(x => x.Index)
                        .Select(
                            t => new FileOfOptionItemDetailResponse(
                                t.FileId,
                                t.File!.Code ?? string.Empty,
                                t.Index,
                                t.File!.IsEnabled,
                                t.File!.Description
                            )
                        ),
                    src.IsEnabled
                )
            );

        CreateMap<OptionItem, OptionItemOfPlanResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name!.GetValueByHeader()))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(
                dest => dest.SellNumber,
                opt => opt.MapFrom(
                    src => src.OptionItemAppDates != null && src.OptionItemAppDates.Count > 0
                        ? src.OptionItemAppDates.First().SellNumber
                        : 0
                )
            )
            .ForMember(
                dest => dest.MaxSupplyNumber,
                opt => opt.MapFrom(
                    src => src.MaxSupplyNumber
                )
            )
            .ForMember(
                dest => dest.ReservedNumber,
                opt => opt.MapFrom(
                    src => src.ReservationRoomGroupAppDateOptionItems!
                        .Where(
                            x => x.Reservation!.ReservationState == ReservationStatus.Confirmed
                                || x.Reservation!.ReservationState == ReservationStatus.Reserved
                                || x.Reservation!.ReservationState == ReservationStatus.Modified
                        )
                        .Sum(x => x.Number)
                )
            )
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(
                dest => dest.Files,
                opt => opt.MapFrom(
                    src => src.FileOptionItems!.Select(
                            x => new FileOfOptionItemResponse(
                                x.File!.Code,
                                x.File!.ContentType,
                                x.Index,
                                x.File!.IsEnabled
                            )
                        )
                        .ToList()
                )
            )
            .ForMember(
                dest => dest.Questions,
                opt => opt.MapFrom(src => src.OptionItemQuestions!.Select(x => new QuestionOfBookingResponse(
                            x.QuestionId,
                            MapMultilingualText(x.Question!.Name),
                            MapMultilingualText(x.Question!.Description),
                            MapMultilingualText(x.Question!.FormData),
                            x.Question!.QuestionType,
                            x.Question!.IsRequired
                        )
                    )
                    .ToList()
                )
            );
    }

    private static string? MapMultilingualText(
        MultilingualText? text
    )
    {
        return text?.GetValueByCode(DefaultValues.LanguageCode);
    }
}
