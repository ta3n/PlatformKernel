using Liberty.Reservation.Application.Models;

namespace Liberty.Reservation.Site.WebAPI.Application.Mappings;

public class BookingAuditLogMapperProfile : Profile
{
    public BookingAuditLogMapperProfile()
    {
        CreateMap<Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation, BookingHistoryDataModel>()
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
                dest => dest.Serial,
                opt => opt.MapFrom(
                    src => src.Serial
                )
            )
            .ForMember(
                dest => dest.UpdatedCount,
                opt => opt.MapFrom(
                    src => src.UpdateCount
                )
            )
            .ForMember(
                dest => dest.LastModifiedDateTime,
                opt => opt.MapFrom(
                    src => src.UpdatedAt
                )
            )
            .ForMember(
                dest => dest.LastModifiedBy,
                opt => opt.MapFrom(
                    src => src.UpdatedBy
                )
            )
            .ForMember(
                dest => dest.Basic,
                opt => opt.MapFrom(
                    src => src.Reserver == null
                        ? null
                        : new BasicOfOfBookingDataModel
                        {
                            CheckInTime = src.CheckInTime,
                            Memo = src.Memo,
                            NumberOfNights = src.RestNumber,
                            NumberOfRooms = src.RoomNumber
                        }
                )
            )
            .ForMember(
                dest => dest.MainUser,
                opt => opt.MapFrom(
                    src => src.MainUser == null
                        ? null
                        : new GuestOfBookingDataModel
                        {
                            Address1 = src.MainUser.Address1,
                            Address2 = src.MainUser.Address2,
                            Address3 = src.MainUser.Address3,
                            CountryCode = src.MainUser.CountryCode,
                            Gender = src.MainUser.Gender,
                            Kana = src.MainUser.Kana,
                            Name = src.MainUser.Name,
                            Phone = src.MainUser.Phone,
                            PostCode = src.MainUser.PostCode
                        }
                )
            )
            .ForMember(
                dest => dest.Reserver,
                opt => opt.MapFrom(
                    src => src.Reserver == null
                        ? null
                        : new CustomerOfBookingDataModel
                        {
                            Address1 = src.Reserver.Address1,
                            Address2 = src.Reserver.Address2,
                            Address3 = src.Reserver.Address3,
                            EMail = src.Reserver.EMail,
                            CountryCode = src.Reserver.CountryCode,
                            Gender = src.Reserver.Gender,
                            Kana = src.Reserver.Kana,
                            Name = src.Reserver.Name,
                            Phone = src.Reserver.Phone,
                            PostCode = src.Reserver.PostCode
                        }
                )
            )
            .ForMember(
                dest => dest.BookingData,
                opt => opt.MapFrom(src => src.BookingData)
            );
    }
}
