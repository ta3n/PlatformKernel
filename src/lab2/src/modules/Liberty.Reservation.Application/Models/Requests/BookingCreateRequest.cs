using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Constants;
using Newtonsoft.Json;

namespace Liberty.Reservation.Application.Models.Requests;

public record BookingCreateRequest(
    string LanguageCode,
    long FacilityId,
    long SiteId,
    long PlanId,
    long RoomGroupId,
    long CheckInDate,
    string? CheckInTime,
    TimeSpan? CheckOutTime,
    PaymentTypes PaymentType,
    BookingAdjustRequest Adjust,
    string? FacilityRecordCode,
    IEnumerable<QuestionOfBookingCreateRequest>? PlanQuestions,
    IEnumerable<QuestionOfBookingCreateRequest>? OptionsQuestions
)
{
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDateTime { get; set; }

    public int TimeZoneOffset { get; set; } = 9;
    public int UpdateCount { get; set; } = 0;

    public PlanTypes PlanType { get; set; }

    public long? SelectedCancellation { get; set; } = null;

    [JsonIgnore]
    public string TempCode { get; set; } = EntityUtil.CreateCode();

    [JsonIgnore]
    public bool IsNotCheckValidDateLimit { get; set; } = false;
}

public record QuestionOfBookingCreateRequest(
    long QuestionId,
    string? AnswerData
)
{
    [JsonIgnore]
    public bool IsRequired { get; set; }
};
