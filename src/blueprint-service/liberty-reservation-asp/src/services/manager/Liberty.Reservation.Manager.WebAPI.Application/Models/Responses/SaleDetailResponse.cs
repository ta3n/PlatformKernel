namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record SaleDetailResponse
{
    public long Id { get; init; }
    public string? Code { get; init; }
    public string? State { get; init; }
    public bool IsReserved { get; init; }
    public DateTime? BookingDateTime { get; init; }
    public long CheckInDate { get; init; }
    public long CheckOutDate { get; init; }
    public int NumberOfNights { get; init; }
    public int NumberOfPeople { get; init; }
    public string? ReserverName { get; init; }
    public string? PaymentType { get; init; }
    public decimal TotalPrice { get; init; }
    public string? FacilityCode { get; init; }
    public string? FacilityName { get; init; }
    public string? FacilityRecordCode { get; set; }
    public bool? DayUse { get; init; }
}
