using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class ApplicationUserPoint : EntityRelation
{
    public long UserId { get; set; }
    public User? User { get; set; }

    public long PointId { get; set; }
    public Point? Point { get; set; }

    public ApplicationUserPoint()
    {
    }

    public ApplicationUserPoint(
        User user,
        Point point
    )
    {
        UserId = user.Id;
        User = user;
        PointId = point.Id;
        Point = point;
    }
}
