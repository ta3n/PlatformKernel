using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class ReservationRoomGroupAppDateOptionItem : EntityRelation
{
    public long ReservationId { get; set; }

    public Data.Reservation? Reservation { get; set; }

    public long RoomGroupId { get; set; }

    public RoomGroup? RoomGroup { get; set; }

    public long AppDateId { get; set; }
    public AppDate? AppDate { get; set; }

    public long OptionItemId { get; set; }
    public OptionItem? OptionItem { get; set; }

    public int RestIndex { get; set; }
    public int RoomGroupIndex { get; set; }

    public int Price { get; set; }

    public int Number { get; set; }

    public int TotalPrice => Price * Number;

    /// <summary>
    /// オプションアイテム販売数
    /// </summary>
    public OptionItemAppDate? OptionItemAppDate { get; set; }

    public ReservationRoomGroupAppDateOptionItem()
    {
    }

    public ReservationRoomGroupAppDateOptionItem(
        Data.Reservation reservation,
        RoomGroup roomGroup,
        AppDate appDate,
        OptionItem optionItem,
        int restIndex,
        int roomGroupIndex
    )
    {
        ReservationId = reservation.Id;
        Reservation = reservation;
        RoomGroupId = roomGroup.Id;
        RoomGroup = roomGroup;
        AppDateId = appDate.Id;
        AppDate = appDate;
        OptionItemId = optionItem.Id;
        OptionItem = optionItem;

        RestIndex = restIndex;
        RoomGroupIndex = roomGroupIndex;

        //ReservationRoomGroupAppDate = reservationRoomGroupAppDate;
    }

    /*
    public long ReservationRoomAppDateOptionItemID { get; set; }


    public long ReservationRoomGroupAppDateID { get; set; }
    public ReservationPlanRoomGroupAppDate ReservationRoomGroupAppDate { get; set; }


    public long OptionItemID { get; set; }
    public OptionItem OptionItem { get; set; }

    public int Price { get; set; }

    public int Number { get; set; }

    public int TotalPrice
    {
        get
        {
            return this.Price * this.Number;
        }

    }

    public ReservationRoomAppDateOptionItem() { }

    public ReservationRoomAppDateOptionItem(
        ReservationPlanRoomGroupAppDate reservationRoomGroupAppDate,
        OptionItem optionItem)
    {
        ReservationRoomGroupAppDate = reservationRoomGroupAppDate;

        OptionItemID = optionItem.OptionItemID;
        OptionItem = optionItem;
    }
    */
}
