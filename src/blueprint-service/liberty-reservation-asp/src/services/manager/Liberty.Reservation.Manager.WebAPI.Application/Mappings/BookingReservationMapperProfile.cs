using Liberty.Reservation.Application.Mappings;
using Liberty.UnitOfWork.DbFunctions;

namespace Liberty.Reservation.Manager.WebAPI.Application.Mappings;

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
                dest => dest.Code,
                opt => opt.MapFrom(
                    src => src.Code
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
                    src => src.BookingData!.Site.Name
                )
            );

        CreateMap<Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation, SaleDetailResponse>()
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
                dest => dest.CheckInDate,
                opt => opt.MapFrom(
                    src => src.CheckInDate
                )
            )
            .ForMember(
                dest => dest.CheckOutDate,
                opt => opt.MapFrom(
                    src => src.CheckOutDate
                )
            )
            .ForMember(
                dest => dest.NumberOfNights,
                opt => opt.MapFrom(
                    src => src.RestNumber
                )
            )
            .ForMember(
                dest => dest.NumberOfPeople,
                opt =>
                    opt.MapFrom(
                        src =>
                            src.ReservationRoomGroupAppDatePersonAgeTypes!.Sum(
                                x => x.MaleNumber + x.FemaleNumber + x.GenderNoneNumber
                            )
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
                dest => dest.TotalPrice,
                opt => opt.MapFrom(
                    src => src.BookingData!.AllTotalPrice
                )
            )
            .ForMember(
                dest => dest.FacilityCode,
                opt => opt.MapFrom(
                    src => src.Facility!.Code
                )
            )
            .ForMember(
                dest => dest.FacilityName,
                opt => opt.MapFrom(
                    src => src.BookingData!.Facility.Name
                )
            )
            .ForMember(
                dest => dest.FacilityRecordCode,
                opt => opt.MapFrom(
                    src => src.FacilityRecordCode
                )
            )
            .ForMember(
                dest => dest.DayUse,
                opt => opt.MapFrom(
                    src => src.BookingData!.Plan.DayUse
                )
            );
    }
}
