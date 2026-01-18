using Liberty.Entity;
using Liberty.Entity.ValueObjects;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

public class AlertMessage : EntityData
{
    public MultilingualText? Title { get; set; }
    public MultilingualText? Content { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
}
