using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

public interface ILanguageService : IBaseService<Language>
{
    Task<IEnumerable<Language>> GetMasterLanguagesAsync(
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<Language>> GetLanguagesOfFacilityAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    );
}
