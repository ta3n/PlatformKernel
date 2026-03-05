using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class FacilityPlan : EntityRelation
{
    public long FacilityId { get; set; }
    public Facility? Facility { get; set; }

    public long PlanId { get; set; }
    public Plan? Plan { get; set; }
}
