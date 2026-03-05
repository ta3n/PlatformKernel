using Liberty.Cache.Services;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public class PlanUpdateDisplayCommandHandler(
    ILogger<PlanUpdateDisplayCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPlanService planService,
    IPlanCategoryService planCategoryService,
    ICategoryService categoryService,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor
) : UpdateCommandHandlerBase<PlanUpdateDisplayCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PlanUpdateDisplayCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var existingPlan = await planService.FindByIdWithIncludeAsync(
            request.Id,
            cancellationToken
        );

        var tags = payload.Tags is null ? [] : payload.Tags.Distinct().ToArray();
        existingPlan.Tag ??= [];
        existingPlan.Tag.UpdateLocalized(
            new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), string.Join(",", tags) } }
        );

        var planCategoryIds = payload.PlanCategoryIds ?? [];
        var masterCategoryIds = payload.MasterCategoryIds ?? [];
        var concatenatedIds = planCategoryIds.Concat(masterCategoryIds).Distinct().ToArray();

        var existingCountMaster = await categoryService.CountMasterByIdsAsync(
            [.. masterCategoryIds],
            [CategoryTypes.Plan],
            false,
            cancellationToken
        );

        var existingCountWithFacility = await categoryService.CountByIdsAsync(
            [.. planCategoryIds],
            [CategoryTypes.Plan],
            cancellationToken
        );

        if (existingCountWithFacility + existingCountMaster != concatenatedIds.Length)
        {
            throw new CategoryNotfoundException();
        }

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            _ = await planCategoryService.ChangeCategoriesOfPlanAsync(
                existingPlan.Id,
                [.. concatenatedIds],
                false,
                cancellationToken
            );

            var editPlan = await planService.UpdateAsync(
                existingPlan,
                false,
                cancellationToken: cancellationToken
            );

            await cacheService.ResetAsync(
                string.Format(CacheKeys.ResetPatternSiteBookingSearch, securityContextAccessor.FacilityKey),
                false,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            return editPlan.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Action} {Message}", nameof(PlanUpdateDisplayCommandHandler), ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
