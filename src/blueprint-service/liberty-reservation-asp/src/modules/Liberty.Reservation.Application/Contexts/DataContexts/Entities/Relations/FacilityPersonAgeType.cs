using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
/// 施設年齢別対象人区分
/// 大人、小学生＞低学年などを設定
///
/// </summary>
public class FacilityPersonAgeType : EntityRelation
{
    public long FacilityId { get; set; }

    public Facility? Facility { get; set; }

    public long PersonAgeTypeId { get; set; }

    public PersonAgeType? PersonAgeType { get; set; }
}
