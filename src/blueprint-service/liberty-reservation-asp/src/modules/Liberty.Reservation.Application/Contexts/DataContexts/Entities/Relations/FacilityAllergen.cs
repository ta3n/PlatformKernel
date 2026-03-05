using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class FacilityAllergen : EntityRelation
{
    public long FacilityId { get; set; }
    public Facility? Facility { get; set; }

    public long AllergenId { get; set; }
    public Allergen? Allergen { get; set; }
}
