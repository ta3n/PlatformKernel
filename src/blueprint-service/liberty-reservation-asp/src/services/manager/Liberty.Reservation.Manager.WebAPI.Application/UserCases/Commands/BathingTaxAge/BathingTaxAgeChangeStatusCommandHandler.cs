using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BathingTaxAge;

public class BathingTaxAgeChangeStatusCommandHandler(
    ILogger<BathingTaxAgeChangeStatusCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IFacilityService facilityService,
    IPersonAgeTypeService personAgeTypeService
) : UpdateCommandHandlerBase<BathingTaxAgeChangeStatusCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        BathingTaxAgeChangeStatusCommand request,
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
            var personAgeType = await personAgeTypeService.EnableAsync(payload.Id, payload.IsEnable, false, cancellationToken);
            await UnitOfWork.CommitAsync(cancellationToken);
            return personAgeType.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Action} {Message}", nameof(BathingTaxAgeChangeStatusCommandHandler), ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
