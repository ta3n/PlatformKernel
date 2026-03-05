namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.CancellationPolicy;

public class CancellationPolicyEnableCommandHandler(
    ILogger<CancellationPolicyEnableCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICancellationService cancellationService,
    IPlanService planService
) : UpdateCommandHandlerBase<CancellationPolicyEnableCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        CancellationPolicyEnableCommand request,
        CancellationToken cancellationToken
    )
    {
        var cancellationId = request.Id;

        if (!request.Payload.IsEnabled)
        {
            var checkHasAnyPlanUsingCancellationPolicys =
                await planService.IsAnyPlanEnabledUsingCancellationAsync([cancellationId], cancellationToken);
            if (checkHasAnyPlanUsingCancellationPolicys)
            {
                throw new CancellationPolicyHasPlanUsingException();
            }
        }

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            _ = await cancellationService.EnableAsync(
                cancellationId,
                request.Payload.IsEnabled,
                cancellationToken: cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            return cancellationId;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Enable cancellation failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
