using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class ApplicationUserPoint : EntityRelation
{
    public required string UserCode { get; set; }

    public long PointId { get; set; }
    public Point? Point { get; set; }
}
