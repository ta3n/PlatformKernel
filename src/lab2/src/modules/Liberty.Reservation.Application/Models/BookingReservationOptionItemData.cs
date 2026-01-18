using Liberty.Entity.ValueObjects;

namespace Liberty.Reservation.Application.Models;

public class BookingReservationOptionItemData
{
    public long AppDateId { get; set; }
    public int RoomGroupIndex { get; set; }
    public OptionInfoOfBookingReservationOptionItemData? OptionItemInfo { get; set; }
    public decimal Price { get; set; }
    public int? Number { get; set; }
    public decimal? TotalPrice { get; set; }
}

public class OptionInfoOfBookingReservationOptionItemData
{
    public long Id { get; set; }
    public MultilingualText? Name { get; set; }
    public decimal Price { get; set; }
}
