using Liberty.Cache.Services;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Manager.Application.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.CancellationPolicy;

public class CancellationPolicyUpdateCommandHandler(
    ILogger<CancellationPolicyUpdateCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    ICacheService cacheService,
    IServiceProvider serviceProvider
) : UpdateCommandHandlerBase<CancellationPolicyUpdateCommand, long>(unitOfWork, mapper)
{
    private readonly IFacilityService _facilityService
        = serviceProvider.GetRequiredService<IFacilityService>();

    private readonly ICancellationService _cancellationService
        = serviceProvider.GetRequiredService<ICancellationService>();

    private readonly ICancellationDataService _cancellationDataService
        = serviceProvider.GetRequiredService<ICancellationDataService>();

    private readonly IDataOfCancellationService _dataOfCancellationService
        = serviceProvider.GetRequiredService<IDataOfCancellationService>();

    private readonly ICancellationTableHtmlService _cancellationTableHtmlService
        = serviceProvider.GetRequiredService<ICancellationTableHtmlService>();

    protected override async Task<long> HandleAsync(
        CancellationPolicyUpdateCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var cancellationId = payload.Id;
        var facilityId = securityContextAccessor.FacilityKey;
        var languageCode = LanguageHeaderUtil.GetLanguageCodeFromHeader();
        var languageRevert = languageCode == "en" ? "ja" : "en";
        var facilityCount = await _facilityService.CountByIdsAsync(
            [facilityId],
            cancellationToken
        );
        if (facilityCount == 0)
        {
            throw new FacilityNotfoundException();
        }

        var cancellationCount = await _cancellationService.CountByIdsAsync(
            [cancellationId ?? 0],
            cancellationToken
        );
        if (cancellationCount == 0)
        {
            throw new CancellationNotfoundException();
        }

        var listDataOfCancellation = Mapper.Map<List<CancellationData>>(payload.Data) ?? [];

        if (listDataOfCancellation is { Count: > 0 })
        {
            listDataOfCancellation = [.. listDataOfCancellation.OrderBy(x => x.DisplayOrder)];
        }

        var cancellation = Mapper.Map<Cancellation>(
            payload
        );

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var existingDataOfCancellation = await _dataOfCancellationService.FindAllByCancellationIdAsync(
                payload.Id ?? 0,
                cancellationToken
            );

            var (entitiesToCreate, idsToDelete) = await _cancellationDataService.AdjustRangeAsync(
                listDataOfCancellation,
                existingDataOfCancellation,
                false,
                cancellationToken
            );

            await _dataOfCancellationService.CreateRangeAsync(
                payload.Id ?? 0,
                entitiesToCreate,
                false,
                cancellationToken
            );

            await _dataOfCancellationService.DeleteRangeAsync(
                payload.Id ?? 0,
                idsToDelete,
                false,
                cancellationToken
            );

            var cancellationData = await _dataOfCancellationService.GetAllCancellationDataAsync(
                cancellationId ?? 0,
                cancellationToken
            );

            var cancellationDataDict = cancellationData.ToDictionary(d => d.Id);

            foreach (var data in listDataOfCancellation)
            {
                if (!cancellationDataDict.TryGetValue(data.Id, out var existingData)
                    || existingData.Description == null
                    || data.Description == null)
                {
                    continue;
                }

                var value = existingData.Description.GetValueByCode(languageRevert);
                data.Description.UpdateLocalized(
                    new MultilingualText { { languageRevert, value } },
                    languageRevert
                );
            }

            cancellation.TableSource = BuildCancellationPolicyHtmlTables(listDataOfCancellation);

            var editCancellation = await _cancellationService.UpdateAsync(
                cancellation,
                false,
                cancellationToken: cancellationToken
            );
            await UnitOfWork.CommitAsync(cancellationToken);

            await cacheService.ResetAsync(
                string.Format(CacheKeys.ResetPatternSiteBookingSearch, facilityId),
                false,
                cancellationToken
            );

            return editCancellation.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Update cancellation failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }

    private MultilingualText BuildCancellationPolicyHtmlTables(
        List<CancellationData> listDataOfCancellation
    )
    {
        var jaTableSource = _cancellationTableHtmlService.GenerateHtmlTable(
            listDataOfCancellation,
            LanguageHeaderUtil.DefaultLanguageCode
        );
        var enTableSource = _cancellationTableHtmlService.GenerateHtmlTable(
            listDataOfCancellation,
            "en"
        );

        var tableSource = new MultilingualText
        {
            { LanguageHeaderUtil.DefaultLanguageCode, jaTableSource },
            { "en", enTableSource }
        };

        return tableSource;
    }
}
