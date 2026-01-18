namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.OptionItem;

public class OptionItemDeleteCommandHandler(
    ILogger<OptionItemDeleteCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IOptionItemService optionItemService,
    IFacilityOptionItemService facilityOptionItemService,
    IOptionItemCategoryService optionItemCategoryService,
    IOptionItemQuestionService optionItemQuestionService,
    IFileOptionItemService fileOptionItemService
) : DeleteCommandHandlerBase<OptionItemDeleteCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        OptionItemDeleteCommand request,
        CancellationToken cancellationToken
    )
    {
        var existingOptionItemsInFacility = await facilityOptionItemService.FindAllByOptionItemIdAsync(
            request.Payload.Id,
            cancellationToken
        );

        var existingCategoriesInOptionItem = await optionItemCategoryService.FindAllByOptionItemIdAsync(
            request.Payload.Id,
            cancellationToken
        );

        var existingQuestionsInOptionItem = await optionItemQuestionService.FindAllByOptionItemIdAsync(
            request.Payload.Id,
            cancellationToken
        );

        var existingFilesInOptionItem = await fileOptionItemService.FindAllByOptionItemIdAsync(
            request.Payload.Id,
            cancellationToken
        );

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var removeOptionItem = await optionItemService.DeleteAsync(
                request.Payload.Id,
                false,
                cancellationToken
            );

            _ = await facilityOptionItemService.DeleteRangeAsync(
                existingOptionItemsInFacility,
                false,
                cancellationToken
            );

            _ = await optionItemCategoryService.DeleteRangeAsync(
                existingCategoriesInOptionItem,
                false,
                cancellationToken
            );

            _ = await optionItemQuestionService.DeleteRangeAsync(
                existingQuestionsInOptionItem,
                false,
                cancellationToken
            );

            _ = await fileOptionItemService.DeleteRangeAsync(
                existingFilesInOptionItem,
                false,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            return removeOptionItem.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Delete facility failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
