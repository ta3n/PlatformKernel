namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public class PlanUpdateBasicSettingCommandHandler(
    ILogger<PlanUpdateBasicSettingCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPlanService planService,
    IFilePlanService filePlanService,
    IFileService fileService
) : UpdateCommandHandlerBase<PlanUpdateBasicSettingCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PlanUpdateBasicSettingCommand request,
        CancellationToken cancellationToken
    )
    {
        var existingPlan = await planService.FindByIdWithIncludeAsync(
            request.Id,
            cancellationToken
        );

        if (request.Payload.Files is { Count: > 0 })
        {
            var fileIds = request.Payload.Files.Select(f => f.Id).Distinct().ToArray();
            var existingCount = await fileService.CountByIdsAsync(
                fileIds,
                cancellationToken
            );

            if (existingCount != fileIds.Length)
            {
                throw new ImageNotfoundException();
            }
        }

        var planToUpdate = Mapper.Map<Reservation.Application.Contexts.DataContexts.Entities.Data.Plan>(request.Payload);
        existingPlan.Name ??= [];
        existingPlan.Name?.UpdateLocalized(planToUpdate.Name);
        existingPlan.NameForImport ??= [];
        existingPlan.NameForImport?.UpdateLocalized(planToUpdate.NameForImport);
        existingPlan.Description ??= [];
        existingPlan.Description?.UpdateLocalized(planToUpdate.Description);
        existingPlan.Summary ??= [];
        existingPlan.Summary?.UpdateLocalized(planToUpdate.Summary);

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            List<FilePlan> listFilePlan = [];

            foreach (var file in request.Payload.Files ?? [])
            {
                listFilePlan.Add(
                    new FilePlan
                    {
                        Plan = existingPlan,
                        FileId = file.Id,
                        Index = file.Index
                    }
                );
            }

            _ = await filePlanService.ChangeFilePlanAsync(
                existingPlan.Id,
                listFilePlan,
                false,
                cancellationToken
            );

            var editPlan = await planService.UpdateBasicSettingAsync(existingPlan, false, cancellationToken: cancellationToken);

            await UnitOfWork.CommitAsync(cancellationToken);

            return editPlan.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Action} {Message}", nameof(PlanUpdateBasicSettingCommandHandler), ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
