using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
/// 料金カレンダー、対応料金タイプリレーション
/// </summary>
public class FacilityAppDateType : EntityRelation
{
    /// <summary>
    /// 料金カレンダーID
    /// </summary>
    public long FacilityId { get; set; }

    /// <summary>
    /// 料金カレンダー
    /// </summary>
    public Facility? Facility { get; set; }

    public long AppDateTypeId { get; set; }

    public AppDateType? AppDateType { get; set; }

    public FacilityAppDateType()
    {
    }

    public FacilityAppDateType(
        Facility facility,
        AppDateType appDateType
    )
    {
        FacilityId = facility.Id;
        Facility = facility;
        AppDateTypeId = appDateType.Id;
        AppDateType = appDateType;
    }
}
