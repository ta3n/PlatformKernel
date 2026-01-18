using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class PersonAgeTypeSpaTaxData : EntityRelation
{
    public long PersonAgeTypeId { get; set; }
    public PersonAgeType? PersonAgeType { get; set; }

    public long SpaTaxDataId { get; set; }
    public SpaTaxData? SpaTaxData { get; set; }

    public PersonAgeTypeSpaTaxData()
    {
    }

    public PersonAgeTypeSpaTaxData(
        PersonAgeType personAgeType,
        SpaTaxData spaTaxData
    )
    {
        PersonAgeTypeId = personAgeType.Id;
        PersonAgeType = personAgeType;
        SpaTaxDataId = spaTaxData.Id;
        SpaTaxData = spaTaxData;
    }
}
