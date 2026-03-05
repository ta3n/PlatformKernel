using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Mappings;
using Liberty.Reservation.User.WebAPI.Application.Models.Responses;
using Liberty.UnitOfWork.DbFunctions;

namespace Liberty.Reservation.User.WebAPI.Application.Mappings;

public class ReservationMapperProfile : BookingMapperProfile
{
    public ReservationMapperProfile()
    {
        CreateMap<Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation, ReservationResponse>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(
                    src => src.Id
                )
            )
            .ForMember(
                dest => dest.Code,
                opt => opt.MapFrom(
                    src => src.Code
                )
            )
            .ForMember(
                dest => dest.MediaCode,
                opt => opt.MapFrom(
                    src => src.BookingData!.MediaCode
                )
            )
            .ForMember(
                dest => dest.State,
                opt => opt.MapFrom(
                    src => src.ReservationState
                )
            )
            .ForMember(
                dest => dest.CanOnLinePayment,
                opt => opt.MapFrom(
                    src => src.Facility!.CanOnLinePayment
                        && src.Facility!.IsOnLinePayment
                        && src.Plan!.IsOnLinePayment
                        && src.PaymentType == PaymentTypes.OnSidePayment
                        && (
                            src.ReservationState == ReservationStatus.Reserved
                            || src.ReservationState == ReservationStatus.Modified
                        )
                )
            )
            .ForMember(
                dest => dest.RoomGroupName,
                opt => opt.MapFrom(
                    src => src.BookingData!.RoomGroup.Name
                )
            )
            .ForMember(
                dest => dest.PlanName,
                opt => opt.MapFrom(
                    src => src.BookingData!.Plan.Name
                )
            )
            .ForMember(
                dest => dest.SiteName,
                opt => opt.MapFrom(
                    src => src.BookingData!.Site.Name
                )
            )
            .ForMember(
                dest => dest.CheckInDate,
                opt => opt.MapFrom(
                    src => AppDate.GetDateTime(src.CheckInDate, null)
                )
            )
            .ForMember(
                dest => dest.ReservationDateTime,
                opt => opt.MapFrom(
                    src => src.ReservationDateTime
                )
            )
            .ForMember(
                dest => dest.IsEnabledSmoking,
                opt => opt.MapFrom(
                    src => src.BookingData!.RoomGroup.IsEnabledSmoking
                )
            )
            .ForMember(
                dest => dest.LengthOfStay,
                opt => opt.MapFrom(
                    src => src.RestNumber
                )
            )
            .ForMember(
                dest => dest.NumberOfRooms,
                opt => opt.MapFrom(
                    src => src.RoomNumber
                )
            )
            .ForMember(
                dest => dest.MealTypes,
                opt => opt.MapFrom(src => GetMealTypes(src))
            )
            .ForMember(
                dest => dest.PaymentType,
                opt => opt.MapFrom(
                    src => src.PaymentType.ToString()
                )
            )
            .ForMember(
                dest => dest.Memo,
                opt => opt.MapFrom(
                    src => src.Memo
                )
            )
            .ForMember(
                dest => dest.Facilityname,
                opt => opt.MapFrom(
                    src => src.BookingData!.Facility.Name
                )
            )
            .ForMember(
                dest => dest.TotalPrice,
                opt => opt.MapFrom(
                    src => src.BookingData!.TotalPrice
                )
            )
            .ForMember(
                dest => dest.CheckInTime,
                opt => opt.MapFrom(
                    src => src.CheckInTime
                )
            )
            .ForMember(
                dest => dest.ReceptionDayLimit,
                opt => opt.MapFrom(
                    src => src.Plan!.ReceptionDayLimit
                )
            )
            .ForMember(
                dest => dest.ReceptionLimit,
                opt => opt.MapFrom(
                    src => src.Plan!.ReceptionLimit
                )
            )
            .ForMember(
                dest => dest.IsCancelSameAccept,
                opt => opt.MapFrom(
                    src => src.Plan!.IsCancelSameAccept
                )
            )
            .ForMember(
                dest => dest.CancelDayLimit,
                opt => opt.MapFrom(
                    src => src.Plan!.CancelDayLimit
                )
            )
            .ForMember(
                dest => dest.CancelLimit,
                opt => opt.MapFrom(
                    src => src.Plan!.CancelLimit
                )
            )
            .ForMember(
                dest => dest.TimeZoneOffset,
                opt => opt.MapFrom(
                    src => GetTimeZoneOffset(src)
                )
            )
            .ForMember(
                dest => dest.PlanType,
                opt => opt.MapFrom(
                    src => src.Plan!.PlanType
                )
            )
            .ForMember(
                dest => dest.IsNoShow,
                opt => opt.MapFrom(
                    src => src.IsNoShow
                )
            )
            .ForMember(
                dest => dest.NoShowReason,
                opt => opt.MapFrom(
                    src => src.NoShowReason
                )
            )
            .ForMember(
                dest => dest.NoShowDateTime,
                opt => opt.MapFrom(
                    src => src.NoShowDateTime
                )
            )
            .ForMember(
                dest => dest.CheckOutTime,
                opt => opt.MapFrom(
                    src => src.CheckOutTime
                )
            )
            .ForMember(
                dest => dest.RestNumber,
                opt => opt.MapFrom(
                    src => src.RestNumber
                )
            )
            .ForMember(
                dest => dest.DayUse,
                opt => opt.MapFrom(
                    src => src.BookingData!.Plan.DayUse
                )
            )
            .ForMember(
                dest => dest.UpdateCount,
                opt => opt.MapFrom(
                    src => src.UpdateCount
                )
            )
            .ForMember(
                dest => dest.UpdatedAt,
                opt => opt.MapFrom(
                    src =>
                        src.ModifiedDateTime.HasValue ? src.ModifiedDateTime.Value.ToUniversalTime() : (DateTime?)null
                )
            )
            .ForMember(
                dest => dest.IsDiffAmount,
                opt => opt.MapFrom(
                    src =>
                        src.ParentId.HasValue
                        && src.PaymentType == PaymentTypes.OnLinePayment
                        && src.ParentId.Value.GetRegisterReservationDiffAmountGmoPayment()
                )
            )
            .ForMember(
                dest => dest.RootCode,
                opt => opt.MapFrom(
                    src => src.BookingData!.RootCode ?? src.Code
                )
            );
    }

    private static int GetTimeZoneOffset(
        Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation src
    )
    {
        return src.BookingData?.TimeZoneOffset ?? DefaultValues.TimeZoneOffset;
    }

    private static IEnumerable<MealTypeOfReservationResponse>? GetMealTypes(
        Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation src
    )
    {
        if (src.BookingData?.Plan?.Meals is null)
        {
            return null;
        }

        return src.BookingData.Plan.Meals.Select(
            x => new MealTypeOfReservationResponse(
                x.Id ?? 0,
                x.MealTypeEatType,
                x.Name
            )
        );
    }
}
