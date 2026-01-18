namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record BookingReservationResponse
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public string? SiteCode { get; set; }
    public string? SiteName { get; set; }
    public string? FacilityTimeZone { get; set; }
    public string? FacilityTimeZoneId { get; set; }
    public string? State { get; set; }
    public bool IsReserved { get; set; }
    public DateTime? BookingDateTime { get; set; }
    public DateTime? ConfirmDateTime { get; set; }
    public DateTime? CancelledDateTime { get; set; }
    public long CheckInDate { get; set; }
    public int LengthOfStay { get; set; }
    public int NumberOfRooms { get; set; }
    public string? ReserverName { get; set; }
    public string? PaymentType { get; set; }
    public bool IsOnlinePayment { get; set; }
    public bool IsNoShow { get; init; }
    public DateTime? NoShowDateTime { get; init; }
    public string? NoShowReason { get; init; }
    public bool? DayUse { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int UpdateCount { get; init; }
    public bool IsDiffAmount { get; set; }
    public string? RootCode { get; init; }
}
