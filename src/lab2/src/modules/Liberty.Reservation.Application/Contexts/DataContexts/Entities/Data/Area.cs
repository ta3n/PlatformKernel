using Liberty.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

public class Area : EntityData
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    [ForeignKey("Parent")]
    public long? ParentId { set; get; }

    public Area? Parent { get; set; }

    public ICollection<Area>? Children { get; set; }
}
