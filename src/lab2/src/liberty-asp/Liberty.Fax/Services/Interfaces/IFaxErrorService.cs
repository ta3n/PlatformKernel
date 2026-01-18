using Liberty.Fax.Models;

namespace Liberty.Fax.Services.Interfaces;

/// <summary>
/// Interface for the Fax Error Service, which provides methods to retrieve fax error information.
/// </summary>
public interface IFaxErrorService
{
    /// <summary>
    /// Retrieves all fax error items.
    /// </summary>
    /// <returns>A list of all <see cref="FaxErrorItem"/> objects.</returns>
    List<FaxErrorItem> GetAll();

    /// <summary>
    /// Retrieves a fax error item by its error code.
    /// </summary>
    /// <param name="code">The error code to search for.</param>
    /// <returns>
    /// The <see cref="FaxErrorItem"/> with the specified code, or <c>null</c> if no match is found.
    /// </returns>
    FaxErrorItem? GetByCode(
        string code
    );
}
