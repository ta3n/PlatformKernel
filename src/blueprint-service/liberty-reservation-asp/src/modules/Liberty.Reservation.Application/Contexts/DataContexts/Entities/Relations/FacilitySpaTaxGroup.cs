using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class FacilitySpaTaxGroup : EntityRelation
{
    public long FacilityId { get; set; }
    public Facility? Facility { get; set; }

    public long SpaTaxGroupId { get; set; }
    public SpaTaxGroup? SpaTaxGroup { get; set; }

    public long? EnabledStart { get; set; }
    public long? EnabledEnd { get; set; }
}
