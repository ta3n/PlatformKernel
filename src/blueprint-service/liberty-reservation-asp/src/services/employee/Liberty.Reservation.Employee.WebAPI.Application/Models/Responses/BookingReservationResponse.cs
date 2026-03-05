using Newtonsoft.Json;

namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record BookingReservationResponse
{
    public long Id { get; set; }
    public string? FacilityName { get; set; }
    public string? FacilityCode { get; set; }
    public string? SiteName { get; set; }
    public string? SiteCode { get; set; }
    public string? RoomGroupCode { get; set; }
    public string? RoomGroupName { get; set; }
    public string? Code { get; set; }
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
    public bool? DayUse { get; init; }

    [JsonIgnore]
    public string? ParentCode { get; set; }

    [JsonIgnore]
    public string? GmoOrderId { get; init; }

    public DateTime CheckOutDate { get; set; }
    public decimal AllTotalPrice { get; set; }
    public string? AccessID { get; set; }

    public string? RootCode { get; init; }
}
