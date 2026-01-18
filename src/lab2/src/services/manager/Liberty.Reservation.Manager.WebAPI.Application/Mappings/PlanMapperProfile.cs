using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;

namespace Liberty.Reservation.Manager.WebAPI.Application.Mappings;

public class PlanMapperProfile : Profile
{
    public PlanMapperProfile()
    {
        PlanInformationMapper();

        PlanPriceMapper();
    }

    private void PlanPriceMapper()
    {
        CreateMap<PlanRoomGroupSiteAppDateTypePriceData, RomTypeStandardPrinceDataResponse>()
            .ForMember(
                dest => dest.DateTypeId,
                opt => opt.MapFrom(
                    src => src.AppDateTypeId
                )
            )
            .ForMember(
                dest => dest.PersonMin,
                opt => opt.MapFrom(
                    src => src.PriceData!.PersonMin
                )
            )
            .ForMember(
                dest => dest.PersonMax,
                opt => opt.MapFrom(
                    src => src.PriceData!.PersonMax
                )
            )
            .ForMember(
                dest => dest.Price,
                opt => opt.MapFrom(
                    src => src.PriceData!.Price
                )
            );

        CreateMap<PlanRoomGroupSitePersonAgeType, RoomTypeChildrenPersonAgeTypeResponse>()
            .ForMember(
                dest => dest.PersonAgeTypeId,
                opt => opt.MapFrom(
                    src => src.PersonAgeTypeId
                )
            )
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(
                    src => src.PersonAgeType!.Name!.GetValueByHeader()
                )
            )
            .ForMember(
                dest => dest.IsEnabled,
                opt => opt.MapFrom(
                    src => src.IsEnabled
                )
            )
            .ForMember(
                dest => dest.IsRegardAdult,
                opt => opt.MapFrom(
                    src => src.IsRegardAdult
                )
            )
            .ForMember(
                dest => dest.Value,
                opt => opt.MapFrom(
                    src => src.Value
                )
            )
            .ForMember(
                dest => dest.PriceSettingType,
                opt => opt.MapFrom(
                    src => src.PriceSettingType
                )
            )
            .ForMember(
                dest => dest.IsMain,
                opt => opt.MapFrom(
                    src => src.PersonAgeType!.IsMain
                )
            )
            .ForMember(
                dest => dest.DisplayOrder,
                opt => opt.MapFrom(
                    src => src.PersonAgeType!.DisplayOrder
                )
            );

        CreateMap<RoomGroupPriceUpdateSaleRequest, PlanRoomGroupSite>()
            .ForMember(
                dest => dest.AutoExtendEveryMonthDay,
                opt => opt.MapFrom(
                    src => src.AutoExtendEveryMonthDay
                )
            )
            .ForMember(
                dest => dest.AutoExtendMonth,
                opt => opt.MapFrom(
                    src => src.AutoExtendMonth
                )
            )
            .ForMember(
                dest => dest.UseAutoExtend,
                opt => opt.MapFrom(
                    src => src.UseAutoExtend
                )
            );

        CreateMap<PlanRoomGroupSiteDiscountData, RoomTypeDiscountDataResponse>()
            .ForMember(
                dest => dest.StartPrevDay,
                opt => opt.MapFrom(
                    src => src.DiscountData!.StartPrevDay
                )
            )
            .ForMember(
                dest => dest.EndPrevDay,
                opt => opt.MapFrom(
                    src => src.DiscountData!.EndPrevDay
                )
            )
            .ForMember(
                dest => dest.PersonMin,
                opt => opt.MapFrom(
                    src => src.DiscountData!.PersonMin
                )
            )
            .ForMember(
                dest => dest.PersonMax,
                opt => opt.MapFrom(
                    src => src.DiscountData!.PersonMax
                )
            )
            .ForMember(
                dest => dest.Value,
                opt => opt.MapFrom(
                    src => src.DiscountData!.Value
                )
            )
            .ForMember(
                dest => dest.PriceSettingType,
                opt => opt.MapFrom(
                    src => src.DiscountData!.PriceSettingType
                )
            );

