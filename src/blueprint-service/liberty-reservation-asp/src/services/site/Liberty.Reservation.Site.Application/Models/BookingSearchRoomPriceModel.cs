namespace Liberty.Reservation.Site.Application.Models;

public record BookingSearchRoomPriceModel
{
    public long AppDateId { get; set; }
    public IEnumerable<PricePerRoomModel> PricePerRooms { get; set; } = [];
}

public record PricePerRoomModel
{
    public int RoomGroupIndex { get; set; }

    public int? Persons { get; set; }

    public int? Price { get; set; }

    public int DiscountPrice { get; set; } = 0;

    public long PersonAgeTypeId { get; set; }
}
