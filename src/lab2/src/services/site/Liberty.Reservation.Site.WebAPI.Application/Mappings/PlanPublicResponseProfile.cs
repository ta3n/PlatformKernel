namespace Liberty.Reservation.Site.WebAPI.Application.Mappings;

public sealed class PlanPublicResponseProfile : Profile
{
    public PlanPublicResponseProfile()
    {
        MapPlanDetail();
        MapRoomGroup();
        MapCancellationPolicy();
        MapSpecialNote();
    }

    #region Plan detail

    private void MapPlanDetail()
    {
        CreateMap<BookingDetailsResponse, BookingPlanDetailResponse>()
            .ForMember(
                d => d.Code,
                o => o.MapFrom(s => s.Code)
            )
            .ForMember(
                d => d.Name,
                o => o.MapFrom(s => s.Name)
            )
            .ForMember(
                d => d.Tag,
                o => o.MapFrom(s => s.Tag)
            )
            .ForMember(
                d => d.Summary,
                o => o.MapFrom(s => s.Summary)
            )
            .ForMember(
                d => d.Description,
                o => o.MapFrom(s => s.Description)
            )
            .ForMember(
                d => d.PlanType,
                o => o.MapFrom(s => s.PlanType)
            )
            .ForMember(
                d => d.IsOnLinePayment,
                o => o.MapFrom(s => s.IsOnLinePayment)
            )
            .ForMember(
                d => d.IsOnSidePayment,
                o => o.MapFrom(s => s.IsOnSidePayment)
            )
            .ForMember(
                d => d.CheckInStart,
                o => o.MapFrom(s => s.CheckInStart)
            )
            .ForMember(
                d => d.CheckInEnd,
                o => o.MapFrom(s => s.CheckInEnd)
            )
            .ForMember(
                d => d.CheckOut,
                o => o.MapFrom(s => s.CheckOut)
            )
            .ForMember(
                d => d.TimeIntervalMinutes,
                o => o.MapFrom(s => s.TimeIntervalMinutes)
            )
             .ForMember(
                d => d.SiteCode,
                o => o.MapFrom(s => s.SiteCode)
            )
            .ForMember(
                d => d.TimeZone,
                o => o.MapFrom(s => s.TimeZone)
            )
            .ForMember(
                d => d.UseDisplayDate,
                o => o.MapFrom(s => s.UseDisplayDate)
            )
            .ForMember(
                d => d.DisplayDateStart,
                o => o.MapFrom(s => s.DisplayDateStart)
            )
            .ForMember(
                d => d.DisplayDateEnd,
                o => o.MapFrom(s => s.DisplayDateEnd)
            )
            .ForMember(
                d => d.UseAcceptDate,
                o => o.MapFrom(s => s.UseAcceptDate)
            )
            .ForMember(
                d => d.AcceptDateStart,
                o => o.MapFrom(s => s.AcceptDateStart)
            )
            .ForMember(
                d => d.AcceptDateEnd,
                o => o.MapFrom(s => s.AcceptDateEnd)
            )
            .ForMember(
                d => d.LastUpdateString,
                o => o.MapFrom(s => s.LastUpdateString)
            )
            .ForMember(
                d => d.UseBookingReception,
                o => o.MapFrom(s => s.UseBookingReception)
            )
            .ForMember(
                d => d.BookingReceptionStart,
                o => o.MapFrom(s => s.BookingReceptionStart)
            )
            .ForMember(
                d => d.BookingReceptionEnd,
                o => o.MapFrom(s => s.BookingReceptionEnd)
            )
            .ForMember(
                d => d.Categories,
                o => o.MapFrom(s => s.Categories)
            )
            .ForMember(
                d => d.DayUse,
                o => o.MapFrom(s => s.DayUse)
            )
            .ForMember(
                d => d.Meals,
                o => o.MapFrom(s => s.Meals)
            )
            .ForMember(
                d => d.Questions,
                o => o.MapFrom(s => s.Questions)
            )
            .ForMember(
                d => d.PersonAgeTypes,
                o => o.MapFrom(s => s.PersonAgeTypes)
            )
            .ForMember(
                d => d.Files,
                o => o.MapFrom(s => s.Files)
            )
            .ForMember(
                d => d.IsAvailableBookingReception,
                o => o.MapFrom(s => s.IsAvailableBookingReception)
            );

    }

    #endregion

    #region Room group

    private void MapRoomGroup()
    {
        CreateMap<BookingDetailsResponse, BookingRoomGroupResponse>()
            .ForMember(
                d => d.Code,
                o => o.MapFrom(s => s.RoomGroup!.Code)
            )
            .ForMember(
                d => d.Name,
                o => o.MapFrom(s => s.RoomGroup!.Name)
            )
            .ForMember(
                d => d.Tag,
                o => o.MapFrom(s => s.RoomGroup!.Tag)
            )
            .ForMember(
                d => d.Description,
                o => o.MapFrom(s => s.RoomGroup!.Description)
            )
            .ForMember(
                d => d.Overview,
                o => o.MapFrom(s => s.RoomGroup!.Overview)
            )
            .ForMember(
                d => d.CapacityMin,
                o => o.MapFrom(s => s.RoomGroup!.CapacityMin)
            )
            .ForMember(
                d => d.CapacityMax,
                o => o.MapFrom(s => s.RoomGroup!.CapacityMax)
            )
            .ForMember(
                d => d.Size,
                o => o.MapFrom(s => s.RoomGroup!.Size)
            )
            .ForMember(
                d => d.RoomGroupSizeUnitType,
                o => o.MapFrom(s => s.RoomGroup!.RoomGroupSizeUnitType)
            )
            .ForMember(
                d => d.IsEnabledSmoking,
                o => o.MapFrom(s => s.RoomGroup!.IsEnabledSmoking)
            )
            .ForMember(
                d => d.IsRoomSizeVisible,
                o => o.MapFrom(s => s.RoomGroup!.IsEnabledSmoking)
            )
            .ForMember(
                d => d.IsBedTypeVisible,
                o => o.MapFrom(s => s.RoomGroup!.IsEnabledSmoking)
            )
            .ForMember(
                d => d.BedTypes,
                o => o.MapFrom(s => s.RoomGroup!.BedTypes)
            )
            .ForMember(
                d => d.Files,
                o => o.MapFrom(s => s.RoomGroup!.Files)
            )
            .ForMember(
                d => d.Categories,
                o => o.MapFrom(s => s.RoomGroup!.Categories)
            )
            .ForMember(
                d => d.Amenities,
                o => o.MapFrom(s => s.RoomGroup!.Amenities)
            );
    }

    #endregion

    #region Cancellation policy

    private void MapCancellationPolicy()
    {
        CreateMap<BookingDetailsResponse, BookingCancellationResponse>()
            .ForMember(
                d => d.Name,
                o => o.MapFrom(s => s.Cancellation!.Name)
            )
            .ForMember(
                d => d.Description,
                o => o.MapFrom(s => s.Cancellation!.Description)
            )
            .ForMember(
                d => d.TableSource,
                o => o.MapFrom(s => s.Cancellation!.TableSource)
            );
    }

    #endregion

    #region Special note

    private void MapSpecialNote()
    {
        CreateMap<BookingDetailsResponse, BookingSpecialNoteResponse>()
            .ForMember(
                d => d.Meal,
                o => o.MapFrom(s => s.Meal)
            )
            .ForMember(
                d => d.Other,
                o => o.MapFrom(s => s.Other)
            )
            .ForMember(
                d => d.Payment,
                o => o.MapFrom(s => s.Payment)
            );
    }

    #endregion
}
