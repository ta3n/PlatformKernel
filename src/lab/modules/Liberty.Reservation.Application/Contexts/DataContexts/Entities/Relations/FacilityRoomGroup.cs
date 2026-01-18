using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class FacilityRoomGroup : EntityRelation
{
    public long FacilityId { get; set; }
    public Facility? Facility { get; set; }

    public long RoomGroupId { get; set; }
    public RoomGroup? RoomGroup { get; set; }

    /// <summary>
    /// 基本部屋数
    /// </summary>
    public int Number { get; set; }

    public FacilityRoomGroup()
    {
    }

    public FacilityRoomGroup(
        Facility facility,
        RoomGroup roomGroup
    )
    {
        FacilityId = facility.Id;
        Facility = facility;
        RoomGroupId = roomGroup.Id;
        RoomGroup = roomGroup;
    }
}
