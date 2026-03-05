namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.OptionItem;

public class OptionItemUpdateCommandHandler(
    ILogger<OptionItemUpdateCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IOptionItemService optionItemService,
    ICategoryService categoryService,
    IQuestionService questionService,
    IOptionItemCategoryService optionItemCategoryService,
    IOptionItemQuestionService optionItemQuestionService,
    IFileOptionItemService fileOptionItemService
) : UpdateCommandHandlerBase<OptionItemUpdateCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        OptionItemUpdateCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var optionCategoryIds = payload.OptionCategoryIds ?? [];
        var masterCategoryIds = payload.MasterCategoryIds ?? [];
        var concatenatedIds = optionCategoryIds.Concat(masterCategoryIds).Distinct().ToArray();

        var existingCountMaster = await categoryService.CountMasterByIdsAsync(
            [.. masterCategoryIds],
            [CategoryTypes.OptionItem],
            false,
            cancellationToken
        );

        var existingCountWithFacility = await categoryService.CountByIdsAsync(
            optionCategoryIds,
            [CategoryTypes.OptionItem],
            cancellationToken
        );

        if (existingCountWithFacility + existingCountMaster != concatenatedIds.Length)
        {
            throw new CategoryNotfoundException();
        }

        var existingCountQuestion = await questionService.CountByIdsAsync(
            payload.QuestionIds ?? [],
            cancellationToken
        );
        if (existingCountQuestion != (payload.QuestionIds ?? []).Length)
        {
            throw new QuestionNotfoundException();
        }

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var optionItem =
                Mapper.Map<Reservation.Application.Contexts.DataContexts.Entities.Data.OptionItem>(payload);
            var editOptionItem = await optionItemService.UpdateAsync(
                optionItem,
                false,
                cancellationToken: cancellationToken
            );

            _ = await optionItemCategoryService.ChangeCategoriesOfOptionItemAsync(
                editOptionItem.Id,
                concatenatedIds,
                false,
                cancellationToken
            );

            _ = await optionItemQuestionService.ChangeQuestionsOfOptionItemAsync(
                editOptionItem.Id,
                payload.QuestionIds ?? [],
                false,
                cancellationToken
            );

            _ = await fileOptionItemService.ChangeFilesOfOptionItemAsync(
                editOptionItem.Id,
                payload.Images?.Select(x => (x.Id, x.Index)) ?? [],
                false,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            return editOptionItem.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Update option item failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
