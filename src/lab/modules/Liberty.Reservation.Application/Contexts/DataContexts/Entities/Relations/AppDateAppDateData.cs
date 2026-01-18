using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class AppDateAppDateData : EntityRelation
{
    public long AppDateId { get; set; }
    public AppDate? AppDate { get; set; }

    public long AppDateDataId { get; set; }
    public AppDateData? AppDateData { get; set; }

    public AppDateAppDateData()
    {
    }

    public AppDateAppDateData(
        AppDate appDate,
        AppDateData appDateData
    )
    {
        AppDateId = appDate.Id;
        AppDate = appDate;
        AppDateDataId = appDateData.Id;
        AppDateData = appDateData;
    }
}
