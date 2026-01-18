using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class FacilitySite : EntityRelation
{
    public long FacilityId { get; set; }
    public Facility? Facility { get; set; }

    public long SiteId { get; set; }
    public Site? Site { get; set; }

    public bool IsDefault { get; set; }

    /// <summary>
    /// 宿が利用するかの有無
    /// </summary>
    public bool IsUsed { get; set; }

    public FacilitySite()
    {
    }

    public FacilitySite(
        Facility facility,
        Site site
    )
    {
        FacilityId = facility.Id;
        Facility = facility;
        SiteId = site.Id;
        Site = site;
    }
}
