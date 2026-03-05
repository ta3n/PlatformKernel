using AutoMapper;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Application.Mappings;

public class BookingMapperProfile : Profile
{
    protected BookingMapperProfile()
    {
        CreateMap<BookingAdjustRequest, Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation>()
            .ForMember(
                dest => dest.CheckInTime,
                opt => opt.MapFrom(
                    src => ConvertUtil.ToNullableTimeSpan(src.CheckInTime)
                )
            )
            .ForMember(
                dest => dest.Memo,
                opt => opt.MapFrom(
                    src => src.FreeInput
                )
            )
            .ForMember(
                dest => dest.Reserver,
                opt => opt.MapFrom(
                    src => src.Reserver
                )
            )
            .ForMember(
                dest => dest.IsSameMainUser,
                opt => opt.MapFrom(
                    src => src.MainUser != null
                )
            )
            .ForMember(
                dest => dest.MainUser,
                opt => opt.MapFrom(
                    src => src.MainUser
                )
            )
            .ForMember(
                dest => dest.UseRoomUser,
                opt => opt.MapFrom(
                    src => src.RoomRepresentatives != null
                )
            )
            .ForMember(
                dest => dest.ReservationPlanRoomGroupAppDates,
                opt => opt.MapFrom(
                    src => src.RoomRepresentatives
                )
            );

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
            )
            .ForMember(
                dest => dest.Gender,
                opt => opt.MapFrom(
                    src => src.Gender ?? Genders.None
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
            )
            .ForMember(
                dest => dest.Gender,
                opt => opt.MapFrom(
                    src => src.Gender ?? Genders.None
                )
            );

        CreateMap<RoomRepresentativeOfReservationAdjustRequest, ReservationPlanRoomGroupAppDate>()
            .ConstructUsing(
                src => new ReservationPlanRoomGroupAppDate
                {
                    RoomGroupIndex = src.RoomIndex,
                    CustomerInfo = new CustomerInfo
                    {
                        Name = src.FullName,
                        Kana = src.Kana
                    }
                }
            );

        CreateMap<CustomerInfo, ReservationUserData>();

        CreateMap<ReservationPlanRoomGroupAppDate, BookingAppDateData>()
            .ForMember(
                dest => dest.AppDateId,
                opt => opt.MapFrom(
                    src => src.BookingDateId
                )
            )
            .ForMember(
                dest => dest.RestIndex,
                opt => opt.MapFrom(
                    src => src.RestIndex
                )
            );
    }
}