        CreateMap<PlanRoomGroupSite, PlanRoomDetailSaleResponse>()
            .ForMember(
                dest => dest.PlanId,
                opt => opt.MapFrom(
                    src => src.PlanId
                )
            )
            .ForMember(
                dest => dest.RomTypeId,
                opt => opt.MapFrom(
                    src => src.RoomGroupId
                )
            )
            .ForMember(
                dest => dest.SiteId,
                opt => opt.MapFrom(
                    src => src.SiteId
                )
            )
            .ForMember(
                dest => dest.AutoExtendEveryMonthDay,
                opt => opt.MapFrom(
                    src => src.AutoExtendEveryMonthDay
                )
            )
            .ForMember(
                dest => dest.AutoExtendMonth,
                opt => opt.MapFrom(
                    src => src.AutoExtendMonth
                )
            )
            .ForMember(
                dest => dest.UseAutoExtend,
                opt => opt.MapFrom(
                    src => src.UseAutoExtend
                )
            );

        CreateMap<PlanRoomGroupSiteAppDatePriceData, RoomTypePriceDataCalendarResponse>()
            .ForMember(
                dest => dest.DateCalendar,
                opt => opt.MapFrom(
                    src => src.DateCalendar
                )
            )
            .ForMember(
                dest => dest.PersonMin,
                opt => opt.MapFrom(
                    src => src.PriceData!.PersonMin
                )
            )
            .ForMember(
                dest => dest.PersonMax,
                opt => opt.MapFrom(
                    src => src.PriceData!.PersonMax
                )
            )
            .ForMember(
                dest => dest.Price,
                opt => opt.MapFrom(
                    src => src.PriceData!.Price
                )
            );

        CreateMap<PlanRoomGroupSiteAppDate, RoomTypePriceCalendarDataResponse>()
            .ForMember(
                dest => dest.DateCalendar,
                opt => opt.MapFrom(
                    src => src.DateCalendar
                )
            )
            .ForMember(
                dest => dest.UseAutoDiscount,
                opt => opt.MapFrom(
                    src => src.UseAutoDiscount
                )
            )
            .ForMember(
                dest => dest.PointRate,
                opt => opt.MapFrom(
                    src => src.PointRate
                )
            );

