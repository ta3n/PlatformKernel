using System.Globalization;

namespace Liberty.Reservation.Site.WebAPI.Application.Models.Responses;

public record BookingDetailsResponse
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Tag { get; set; }
    public string? Summary { get; set; }
    public string? Description { get; set; }
    public string? Payment { get; set; }
    public string? Meal { get; set; }
    public string? Other { get; set; }
    public TimeSpan? CheckInStart { get; set; }
    public TimeSpan? CheckInEnd { get; set; }
    public TimeSpan? CheckOut { get; set; }
    public int TimeIntervalMinutes { get; set; }
    public bool IsOnLinePayment { get; set; }
    public bool IsOnSidePayment { get; set; }
    public long SiteId { get; set; }
    public string? SiteCode { get; set; }
    public string? TimeZone { get; set; }
    public bool UseDisplayDate { get; set; }
    public long? DisplayDateStart { get; set; }
    public long? DisplayDateEnd { get; set; }
    public bool UseAcceptDate { get; set; }
    public long? AcceptDateStart { get; set; }
    public long? AcceptDateEnd { get; set; }
    public PlanTypes PlanType { get; set; }
    public CancellationResponse? Cancellation { get; set; }
    public RoomGroupResponse? RoomGroup { get; set; }
    public string? LastUpdateString { get; set; }
    public bool UseBookingReception { get; set; }
    public long? BookingReceptionStart { get; set; }
    public long? BookingReceptionEnd { get; set; }
    public List<string>? Categories { get; set; }
    public bool DayUse { get; set; }
    public List<FileOfBookingResponse>? Files { get; set; }
    public List<MealOfBookingResponse>? Meals { get; set; }
    public List<QuestionOfBookingResponse>? Questions { get; set; }
    public List<PersonAgeTypeOfBookingFacilityResponse>? PersonAgeTypes { get; set; }

    public bool IsAvailableBookingReception
    {
        get
        {
            var offset = TimeSpan.TryParse(TimeZone, CultureInfo.InvariantCulture, out var parsed)
                ? parsed
                : DefaultValues.DefaultTimeZoneOffset;

            var bookingAtDateTime = DateTime.UtcNow.Add(offset);
            var bookingAtDate = AppDate.GetId(bookingAtDateTime.Date);

            if (!UseDisplayDate)
            {
                return true;
            }

            var isAvailable = DisplayDateStart <= bookingAtDate && DisplayDateEnd >= bookingAtDate;
            if (!isAvailable)
            {
                return false;
            }

            if (!UseAcceptDate)
            {
                return true;
            }

            isAvailable = AcceptDateStart <= bookingAtDate && AcceptDateEnd >= bookingAtDate;

            return isAvailable;
        }
    }
}
