namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public class PlanUpdatePublishAcceptCommandHandler(
    ILogger<PlanUpdatePublishAcceptCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPlanService planService,
    ISiteService siteService,
    IPlanSiteService planSiteService
) : UpdateCommandHandlerBase<PlanUpdatePublishAcceptCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PlanUpdatePublishAcceptCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var isEditingSites = payload.Sites is { Count: > 0 };

        var existingPlan = await planService.FindByIdAsync(
            request.Id,
            cancellationToken
        );

        if (isEditingSites)
        {
            var existingSitesCount = await siteService.CountByIdsAsync(
                payload.Sites?.ToArray() ?? [],
                cancellationToken
            );

            if (existingSitesCount != payload.Sites?.Count)
            {
                throw new SiteNotfoundException();
            }
        }

        Mapper.Map(request.Payload, existingPlan);

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            _ = await planSiteService.ChangeSitesOfPlanAsync(
                existingPlan.Id,
                payload.Sites ?? [],
                false,
                cancellationToken
            );

            var editPlan = await planService.UpdateAsync(
                existingPlan,
                false,
                cancellationToken: cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            return editPlan.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Action} {Message}", nameof(PlanUpdatePublishAcceptCommandHandler), ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
