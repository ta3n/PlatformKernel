namespace Liberty.Reservation.Manager.WebAPI.Application.Models;

public class ReservationQueryResult
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public bool IsDeleted { get; set; }
    public string? Code { get; set; }
    public int ReservationState { get; set; }
    public int CancellationStatus { get; set; }
    public int CancellationFeeType { get; set; }
    public decimal CancellationPrice { get; set; }
    public long CheckInDate { get; set; }
    public TimeSpan? CheckInTime { get; set; }
    public int NumberOfNights { get; set; }
    public int NumberOfRooms { get; set; }
    public int PaymentType { get; set; }
    public string? FreeInput { get; set; }
    public bool IsSameMainUser { get; set; }
    public bool UseRoomUser { get; set; }
    public bool IsNoShow { get; set; }
    public DateTime? NoShowDateTime { get; set; }
    public string? NoShowReason { get; set; }
    public bool UseDailyPerson { get; set; }
    public bool CanAddRoomOnModify { get; set; }
    public string? BarrierFreeInfoComment { get; set; }
    public string? SpaTaxComment { get; set; }
    public string? SpaTaxTable { get; set; }
    public int? ReceptionDayLimit { get; set; }
    public TimeSpan? ReceptionLimit { get; set; }
    public bool IsCancelSameAccept { get; set; }
    public int? CancelDayLimit { get; set; }
    public TimeSpan? CancelLimit { get; set; }
    public TimeSpan? CheckInStart { get; set; }
    public TimeSpan? CheckInEnd { get; set; }
    public int TimeIntervalMinutes { get; set; }
    public int PlanType { get; set; }
    public string? Payment { get; set; }
    public string? Meal { get; set; }
    public string? Other { get; set; }
    public DateTime? ModifiedDateTime { get; set; }
    public int UpdateCount { get; set; }

    // Reserver info
    public string? ReserverName { get; set; }
    public string? ReserverKana { get; set; }
    public string? ReserverEmail { get; set; }
    public string? ReserverAddress1 { get; set; }
    public string? ReserverAddress2 { get; set; }
    public string? ReserverAddress3 { get; set; }
    public string? ReserverCountryCode { get; set; }
    public string? ReserverPostCode { get; set; }
    public string? ReserverPhone { get; set; }
    public int? ReserverGender { get; set; }

    // Main user info
    public string? MainUserName { get; set; }
    public string? MainUserKana { get; set; }
    public string? MainUserAddress1 { get; set; }
    public string? MainUserAddress2 { get; set; }
    public string? MainUserAddress3 { get; set; }
    public string? MainUserCountryCode { get; set; }
    public string? MainUserPostCode { get; set; }
    public long MainUserBirthDay { get; set; }
    public int? MainUserGender { get; set; }
    public string? MainUserPhone { get; set; }

    // Cancellation info
    public string? CancellationName { get; set; }
    public string? CancellationDescription { get; set; }
    public string? CancellationTableSource { get; set; }
    public int CapacityMax { get; set; }

    // JSON fields
    public string? MediaCode { get; set; }
    public string? PlanData { get; set; }
    public string? SiteData { get; set; }

    public string? FacilityData { get; set; }
    public string? RoomGroupData { get; set; }
    public decimal AccommodationFee { get; set; }
    public decimal OptionalFee { get; set; }
    public decimal TaxFee { get; set; }
    public decimal TotalFee { get; set; }
    public string? PersonAgeTypes { get; set; }
    public string? AppDates { get; set; }
    public string? PlanQuestions { get; set; }
    public string? OptionQuestions { get; set; }
    public string? TimeZoneOffset { get; set; }
    public string? LanguageCode { get; set; }
    public string? IsBarrierFree { get; set; }
    public string? UseSpaTax { get; set; }
    public string? CancellationData { get; set; }
    public float? CancellationRateFee { get; set; }
    public string? CancellationDate { get; set; }
}
