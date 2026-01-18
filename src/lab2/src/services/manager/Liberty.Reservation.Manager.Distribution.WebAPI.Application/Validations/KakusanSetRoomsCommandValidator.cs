using FluentValidation;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Settings;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Commands.Kakusan;
using static Liberty.Reservation.Application.Models.Responses.HotelModel;
using static Liberty.Reservation.Application.Models.Responses.SetRoomsResponse;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Validations;

public class KakusanSetRoomsCommandValidator : AbstractValidator<KakusanSetRoomsCommand>
{
    private readonly C004Setting _setting;

    public KakusanSetRoomsCommandValidator(
        C004Setting setting
    )
    {
        _setting = setting;

        RuleFor(x => x.Payload.Hotels)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty();
    }

    public (SetRoomsResponse, bool) ValidateRequest(
        SetRoomsRequest payload
    )
    {
        var response = new SetRoomsResponse { Reasons = [] };

        if (payload.Hotels.Count < _setting.HotelCountMin || payload.Hotels.Count > _setting.HotelCountMax)
        {
            response.Result = EnumResultType.Failure;
            response.Reasons = GetAllReasons(payload.Hotels);
            return (response, true);
        }

        var hasDuplicates = GetDuplicatedInputs(payload.Hotels);
        if (hasDuplicates is { Count: > 0 })
        {
            response.Result = EnumResultType.UpdateFailed;
            response.Reasons =
            [
                new()
                {
                    Message = FailureReason.HasDuplicateInput,
                    Hotels = hasDuplicates
                }
            ];
            return (response, true);
        }

        var invalidHotels = FetchInvalidParams(payload);
        if (invalidHotels.Count <= 0)
        {
            return (response, false);
        }

        response.Result = EnumResultType.Failure;
        response.Reasons = GetAllReasonsErrorParams(invalidHotels);
        return (response, true);
    }

    private static List<Reason> GetAllReasons(
        List<HotelModel> hotels
    )
    {
        return
        [
            new()
            {
                Message = FailureReason.InvalidParameterExcessHotels,
                Hotels = hotels
            }
        ];
    }

    // private static List<Reason> GetAllReasons(
    //     List<HotelModel> hotels,
    //     FailureReason reason
    // )
    // {
    //     return
    //     [
    //         .. hotels.Select(_ => new Reason { Message = reason })
    //     ];
    // }

    private static List<Reason> GetAllReasonsErrorParams(
        List<GetBookingRequest.C004InvalidParameter> invalidParams
    )
    {
        var reasons = new List<Reason>();

        foreach (var message in Enum.GetValues<FailureReason>())
        {
            var reason = new Reason
            {
                Message = message,
                Hotels =
                [
                    .. invalidParams
                        .Where(x => x.Message.Equals(message))
                        .Where(x => x.Hotel != null)
                        .Select(x => x.Hotel!)
                ]
            };
            if (reason.Hotels.Count > 0)
            {
                reasons.Add(reason);
            }
        }

        return reasons;
    }

    private List<GetBookingRequest.C004InvalidParameter> FetchInvalidParams(
        SetRoomsRequest request
    )
    {
        var invalidParams = new List<GetBookingRequest.C004InvalidParameter>();

        var keyCounter = 1;
        foreach (var hotel in request.Hotels)
        {
            // 処理用にレコードを一意に特定するためのKeyをセット
            hotel.Key = keyCounter++;

            var invalidMessage = ValidateParams(
                hotel,
                _setting.DaysMin,
                _setting.DaysMax
            );
            if (invalidMessage == FailureReason.None)
            {
                continue;
            }

            invalidParams.Add(
                new GetBookingRequest.C004InvalidParameter
                {
                    Message = invalidMessage,
                    Hotel = hotel
                }
            );
        }

        return invalidParams;
    }

    private static FailureReason ValidateParams(
        HotelModel hotelModel,
        int daysMin,
        int daysMax
    )
    {
        var hotelId = hotelModel.HotelId;
        var roomId = hotelModel.RoomId;
        var roomCount = hotelModel.RoomCount;
        var adjustType = hotelModel.AdjustType;

        if (string.IsNullOrEmpty(hotelId))
        {
            return FailureReason.InvalidParameterOutOfRangeHotelId;
        }

        if (string.IsNullOrEmpty(roomId))
        {
            return FailureReason.InvalidParameterOutOfRangeRoomId;
        }

        switch (adjustType)
        {
            case EnumAdjustType.RelativeDown:
            case EnumAdjustType.RelativeUp:
            case EnumAdjustType.Absolute:
                if (roomCount is < 0)
                {
                    return FailureReason.InvalidParameterOutOfRangeRoomCount;
                }

                break;
            case EnumAdjustType.Stop:
            case EnumAdjustType.Sale:
                break;
            default:
                return FailureReason.InvalidParameterOutOfRangeAdjustType;
        }

        return ValidateDateParams(
            hotelModel,
            daysMin,
            daysMax
        );
    }

