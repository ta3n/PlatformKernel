using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Application.Domains.Services;

public class LanguageService(
    ILogger<LanguageService> logger,
    ILanguageRepository languageRepository,
    IFacilityLanguageRepository facilityLanguageRepository
) : BaseService<Language>(logger, languageRepository, new LanguageNotfoundException()), ILanguageService
{
    public async Task<IEnumerable<Language>> GetMasterLanguagesAsync(
        CancellationToken cancellationToken = default
    )
    {
        var queryable = GetQueryable()
            .Where(x => x.IsMaster);

        return await queryable.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Language>> GetLanguagesOfFacilityAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = facilityLanguageRepository.GetQueryable()
            .Include(x => x.Language)
            .Where(x => x.FacilityId == facilityId)
            .Select(x => x.Language!);

        return await queryable.ToListAsync(cancellationToken);
    }
}
