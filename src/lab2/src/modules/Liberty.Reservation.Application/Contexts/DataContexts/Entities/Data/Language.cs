using Liberty.Entity;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

public class Language : EntityData
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool IsMaster { get; set; }
}
