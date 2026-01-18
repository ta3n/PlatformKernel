using Liberty.ApplicationShared.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.UnitOfWork.DbFunctions;
using static Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses.GetBookingResponse;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Mappings;

public class BookingMapperProfile : Profile
{
    public BookingMapperProfile()
    {
        CreateMap<MultilingualText, string>().ConvertUsing(src => src == null ? string.Empty : src.GetValueByHeader());

        CreateMap<ReservationEntity, Booking>()
            .ForMember(
                dest => dest.BookingId,
                opt => opt.MapFrom(
                    src => src.Id.GetRootReservationCode()
                )
            )
            .ForMember(
                dest => dest.FacilityId,
                opt => opt.MapFrom(
                    src => src.FacilityId
                )
            )
            .ForMember(
                dest => dest.FacilityCode,
                opt => opt.MapFrom(
                    src => src.Facility!.Code
                )
            )
            .ForMember(
                dest => dest.BookingTime,
                opt => opt.MapFrom(
                    src =>
                        ConvertUtil.ToString(
                            src.Id.GetRootReservationDateTime(),
                            Booking.BookingTimeFormat
                        )
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
                        src.ModifiedDateTime.HasValue
                            ? ConvertUtil.ToString(src.ModifiedDateTime.Value, Booking.BookingTimeFormat)
                            : string.Empty
                )
            )
            .ForMember(
                dest => dest.PlanName,
                opt => opt.MapFrom(
                    src => src.BookingData != null ? src.BookingData.Plan.Name : string.Empty
                )
            )
            .ForMember(
                dest => dest.RoomId,
                opt => opt.MapFrom(
                    src => src.RoomGroup!.GroupName
                )
            )
            .ForMember(
                dest => dest.RoomCount,
                opt => opt.MapFrom(
                    src => src.RoomNumber
                )
            )
            .ForMember(
                dest => dest.StayDaysCount,
                opt => opt.MapFrom(
                    src => src.BookingData!.Plan.DayUse ? 0 : src.RestNumber
                )
            )
            .ForMember(
                dest => dest.ArriveDate,
                opt => opt.MapFrom(
                    src => AppDate.GetDateTime(src.CheckInDate, null).ToString("yyyy-MM-dd")
                )
            )
            .ForMember(
                dest => dest.TotalPrice,
                opt => opt.MapFrom(
                    src => src.BookingData != null ? src.BookingData.AllTotalPrice : 0
                )
            )
            .ForMember(
                dest => dest.BookingStatus,
                opt => opt.MapFrom(
                    src => MapBookingStatus(src.ReservationState)
                )
            )
            .ForMember(
                dest => dest.CancelledDateTime,
                opt => opt.MapFrom(
                    src => src.CancelledDateTime.HasValue
                        ? ConvertUtil.ToString(src.CancelledDateTime.Value, Booking.BookingTimeFormat)
                        : string.Empty
                )
            )
            .ForMember(
                dest => dest.NoShow,
                opt => opt.MapFrom(
                    src => src.IsNoShow
                )
            )
            .ForMember(
                dest => dest.NoShowDateTime,
                opt => opt.MapFrom(
                    src => src.NoShowDateTime.HasValue
                        ? ConvertUtil.ToString(src.NoShowDateTime.Value, Booking.BookingTimeFormat)
                        : string.Empty
                )
            )
            .ForMember(
                dest => dest.BookingData,
                opt => opt.MapFrom(
                    src => src.BookingData
                )
            )
            .ForMember(
                dest => dest.Attention,
                opt => opt.MapFrom(
                    src => src.Memo
                )
            )
            .ForMember(
                dest => dest.GuestName,
                opt => opt.MapFrom(
                    src => src.Reserver!.Name
                )
            )
            .ForMember(
                dest => dest.Furigana,
                opt => opt.MapFrom(
                    src => src.Reserver!.Kana
                )
            )
            .ForMember(
                dest => dest.PostCode,
                opt => opt.MapFrom(
                    src => src.Reserver!.PostCode
                )
            )
            .ForMember(
                dest => dest.Address1,
                opt => opt.MapFrom(
                    src => string.Concat(src.Reserver!.Address1, src.Reserver!.Address2)
                )
            )
            .ForMember(
                dest => dest.Address2,
                opt => opt.MapFrom(
                    src => src.Reserver!.Address3 ?? string.Empty
                )
            )
            .ForMember(
                dest => dest.Mail,
                opt => opt.MapFrom(src => src.Reserver!.EMail ?? string.Empty)
            )
            .ForMember(
                dest => dest.Tel,
                opt => opt.MapFrom(src => src.Reserver!.Phone ?? string.Empty)
            )
            .ForMember(
                dest => dest.Sex,
                opt => opt.MapFrom(src => MapGender(src.Reserver!.Gender))
            )
            .ForMember(
                dest => dest.Age,
                opt => opt.MapFrom(src => MapAge(src.Reserver!.BirthDay))
            )
            .ForMember(
                dest => dest.StayName,
                opt => opt.MapFrom(src => src.MainUser != null ? src.MainUser!.Name : string.Empty)
            )
            .ForMember(
                dest => dest.StayFurigana,
                opt => opt.MapFrom(src => src.MainUser != null ? src.MainUser!.Kana : string.Empty)
            )
            .ForMember(
                dest => dest.StayPostCode,
                opt => opt.MapFrom(src => src.MainUser != null ? src.MainUser!.PostCode : string.Empty)
            )
            .ForMember(
                dest => dest.PaymentType,
                opt => opt.MapFrom(src => src.IsOnlinePayment ? "オンライン決済" : "現地決済")
            )
            .ForMember(
                dest => dest.StayAddress1,
                opt => opt.MapFrom(
                    src => src.MainUser != null ? string.Concat(src.MainUser!.Address1, src.MainUser!.Address2) : string.Empty
                )
            )
            .ForMember(
                dest => dest.StayAddress2,
                opt => opt.MapFrom(src => src.MainUser != null ? src.MainUser!.Address3 : string.Empty)
            )
            .ForMember(
                dest => dest.StayTel,
                opt => opt.MapFrom(src => src.MainUser != null ? src.MainUser!.Phone : string.Empty)
            )
            .ForMember(
                dest => dest.StaySex,
                opt => opt.MapFrom(src => src.MainUser != null ? MapGender(src.MainUser!.Gender) : EnumSex.Unselected)
            )
            .ForMember(
                dest => dest.StayAge,
                opt => opt.MapFrom(src => src.MainUser != null ? MapAge(src.MainUser!.BirthDay) : MapAge(0))
            )
            .ForMember(
                dest => dest.ArriveTime,
                opt => opt.MapFrom(src => ToHmsFormat(src.CheckInTime))
            )
            .ForMember(
                dest => dest.PlanType,
                opt => opt.MapFrom(
                    src =>
                        src!.Plan!.PlanType == PlanTypes.Combo
                            ? RateTypeLabels.PerPerson
                            : RateTypeLabels.PerRoom
                )
            )
            .ForMember(
                dest => dest.UnitPriceAndPeopleCounts,
                opt => opt.MapFrom(
                    src => src.ReservationRoomGroupAppDatePersonAgeTypes!
                        .Where(x => x.ReservationId == src.Id)
                        .GroupBy(x => x.BookingDateId)
                        .Select(
                            item => new UnitPriceAndPeopleCount
                            {
                                BookingDateId = item.FirstOrDefault()!.BookingDateId,
                                PersonAgeTypes = item
                                    .Select(
                                        t => new AppDatePersonAgeType(
                                            t.RestIndex,
                                            t.RoomGroupIndex,
                                            t.UnitPrice,
                                            t.SpaTax,
                                            t.MaleNumber,
                                            t.FemaleNumber,
                                            t.GenderNoneNumber,
                                            t.Number,
                                            t.PersonAgeType!.Meta!.PersonAgeGroup,
                                            t.PersonAgeType!.Meta!.FoodBed
                                        )
                                    )
                                    .ToList(),
                                // NOTE: 新プラン予約は日帰り入湯税の設定がないため0固定で返す。
                                BathtaxDate = new NameAndRate
                                {
                                    Count = 0,
                                    Rate = 0
                                },
                                // NOTE: 部屋ごとのオプション
                                AddMenus = src.ReservationRoomGroupAppDateOptionItems!
                                    .Where(x => x.BookingDateId == item.Key)
                                    .Select(
                                        x => new AddMenu
                                        {
                                            Name = x.OptionItem!.Name!.GetValueByHeader(),
                                            Type = 0,
                                            Rate = x.Price,
                                            Count = x.Number
                                        }
                                    )
                                    .ToList(),
                                BookingData = src.BookingData,
                                PlanType = src.Plan!.PlanType == PlanTypes.Combo
                                    ? RateTypeLabels.PerPerson
                                    : RateTypeLabels.PerRoom,
                                UseSpaTax = src.BookingData!.UseSpaTax ?? true
                            }
                        )
                )
            );
    }

