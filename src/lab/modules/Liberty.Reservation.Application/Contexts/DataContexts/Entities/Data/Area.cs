using Liberty.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

public class Area : EntityData
{
    public string? Name { get; set; }

    [ForeignKey("Parent")]
    public long? ParentID { set; get; }

    public Area? Parent { get; set; }
    public ICollection<Area>? Children { get; set; }
}
