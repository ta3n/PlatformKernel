using Liberty.Entity;
using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

public class Permission : EntityData
{
    public string? Key { get; set; }
    public string? GroupName { get; set; }
    public string? ParentCode { get; set; }
    public ItemTypes ItemType { get; set; } = ItemTypes.None;
    public string? Name { get; set; }
}