    private static FailureReason ValidateDateParams(
        HotelModel hotelModel,
        int daysMin,
        int daysMax
    )
    {
        var dateType = hotelModel.GetDateType();

        return dateType switch
        {
            EnumDateType.NoUse => FailureReason.InvalidParameterNoUseDateParams,
            EnumDateType.UseAll => FailureReason.InvalidParameterUseAllDateParams,
            EnumDateType.UseFromDay => ValidateFromDayParams(hotelModel, daysMin, daysMax),
            EnumDateType.UseDates => ValidateUseDatesParams(hotelModel, daysMax),
            _ => FailureReason.None
        };
    }

    private static FailureReason ValidateFromDayParams(
        HotelModel hotelModel,
        int daysMin,
        int daysMax
    )
    {
        DateTime fromDay;
        try
        {
            fromDay = hotelModel.GetDateTimeFromDay();
        }
        catch (FormatException)
        {
            return FailureReason.InvalidParameterFormatFromday;
        }

        if (fromDay < DateTime.Now.Date)
        {
            return FailureReason.InvalidParameterOldFromday;
        }

        if (hotelModel.Days < daysMin || hotelModel.Days > daysMax)
        {
            return FailureReason.InvalidParameterOutOfRangeDays;
        }

        return FailureReason.None;
    }

    private static FailureReason ValidateUseDatesParams(
        HotelModel hotelModel,
        int daysMax
    )
    {
        List<DateTime> dates;
        try
        {
            dates = hotelModel.GetDateTimeDates();
        }
        catch (FormatException)
        {
            return FailureReason.InvalidParameterFormatDates;
        }

        if (dates.Exists(x => x.Date < DateTime.Now.Date))
        {
            return FailureReason.InvalidParameterOldDates;
        }

        if (FindDuplication(dates))
        {
            return FailureReason.InvalidParameterDuplicationDates;
        }

        return dates.Count > daysMax
            ? FailureReason.InvalidParameterExcessDates
            : FailureReason.None;

        static bool FindDuplication<T>(
            List<T> list
        )
        {
            return list.GroupBy(x => x).Any(x => x.Count() > 1);
        }
    }

    private static List<HotelModel> GetDuplicatedInputs(
        List<HotelModel> hotels
    )
    {
        var duplicates = new List<HotelModel>();

        // Check for exact duplicates first (same HotelId, RoomId, and FromDay)
        var exactDuplicates = hotels
            .GroupBy(
                hotelModel => (
                    hotelModel.HotelId,
                    hotelModel.RoomId,
                    hotelModel.FromDay
                )
            )
            .Where(grouping => grouping.Count() > 1)
            .SelectMany(grouping => grouping)
            .ToList();

        duplicates.AddRange(exactDuplicates);

        // Check for date range overlaps
        var dateRangeOverlaps = GetDateRangeOverlaps(hotels);
        duplicates.AddRange(dateRangeOverlaps);

        return duplicates.Distinct().ToList();
    }

    private static List<HotelModel> GetDateRangeOverlaps(
        List<HotelModel> hotels
    )
    {
        return
        [
            .. hotels
                .GroupBy(
                    h => new
                    {
                        h.HotelId,
                        h.RoomId
                    }
                )
                .Where(g => g.Count() > 1)
                .SelectMany(
                    group =>
                    {
                        var hotelsList = group.ToList();
                        var overlaps = new List<HotelModel>();

                        for (var i = 0; i < hotelsList.Count; i++)
                        {
                            for (var j = i + 1; j < hotelsList.Count; j++)
                            {
                                if (!HasDateRangeOverlap(hotelsList[i], hotelsList[j]))
                                {
                                    continue;
                                }

                                overlaps.Add(hotelsList[i]);
                                overlaps.Add(hotelsList[j]);
                            }
                        }

                        return overlaps;
                    }
                )
                .Distinct()
        ];
    }

    private static bool HasDateRangeOverlap(
        HotelModel hotel1,
        HotelModel hotel2
    )
    {
        try
        {
            var dateType1 = hotel1.GetDateType();
            var dateType2 = hotel2.GetDateType();

            // Get date ranges for both hotels
            var dates1 = GetDateRange(hotel1, dateType1);
            var dates2 = GetDateRange(hotel2, dateType2);

            if (dates1 is null || dates2 is null)
            {
                return false;
            }

            // Check if any date in dates1 overlaps with dates2
            return dates1.Exists(date => dates2.Contains(date));
        }
        catch
        {
            // If there's any error parsing dates, consider it as no overlap
            return false;
        }
    }

    private static List<DateTime>? GetDateRange(
        HotelModel hotel,
        EnumDateType dateType
    )
    {
        return dateType switch
        {
            EnumDateType.UseFromDay => GetFromDayRange(hotel),
            EnumDateType.UseDates => GetDatesRange(hotel),
            _ => null
        };
    }

    private static List<DateTime>? GetFromDayRange(
        HotelModel hotel
    )
    {
        try
        {
            var fromDay = hotel.GetDateTimeFromDay();
            var days = hotel.Days;

            var dateRange = new List<DateTime>();
            for (var i = 0; i < days; i++)
            {
                dateRange.Add(fromDay.AddDays(i));
            }

            return dateRange;
        }
        catch
        {
            return null;
        }
    }

    private static List<DateTime>? GetDatesRange(
        HotelModel hotel
    )
    {
        try
        {
            return hotel.GetDateTimeDates();
        }
        catch
        {
            return null;
        }
    }
}
