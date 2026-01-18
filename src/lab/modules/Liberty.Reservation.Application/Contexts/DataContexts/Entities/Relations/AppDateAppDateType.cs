using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class AppDateAppDateType : EntityRelation
{
    public long AppDateId { get; set; }
    public AppDate? AppDate { get; set; }

    public long AppDateTypeId { get; set; }
    public AppDateType? AppDateType { get; set; }

    public AppDateAppDateType()
    {
    }

    public AppDateAppDateType(
        AppDate appDate,
        AppDateType appDateType
    )
    {
        AppDateId = appDate.Id;
        AppDate = appDate;
        AppDateTypeId = appDateType.Id;
        AppDateType = appDateType;
    }
}
