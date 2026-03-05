namespace Liberty.Reservation.Manager.File.WebAPI.Application.UserCases.Commands.Image;

public class ImageUpdateCommandHandler(
    ILogger<ImageUpdateCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IFacilityFileService facilityFileService,
    IFileCategoryService fileCategoryService,
    ICategoryService categoryService
) : UpdateCommandHandlerBase<ImageUpdateCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        ImageUpdateCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var imageCategoryIds = payload.ImageCategoryIds ?? [];
        var masterCategoryIds = payload.MasterCategoryIds ?? [];

        var concatenatedIds = imageCategoryIds.Concat(masterCategoryIds).Distinct().ToList();

        var existingCountMaster = await categoryService.CountMasterByIdsAsync(
            [.. masterCategoryIds],
            [CategoryTypes.File],
            false,
            cancellationToken
        );

        var existingCountWithFacility = await categoryService.CountByIdsAsync(
            [.. imageCategoryIds],
            [CategoryTypes.File],
            cancellationToken
        );

        if (existingCountWithFacility + existingCountMaster != concatenatedIds.Count)
        {
            throw new CategoryNotfoundException();
        }

        var existingFileOfFacility = await facilityFileService.GetByFileIdAsync(
            payload.Id ?? 0,
            cancellationToken
        );

        existingFileOfFacility.File!.IsEnabled = payload.IsEnabled;
        var tags = payload.Tags is null ? [] : payload.Tags.Distinct().ToArray();
        existingFileOfFacility.File!.Tag = string.Join(",", tags);
        existingFileOfFacility.File!.Description = payload.Description;
        existingFileOfFacility.IsEnabled = payload.IsEnabled;
        existingFileOfFacility.Index = payload.Index ?? 0;

        var filePurposeType = payload.FilePurposeTypes?.Aggregate(
                FilePurposeTypes.None,
                (
                    acc,
                    flag
                ) => acc | flag
            )
            ?? FilePurposeTypes.None;

        existingFileOfFacility.FilePurposeType = filePurposeType;

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            await facilityFileService.UpdateAsync(
                existingFileOfFacility,
                false,
                cancellationToken
            );

            await fileCategoryService.ChangeFileCategoryAsync(
                existingFileOfFacility.FileId,
                concatenatedIds,
                false,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            return payload.Id ?? 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Action} {Message}", nameof(ImageUpdateCommandHandler), ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
