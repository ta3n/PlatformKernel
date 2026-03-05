using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class FacilityLanguage : EntityRelation
{
    public long FacilityId { get; set; }
    public Facility? Facility { get; set; }

    public long LanguageId { get; set; }
    public Language? Language { get; set; }
}
