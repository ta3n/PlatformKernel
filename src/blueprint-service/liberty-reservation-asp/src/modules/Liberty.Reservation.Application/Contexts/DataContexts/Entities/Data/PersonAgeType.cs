using Liberty.Entity;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// 年齢種別
/// 大人、小学生、幼児
/// 小学生＞高学年、小学生＞低学年等は、施設側で決める
/// </summary>
public class PersonAgeType : EntityData
{
    public MultilingualText? Name { get; set; }

    public bool IsMaster { get; set; }

    /// <summary>
    /// 大人〇名として料金指定の際のメインとなる区分としてあつかうか？
    /// </summary>
    public bool IsMain { get; set; }

    /// <summary>
    /// 年齢上限
    /// </summary>
    public int? AgeMax { get; set; }

    /// <summary>
    /// 年齢下限
    /// </summary>
    public int? AgeMin { get; set; }

    /// <summary>
    /// 情報JSON
    /// </summary>
    public PersonAgeTypeMeta? Meta { get; set; }

    public ICollection<FacilityPersonAgeType>? FacilityPersonAgeTypes { get; set; }
    public ICollection<PersonAgeTypeSpaTaxData>? PersonAgeTypeSpaTaxDatas { get; set; }
    public ICollection<PlanRoomGroupSitePersonAgeType>? PlanRoomGroupSitePersonAgeTypes { get; set; }

    public ICollection<ReservationRoomGroupAppDatePersonAgeType>? ReservationRoomGroupAppDatePersonAgeTypes
    {
        get;
        set;
    }

    public void SetIsMainSetting()
    {
        if (IsMain)
        {
            IsEnabled = true;
        }
    }
}
