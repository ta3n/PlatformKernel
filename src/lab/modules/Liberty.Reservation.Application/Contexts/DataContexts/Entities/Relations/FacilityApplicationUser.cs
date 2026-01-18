using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
/// 施設会員
/// </summary>
public class FacilityApplicationUser : EntityRelation
{
    public long FacilityId { get; set; }
    public Facility? Facility { get; set; }

    public long UserId { get; set; }

    public User? User { get; set; }

    public FacilityApplicationUser()
    {
    }

    public FacilityApplicationUser(
        Facility facility,
        User user
    )
    {
        FacilityId = facility.Id;
        Facility = facility;
        UserId = user.Id;
        User = user;
    }
}
