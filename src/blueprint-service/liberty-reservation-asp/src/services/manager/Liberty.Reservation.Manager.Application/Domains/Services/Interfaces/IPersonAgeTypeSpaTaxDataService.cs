using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IPersonAgeTypeSpaTaxDataService : IBaseServiceRelation<PersonAgeTypeSpaTaxData>
{
    Task<(
        List<PersonAgeTypeSpaTaxData> addSpaTaxDataOfPersonAgeTypes,
        List<PersonAgeTypeSpaTaxData> updateSpaTaxDataOfPersonAgeTypes,
        List<PersonAgeTypeSpaTaxData> removeSpaTaxDataOfPersonAgeTypes
        )> ChangeSpaTaxDataOfPersonAgeType(
        long id,
        long facilityId,
        List<PersonAgeTypeSpaTaxData> listPersonAgeTypeSpaTaxData,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );
}
