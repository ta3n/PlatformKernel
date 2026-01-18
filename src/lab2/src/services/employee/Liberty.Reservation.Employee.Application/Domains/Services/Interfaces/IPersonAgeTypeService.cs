using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

public interface IPersonAgeTypeService : IBaseService<PersonAgeType>
{
    /// <summary>
    /// Checks whether a person age group with the specified name already exists.
    /// Useful for validating duplicates when creating or updating a record.
    /// </summary>
    /// <param name="groupName">The name of the age group to check.</param>
    /// <param name="personAgeTypeId">
    /// The ID of the current record (optional). Use this to exclude the record itself during update checks.
    /// </param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>True if the age group name already exists; otherwise, false.</returns>
    Task<bool> CheckExistingPersonAgeGroupAsync(
        string groupName,
        long personAgeTypeId = 0,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a PersonAgeType by its Id where IsMain is true.
    /// Returns a new PersonAgeType instance if no matching record is found.
    /// </summary>
    /// <param name="id">The Id of the PersonAgeType to retrieve.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    /// The matching PersonAgeType, or a new PersonAgeType instance if none is found.
    /// </returns>
    Task<PersonAgeType> GetPersonAgeTypeIsMainAsync(
        long id,
        CancellationToken cancellationToken = default
    );
}