    private static EnumBookingStatus MapBookingStatus(
        ReservationStatus reservationState
    )
    {
        return reservationState switch
        {
            ReservationStatus.Confirmed
                or ReservationStatus.Reserved => EnumBookingStatus.Reserve,
            ReservationStatus.UserCanceled
                or ReservationStatus.ManagerCanceled
                or ReservationStatus.GuestCanceled => EnumBookingStatus.Cancel,
            _ => EnumBookingStatus.Modified
        };
    }

    private static string ToHmsFormat(
        TimeSpan? timeSpan
    )
    {
        var ts = timeSpan ?? TimeSpan.Zero;
        var totalHours = (int)ts.TotalHours;
        return $"{totalHours}:{ts.Minutes:D2}:{ts.Seconds:D2}";
    }

    private static EnumSex MapGender(
        Genders gender
    )
    {
        return gender switch
        {
            Genders.Female => EnumSex.Female,
            Genders.Male => EnumSex.Male,
            _ => EnumSex.Unselected
        };
    }

    public static class RateTypeLabels
    {
        public const string PerPerson = "人数単価";
        public const string PerRoom = "部屋単価";
    }

    private static EnumAge MapAge(
        long? longBirthDay
    )
    {
        if (longBirthDay is null or 0)
        {
            return EnumAge.Unselected;
        }

        var today = DateTime.Now;
        var birthDay = AppDate.GetDateTime((long)longBirthDay);

        if (birthDay > today)
        {
            return EnumAge.Unselected;
        }

        var age = today.Year - birthDay.Year;

        // 今年の誕生日前の場合、1歳マイナス
        if (today < birthDay.AddYears(age))
        {
            age--;
        }

        return (age / 10) switch
        {
            1 => EnumAge.Age10,
            2 => EnumAge.Age20,
            3 => EnumAge.Age30,
            4 => EnumAge.Age40,
            5 => EnumAge.Age50,
            6 => EnumAge.Age60,
            7 => EnumAge.Age70,
            8 => EnumAge.Age80,
            _ => EnumAge.Unselected
        };
    }
}
