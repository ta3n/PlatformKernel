using Liberty.Reservation.Application.Mappings;
using Liberty.UnitOfWork.DbFunctions;

namespace Liberty.Reservation.Employee.WebAPI.Application.Mappings;

public class BookingReservationMapperProfile : BookingMapperProfile
{
    public BookingReservationMapperProfile()
    {
        CreateMap<Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation, BookingReservationResponse>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(
                    src => src.Id
                )
            )
            .ForMember(
                dest => dest.FacilityCode,
                opt => opt.MapFrom(
                    src => src.FacilityRecordCode
                )
            )
            .ForMember(
                dest => dest.FacilityName,
                opt => opt.MapFrom(
                    src => src.BookingData!.Facility!.Name
                )
            )
            .ForMember(
                dest => dest.SiteCode,
                opt => opt.MapFrom(
                    src => src.Site!.Code
                )
            )
            .ForMember(
                dest => dest.SiteName,
                opt => opt.MapFrom(
                    src => src.BookingData!.Site!.Name
                )
            )
            .ForMember(
                dest => dest.RoomGroupCode,
                opt => opt.MapFrom(
                    src => src.BookingData!.RoomGroup!.GroupName
                )
            )
            .ForMember(
                dest => dest.RoomGroupName,
                opt => opt.MapFrom(
                    src => src.BookingData!.RoomGroup!.Name
                )
            )
            .ForMember(
                dest => dest.Code,
                opt => opt.MapFrom(
                    src => src.Code
                )
            )
            .ForMember(
                dest => dest.ParentCode,
                opt => opt.MapFrom(
                    src => src.Parent!.Id.GetRootReservationCode()
                )
            )
            .ForMember(
                dest => dest.State,
                opt => opt.MapFrom(
                    src => src.ReservationState.ToString()
                )
            )
            .ForMember(
                dest => dest.IsReserved,
                opt => opt.MapFrom(
                    src => src.IsReserved
                )
            )
            .ForMember(
                dest => dest.BookingDateTime,
                opt => opt.MapFrom(
                    src => src.ReservationDateTime
                )
            )
            .ForMember(
                dest => dest.ConfirmDateTime,
                opt => opt.MapFrom(
                    src => src.ConfirmedDateTime
                )
            )
            .ForMember(
                dest => dest.CancelledDateTime,
                opt => opt.MapFrom(
                    src => src.CancelledDateTime
                )
            )
            .ForMember(
                dest => dest.CheckInDate,
                opt => opt.MapFrom(
                    src => src.CheckInDate
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
                dest => dest.ReserverName,
                opt => opt.MapFrom(
                    src => src.Reserver!.Name
                )
            )
            .ForMember(
                dest => dest.PaymentType,
                opt => opt.MapFrom(
                    src => src.PaymentType.ToString()
                )
            )
            .ForMember(
                dest => dest.IsOnlinePayment,
                opt => opt.MapFrom(
                    src => src.IsOnlinePayment
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
                dest => dest.DayUse,
                opt => opt.MapFrom(
                    src => src.BookingData!.Plan.DayUse
                )
            )
            .ForMember(
                dest => dest.GmoOrderId,
                opt => opt.MapFrom(
                    src => src.OrderReservations!.FirstOrDefault()!
                        .Order!.OrderGmoPaymentResultRequests!
                        .OrderBy(x => x.GmoPaymentResultRequestId)
                        .LastOrDefault()!
                        .GmoPaymentResultRequest!.OrderId
                )
            )
            .ForMember(
                dest => dest.CheckOutDate,
                opt => opt.MapFrom(
                    src => AppDate.GetDateTime(src.CheckInDate, null).AddDays(src.RestNumber)
                )
            )
            .ForMember(
                dest => dest.AllTotalPrice,
                opt => opt.MapFrom(
                    src => src.BookingData!.AllTotalPrice
                )
            )
            .ForMember(
                dest => dest.AccessID,
                opt => opt.MapFrom(
                    src => src.OrderReservations!.FirstOrDefault()!
                        .Order!.AccessID
                )
            )
            .ForMember(
                dest => dest.RootCode,
                opt => opt.MapFrom(
                    src => src.BookingData!.RootCode ?? src.Code
                )
            );

        CreateMap<BookingReservationResponse, BookingReservationCsv>()
            .ForMember(
                dest => dest.CheckInDate,
                opt => opt.MapFrom(
                    src => AppDate.GetDateTime(src.CheckInDate, null)
                )
            )
            .ForMember(
                dest => dest.PaymentType,
                opt => opt.MapFrom(
                    src => DefaultValues.ConvertPaymentTypeToJapanese(src.PaymentType ?? string.Empty)
                )
            )
            .ForMember(
                dest => dest.State,
                opt => opt.MapFrom(
                    src => DefaultValues.ConvertReservationStatusToJapanese(src.State ?? string.Empty)
                )
            )
            .ForMember(
                dest => dest.BookingDateTime,
                opt => opt.MapFrom(
                    src => src.BookingDateTime!.Value.AddHours(DefaultValues.TimeZoneOffset)
                )
            )
            .ForMember(
                dest => dest.CancelledDateTime,
                opt => opt.MapFrom(
                    src => src.CancelledDateTime!.Value.AddHours(DefaultValues.TimeZoneOffset)
                )
            )
            .ForMember(
                dest => dest.ConfirmDateTime,
                opt => opt.MapFrom(
                    src => src.ConfirmDateTime!.Value.AddHours(DefaultValues.TimeZoneOffset)
                )
            )
            .ForMember(
                dest => dest.NoShowDateTime,
                opt => opt.MapFrom(
                    src => src.NoShowDateTime!.Value.AddHours(DefaultValues.TimeZoneOffset)
                )
            );
    }
}
