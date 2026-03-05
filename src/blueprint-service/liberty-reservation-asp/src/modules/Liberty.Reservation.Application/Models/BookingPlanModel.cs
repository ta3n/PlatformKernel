using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Models;

public record BookingPlanModel(
    long Id,
    bool IsEnabled,
    bool UseAcceptPersonNumber,
    int? AcceptPersonNumberMin,
    int? AcceptPersonNumberMax,
    int? NumberOfStayLimitMin,
    int? NumberOfStayLimitMax,
    bool IsOnLinePayment,
    TimeSpan? CheckInStart,
    TimeSpan? CheckInEnd,
    TimeSpan? CheckOut,
    bool IsOnSidePayment,
    bool UseDaySaleLimit,
    PlanDaySaleLimitTypes PlanDaySaleLimitType,
    int? RoomNumberDaySaleLimit,
    bool UseDisplayDate,
    long? DisplayDateStart,
    long? DisplayDateEnd,
    bool UseAcceptDate,
    long? AcceptDateStart,
    long? AcceptDateEnd,
    int? GroupNumberDaySaleLimit,
    int? ReceptionDayLimit,
    TimeSpan? ReceptionLimit,
    bool UseBookingReception,
    long? BookingReceptionStart,
    long? BookingReceptionEnd,
    MultilingualText? Name,
    MultilingualText? Tag,
    MultilingualText? Summary,
    MultilingualText? Description,
    PlanTypes PlanType,
    bool? FacilityUseDailyPerson,
    long DisplayOrder,
    bool DayUse,
    bool FacilityUseSpaTax,
    IEnumerable<CategoryOfBookingPlanModel> Categories,
    IEnumerable<MediaOfBookingPlanModel> Media,
    IEnumerable<MealTypeOfBookingPlanModel> MealTypes,
    IEnumerable<RoomGroupOfBookingPlanModel> RoomGroups,
    bool IsCancelSameAccept,
    int? CancelDayLimit,
    TimeSpan? CancelLimit,
    BookingCancellationPolicyModel? CancellationDataPolicy
)
{
    public string? Code { get; set; }
    public IEnumerable<BookingMetaOptionItemModel> OptionItems { get; set; } = [];
    public IEnumerable<BookingMetaReservationModel> Reservations { get; set; } = [];

    /// <summary>
    /// Checks if the display date range is properly set up.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the display date range is enabled and both start and end dates are set; otherwise, <c>false</c>.
    /// </returns>
    public bool IsSetupDisplayDate()
    {
        return UseDisplayDate && DisplayDateStart is not null && DisplayDateEnd is not null;
    }

    /// <summary>
    /// Checks if the acceptance date range is properly set up.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the acceptance date range is enabled and both start and end dates are set; otherwise, <c>false</c>.
    /// </returns>
    public bool IsSetupAcceptDate()
    {
        return UseAcceptDate && AcceptDateStart is not null && AcceptDateEnd is not null;
    }

    /// <summary>
    /// Checks if the booking reception period is properly set up.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the booking reception is enabled and both start and end dates are set; otherwise, <c>false</c>.
    /// </returns>
    public bool IsSetupBookingReception()
    {
        return UseBookingReception && BookingReceptionStart is not null && BookingReceptionEnd is not null;
    }

    /// <summary>
    /// Determines whether the specified date is within the acceptable date range for booking.
    /// </summary>
    /// <param name="appDateId">The application date ID to check.</param>
    /// <returns>
    /// <c>true</c> if the date is within the acceptable range or if the acceptance date range is not used; otherwise, <c>false</c>.
    /// </returns>
    public bool CanAcceptDate(
        long appDateId
    )
    {
        // 使用しないのであればtrue
        if (!UseAcceptDate)
        {
            return true;
        }

        if (AcceptDateStart > appDateId)
        {
            return false;
        }

        return !(AcceptDateEnd < appDateId);
    }

    /// <summary>
    /// Determines whether the specified date is within the displayable date range for booking.
    /// </summary>
    /// <param name="appDateId">The application date ID to check.</param>
    /// <returns>
    /// <c>true</c> if the date is within the displayable range or if the display date range is not used; otherwise, <c>false</c>.
    /// </returns>
    public bool CanDisplayDate(
        long appDateId
    )
    {
        // 使用しないのであればtrue
        if (!UseDisplayDate)
        {
            return true;
        }

        if (DisplayDateStart > appDateId)
        {
            return false;
        }

        return !(DisplayDateEnd < appDateId);
    }

    /// <summary>
    /// Determines whether the specified application date is within the booking reception period.
    /// </summary>
    /// <param name="appDateId">
    /// The application date ID to check, represented as a long value.
    /// </param>
    /// <returns>
    /// <c>true</c> if the booking reception is not used or the specified date is within the reception period; otherwise, <c>false</c>.
    /// </returns>
    /// <example>
    /// <code>
    /// var isReceptionOpen = bookingPlanModel.CanBookingReception(20231015);
    /// </code>
    /// </example>
    public bool CanBookingReceptionDate(
        long appDateId
    )
    {
        // If booking reception is not used, return true
        if (!UseBookingReception)
        {
            return true;
        }

        // Check if the application date is before the start of the booking reception period
        if (BookingReceptionStart > appDateId)
        {
            return false;
        }

        // Check if the application date is after the end of the booking reception period
        return !(BookingReceptionEnd < appDateId);
    }

    /// <summary>
    /// Determines whether reception is available for the specified check-in date.
    /// </summary>
    /// <param name="checkInDate">
    /// The check-in date to check, represented as a long value.
    /// </param>
    /// <param name="isNotCheckValidDateLimit">
    /// A boolean flag indicating whether to skip the validation of the date limit.
    /// Defaults to <c>false</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if reception is available for the specified check-in date; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method checks the availability of reception based on the following conditions:
    /// <list type="bullet">
    /// <item>If <see cref="ReceptionDayLimit"/> is <c>null</c>, reception is always available.</item>
    /// <item>If the current date and time exceed the calculated limit (based on <see cref="ReceptionDayLimit"/> and <see cref="ReceptionLimit"/>),
    /// and <paramref name="isNotCheckValidDateLimit"/> is <c>false</c>, reception is unavailable.</item>
    /// <item>If <see cref="UseBookingReception"/> is <c>false</c>, reception is always available.</item>
    /// <item>Otherwise, the method checks if the <paramref name="checkInDate"/> falls within the range defined by
    /// <see cref="BookingReceptionStart"/> and <see cref="BookingReceptionEnd"/>.</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// var isAvailable = bookingPlanModel.IsReceptionAvailable(20231015);
    /// </code>
    /// </example>
    public bool IsReceptionAvailable(
        long checkInDate,
        bool isNotCheckValidDateLimit = false
    )
    {
        // Convert the check-in date to a DateTime object
        var checkInDateTime = AppDate.GetDateTime(checkInDate);

        // Get the current UTC time adjusted for the default time zone offset
        var now = DateTime.UtcNow.AddHours(DefaultValues.TimeZoneOffset);

        var cancelDayLimit = ReceptionDayLimit ?? 0;
        var cancelLimit = ReceptionLimit ?? TimeSpan.FromHours(24);

        var limit = checkInDateTime;

        limit = limit.AddDays(-cancelDayLimit);

        limit += cancelLimit;

        // If the current time exceeds the limit and validation is not skipped, reception is unavailable
        if (now > limit && !isNotCheckValidDateLimit)
        {
            return false;
        }

        // If booking reception is not used, reception is always available
        if (!UseBookingReception)
        {
            return true;
        }

        // Check if the check-in date falls within the booking reception period
        var isAvailable = BookingReceptionStart <= checkInDate
            && BookingReceptionEnd >= checkInDate;

        return isAvailable;
    }

    /// <summary>
    /// Determines whether the reserved number of rooms is within the daily sale limit.
    /// </summary>
    /// <param name="reservedNumber">The number of reserved rooms to check.</param>
    /// <returns>
    /// <c>true</c> if the reserved number is within the limit or if the daily sale limit is not used; otherwise, <c>false</c>.
    /// </returns>
    public bool CanDaySaleLimitReservedNumber(
        int reservedNumber
    )
    {
        // 使用しないのであればtrue
        if (!UseDaySaleLimit)
        {
            return true;
        }

        // 組数で制限する場合
        if (PlanDaySaleLimitType != PlanDaySaleLimitTypes.RoomGroup || RoomNumberDaySaleLimit is null)
        {
            return true;
        }

        return !(RoomNumberDaySaleLimit <= reservedNumber);
    }

    /// <summary>
    /// Determines whether the reserved number of pairs is within the daily sale limit.
    /// </summary>
    /// <param name="reservationPairs">The number of reserved pairs to check.</param>
    /// <returns>
    /// <c>true</c> if the reserved pairs are within the limit or if the daily sale limit is not used; otherwise, <c>false</c>.
    /// </returns>
    public bool CanDaySaleLimitReservedPairs(
        int reservationPairs
    )
    {
        // 使用しないのであればtrue
        if (!UseDaySaleLimit)
        {
            return true;
        }

        // 組数で制限する場合
        if (PlanDaySaleLimitType != PlanDaySaleLimitTypes.Pair || GroupNumberDaySaleLimit is null)
        {
            return true;
        }

        return !(GroupNumberDaySaleLimit <= reservationPairs);
    }

    /// <summary>
    /// Determines whether the total number of persons is within the acceptable range.
    /// </summary>
    /// <param name="allPersons">The total number of persons to check.</param>
    /// <returns>
    /// <c>true</c> if the total number of persons is within the range or if the acceptance limit is not used; otherwise, <c>false</c>.
    /// </returns>
    public bool CanDaySaleLimitAllPersons(
        int allPersons
    )
    {
        // 使用しないのであればtrue
        if (!UseAcceptPersonNumber)
        {
            return true;
        }

        // 受け入れ人数最大
        if (AcceptPersonNumberMax < allPersons)
        {
            return false;
        }

        // 受け入れ人数最小
        return !(AcceptPersonNumberMin > allPersons);
    }

    /// <summary>
    /// Determines whether the number of remaining stays is within the acceptable range.
    /// </summary>
    /// <param name="restNumber">The number of remaining stays to check.</param>
    /// <returns>
    /// <c>true</c> if the number of remaining stays is within the range; otherwise, <c>false</c>.
    /// </returns>
    public bool CanNumberOfStayLimitMax(
        int restNumber
    )
    {
        if (NumberOfStayLimitMax < restNumber)
        {
            return false;
        }

        return !(NumberOfStayLimitMin > restNumber);
    }

    /// <summary>
    /// Retrieves the media code of the first media item, ordered by index.
    /// </summary>
    /// <returns>
    /// The media code as a <see cref="string"/> if available; otherwise, <c>null</c>.
    /// </returns>
    public string? GetMediaCode()
    {
        var media = Media.OrderBy(x => x.Index).FirstOrDefault();
        return media?.Code;
    }
}

