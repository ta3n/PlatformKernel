using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class ReservationRoomGroupAppDatePersonAgeType : EntityRelation
{
    public long ReservationId { get; set; }
    public Data.Reservation? Reservation { get; set; }

    public long RoomGroupId { get; set; }
    public RoomGroup? RoomGroup { get; set; }

    public long PersonAgeTypeId { get; set; }
    public PersonAgeType? PersonAgeType { get; set; }

    public long BookingDateId { get; set; }

    /// <summary>
    /// 日付順
    /// </summary>
    public int RestIndex { get; set; }

    /// <summary>
    /// 部屋順
    /// </summary>
    public int RoomGroupIndex { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal SpaTax { get; set; }

    /// <summary>
    /// 男性人数
    /// </summary>
    public int MaleNumber { get; set; }

    /// <summary>
    /// 女性人数
    /// </summary>
    public int FemaleNumber { get; set; }

    /// <summary>
    /// 性別未設定人数
    /// </summary>
    public int GenderNoneNumber { get; set; }

    /// <summary>
    /// 人数
    /// </summary>
    public int Number => MaleNumber + FemaleNumber + GenderNoneNumber;

    // public decimal TotalRoomGroupPrice => UnitPrice * Number;
    //
    // public decimal TotalSpaTax => SpaTax * Number;
    //
    // public decimal TotalPrice => TotalRoomGroupPrice + TotalSpaTax;
}
