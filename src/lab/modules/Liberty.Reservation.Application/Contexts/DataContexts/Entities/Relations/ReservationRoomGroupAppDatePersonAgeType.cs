using Liberty.Entity;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

//
public class ReservationRoomGroupAppDatePersonAgeType : EntityRelation
{
    public long ReservationId { get; set; }
    public Data.Reservation? Reservation { get; set; }

    public long RoomGroupId { get; set; }
    public RoomGroup? RoomGroup { get; set; }

    public long AppDateId { get; set; }
    public AppDate? AppDate { get; set; }

    public long PersonAgeTypeId { get; set; }
    public PersonAgeType? PersonAgeType { get; set; }

    /// <summary>
    /// 日付順
    /// </summary>
    public int RestIndex { get; set; }

    /// <summary>
    /// 部屋順
    /// </summary>
    public int RoomGroupIndex { get; set; }

    public int UnitPrice { get; set; }

    public int SpaTax { get; set; }

    /// <summary>
    /// 人数
    /// </summary>
    //public int Number { get; set; }
    public int Number => UserInfos?.Count ?? 0;

    /// <summary>
    /// 男性人数
    /// </summary>
    //public int Number { get; set; }
    public int MaleNumber => UserInfos?.Count(a => a.Gender == Genders.Male) ?? 0;

    /// <summary>
    /// 女性人数
    /// </summary>
    //public int Number { get; set; }
    public int FemaleNumber => UserInfos?.Count(a => a.Gender == Genders.Female) ?? 0;

    /// <summary>
    /// 性別未設定人数
    /// </summary>
    //public int Number { get; set; }
    public int GenderNoneNumber => UserInfos?.Count(a => a.Gender == Genders.None) ?? 0;

    /// <summary>
    /// 人数内訳
    /// </summary>
    public ICollection<UserInfo>? UserInfos { get; set; }

    public int TotalRoomGroupPrice => UnitPrice * Number;

    public int TotalSpaTax => SpaTax * Number;

    public int TotalPrice => TotalRoomGroupPrice + TotalSpaTax;

    public ReservationRoomGroupAppDatePersonAgeType()
    {
    }

    public ReservationRoomGroupAppDatePersonAgeType(
        Data.Reservation reservation,
        RoomGroup roomGroup,
        int roomGroupIndex,
        AppDate appDate,
        int restIndex,
        PersonAgeType personAgeType
    )
    {
        ReservationId = reservation.Id;
        Reservation = reservation;
        RoomGroupId = roomGroup.Id;
        RoomGroup = roomGroup;
        AppDateId = appDate.Id;
        AppDate = appDate;
        PersonAgeTypeId = personAgeType.Id;
        PersonAgeType = personAgeType;

        RoomGroupIndex = roomGroupIndex;
        RestIndex = restIndex;
    }
}