public record CategoryOfBookingPlanModel(
    long Id,
    MultilingualText? Name,
    CategoryTypes CategoryType,
    bool IsMaster
)
{
    public string? Code { get; set; }
}

public record MealTypeOfBookingPlanModel(
    long Id,
    string? Name,
    MealTypeEatTypes MealTypeEatType
)
{
    public string? Code { get; set; }
}

public record RoomGroupOfBookingPlanModel(
    long Id,
    MultilingualText? Name,
    string? GroupName,
    string? Tag,
    MultilingualText? Description,
    MultilingualText? Overview,
    long DisplayOrder,
    bool IsEnabledSmoking,
    bool IsOverviewVisible,
    bool IsDescriptionVisible,
    bool IsRoomSizeVisible,
    bool IsBedTypeVisible,
    int? CapacityMin,
    int? CapacityMax,
    IEnumerable<MediaOfBookingPlanModel> Media
)
{
    public string? Code { get; set; }
    public IEnumerable<BookingMetaRoomAppDateModel> AppDates { get; set; } = [];
    public IEnumerable<BookingMetaPlanAppDateModel> PlanAppDates { get; set; } = [];
    public IEnumerable<BookingMetaPersonTypeModel> PersonTypes { get; set; } = [];
    public IEnumerable<BookingMetaPriceDataModel> PriceData { get; set; } = [];
    public IEnumerable<BookingMetaDiscountDataModel> DiscountData { get; set; } = [];
}

public record MediaOfBookingPlanModel(
    string? Code,
    string? ContentType,
    int Index,
    bool IsEnabled
);

public class BookingCancellationPolicyModel
{
    public long Id { get; set; }
    public MultilingualText? Name { get; set; }
    public bool CanOnLinePayment { get; set; }
    public int? PaymentLimit { get; set; }
    public MultilingualText? TableSource { get; set; }
    public MultilingualText? Description { get; set; }
    public MultilingualText? RuleDetail { get; set; }
    public IEnumerable<BookingCancellationDataPolicyModel>? CancellationData { get; set; }
}

public class BookingCancellationDataPolicyModel
{
    public int DayStart { get; set; }

    public int DayEnd { get; set; }

    public float Rate { get; set; }

    public bool IsRange(
        int day
    )
    {
        return DayStart <= day && DayEnd >= day;
    }

    public decimal Calc(
        decimal totalPrice
    )
    {
        return Math.Truncate(totalPrice / 100 * (decimal)Rate);
    }
}
