using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class FacilityCategory : EntityRelation
{
    public long FacilityId { get; set; }
    public Facility? Facility { get; set; }

    public long CategoryId { get; set; }
    public Category? Category { get; set; }
}