        CreateMap<PlanRoomGroupSitePriceData, RangePersons>()
            .ForMember(
                dest => dest.PersonMin,
                opt => opt.MapFrom(
                    src => src.PersonMin
                )
            )
            .ForMember(
                dest => dest.PersonMax,
                opt => opt.MapFrom(
                    src => src.PersonMax
                )
            );
    }

    private void PlanInformationMapper()
    {
        CreateMap<PlanCreateRequest, Plan>()
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Name ?? string.Empty } }
                )
            )
            .ForMember(
                dest => dest.Summary,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Summary ?? string.Empty } }
                )
            )
            .ForMember(
                dest => dest.DayUse,
                opt => opt.MapFrom(
                    src => src.DayUse
                )
            );

        CreateMap<Plan, PlanResponse>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(
                    src => src.Id
                )
            )
            .ForMember(
                dest =>
                    dest.Name,
                opt => opt.MapFrom(
                    src => src.Name!.GetValueByHeader()
                )
            )
            .ForMember(
                dest =>
                    dest.AcceptDateEnd,
                opt => opt.MapFrom(
                    src => src.AcceptDateEnd
                )
            )
            .ForMember(
                dest =>
                    dest.AcceptDateStart,
                opt => opt.MapFrom(
                    src => src.AcceptDateStart
                )
            )
            .ForMember(
                dest =>
                    dest.DisplayOrder,
                opt => opt.MapFrom(
                    src => src.DisplayOrder
                )
            )
            .ForMember(
                dest =>
                    dest.DisplayDateEnd,
                opt => opt.MapFrom(
                    src => src.DisplayDateEnd
                )
            )
            .ForMember(
                dest =>
                    dest.DisplayDateStart,
                opt => opt.MapFrom(
                    src => src.DisplayDateStart
                )
            )
            .ForMember(
                dest =>
                    dest.HasWarning,
                opt => opt.MapFrom(src => !src.HasPayment)
            )
            .ForMember(
                dest =>
                    dest.IsEnabled,
                opt => opt.MapFrom(
                    src => src.IsEnabled
                )
            )
            .ForMember(
                dest =>
                    dest.IsOnLinePayment,
                opt => opt.MapFrom(
                    src => src.IsOnLinePayment
                )
            )
            .ForMember(
                dest =>
                    dest.IsOnSidePayment,
                opt => opt.MapFrom(
                    src => src.IsOnSidePayment
                )
            )
            .ForMember(
                dest => dest.Tags,
                opt => opt.MapFrom(
                    src => src.Tag != null && !string.IsNullOrEmpty(src.Tag.GetValueByHeader())
                        ? src.Tag.GetValueByHeader().Split(',', StringSplitOptions.None)
                        : Array.Empty<string>()
                )
            )
            .ForMember(
                dest =>
                    dest.UseAcceptDate,
                opt => opt.MapFrom(
                    src => src.UseAcceptDate
                )
            )
            .ForMember(
                dest =>
                    dest.UseDisplayDate,
                opt => opt.MapFrom(
                    src => src.UseDisplayDate
                )
            )
            .ForMember(
                dest => dest.Warnings,
                opt => opt.MapFrom(
                    src => !src.HasPayment ? new List<long> { src.Id } : null
                )
            )
            .ForMember(
                dest => dest.Categories,
                opt => opt.MapFrom(
                    src => src.PlanCategories!
                        .Select(
                            x => new CategoryOfPlanResponse(
                                x.CategoryId,
                                x.Category!.Name!.GetValueByHeader(),
                                x.Category!.IsEnabled
                            )
                        )
                )
            )
            .ForMember(
                dest => dest.Files,
                opt =>
                    opt.MapFrom(
                        src => src.FilePlans!.OrderBy(x => x.Index)
                            .Select(
                                x => new FileOfPlanResponse(
                                    x.FileId,
                                    x.File!.Code ?? string.Empty,
                                    x.File!.IsEnabled,
                                    x.Index,
                                    x.File.Description
                                )
                            )
                    )
            )
            .ForMember(
                dest => dest.PlanRoomTypes,
                opt => opt.MapFrom(
                    src => src.PlanRoomGroups!
                        .OrderByDescending(x => x.RoomGroup!.DisplayOrder)
                        .Select(
                            x => new
                                RoomGroupOfPlanResponse(
                                    x.IsUsed,
                                    x.RoomGroupId,
                                    x.RoomGroup!.Name!.GetValueByHeader(),
                                    x.RoomGroup.IsEnabled,
                                    x.IsEnabled,
                                    x.RoomGroup.BaseNumber,
                                    x.RoomGroup.CapacityMin,
                                    x.RoomGroup.CapacityMax,
                                    x.RoomGroup.Size,
                                    x.RoomGroup.RoomGroupSizeUnitType,
                                    x.RoomGroup.IsEnabledSmoking
                                )
                        )
                )
            )
            .ForMember(
                dest => dest.MealTypes,
                opt => opt.MapFrom(
                        src => src.PlanMealTypes!.Select(
                            x => x.MealType!.Name
                        )
                )
            )
            .ForMember(
                dest => dest.Sites,
                opt => opt.MapFrom(
                        src => src.PlanSites!.Select(
                            x => x.Site!.Name!.GetValueByHeader()
                        )
                )
            );

        PlanBasicSettingMapper();

        PlanCancelMapper();

        PlanDisplayMapper();

        PlanImportantMapper();

        PlanMealMapper();

        PlanOptionMapper();

        PlanPaymentMethodMapper();

        PlanPublishAcceptMapper();

        PlanQuestionMapper();

        PlanRoomTypeMapper();

        PlanSaleMapper();

        PlanSpecialMapper();

        PlanRoomGroupSiteMinimumPriceMapper();
    }

    private void PlanRoomTypeMapper()
    {
        CreateMap<Plan, PlanRoomTypeResponse>()
            .ForMember(
                dest => dest.RoomGroups,
                opt => opt.MapFrom(
                    src => src!.PlanRoomGroups!.Select(
                        x => new RoomGroupOfPlanRoomTypeResponse(
                            x.RoomGroupId,
                            x.RoomGroup!.Name!.GetValueByHeader(),
                            x.RoomGroup.IsEnabled
                        )
                    )
                )
            );
    }

    private void PlanQuestionMapper()
    {
        CreateMap<Plan, PlanQuestionResponse>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(
                    src => src.Id
                )
            )
            .ForMember(
                dest => dest.Questions,
                opt => opt.MapFrom(
                    src => src!.PlanQuestions!.Select(
                        x => new QuestionOfPlanQuestionResponse(
                            x.QuestionId,
                            x.Question!.Name!.GetValueByHeader(),
                            x.Question.IsEnabled
                        )
                    )
                )
            );
    }

    private void PlanMealMapper()
    {
        CreateMap<Plan, PlanMealResponse>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(
                    src => src.Id
                )
            )
            .ForMember(
                dest => dest.MealTypes,
                opt =>
                    opt.MapFrom(
                        src => src.PlanMealTypes!.Select(
                            x => new TypeOfPlanMealResponse(
                                x.MealTypeId,
                                x.MealTypeEatType
                            )
                        )
                    )
            );
    }

    private void PlanSpecialMapper()
    {
        CreateMap<Plan, PlanSpecialResponse>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(
                    src => src.Id
                )
            )
            .ForMember(
                dest => dest.IsSecret,
                opt => opt.MapFrom(
                    src => src.IsSecret
                )
            )
            .ForMember(
                dest => dest.SecretWord,
                opt => opt.MapFrom(
                    src => src.SecretWord
                )
            );

        CreateMap<PlanUpdateSpecialRequest, Plan>()
            .ForMember(
                dest => dest.IsSecret,
                opt => opt.MapFrom(
                    src => src.IsSecret
                )
            )
            .ForMember(
                dest => dest.SecretWord,
                opt => opt.MapFrom(
                    src => src.SecretWord
                )
            );
    }

    private void PlanSaleMapper()
    {
        CreateMap<Plan, PlanSaleResponse>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(
                    src => src.Id
                )
            )
            .ForMember(
                dest => dest.CheckInStart,
                opt => opt.MapFrom(
                    src => src.CheckInStart
                )
            )
            .ForMember(
                dest => dest.CheckInEnd,
                opt => opt.MapFrom(
                    src => src.CheckInEnd
                )
            )
            .ForMember(
                dest => dest.TimeIntervalMinutes,
                opt => opt.MapFrom(
                    src => src.TimeIntervalMinutes
                )
            )
            .ForMember(
                dest => dest.RoomNumberDaySaleLimit,
                opt => opt.MapFrom(
                    src => src.RoomNumberDaySaleLimit
                )
            )
            .ForMember(
                dest => dest.GroupNumberDaySaleLimit,
                opt => opt.MapFrom(
                    src => src.GroupNumberDaySaleLimit
                )
            )
            .ForMember(
                dest => dest.PlanDaySaleLimitType,
                opt => opt.MapFrom(
                    src => src.PlanDaySaleLimitType
                )
            )
            .ForMember(
                dest => dest.UseDaySaleLimit,
                opt => opt.MapFrom(
                    src => src.UseDaySaleLimit
                )
            )
            .ForMember(
                dest => dest.UseAcceptPersonNumber,
                opt => opt.MapFrom(
                    src => src.UseAcceptPersonNumber
                )
            )
            .ForMember(
                dest => dest.AcceptPersonNumberMin,
                opt => opt.MapFrom(
                    src => src.AcceptPersonNumberMin
                )
            )
            .ForMember(
                dest => dest.AcceptPersonNumberMax,
                opt => opt.MapFrom(
                    src => src.AcceptPersonNumberMax
                )
            )
            .ForMember(
                dest => dest.NumberOfStayLimitMin,
                opt => opt.MapFrom(
                    src => src.NumberOfStayLimitMin
                )
            )
            .ForMember(
                dest => dest.NumberOfStayLimitMax,
                opt => opt.MapFrom(
                    src => src.NumberOfStayLimitMax
                )
            )
            .ForMember(
                dest => dest.PointRate,
                opt => opt.MapFrom(
                    src => src.PointRate!.Rate
                )
            )
            .ForMember(
                dest => dest.PointExpire,
                opt => opt.MapFrom(
                    src => src.PointRate!.Expire
                )
            )
            .ForMember(
                dest => dest.MaxRoomNumberDaySaleLimit,
                opt => opt.MapFrom(
                    src => src.PlanRoomGroups!.Sum(y => y.RoomGroup!.BaseNumber)
                )
            )
            .ForMember(
                dest => dest.DayUse,
                opt => opt.MapFrom(
                    src => src.DayUse
                )
            );

        CreateMap<PlanUpdateSaleRequest, Plan>()
            .ForMember(
                dest => dest.CheckInStart,
                opt => opt.MapFrom(
                    src => src.CheckInStart
                )
            )
            .ForMember(
                dest => dest.CheckInEnd,
                opt => opt.MapFrom(
                    src => src.CheckInEnd
                )
            )
            .ForMember(
                dest => dest.TimeIntervalMinutes,
                opt => opt.MapFrom(
                    src => src.TimeIntervalMinutes
                )
            )
            .ForMember(
                dest => dest.RoomNumberDaySaleLimit,
                opt => opt.MapFrom(
                    src => src.RoomNumberDaySaleLimit
                )
            )
            .ForMember(
                dest => dest.GroupNumberDaySaleLimit,
                opt => opt.MapFrom(
                    src => src.GroupNumberDaySaleLimit
                )
            )
            .ForMember(
                dest => dest.PlanDaySaleLimitType,
                opt => opt.MapFrom(
                    src => src.PlanDaySaleLimitType
                )
            )
            .ForMember(
                dest => dest.UseAcceptPersonNumber,
                opt => opt.MapFrom(
                    src => src.UseAcceptPersonNumber
                )
            )
            .ForMember(
                dest => dest.AcceptPersonNumberMin,
                opt => opt.MapFrom(
                    src => src.AcceptPersonNumberMin
                )
            )
            .ForMember(
                dest => dest.AcceptPersonNumberMax,
                opt => opt.MapFrom(
                    src => src.AcceptPersonNumberMax
                )
            )
            .ForMember(
                dest => dest.NumberOfStayLimitMin,
                opt => opt.MapFrom(
                    src => src.NumberOfStayLimitMin
                )
            )
            .ForMember(
                dest => dest.NumberOfStayLimitMax,
                opt => opt.MapFrom(
                    src => src.NumberOfStayLimitMax
                )
            )
            .ForPath(
                dest => dest.PointRate!.Rate,
                opt => opt.MapFrom(
                    src => src.PointRate
                )
            )
            .ForPath(
                dest => dest.PointRate!.Expire,
                opt => opt.MapFrom(
                    src => src.PointExpire
                )
            );
    }

    private void PlanPublishAcceptMapper()
    {
        CreateMap<Plan, PlanPublishAcceptResponse>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(
                    src => src.Id
                )
            )
            .ForMember(
                dest => dest.UseBookingReception,
                opt => opt.MapFrom(
                    src => src.UseBookingReception
                )
            )
            .ForMember(
                dest => dest.BookingReceptionStart,
                opt => opt.MapFrom(
                    src => src.BookingReceptionStart
                )
            )
            .ForMember(
                dest => dest.BookingReceptionEnd,
                opt => opt.MapFrom(
                    src => src.BookingReceptionEnd
                )
            )
            .ForMember(
                dest => dest.UseDisplayDate,
                opt => opt.MapFrom(
                    src => src.UseDisplayDate
                )
            )
            .ForMember(
                dest => dest.DisplayDateStart,
                opt => opt.MapFrom(
                    src => src.DisplayDateStart
                )
            )
            .ForMember(
                dest => dest.DisplayDateEnd,
                opt => opt.MapFrom(
                    src => src.DisplayDateEnd
                )
            )
            .ForMember(
                dest => dest.UseAcceptDate,
                opt => opt.MapFrom(
                    src => src.UseAcceptDate
                )
            )
            .ForMember(
                dest => dest.AcceptDateStart,
                opt => opt.MapFrom(
                    src => src.AcceptDateStart
                )
            )
            .ForMember(
                dest => dest.AcceptDateEnd,
                opt => opt.MapFrom(
                    src => src.AcceptDateEnd
                )
            )
            .ForMember(
                dest => dest.AcceptDays,
                opt => opt.MapFrom(
                    src => src.AcceptDays
                )
            )
            .ForMember(
                dest => dest.AcceptMonths,
                opt => opt.MapFrom(
                    src => src.AcceptMonths
                )
            )
            .ForMember(
                dest => dest.AcceptEndLimitType,
                opt => opt.MapFrom(
                    src => src.AcceptEndLimitType
                )
            )
            .ForMember(
                dest => dest.ReceptionDayLimit,
                opt => opt.MapFrom(
                    src => src.ReceptionDayLimit
                )
            )
            .ForMember(
                dest => dest.ReceptionLimit,
                opt => opt.MapFrom(
                    src => src.ReceptionLimit
                )
            )
            .ForMember(
                dest => dest.Sites,
                opt => opt.MapFrom(
                    src => src!.PlanSites!.Select(
                        x => new SiteOfPlanPublishAcceptResponse(
                            x.SiteId,
                            x.Site!.Name!.GetValueByHeader(),
                            x.Site.IsEnabled
                        )
                    )
                )
            );

        CreateMap<PlanUpdatePublishAcceptRequest, Plan>()
            .ForMember(
                dest => dest.UseBookingReception,
                opt => opt.MapFrom(
                    src => src.UseBookingReception
                )
            )
            .ForMember(
                dest => dest.BookingReceptionStart,
                opt => opt.MapFrom(
                    src => src.BookingReceptionStart
                )
            )
            .ForMember(
                dest => dest.BookingReceptionEnd,
                opt => opt.MapFrom(
                    src => src.BookingReceptionEnd
                )
            )
            .ForMember(
                dest => dest.UseDisplayDate,
                opt => opt.MapFrom(
                    src => src.UseDisplayDate
                )
            )
            .ForMember(
                dest => dest.DisplayDateStart,
                opt => opt.MapFrom(
                    src => src.DisplayDateStart
                )
            )
            .ForMember(
                dest => dest.DisplayDateEnd,
                opt => opt.MapFrom(
                    src => src.DisplayDateEnd
                )
            )
            .ForMember(
                dest => dest.UseAcceptDate,
                opt => opt.MapFrom(
                    src => src.UseAcceptDate
                )
            )
            .ForMember(
                dest => dest.AcceptDateStart,
                opt => opt.MapFrom(
                    src => src.AcceptDateStart
                )
            )
            .ForMember(
                dest => dest.AcceptDateEnd,
                opt => opt.MapFrom(
                    src => src.AcceptDateEnd
                )
            )
            .ForMember(
                dest => dest.AcceptDays,
                opt => opt.MapFrom(
                    src => src.AcceptDays
                )
            )
            .ForMember(
                dest => dest.AcceptMonths,
                opt => opt.MapFrom(
                    src => src.AcceptMonths
                )
            )
            .ForMember(
                dest => dest.AcceptEndLimitType,
                opt => opt.MapFrom(
                    src => src.AcceptEndLimitType
                )
            )
            .ForMember(
                dest => dest.ReceptionDayLimit,
                opt => opt.MapFrom(
                    src => src.ReceptionDayLimit
                )
            )
            .ForMember(
                dest => dest.ReceptionLimit,
                opt => opt.MapFrom(
                    src => src.ReceptionLimit
                )
            );
    }

    private void PlanPaymentMethodMapper()
    {
        CreateMap<Plan, PlanPaymentMethodResponse>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(
                    src => src.Id
                )
            )
            .ForMember(
                dest => dest.IsOnSidePayment,
                opt => opt.MapFrom(
                    src => src.IsOnSidePayment
                )
            )
            .ForMember(
                dest => dest.IsOnLinePayment,
                opt => opt.MapFrom(
                    src => src.IsOnLinePayment
                )
            );

        CreateMap<PlanUpdatePaymentMethodRequest, Plan>()
            .ForMember(
                dest => dest.IsOnSidePayment,
                opt => opt.MapFrom(
                    src => src.IsOnSidePayment
                )
            )
            .ForMember(
                dest => dest.IsOnLinePayment,
                opt => opt.MapFrom(
                    src => src.IsOnLinePayment
                )
            );
    }

    private void PlanOptionMapper()
    {
        CreateMap<Plan, PlanOptionResponse>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(
                    src => src.Id
                )
            )
            .ForMember(
                dest => dest.UseFixedOptionItem,
                opt => opt.MapFrom(
                    src => src.UseFixedOptionItem
                )
            )
            .ForMember(
                dest => dest.OptionItems,
                opt => opt.MapFrom(
                    src => src!.PlanOptionItems!.Select(
                        x => new OptionItemOfPlanOptionResponse(
                            x.OptionItemId,
                            x.OptionItem!.Name!.GetValueByHeader(),
                            x.OptionItem!.IsEnabled
                        )
                    )
                )
            );

        CreateMap<PlanUpdateOptionRequest, Plan>()
            .ForMember(
                dest => dest.UseFixedOptionItem,
                opt => opt.MapFrom(
                    src => src.UseFixedOptionItem
                )
            );
    }

    private void PlanImportantMapper()
    {
        CreateMap<Plan, PlanImportantNoteResponse>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(
                    src => src.Id
                )
            );

        CreateMap<PlanUpdateImportantNoteRequest, Plan>()
            .ForPath(
                dest => dest.Payment,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Payment ?? string.Empty } }
                )
            )
            .ForPath(
                dest => dest.Meal,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Meal ?? string.Empty } }
                )
            )
            .ForPath(
                dest => dest.Other,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Other ?? string.Empty } }
                )
            );
    }

    private void PlanDisplayMapper()
    {
        CreateMap<Plan, PlanDisplayResponse>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(
                    src => src.Id
                )
            )
            .ForMember(
                dest =>
                    dest.Tags,
                opt => opt.MapFrom(
                    src => src.Tag == null
                        ? Array.Empty<string>()
                        : src.Tag!.GetValueByHeader().Split(',', StringSplitOptions.RemoveEmptyEntries)
                )
            )
            .ForMember(
                dest => dest.MasterCategories,
                opt => opt.MapFrom(
                    src => src!.PlanCategories!
                        .Where(
                            x => x.Category!.CategoryType == CategoryTypes.Plan
                                && x.Category!.IsMaster
                        )
                        .Select(
                            x => new CategoryOfPlanDisplayResponse(
                                x.CategoryId,
                                x.Category!.Name!.GetValueByHeader(),
                                x.Category!.IsEnabled
                            )
                        )
                )
            )
            .ForMember(
                dest => dest.PlanCategories,
                opt => opt.MapFrom(
                    src => src!.PlanCategories!
                        .Where(
                            x => x.Category!.CategoryType == CategoryTypes.Plan
                                && !x.Category!.IsMaster
                                && x.Category.FacilityCategories!.Any(
                                    y => y.Facility!.Id == src.FacilityPlans!.First().FacilityId
                                )
                        )
                        .Select(
                            x => new CategoryOfPlanDisplayResponse(
                                x.CategoryId,
                                x.Category!.Name!.GetValueByHeader(),
                                x.Category!.IsEnabled
                            )
                        )
                )
            );

        CreateMap<PlanUpdateDisplayRequest, Plan>()
            .ForMember(
                dest => dest.Tag,
                opt => opt.MapFrom(
                    src => new MultilingualText
                    {
                        { LanguageHeaderUtil.GetLanguageCodeFromHeader(), string.Join(",", src.Tags!.Distinct().ToArray()) }
                    }
                )
            );
    }

    private void PlanCancelMapper()
    {
        CreateMap<Plan, PlanCancelResponse>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(
                    src => src.Id
                )
            )
            .ForMember(
                dest => dest.IsCancelSameAccept,
                opt => opt.MapFrom(
                    src => src.IsCancelSameAccept
                )
            )
            .ForMember(
                dest => dest.CancelDayLimit,
                opt => opt.MapFrom(
                    src => src.CancelDayLimit
                )
            )
            .ForMember(
                dest => dest.CancelLimit,
                opt => opt.MapFrom(
                    src => src.CancelLimit
                )
            )
            .ForMember(
                dest => dest.CancellationId,
                opt => opt.MapFrom(
                    src => src.CancellationId
                )
            );

        CreateMap<PlanUpdateCancelRequest, Plan>()
            .ForMember(
                dest => dest.IsCancelSameAccept,
                opt => opt.MapFrom(
                    src => src.IsCancelSameAccept
                )
            )
            .ForMember(
                dest => dest.CancelDayLimit,
                opt => opt.MapFrom(
                    src => src.CancelDayLimit
                )
            )
            .ForMember(
                dest => dest.CancelLimit,
                opt => opt.MapFrom(
                    src => src.CancelLimit
                )
            )
            .ForMember(
                dest => dest.CancellationId,
                opt => opt.MapFrom(
                    src => src.CancellationId
                )
            );
    }

    private void PlanBasicSettingMapper()
    {
        CreateMap<Plan, PlanBasicSettingResponse>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(
                    src => src.Id
                )
            )
            .ForMember(
                dest => dest.Files,
                opt =>
                    opt.MapFrom(
                        src => src.FilePlans!.OrderBy(x => x.Index)
                            .Select(
                                x => new FileOfPlanBasicSettingResponse(
                                    x.FileId,
                                    x.File!.Code ?? string.Empty,
                                    x.Index,
                                    x.File!.IsEnabled,
                                    x.File!.Description
                                )
                            )
                    )
            );

        CreateMap<PlanUpdateBasicSettingRequest, Plan>()
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Name ?? string.Empty } }
                )
            )
            .ForMember(
                dest => dest.NameForImport,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.NameForImport ?? string.Empty } }
                )
            )
            .ForMember(
                dest => dest.Summary,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Summary ?? string.Empty } }
                )
            )
            .ForMember(
                dest => dest.Description,
                opt => opt.MapFrom(
                    src => new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), src.Description ?? string.Empty } }
                )
            );
    }

    private void PlanRoomGroupSiteMinimumPriceMapper()
    {
        CreateMap<PlanRoomGroupSiteUpdateMinimumPriceRequest, PlanRoomGroupSite>()
            .ForMember(
                dest => dest.IsEnabledMinimumPrice,
                opt => opt.MapFrom(
                    src => src.IsEnabledMinimumPrice
                )
            )
            .ForMember(
                dest => dest.MinimumPrice,
                opt => opt.MapFrom(
                    src => src.MinimumPrice
                )
            );

        CreateMap<PlanRoomGroupSite, PlanRoomSiteMinimumPriceResponse>()
            .ForMember(
                dest => dest.PlanId,
                opt => opt.MapFrom(
                    src => src.PlanId
                )
            )
            .ForMember(
                dest => dest.RoomId,
                opt => opt.MapFrom(
                    src => src.RoomGroupId
                )
            )
            .ForMember(
                dest => dest.SiteId,
                opt => opt.MapFrom(
                    src => src.SiteId
                )
            )
            .ForMember(
                dest => dest.IsEnabledMinimumPrice,
                opt => opt.MapFrom(
                    src => src.IsEnabledMinimumPrice
                )
            )
            .ForMember(
                dest => dest.MinimumPrice,
                opt => opt.MapFrom(
                    src => src.MinimumPrice
                )
            );
    }
}
