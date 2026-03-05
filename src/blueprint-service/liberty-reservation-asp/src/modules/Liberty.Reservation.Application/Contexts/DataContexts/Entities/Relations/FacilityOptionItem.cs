using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class FacilityOptionItem : EntityRelation
{
    public long FacilityId { get; set; }
    public Facility? Facility { get; set; }

    public long OptionItemId { get; set; }
    public OptionItem? OptionItem { get; set; }
}
