using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Site.Application.Models;

namespace Liberty.Reservation.Site.WebAPI.Application.Mappings;

public class BookingMapperProfile : Profile
{
    public BookingMapperProfile()
    {
        CreateMap<FileOfBookingResponse, FileOfBookingResultModel>().ReverseMap();

        CreateMap<MealOfBookingResponse, MealOfBookingResultModel>().ReverseMap();

        MapperBookingResponse();

        MapperBookingDetailResponse();

        CreateMap<ChangePersonsOfBookingRequest, BookingCheckPriceModel>().ReverseMap();

        CreateMap<FileOfBookingResponse, FileOfBookingResultModel>().ReverseMap();

        CreateMap<MealOfBookingResponse, MealOfBookingResultModel>().ReverseMap();

        MapperBookingOptionResponse();

        MapperBookingRequest();

        CreateMap<OptionOfBookingResultRequest, OptionOfBookingResult>().ReverseMap();

        CreateMap<PersonOfBookingResultRequest, PersonOfBookingResult>().ReverseMap();

        MapperBookingOptionResponse();
    }

    private void MapperBookingResponse()
    {
        CreateMap<Facility, BookingFacilityResponse>()
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(
                    src => src.Name!.GetValueByHeader(DefaultValues.LanguageCode)
                )
            )
            .ForMember(
                dest => dest.Description,
                opt => opt.MapFrom(
                    src => src.Description
                )
            )
            .ForMember(
                dest => dest.TimeZone,
                opt => opt.MapFrom(
                    src => src.TimeZone
                )
            )
            .ForMember(
                dest => dest.IsOnLinePayment,
                opt => opt.MapFrom(
                    src => src.CanOnLinePayment && src.IsOnLinePayment
                )
            )
            .ForMember(
                dest => dest.PersonAgeTypes,
                opt =>
                    opt.MapFrom(
                        src => src.FacilityPersonAgeTypes!
                            .Where(x => x.PersonAgeType!.IsEnabled && x.PersonAgeType!.IsVisible)
                            .OrderBy(x => x.PersonAgeType!.DisplayOrder)
                            .ThenBy(x => x.PersonAgeType!.Id)
                            .Select(
                                x => new PersonAgeTypeOfBookingFacilityResponse(
                                    x.PersonAgeTypeId,
                                    x.PersonAgeType!.IsMain,
                                    x.PersonAgeType!.AgeMin,
                                    x.PersonAgeType!.AgeMax,
                                    x.PersonAgeType!.Name!.GetValueByHeader(DefaultValues.LanguageCode),
                                    x.PersonAgeType!.UpdatedAt
                                )
                            )
                            .ToList()
                    )
            )
            .ForMember(
                dest => dest.Heading,
                opt => opt.MapFrom(
                    src => MapMultilingualText(src.Heading1)
                )
            )
            .ForMember(
                dest => dest.IsBarrierFree,
                opt => opt.MapFrom(
                    src => src.Meta!.IsBarrierFree
                )
            )
            .ForMember(
                dest => dest.BarrierFreeInfoComment,
                opt => opt.MapFrom(
                    src => MapMultilingualText(src.BarrierFreeInfoComment)
                )
            )
            .ForMember(
                dest => dest.Address1,
                opt => opt.MapFrom(
                    src => MapMultilingualText(src.Address1)
                )
            )
            .ForMember(
                dest => dest.Address2,
                opt => opt.MapFrom(
                    src => MapMultilingualText(src.Address2)
                )
            )
            .ForMember(
                dest => dest.Address3,
                opt => opt.MapFrom(
                    src => MapMultilingualText(src.Address3)
                )
            )
            .ForMember(
                dest => dest.Address4,
                opt => opt.MapFrom(
                    src => MapMultilingualText(src.Address4)
                )
            )
            .ForMember(
                dest => dest.Email,
                opt => opt.MapFrom(
                    src => src.Meta!.SystemEMail
                )
            )
            .ForMember(
                dest => dest.UseSpaTax,
                opt => opt.MapFrom(
                    src => src.Meta!.UseSpaTax
                )
            )
            .ForMember(
                dest => dest.SpaTaxComment,
                opt => opt.MapFrom(
                    src => MapMultilingualText(src.SpaTaxComment)
                )
            )
            .ForMember(
                dest => dest.SpaTaxTable,
                opt => opt.MapFrom(
                    src => MapMultilingualText(src.SpaTaxTable)
                )
            )
            .ForMember(
                dest => dest.Logo,
                opt => opt.MapFrom(
                    src => src.Meta!.Logo
                )
            )
            .ForMember(
                dest => dest.File,
                opt => opt.MapFrom(
                    src => src.FacilityFiles != null && src.FacilityFiles.Count != 0
                        ? new FileOfBookingResponse(
                            src.Meta!.Logo,
                            src.FacilityFiles.First().File!.ContentType
                        )
                        : null
                )
            );
    }

    private static string? MapMultilingualText(
        MultilingualText? text
    )
    {
        return text?.GetValueByHeader(DefaultValues.LanguageCode);
    }

    private void MapperBookingRequest()
    {
        CreateMap<ReserverOfReservationAdjustRequest, CustomerInfo>()
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(
                    src => src.FullName
                )
            )
            .ForMember(
                dest => dest.EMail,
                opt => opt.MapFrom(
                    src => src.Email
                )
            )
            .ForMember(
                dest => dest.Phone,
                opt => opt.MapFrom(
                    src => src.PhoneNumber
                )
            );

        CreateMap<GuestOfReservationAdjustRequest, CustomerInfo>()
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(
                    src => src.FullName
                )
            )
            .ForMember(
                dest => dest.BirthDay,
                opt => opt.MapFrom(
                    src => src.Birthday
                )
            )
            .ForMember(
                dest => dest.Phone,
                opt => opt.MapFrom(
                    src => src.PhoneNumber
                )
            );

        CreateMap<ChangePersonsOfBookingRequest, BookingCheckPriceModel>()
            .ForMember(
                dest => dest.CheckInDate,
                opt => opt.MapFrom(
                    src => src.CheckInDate
                )
            )
            .ForMember(
                dest => dest.RestNumber,
                opt => opt.MapFrom(
                    src => src.RestNumber
                )
            )
            .ForMember(
                dest => dest.RoomNumber,
                opt => opt.MapFrom(
                    src => src.RoomNumber
                )
            )
            .ForMember(
                dest => dest.OptionsResult,
                opt => opt.MapFrom(
                    src => src.OptionsResult
                )
            )
            .ForMember(
                dest => dest.PersonOfBooking,
                opt => opt.MapFrom(
                    src => src.PersonOfBooking
                )
            );

        CreateMap<AdjustOptionsBookingRequest, BookingCheckPriceModel>()
            .ForMember(
                dest => dest.CheckInDate,
                opt => opt.MapFrom(
                    src => src.CheckInDate
                )
            )
            .ForMember(
                dest => dest.RestNumber,
                opt => opt.MapFrom(
                    src => src.RestNumber
                )
            )
            .ForMember(
                dest => dest.RoomNumber,
                opt => opt.MapFrom(
                    src => src.RoomNumber
                )
            )
            .ForMember(
                dest => dest.PersonResult,
                opt => opt.MapFrom(
                    src => src.PersonResult
                )
            )
            .ForMember(
                dest => dest.OptionOfBooking,
                opt => opt.MapFrom(
                    src => src.OptionOfBooking
                )
            );
    }

    private void MapperBookingDetailResponse()
    {
        CreateMap<Plan, BookingDetailsResponse>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.Id)
            )
            .ForMember(
                dest => dest.Code,
                opt => opt.MapFrom(src => src.Code)
            )
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(src => src.Name!.GetValueByHeader(DefaultValues.LanguageCode))
            )
            .ForMember(
                dest => dest.CheckInStart,
                opt => opt.MapFrom(src => src.CheckInStart)
            )
            .ForMember(
                dest => dest.CheckInEnd,
                opt => opt.MapFrom(src => src.CheckInEnd)
            )
            .ForMember(
                dest => dest.CheckOut,
                opt => opt.MapFrom(src => src.CheckOut)
            )
            .ForMember(
                dest => dest.TimeIntervalMinutes,
                opt => opt.MapFrom(src => src.TimeIntervalMinutes)
            )
            .ForMember(
                dest => dest.Tag,
                opt => opt.MapFrom(src => src.Tag!.GetValueByCode(DefaultValues.LanguageCode))
            )
            .ForMember(
                dest => dest.IsOnSidePayment,
                opt => opt.MapFrom(
                    src => src.IsOnSidePayment
                        && src.FacilityPlans!.First().Facility!.IsOnSidePayment
                )
            )
            .ForMember(
                dest => dest.UseDisplayDate,
                opt => opt.MapFrom(src => src.UseDisplayDate)
            )
            .ForMember(
                dest => dest.DisplayDateStart,
                opt => opt.MapFrom(src => src.DisplayDateStart)
            )
            .ForMember(
                dest => dest.DisplayDateEnd,
                opt => opt.MapFrom(src => src.DisplayDateEnd)
            )
            .ForMember(
                dest => dest.UseAcceptDate,
                opt => opt.MapFrom(src => src.UseAcceptDate)
            )
            .ForMember(
                dest => dest.AcceptDateStart,
                opt => opt.MapFrom(src => src.AcceptDateStart)
            )
            .ForMember(
                dest => dest.AcceptDateEnd,
                opt => opt.MapFrom(src => src.AcceptDateEnd)
            )
            .ForMember(
                dest => dest.PlanType,
                opt => opt.MapFrom(src => src.PlanType)
            )
            .ForMember(
                dest => dest.UseBookingReception,
                opt => opt.MapFrom(src => src.UseBookingReception)
            )
            .ForMember(
                dest => dest.BookingReceptionStart,
                opt => opt.MapFrom(src => src.BookingReceptionStart)
            )
            .ForMember(
                dest => dest.BookingReceptionEnd,
                opt => opt.MapFrom(src => src.BookingReceptionEnd)
            )
            .ForMember(
                dest => dest.IsOnLinePayment,
                opt => opt.MapFrom(
                    src => src.IsOnLinePayment
                        && src.FacilityPlans != null
                        && src.FacilityPlans.Count > 0
                        && src.FacilityPlans.First().Facility!.CanOnLinePayment
                        && src.FacilityPlans.First().Facility!.IsOnLinePayment
                )
            )
            .ForMember(
                dest => dest.SiteId,
                opt => opt.MapFrom(
                    src => src.PlanSites != null && src.PlanSites.Count != 0
                        ? src.PlanSites.First().SiteId
                        : 0
                )
            )
            .ForMember(
                dest => dest.SiteCode,
                opt => opt.MapFrom(
                     src => src.PlanSites != null && src.PlanSites.Count != 0
                        ? src.PlanSites.First().Site!.Code
                        : string.Empty
                )
            )
            .ForMember(
                dest => dest.TimeZone,
                opt => opt.MapFrom(
                    src => src.FacilityPlans!.First().Facility!.TimeZone
                )
            )
            .ForMember(
                dest => dest.Files,
                opt => opt.MapFrom(
                    src => src.FilePlans!
                        .Where(filePlan => filePlan.File!.IsEnabled)
                        .OrderBy(x => x.Index)
                        .Select(
                            x => new FileOfBookingResponse(
                                x.File!.Code,
                                x.File!.ContentType
                            )
                        )
                        .ToList()
                )
            )
            .ForMember(
                dest => dest.Meals,
                opt => opt.MapFrom(
                    src => src.PlanMealTypes!
                        .OrderBy(x => x.MealType!.DisplayOrder)
                        .Select(
                            x => new MealOfBookingResponse(
                                x.MealTypeId,
                                x.IsEnabled,
                                x.MealTypeEatType,
                                x.MealType!.Name
                            )
                        )
                        .ToList()
                )
            )
            .ForMember(
                dest => dest.Cancellation,
                opt => opt.MapFrom(
                    src => new CancellationResponse(
                        src.CancellationId,
                        src.Cancellation != null ? src.Cancellation.Code : string.Empty,
                        src.Cancellation != null ? src.Cancellation.Name!.GetValueByHeader(DefaultValues.LanguageCode) : string.Empty,
                        src.Cancellation != null && src.Cancellation!.Description != null
                            ? src.Cancellation.Description!.GetValueByHeader(DefaultValues.LanguageCode)
                            : string.Empty,
                        src.Cancellation != null && src.Cancellation!.TableSource != null
                            ? src.Cancellation.TableSource.GetValueByHeader(DefaultValues.LanguageCode)
                            : string.Empty
                    )
                )
            )
            .ForMember(
                dest => dest.Categories,
                opt => opt.MapFrom(
                    src => src.PlanCategories!
                        .Where(
                            x => x.Category!.CategoryType == CategoryTypes.Plan
                                && !x.Category!.IsMaster
                                && x.Category.FacilityCategories!.Any(
                                    y => y.Facility!.Id == src.FacilityPlans!.First().FacilityId
                                )
                        )
                        .Select(x => x.Category!.Name!.GetValueByHeader(DefaultValues.LanguageCode))
                )
            )
            .ForMember(
                dest => dest.LastUpdateString,
                opt => opt.MapFrom(
                    src =>
                        string.Join(
                            ';',
                            src.UpdatedAt.ToString(),
                            src.PlanSites != null && src.PlanSites.Count != 0
                                ? src.PlanSites.First().Site!.UpdatedAt.ToString()
                                : string.Empty,
                            src.Cancellation != null ? src.Cancellation!.UpdatedAt.ToString() : string.Empty
                        )
                )
            )
            .ForMember(
                dest => dest.Questions,
                opt => opt.MapFrom(
                    src => src.PlanQuestions!
                        .Where(x => x.Question!.IsEnabled)
                        .OrderByDescending(o => o.Question!.DisplayOrder)
                        .Select(
                            x => new QuestionOfBookingResponse(
                                x.QuestionId,
                                x.Question!.Name != null ? x.Question!.Name!.GetValueByHeader(DefaultValues.LanguageCode) : string.Empty,
                                x.Question!.Description != null
                                    ? x.Question!.Description!.GetValueByHeader(DefaultValues.LanguageCode)
                                    : string.Empty,
                                x.Question!.FormData != null
                                    ? x.Question!.FormData!.GetValueByHeader(DefaultValues.LanguageCode)
                                    : string.Empty,
                                x.Question!.QuestionType,
                                x.Question!.IsRequired
                            )
                        )
                        .ToList()
                )
            )
            .ForMember(
                dest => dest.DayUse,
                opt => opt.MapFrom(src => src.DayUse)
            );
    }

    private void MapperBookingOptionResponse()
    {
        CreateMap<OptionItem, OptionItemOfBookingResponse>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(
                    src => src.Id
                )
            )
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(
                    src => src.Name!.GetValueByHeader(DefaultValues.LanguageCode)
                )
            )
            .ForMember(
                dest => dest.Description,
                opt => opt.MapFrom(
                    src => src.Description
                )
            )
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
                            x =>
                                x.Reservation!.ReservationState == ReservationStatus.Confirmed
                                || x.Reservation!.ReservationState == ReservationStatus.Reserved
                                || x.Reservation!.ReservationState == ReservationStatus.Modified
                        )
                        .Sum(x => x.Number)
                )
            )
            .ForMember(
                dest => dest.Price,
                opt => opt.MapFrom(
                    src => src.Price
                )
            )
            .ForMember(
                dest => dest.Files,
                opt => opt.MapFrom(
                    src => src.FileOptionItems!.Select(
                            x => new FileOfBookingResponse(
                                x.File!.Code,
                                x.File!.ContentType
                            )
                        )
                        .ToList()
                )
            )
            .ForMember(
                dest => dest.Questions,
                opt => opt.MapFrom(
                    src => src.OptionItemQuestions!.Select(
                            x => new QuestionOfBookingResponse(
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
            )
            .ForMember(
                dest => dest.LastUpdatedAt,
                opt => opt.MapFrom(
                    src => src.UpdatedAt.ToString()
                )
            );
    }
}
