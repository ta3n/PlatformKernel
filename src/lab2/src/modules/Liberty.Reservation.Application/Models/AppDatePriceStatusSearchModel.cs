namespace Liberty.Reservation.Application.Models;

public record AppDatePriceStatusSearchModel
{
    public bool IsRoomAvailable { get; set; }
    public bool IsRoomUnderRequested { get; set; }
    public bool IsAcceptDate { get; set; }
    public bool IsDayBookable { get; set; }
    public bool IsNight { get; set; }
    public bool IsAvailable => IsDayBookable && IsRoomAvailable && IsAcceptDate && IsNight;
}
