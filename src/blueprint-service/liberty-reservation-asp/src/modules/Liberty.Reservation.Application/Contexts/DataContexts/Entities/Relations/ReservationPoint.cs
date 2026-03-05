using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class ReservationPoint : EntityRelation
{
    public long ReservationId { get; set; }
    public Data.Reservation? Reservation { get; set; }

    public long PointId { get; set; }
    public Point? Point { get; set; }
}
