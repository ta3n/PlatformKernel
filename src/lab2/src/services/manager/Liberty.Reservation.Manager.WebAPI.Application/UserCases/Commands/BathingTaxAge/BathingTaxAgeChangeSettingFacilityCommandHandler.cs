using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BathingTaxAge;

public class BathingTaxAgeChangeSettingFacilityCommandHandler(
    ILogger<BathingTaxAgeChangeSettingFacilityCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IFacilityService facilityService
) : UpdateCommandHandlerBase<BathingTaxAgeChangeSettingFacilityCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        BathingTaxAgeChangeSettingFacilityCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var facilityId = securityContextAccessor.FacilityKey;

        var existingFacilityCount = await facilityService.CountByIdsAsync(
            [facilityId],
            cancellationToken
        );
        if (existingFacilityCount == 0)
        {
            throw new FacilityNotfoundException();
        }

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var entityToUpdate = new Reservation.Application.Contexts.DataContexts.Entities.Data.Facility
            {
                Id = facilityId,
                Meta = new FacilityMeta { UseSpaTax = payload.UseSpaTax },
                SpaTaxComment =
                    new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), payload.SpaTaxComment ?? string.Empty } },
                SpaTaxTable = new MultilingualText
                {
                    { LanguageHeaderUtil.GetLanguageCodeFromHeader(), payload.SpaTaxTable ?? string.Empty }
                }
            };
            _ = await facilityService.UpdateSpaTaxChangeAsync(
                entityToUpdate,
                false,
                cancellationToken: cancellationToken
            );
            await UnitOfWork.CommitAsync(cancellationToken);

            return entityToUpdate.Id;
        }
        catch
            (Exception ex)
        {
            logger.LogError(ex, "{Action} {Message}", nameof(BathingTaxAgeChangeSettingFacilityCommandHandler), ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
