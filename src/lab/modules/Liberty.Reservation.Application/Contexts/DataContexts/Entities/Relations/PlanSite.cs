using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
/// プランサイトリレーション
/// </summary>
public class PlanSite : EntityRelation
{
    /// <summary>
    /// プランID
    /// </summary>
    public long PlanId { get; set; }

    /// <summary>
    /// プラン
    /// </summary>
    public Plan? Plan { get; set; }

    public long SiteId { get; set; }

    public Site? Site { get; set; }

    public PlanSite()
    {
    }

    public PlanSite(
        Plan plan,
        Site site
    )
    {
        PlanId = plan.Id;
        Plan = plan;
        SiteId = site.Id;
        Site = site;
    }
}
