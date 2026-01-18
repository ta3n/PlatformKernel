using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.User.WebAPI.Application.Models.Responses;

namespace Liberty.Reservation.User.WebAPI.Application.Mappings;

public class OptionItemMapperProfile : Profile
{
    public OptionItemMapperProfile()
    {
        CreateMap<OptionItem, OptionItemOfPlanResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
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
                                x.File!.ContentType
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
        return text?.GetValueByHeader(DefaultValues.LanguageCode);
    }
}
