using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
///  掲載先・ポイント付与率設定
/// </summary>
public class SitePointRate : EntityRelation
{
    public long SiteId { get; set; }

    public Site? Site { get; set; }

    public long PointRateId { get; set; }

    public PointRate? PointRate { get; set; }
}
