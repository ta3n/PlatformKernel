namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record GmoPaymentResultResponse
{
    public long ReservationId { get; set; }
    public string ReservationCode { get; set; } = string.Empty;
    public string ErrorMesssage { get; set; } = string.Empty;
    public string? ShopId { get; set; }
    public string? ShopPass { get; set; }
    public string? AccessId { get; set; }
    public string? AccessPass { get; set; }
    public string? OrderId { get; set; }
    public string? Status { get; set; }
    public string? JobCd { get; set; }
    public string? Amount { get; set; }
    public string? Tax { get; set; }
    public string? Currency { get; set; }
    public string? Forward { get; set; }
    public string? Method { get; set; }
    public string? PayTimes { get; set; }
    public string? TranId { get; set; }
    public string? Approve { get; set; }
    public string? TranDate { get; set; }
    public string? ErrCode { get; set; }
    public string? ErrInfo { get; set; }
    public string? PayType { get; set; }
}
