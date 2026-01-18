using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class ReservationRoomGroupAppDateOptionItem : EntityRelation
{
    public long ReservationId { get; set; }

    public Data.Reservation? Reservation { get; set; }

    public long RoomGroupId { get; set; }

    public RoomGroup? RoomGroup { get; set; }

    public long OptionItemId { get; set; }
    public OptionItem? OptionItem { get; set; }

    public long BookingDateId { get; set; }

    public int RestIndex { get; set; }
    public int RoomGroupIndex { get; set; }

    public decimal Price { get; set; }

    public int Number { get; set; }

    public decimal TotalPrice => Price * Number;
}
