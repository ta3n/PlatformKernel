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

    public FacilitySpaTaxGroup()
    {
    }

    public FacilitySpaTaxGroup(
        Facility facility,
        SpaTaxGroup spaTaxGroup
    )
    {
        FacilityId = facility.Id;
        Facility = facility;
        SpaTaxGroupId = spaTaxGroup.Id;
        SpaTaxGroup = spaTaxGroup;
    }
}
