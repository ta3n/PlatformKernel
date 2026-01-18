using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

public interface IPersonAgeTypeSpaTaxDataService : IBaseServiceRelation<PersonAgeTypeSpaTaxData>
{
    /// <summary>
    /// Changes the Spa Tax data associated with a specific PersonAgeType.
    /// Determines which items should be added, updated, or removed,
    /// and optionally saves the changes.
    /// </summary>
    /// <param name="id">The ID of the PersonAgeType to update.</param>
    /// <param name="listPersonAgeTypeSpaTaxData">
    /// The new set of Spa Tax data to compare against the existing records.
    /// </param>
    /// <param name="autoSave">
    /// Whether to automatically save changes to the database. Default is true.
    /// </param>
    /// <param name="cancellationToken">
    /// Token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A tuple containing three lists:
    /// - Items to add
    /// - Items to update
    /// - Items to remove
    /// </returns>
    Task<(
        List<PersonAgeTypeSpaTaxData> addSpaTaxDataOfPersonAgeTypes,
        List<PersonAgeTypeSpaTaxData> updateSpaTaxDataOfPersonAgeTypes,
        List<PersonAgeTypeSpaTaxData> removeSpaTaxDataOfPersonAgeTypes
        )> ChangeSpaTaxDataOfPersonAgeTypeAsync(
        long id,
        List<PersonAgeTypeSpaTaxData> listPersonAgeTypeSpaTaxData,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );
}
